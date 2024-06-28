using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using App.Extensions;
using App.Models;

namespace App.Controls {
	public partial class MainForm : Form {
		private readonly RdpFormFactory _factory = new();
		private CancellationTokenSource _source = new();
		private Thread? _execution_thread;
        
		public MainForm() {
			InitializeComponent();
		}

		private async void btn_Select_Click(object sender, EventArgs e) {
			// Validate settings
			if (!int.TryParse(box_Threads.Text, out var threads)) {
				return;
			}
			
			//
			if (_PickFile() is not { } file || !File.Exists(file.path)) {
				return;
			}
			
			Directory.CreateDirectory(file.name);
			btn_Cancel.Enabled = true;
			
			//
			_execution_thread = new(() => _StartChecking(file, threads, _source.Token));
			_execution_thread.Start();
		}

		private void btn_Cancel_Click(object sender, EventArgs e) {
			_source.Cancel();
			_source.Dispose();
			_source = new();

			btn_Cancel.Enabled = false;
		}
		
		// private
		private (string path, string name)? _PickFile() {
			var dialog = new OpenFileDialog {
				Filter = "Text|*.txt",
				Multiselect = false
			};
			
			if (dialog.ShowDialog() == DialogResult.OK) {
				var info = new FileInfo(dialog.FileName);
				
				return (dialog.FileName, info.Name.Substring(0, info.Name.Length - info.Extension.Length));
			}

			return null;
		}
		
		public (RdpForm form, Thread thread, bool with_nla) CreateApp(string server, int? port) {
			var form = _factory.Create();
			var thread = new Thread(
				() => {
					_factory.InitializeRdp(form);

					form.Shown += async (sender, args) => await form.Rdp.TryConnectAsync(server, port: port);
                    
					Application.Run(form);
				}
			);
			
			thread.SetApartmentState(ApartmentState.STA);
			thread.Start();

			return (form, thread, port is not null);
		}

		private async void _StartChecking((string path, string name) file, int threads, CancellationToken token) {
			var list = new List<(RdpForm form, Thread thread, bool with_nla)>();
			
			using (var reader = new StreamReader(file.path)) {
				while (!reader.EndOfStream && !token.IsCancellationRequested) {
					// Create apps
					if (list.Count < threads) {
						for (int i = 0; i < 5; i++) {
							var line = (await reader.ReadLineAsync()).Split(':');

							list.Add(CreateApp(line[0], line.Length == 2 ? int.Parse(line[1]) : null));
						}
						
						continue;
					}
					
					// Check rdp servers
					var array = list.ToArray();
                    
					for (var index = 0; index < array.Length && !token.IsCancellationRequested; index++) {
						var data = array[index];

						try {
							data.form.Invoke(
								() => {
									data.form.Visible = true;
									data.form.WindowState = FormWindowState.Normal;
								}
							);
						} catch {
							continue;
						}

						data.form.Invoke(
							async () => {
								try {
									// Confirm NLA warning
									if (data.with_nla) {
										data.form.Rdp.Connect();

										await Task.Delay(1000, token);

										SendKeys.Send("{LEFT}");
										SendKeys.Send("{ENTER}");
									}

									// Wait connection
									if (!await data.form.Rdp.WaitAsync(x => x.Connected == 1, 4000)) {
										return;
									}

									//
									await Task.Delay(1000, token);
									data.form.Rdp.ClickKeys();

									await Task.Delay(1000, token);
									data.form.Rdp.CreateScreenshot(file.name);

								} catch {
									// ignore
								}
								finally {
									data.form.Rdp.Dispose();

									data.form.Dispose();
									data.thread.Interrupt();
								}
							}
						);

						data.thread.Join();
						list.Remove(data);
					}
				}
			}
			
			// Dispose on cancel
			foreach (var (form, thread, _) in list) {
				form.Invoke(
					() => {
						form.Close();
						form.Dispose();
					}
				);
				
				thread.Interrupt();
			}
		}
	}
}
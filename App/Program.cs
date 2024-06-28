using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using App.Controls;
using App.Extensions;
using App.Models;

namespace App {
	internal static class Program {
		public static RdpFormFactory RdpFormFactory { get; } = new();
		
		public static async Task Main() {
			var filepath = Program.PickFile();

			if (!File.Exists(filepath)) {
				return;
			}
			
			//
			var info = new FileInfo(filepath);
			var filename = info.Name.Substring(0, info.Name.Length - info.Extension.Length);

			var source = new CancellationTokenSource();

			var list = new List<(RdpForm form, Thread thread, bool with_nla)>();
			
			//
			Directory.CreateDirectory(filename);

			using (var reader = new StreamReader(filepath)) {
				while (!reader.EndOfStream) {
					// Create windows
					if (list.Count < 5) {
						for (int i = 0; i < 5; i++) {
							var line = (await reader.ReadLineAsync()).Split(':');

							list.Add(CreateForm(line[0], line.Length == 2 ? int.Parse(line[1]) : null));
						}
						
						continue;
					}
					
					// Check rdp server
					foreach (var data in list.ToArray()) {
						try {
							data.form.Invoke(
								() => {
									data.form.Visible = true;
									data.form.WindowState = FormWindowState.Normal;
								}
							);
						}
						catch {
							continue;
						}
						
						data.form.Invoke(
							async () => {
								//
								if (data.with_nla) {
									data.form.Rdp.Connect();

									await Task.Delay(1000, source.Token);

									SendKeys.Send("{LEFT}");
									SendKeys.Send("{ENTER}");
								}

								//
								if (!await data.form.Rdp.WaitAsync(x => x.Connected == 1, 4000)) {
									data.form.Rdp.Dispose();
									
									data.form.Dispose();
									data.thread.Interrupt();
									
									return;
								}

								//
								await Task.Delay(1000, source.Token);
								data.form.Rdp.ClickKeys();

								await Task.Delay(1000, source.Token);
								data.form.Rdp.CreateScreenshot(filename);

								// Dispose
								data.form.Rdp.Dispose();
								
								data.form.Dispose();
								data.thread.Interrupt();
							}
						);

						data.thread.Join();
						list.Remove(data);
					}
				}
			}
			
			//
			source.Cancel();
		}

		public static string PickFile() {
			string path = "";

			var thread = new Thread(OpenFile);
			thread.SetApartmentState(ApartmentState.STA);
			thread.Start();
			thread.Join();

			return path;
			
			void OpenFile() {
				var dialog = new OpenFileDialog() {
					Filter = "Text|*.txt",
					Multiselect = false
				};
			
				if (dialog.ShowDialog() == DialogResult.OK) {
					path = dialog.FileName;
				}
			}
		}

		public static (RdpForm form, Thread thread, bool with_nla) CreateForm(string server, int? port) {
			var form = RdpFormFactory.Create();
			var thread = new Thread(
				() => {
					RdpFormFactory.InitializeRdp(form);

					form.Shown += async (sender, args) => await form.Rdp.TryConnectAsync(server, port: port);
                    
					Application.Run(form);
				}
			);
			
			thread.SetApartmentState(ApartmentState.STA);
			thread.Start();

			return (form, thread, port is not null);
		}
	}
}
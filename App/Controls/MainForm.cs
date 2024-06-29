using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using App.Extensions;
using App.Models;

using Microsoft.Win32;

using Newtonsoft.Json;

namespace App.Controls
{
    public partial class MainForm : Form {
		private readonly Settings _settings = new();
		private readonly RdpFormFactory _factory = new();
		private readonly List<(RdpForm form, Thread thread, bool with_nla)> _worker_threads = new();
		private CancellationTokenSource _source = new();
		private Thread? _execution_thread;
        
		public MainForm() {
			InitializeComponent();
		}

		#region Settings

		protected override void OnLoad(EventArgs e) {
			if (File.Exists("settings.json")) {
				JsonConvert.PopulateObject(File.ReadAllText("settings.json"), _settings);
			}

			box_Threads.Text = _settings.Threads.ToString();
			box_ConnectionTimeout.Text = _settings.ConnectionTimeout.ToString();
			box_LoadingDelay.Text = _settings.LoadingDelay.ToString();
			box_CertificateWarningDelay.Text = _settings.CertificateWarningDelay.ToString();
			box_StickyKeysWarningDelay.Text = _settings.StickyKeysWarningDelay.ToString();
			
			base.OnLoad(e);
		}

		protected override void OnClosing(CancelEventArgs e) {
			_settings.Threads = _ParseUint(box_Threads.Text);
			_settings.ConnectionTimeout = _ParseUint(box_ConnectionTimeout.Text);
			_settings.LoadingDelay = _ParseUint(box_LoadingDelay.Text);
			_settings.CertificateWarningDelay = _ParseUint(box_CertificateWarningDelay.Text);
			_settings.StickyKeysWarningDelay = _ParseUint(box_StickyKeysWarningDelay.Text);
			
			File.WriteAllText("settings.json", JsonConvert.SerializeObject(_settings, Formatting.None));
			
			base.OnClosing(e);
		}

		#endregion

		private void btn_Select_Click(object sender, EventArgs e) {
			// Validate settings
			if (!uint.TryParse(box_Threads.Text, out var threads) || 
				!uint.TryParse(box_ConnectionTimeout.Text, out var connection_timeout) || 
				!uint.TryParse(box_LoadingDelay.Text, out var loading_delay) ||
				!uint.TryParse(box_CertificateWarningDelay.Text, out var certificate_warning_delay) ||
				!uint.TryParse(box_StickyKeysWarningDelay.Text, out var sticky_keys_delay)
				) {
				return;
			}
			
			//
			if (_PickFile() is not { } file || !File.Exists(file.path)) {
				return;
			}
			
			Directory.CreateDirectory(file.name);
			btn_Cancel.Enabled = true;
			btn_Select.Enabled = false;
			
			//
			_execution_thread = new(() => _StartChecking(file, (int)threads, (int)connection_timeout, (int)loading_delay, (int)certificate_warning_delay, (int)sticky_keys_delay, _source.Token));
			_execution_thread.Start();
		}

		private void btn_Cancel_Click(object sender, EventArgs e) {
			btn_Cancel.Enabled = false;
			
			_source.Cancel();
			_source.Dispose();
			_source = new();
			
			_execution_thread?.Interrupt();

			foreach (var (form, thread, _) in _worker_threads.ToArray()) {
				if (form.IsDisposed) {
					continue;
				}
				
				form.Close();
				form.Dispose();
				
				thread.Interrupt();
			}
			
			btn_Select.Enabled = true;
		}
		
		// private
		private static void _WaitInvokeAsync(Control control, Delegate @delegate) {
			while (true) {
				try {
					control.Invoke(@delegate);
				} catch {
					continue;
				}
				
				break;
			}
		}
		
		private static uint _ParseUint(string source) => !uint.TryParse(source, out var result) ? 0 : result;

		private static (string path, string name)? _PickFile() {
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
		
		private (RdpForm form, Thread thread, bool with_nla) _CreateApp(string server, int? port) {
			var form = _factory.Create();
			var thread = new Thread(
				() => {
					_factory.InitializeRdp(form);

					form.Shown += async (sender, args) => {
						await form.Rdp.TryConnectAsync(server, port: port);
					};
                    
					Application.Run(form);
				}
			);
			
			thread.SetApartmentState(ApartmentState.STA);
			thread.Start();

			return (form, thread, port is not null);
		}

		private async void _StartChecking(
			(string path, string name) file, 
			int threads,
			int connection_timeout,
            int loading_delay,
			int certificate_warning_delay,
            int sticky_keys_delay,
			CancellationToken token) {
			
			using (var reader = new StreamReader(file.path)) {
				while (!reader.EndOfStream && !token.IsCancellationRequested) {
					if (!token.IsCancellationRequested) {
						_worker_threads.Clear();
					}
					
					// Create apps
					if (_worker_threads.Count < threads) {
						for (int i = 0; i < 5 && !reader.EndOfStream; i++) {
							var line = (await reader.ReadLineAsync()).Split(':');
							
							_worker_threads.Add(_CreateApp(line[0], line.Length == 2 ? int.Parse(line[1]) : null));
						}
					}
					
					// Check rdp servers
					for (var index = 0; index < _worker_threads.Count && !token.IsCancellationRequested; index++) {
						var data = _worker_threads[index];

						_WaitInvokeAsync(
							data.form,
							 async () => {
								data.form.Visible = true;
								data.form.WindowState = FormWindowState.Normal;
								
								await Task.Delay(connection_timeout + loading_delay + certificate_warning_delay + sticky_keys_delay, token);
							
							
								data.form.Close();
							}
						);

						data.form.Invoke(
							async () => {
								try {
									// Confirm NLA warning
									if (data.with_nla) {
										data.form.Rdp.Connect();

										await Task.Delay(certificate_warning_delay, token);

										SendKeys.Send("{LEFT}");
										SendKeys.Send("{ENTER}");
									}

									// Wait connection
									if (!await data.form.Rdp.WaitConnectionAsync(connection_timeout, token)) {
										return;
									}

									//
									await Task.Delay(loading_delay, token);
									data.form.Rdp.ClickKeys();

									await Task.Delay(sticky_keys_delay, token);
									data.form.Rdp.CreateScreenshot(file.name);
								}
								finally {
									data.form.Rdp.Disconnect();

									data.form.Close();
								}
							}
						);

						try {
							data.thread.Join();
						}
						catch {
							// ignore
						}
					}
				}
			}

			Invoke(
				() => {
					btn_Cancel.Enabled = false;
					btn_Select.Enabled = true;
				}
			);
		}
	}
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows.Forms;

using App.Extensions;
using App.Models;

using Newtonsoft.Json;

namespace App.Controls
{
    public partial class MainForm : Form {
		private readonly Settings _settings = new();
		private readonly RdpFormFactory _factory = new();
		private readonly List<(RdpForm form, Thread thread)> _worker_threads = new();
		private readonly ManualResetEvent _event = new(false);
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
			_SetSettings();
			
			File.WriteAllText("settings.json", JsonConvert.SerializeObject(_settings, Formatting.None));
			
			base.OnClosing(e);
		}

		#endregion

		private void btn_Select_Click(object sender, EventArgs e) {
			// Validate settings
			_SetSettings();
			
			//
			if (_PickFile() is not { } file || !File.Exists(file.path)) {
				return;
			}
			
			Directory.CreateDirectory(file.name);
			btn_Cancel.Enabled = true;
			btn_Select.Enabled = false;
			
			//
			_execution_thread = new(() => _StartChecking(file, (int)_settings.Threads, _source.Token));
			_execution_thread.Start();
		}

		private void btn_Cancel_Click(object sender, EventArgs e) {
			btn_Cancel.Enabled = false;
			
			_source.Cancel();
			_source.Dispose();
			_source = new();
			
			_execution_thread?.Interrupt();

			foreach (var (form, thread) in _worker_threads.ToArray()) {
				if (form.IsDisposed) {
					continue;
				}

				form.Invoke(
					() => {
						form.Close();
					}
				);

				form.Dispose();
				
				thread.Interrupt();
			}
			
			btn_Select.Enabled = true;
		}
		
		// private
		private static uint _ParseUint(string source) => !uint.TryParse(source, out var result) ? 0 : result;
        
		private void _SetSettings() {
			_settings.Threads = _ParseUint(box_Threads.Text);
			_settings.ConnectionTimeout = _ParseUint(box_ConnectionTimeout.Text);
			_settings.LoadingDelay = _ParseUint(box_LoadingDelay.Text);
			_settings.CertificateWarningDelay = _ParseUint(box_CertificateWarningDelay.Text);
			_settings.StickyKeysWarningDelay = _ParseUint(box_StickyKeysWarningDelay.Text);
		}
		
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
		
		private unsafe (RdpForm form, Thread thread) _CreateApp(string server, int? port, int* count, string directory) {
			var form = _factory.Create(_settings, _event, port is not null, count, directory);
			var thread = new Thread(
				() => {
					_factory.InitializeRdp(form);
					form.Rdp.ConfigureConnection(server, port);

					form.Show();
                    
					Application.Run(form);
				}
			);
			
			thread.SetApartmentState(ApartmentState.STA);
			thread.Start();

			return (form, thread);
		}

		private async void _StartChecking((string path, string name) file, int threads, CancellationToken token) {
			
			using (var reader = new StreamReader(file.path)) {
				while (!reader.EndOfStream && !token.IsCancellationRequested) {
					if (!token.IsCancellationRequested) {
						_worker_threads.Clear();
					}
					
					var count = threads;
					_event.Reset();
					
					// Create apps
					if (_worker_threads.Count < threads) {
						for (int i = 0; i < threads && !reader.EndOfStream; i++) {
							var line = (await reader.ReadLineAsync()).Split(':');

							unsafe {
								_worker_threads.Add(
									_CreateApp(
										line[0],
										line.Length == 2 ? int.Parse(line[1]) : null,
										&count,
										file.name
									)
								);
							}
						}
					}
					
					try {
						_event.WaitOne();
					}
					catch {
						// ignore
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
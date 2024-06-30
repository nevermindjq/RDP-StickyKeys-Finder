using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows.Forms;

using App.Extensions;
using App.Models;

using Newtonsoft.Json;

#nullable enable

namespace App.Controls
{
    public partial class MainForm : Form {
		private readonly Settings _settings = new();
		private readonly RdpFormFactory _factory = new();
		private readonly List<(RdpForm form, Thread thread)> _worker_threads = new();
		private readonly ManualResetEvent _event = new(false);
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
			_execution_thread = new(() => _StartChecking(file, (int)_settings.Threads));
			_execution_thread.Start();
		}

		private void btn_Cancel_Click(object sender, EventArgs e) {
			btn_Cancel.Enabled = false;
			
			_execution_thread?.Interrupt();
			_CloseWorkerThreads();
			
			btn_Select.Enabled = true;
		}
		
		// private
		private void _CloseWorkerThreads() {
			var threads = _worker_threads.ToArray();
			_worker_threads.Clear();
			
			foreach (var (form, thread) in threads) {
				if (!form.IsDisposed) {
					continue;
				}

				form.Dispose();
				thread.Interrupt();
			}
		}
		
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
			var form = _factory.Create(_settings, _event, count, directory);
			
			var thread = new Thread(
				() => {
					_factory.InitializeRdp(form);
					form.Rdp.ConfigureConnection(server, port);

					form.Show();

					try {
						Application.Run(form);
					} catch {
						// ignore
					}
				}
			);
			
			thread.SetApartmentState(ApartmentState.STA);
			thread.Start();

			return (form, thread);
		}

		private void _StartChecking((string path, string name) file, int threads) {
			using (var reader = new StreamReader(file.path)) {
				while (!reader.EndOfStream) {
					var count = threads;
					_event.Reset();
					
					// Create apps
					if (_worker_threads.Count < threads) {
						for (int i = _worker_threads.Count; i < threads; i++) {
							var line = reader.ReadLine().Split(':');

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
						_event.WaitOne((int)(_settings.ConnectionTimeout + _settings.LoadingDelay + _settings.StickyKeysWarningDelay));
						
						_CloseWorkerThreads();
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
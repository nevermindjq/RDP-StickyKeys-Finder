using System;
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
		private static int _count;

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
			var @event = new ManualResetEvent(false);

			var list = new List<(RdpForm, Thread)>();
			
			//
			Directory.CreateDirectory(filename);

			using (var reader = new StreamReader(filepath)) {
				while (!reader.EndOfStream) {
					// Create windows
					if (_count < 5) {
						for (int i = 0; i < 5; i++) {
							var line = (await reader.ReadLineAsync()).Split(':');

							list.Add(CreateForm(@event, line[0], line.Length == 2 ? int.Parse(line[1]) : null));
						}
						
						Interlocked.Increment(ref _count);
						
						continue;
					}
					
					// Check rdp server
					foreach (var (form, thread) in list) {
						try {
							form.Invoke(
								() => {
									form.Visible = true;
									form.WindowState = FormWindowState.Normal;
								}
							);
						}
						catch {
							continue;
						}
						
						form.Invoke(
							async () => {
								if (!await form.Rdp.WaitAsync(x => x.Connected == 1, 6000)) {
									form.Close();
									thread.Interrupt();
									return;
								}

								await Task.Delay(1000, source.Token);
								 form.Rdp.ClickKeys();
								 
								 await Task.Delay(1000, source.Token);
								 form.Rdp.CreateScreenshot(filename);
						
								form.Close();
								thread.Interrupt();
							}
						);

						thread.Join();
					}
				}
			}
			
			//
			@event.WaitOne();
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

		public static (RdpForm form, Thread thread) CreateForm(ManualResetEvent reset, string server, int? port) {
			var form = RdpFormFactory.Create();
			var thread = new Thread(
				state => {
					RdpFormFactory.InitializeRdp(form);

					form.Shown += async (sender, args) => await form.Rdp.TryConnectAsync(server, port: port);
                    
					Application.Run(form);

					if (Interlocked.Decrement(ref _count) == 0) {
						((ManualResetEvent)state).Set();
					}
				}
			);
			
			thread.SetApartmentState(ApartmentState.STA);
			thread.Start(reset);

			return (form, thread);
		}
	}
}
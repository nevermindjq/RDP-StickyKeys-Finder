using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using App.Extensions;
using App.Models;

namespace App {
	internal static class Program {
		public static async Task Main() {
			var filepath = _PickFile();
			var info = new FileInfo(filepath);
			var filename = info.Name.Substring(0, info.Name.Length - info.Extension.Length);

			var source = new CancellationTokenSource();
			var @event = new ManualResetEvent(false);
			var count = 0;

			Directory.CreateDirectory(filename);

			using (var reader = new StreamReader(filepath)) {
				while (!reader.EndOfStream) {
					if (count >= 5) {
						await Task.Delay(1000, source.Token);
						continue;
					}

					Interlocked.Increment(ref count);

					var line = await reader.ReadLineAsync();
					var thread = new Thread(
						state => {
							_CheckServer(line, filename);

							if (Interlocked.Decrement(ref count) == 0) {
								((ManualResetEvent)state).Set();
							}
						}
					);

					thread.SetApartmentState(ApartmentState.STA);
					thread.Start(@event);
				}
			}

			@event.WaitOne();
			source.Cancel();
		}

		private static string _PickFile() {
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
			
				while (true) {
					if (dialog.ShowDialog() == DialogResult.OK) {
						path = dialog.FileName;
						return;
					}
				}
			}
		}

		private static void _CheckServer(string server, string screenshots_directory) {
			var form = new RdpFormFactory().Create();
			
			form.Shown += async (sender, args) => {
				await form.Rdp.CheckAsync(server, screenshots_directory);
				
				form.Close();
			};

			Application.Run(form);
		}
	}
}
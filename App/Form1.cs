using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using App.Extensions;

namespace App {
	public partial class Form1 : Form {
		private CancellationTokenSource _source = new();
		
		public Form1() {
			InitializeComponent();
		}

		private async void btn_pick_Click(object sender, EventArgs e) {
			var dialog = new OpenFileDialog {
				Filter = "Text|*.txt",
				Multiselect = false
			};

			if (dialog.ShowDialog() is not DialogResult.OK) {
				return;
			}
			
			await CheckAsync(dialog.FileName, _source.Token);
		}

		private void btn_cancel_Click(object sender, EventArgs e) {
			_source.Cancel();
			_source = new();
		}
		
		//
		private async Task CheckAsync(string filepath, CancellationToken token) {
			var info = new FileInfo(filepath);
			var filename = info.Name.Substring(0, info.Name.Length - info.Extension.Length);
			
			Directory.CreateDirectory(filename);
			
			using (var reader = new StreamReader(filepath)) {
				while (!reader.EndOfStream && !token.IsCancellationRequested) {
					var line = (await reader.ReadLineAsync()).Split(':');
					
					await rdp.CheckAsync(
						line[0],
						filename,
						line.Length == 2 && int.TryParse(line[1], out var port) ? port : null
					);
				}
			}
		}
	}
}
using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;

using App.Extensions;

namespace App {
	public partial class Form1 : Form {
		private CancellationTokenSource _source = new();
		
		public Form1() {
			InitializeComponent();
		}

		protected override void OnLoad(EventArgs e) {
			rdp.AdvancedSettings8.AuthenticationLevel = 0;
			
			base.OnLoad(e);
		}

		private async void btn_pick_Click(object sender, EventArgs e) {
			var dialog = new OpenFileDialog {
				Filter = "Text|*.txt",
				Multiselect = false
			};

			if (dialog.ShowDialog() is not DialogResult.OK) {
				return;
			}

			var info = new FileInfo(dialog.FileName);
			var filename = info.Name.Substring(0, info.Name.Length - info.Extension.Length);

			Directory.CreateDirectory(filename);
			
			using (var reader = new StreamReader(dialog.FileName)) {
				while (!reader.EndOfStream && !_source.Token.IsCancellationRequested) {
					var line = (await reader.ReadLineAsync()).Split(':');
					
					await rdp.Check(
						line[0],
						new() {
							X = Location.X + Width - ClientRectangle.Width,
							Y = Location.Y + Height - ClientRectangle.Height
						},
						filename,
						line.Length == 2 && int.TryParse(line[1], out var port) ? port : null
					);
				}
			}
		}

		private void btn_cancel_Click(object sender, EventArgs e) {
			_source.Cancel();
		}
	}
}
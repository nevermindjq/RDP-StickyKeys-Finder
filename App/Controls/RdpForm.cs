using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using App.Extensions;
using App.Models;

using AxMSTSCLib;

namespace App.Controls {
	public class RdpForm : Form {
		public AxMsRdpClient8NotSafeForScripting Rdp { get; set; }

		public Settings Settings { get; set; }
		public bool WithNLA { get; set; }
		public string Directory { get; set; }
		
		//
		public ManualResetEvent Event { get; set; }
		public unsafe int* count;
		
		protected override async void OnShown(EventArgs e) {
			base.OnShown(e);
			
			Text = Rdp.Server;
			
			try {
				if (!await Rdp.TryConnectAsync((int)Settings.ConnectionTimeout)) {
					return;
				}
				
				// Confirm NLA warning TODO
				if (WithNLA) {
					await Task.Delay((int)Settings.CertificateWarningDelay);

					SendKeys.Send("{LEFT}");
					SendKeys.Send("{ENTER}");
				}

				//
				await Task.Delay((int)Settings.LoadingDelay);
				Rdp.ClickKeys();

				await Task.Delay((int)Settings.StickyKeysWarningDelay);
				Rdp.CreateScreenshot(Directory);
			}
			finally {
				if (Rdp.Connected == 1) {
					Rdp.Disconnect();
				}

				Close();
			}
		}

		protected override void Dispose(bool disposing) {
			Rdp.Dispose();
			
			unsafe {
				*count -= 1;
					
				if (*count == 0) {
					Event.Set();
				}
			}
			
			base.Dispose(disposing);
		}
	}
}
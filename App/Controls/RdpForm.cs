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
		public Settings Settings { get; set; } = new();
		public string Directory { get; set; } = "Test";
		
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
				
				//
				await Task.Delay((int)Settings.LoadingDelay);
				Rdp.ClickKeys();

				await Task.Delay((int)Settings.StickyKeysWarningDelay);
				Rdp.CreateScreenshot(Directory);
			}
			finally {
				Close();
			}
		}

		protected override void Dispose(bool disposing) {
			try {
				if (Rdp.Connected == 1) {
					Rdp.Disconnect();
				}
				
				Rdp.Dispose();
			}
			catch {
				// ignore
			}
			
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
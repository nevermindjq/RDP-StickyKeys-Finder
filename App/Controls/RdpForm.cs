using System;
using System.Windows.Forms;

using AxMSTSCLib;

namespace App.Controls {
	public class RdpForm : Form {
		public AxMsRdpClient8NotSafeForScripting Rdp { get; set; }

		protected override void OnShown(EventArgs e) {
			base.OnShown(e);
			
			Text = Rdp.Server;
		}
	}
}
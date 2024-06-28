using System;
using System.Windows.Forms;

using AxMSTSCLib;

namespace App.Controls {
	public class RdpForm : Form {
		public string Id { get; } = Guid.NewGuid().ToString();
		public AxMsRdpClient8NotSafeForScripting Rdp { get; set; }
	}
}
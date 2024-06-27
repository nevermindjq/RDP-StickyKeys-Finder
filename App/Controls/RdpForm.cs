using System.Windows.Forms;

using AxMSTSCLib;

namespace App.Controls {
	public class RdpForm : Form {
		public AxMsRdpClient8NotSafeForScripting Rdp { get; } = new();
	}
}
using System;
using System.Windows.Forms;

using App.Controls;

namespace App {
	internal static class Program {
		[STAThread]
		public static void Main() {
			Application.EnableVisualStyles();
			Application.Run(new MainForm());
		}
	}
}
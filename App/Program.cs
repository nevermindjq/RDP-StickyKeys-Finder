using System;
using System.IO;
using System.Windows.Forms;

using App.Extensions;
using App.Models;

using AxMSTSCLib;

namespace App {
	internal static class Program {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
		private static void Main() {
			var form = new RdpFormFactory().Create();
			
			form.Shown += async (sender, args) => {
				Directory.CreateDirectory("test");

				await form.Rdp.CheckAsync("test", "test");
			};

			Application.Run(form);
		}
	}
}
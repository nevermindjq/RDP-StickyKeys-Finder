using System;
using System.IO;
using System.Windows.Forms;

using App.Controls;
using App.Models.Abstractions;

using AxMSTSCLib;

namespace App.Models {
	public class RdpFormFactory : IFactory<RdpForm> {
		private const string _state64 = "AAEAAAD/////AQAAAAAAAAAMAgAAAFdTeXN0ZW0uV2luZG93cy5Gb3JtcywgVmVyc2lvbj00LjAuMC4wLCBDdWx0dXJlPW5ldXRyYWwsIFB1YmxpY0tleVRva2VuPWI3N2E1YzU2MTkzNGUwODkFAQAAACFTeXN0ZW0uV2luZG93cy5Gb3Jtcy5BeEhvc3QrU3RhdGUBAAAABERhdGEHAgIAAAAJAwAAAA8DAAAAKQAAAAIBAAAAAQAAAAAAAAAAAAAAABQAAAAACAAACAACAAAAAAALAAAACwAAAAs=";
		
		public RdpForm Create() {
			var form = new RdpForm();

			form.Rdp.BeginInit();
			
			// Rdp
			_SetOcxState(form.Rdp);
			form.Rdp.Location = new(0, 0);
			form.Rdp.Size = new(900, 500);
			
			// Form
			form.ClientSize = new(900, 500);
			form.Controls.Add(form.Rdp);
			
			form.Rdp.EndInit();

			return form;
		}

		private static void _SetOcxState(AxMsRdpClient8NotSafeForScripting rdp) {
			var buffer = Convert.FromBase64String(_state64);
			
			using (var memory = new MemoryStream()) {
				memory.Write(buffer, 0, buffer.Length);
				memory.Seek(0, SeekOrigin.Begin);

				rdp.OcxState = new(memory, 1, false, null);
			}
		}
	}
}
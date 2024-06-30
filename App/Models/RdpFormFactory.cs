using System;
using System.IO;
using System.Threading;

using App.Controls;

using AxMSTSCLib;

namespace App.Models {
	public unsafe class RdpFormFactory {
		private const string _state64 = "AAEAAAD/////AQAAAAAAAAAMAgAAAFdTeXN0ZW0uV2luZG93cy5Gb3JtcywgVmVyc2lvbj00LjAuMC4wLCBDdWx0dXJlPW5ldXRyYWwsIFB1YmxpY0tleVRva2VuPWI3N2E1YzU2MTkzNGUwODkFAQAAACFTeXN0ZW0uV2luZG93cy5Gb3Jtcy5BeEhvc3QrU3RhdGUBAAAABERhdGEHAgIAAAAJAwAAAA8DAAAAKQAAAAIBAAAAAQAAAAAAAAAAAAAAABQAAAAACAAACAACAAAAAAALAAAACwAAAAs=";
		
		public RdpForm Create(Settings settings, ManualResetEvent @event, bool with_nla, int* count, string directory) {
			var form = new RdpForm {
				Settings = settings,
				Event = @event,
				WithNLA = with_nla,
				Directory = directory,
				count = count
			};

			form.ClientSize = new(900, 500);

			return form;
		}

		public void InitializeRdp(RdpForm form) {
			form.Rdp = new();
            
			form.Rdp.BeginInit();
			
			// Rdp
			_SetOcxState(form.Rdp);
			form.Rdp.Location = new(0, 0);
			form.Rdp.Size = new(900, 500);
			
			form.Controls.Add(form.Rdp);
			
			form.Rdp.EndInit();
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
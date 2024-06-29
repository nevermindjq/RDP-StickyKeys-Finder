using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using App.Models;

using AxMSTSCLib;

namespace App.Extensions
{
    public static class RdpClientExtensions {
        public static async Task<bool> WaitConnectionAsync(this AxMsRdpClient8NotSafeForScripting rdp, int? timeout = null, CancellationToken token = default) {
            var is_infinity = timeout is null;

            if (is_infinity) {
                timeout = 0;
            }
            
            for (; is_infinity || timeout > 0; timeout -= 1000, await Task.Delay(1000, token)) {
                if (rdp.Connected == 0 || token.IsCancellationRequested) {
                    return false;
                }
                
                if (rdp.Connected == 1) {
                    return true;
                }
            }

            return false;
        }

        public static bool ConfigureConnection(this AxMsRdpClient8NotSafeForScripting rdp, string server, int? port) {
            var is_nla = port != null;

            rdp.Server = server;

            if (is_nla) {
                rdp.UserName = "";
                rdp.AdvancedSettings8.ClearTextPassword = "";
                
                rdp.AdvancedSettings8.AuthenticationLevel = 2;
                rdp.AdvancedSettings8.RDPPort = port.Value;
            }
            else {
                rdp.AdvancedSettings8.AuthenticationLevel = 0;
            }

            return is_nla;
        }

        public static async Task<bool> TryConnectAsync(this AxMsRdpClient8NotSafeForScripting rdp, string server, int? timeout = null, int? port = null) {
            if (rdp.ConfigureConnection(server, port)) {
                return false;
            }
            
            rdp.Connect();
            
            return timeout is null || await rdp.WaitConnectionAsync(timeout);
        }

        public static void ClickKeys(this AxMsRdpClient8NotSafeForScripting rdp) {
            rdp.Focus();

            for (int i = 0; i < 10; i++) {
                SendKeys.Send("+");
            }
        }

        public static void CreateScreenshot(this AxMsRdpClient8NotSafeForScripting rdp, string directory) {
            using (var map = new ScreenCapture().CaptureWindow(rdp.Handle)) {
                map.Save(Path.Combine(directory, $"{rdp.Server}.png"));
            }
        }
    }
}
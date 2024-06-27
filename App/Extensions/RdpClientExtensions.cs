using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

using App.Models;

using AxMSTSCLib;

namespace App.Extensions
{
    public static class RdpClientExtensions
    {
        public static async Task<bool> CheckAsync(this AxMsRdpClient8NotSafeForScripting rdp, string server, string directory, int? port = null) {
            if (!await rdp._TryConnectAsync(server, port: port)) {
                return false;
            }
            
            rdp.Focus();

            for (int i = 0; i < 10; i++)
            {
                SendKeys.Send("+");
            }

            await Task.Delay(3000);

            using (var map = new ScreenCapture().CaptureWindow(rdp.Handle)) {
                map.Save(Path.Combine(directory, $"{rdp.Server}.png"));
            }

            rdp.Disconnect();

            return await rdp._WaitAsync(x => x.Connected == 0);
        }
        
        // private
        private static async Task<bool> _WaitAsync(this AxMsRdpClient8NotSafeForScripting rdp, Predicate<AxMsRdpClient8NotSafeForScripting> predicate, int? timeout = null) {
            var is_infinity = timeout is null;

            if (is_infinity) {
                timeout = 0;
            }
            
            for (; is_infinity || timeout > 0; timeout -= 1000, await Task.Delay(1000)) {
                if (predicate.Invoke(rdp)) {
                    return true;
                }
            }

            return false;
        }

        private static async Task<bool> _TryConnectAsync(this AxMsRdpClient8NotSafeForScripting rdp, string server, int timeout = 6000, int? port = null) {
            var is_nla = port != null;
            
            rdp.Server = server;

            if (is_nla) {
                rdp.AdvancedSettings8.AuthenticationLevel = 2;
                rdp.AdvancedSettings8.RDPPort = port.Value;
            }
            else {
                rdp.AdvancedSettings8.AuthenticationLevel = 0;
            }
            
            rdp.Connect();

            if (is_nla) {
                await Task.Delay(1000);
                
                SendKeys.Send("{LEFT}");
                SendKeys.Send("{ENTER}");
            }

            return await rdp._WaitAsync(x => x.Connected == 1, timeout);
        }
    }
}
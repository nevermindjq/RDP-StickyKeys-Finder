using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using AxMSTSCLib;

namespace App.Extensions
{
    public static class RdpClientExtensions
    {
        public static async Task<bool> CheckAsync(this AxMsRdpClient8NotSafeForScripting rdp, string server, Point window_position, string directory, int? port = null) {
            if (!await rdp._TryConnectAsync(server, port: port)) {
                return false;
            }
            
            rdp.Focus();

            for (int i = 0; i < 10; i++)
            {
                SendKeys.Send("+");
            }

            await Task.Delay(3000);

            var margin = new Point {
                X = rdp.Margin.Left + rdp.Margin.Right,
                Y = rdp.Margin.Top + rdp.Margin.Bottom
            };

            using (var map = new Bitmap(rdp.Width - margin.X, rdp.Height - margin.Y)) {
                using (var graphics = Graphics.FromImage(map)) {
                    graphics.CopyFromScreen(
                        window_position.X + rdp.Bounds.X - margin.X,
                        window_position.Y + rdp.Bounds.Y - margin.Y,
                        0,
                        0,
                        map.Size,
                        CopyPixelOperation.SourceCopy
                    );
                }

                map.Save(Path.Combine(directory, $"{rdp.Server}.png"));
            }

            rdp.Disconnect();

            return await rdp._WaitAsync(x => x.Connected == 2);
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
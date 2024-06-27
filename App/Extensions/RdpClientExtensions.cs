using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using AxMSTSCLib;

namespace App.Extensions
{
    public static class RdpClientExtensions
    {
        public static async Task<bool> Check(this AxMsRdpClient8NotSafeForScripting rdp, string server, Point window_position, string directory, int? port = null) {
            rdp.Server = server;

            if (port is not null) {
                rdp.AdvancedSettings8.RDPPort = port.Value;
            }

            rdp.Connect();

            await Task.Delay(3000);

            if (rdp.Connected != 1) {
                return false;
            }
            
            rdp.Focus();

            for (int i = 0; i < 10; i++)
            {
                SendKeys.Send("+");
            }

            await Task.Delay(1000);

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

            await Task.Delay(2000);

            return true;
        }
    }
}
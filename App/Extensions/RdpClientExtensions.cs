using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

using App.Models;

using AxMSTSCLib;

using MSTSCLib;

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
                //rdp.UserName = "username";
                //var secure = (IMsRdpClientNonScriptable)rdp.GetOcx();
                //secure.ClearTextPassword = "password";
                //
                //rdp.AdvancedSettings8.AuthenticationLevel = 2;
                //rdp.AdvancedSettings8.EnableCredSspSupport = true;
                rdp.AdvancedSettings8.RDPPort = port.Value;
            }
            else {
                rdp.AdvancedSettings8.AuthenticationLevel = 0;
            }

            return is_nla;
        }

        public static async Task<bool> TryConnectAsync(this AxMsRdpClient8NotSafeForScripting rdp, int? timeout = null) {
            try {
                rdp.Connect();
            }
            catch {
                return false;
            }
            
            return timeout is null || await rdp.WaitConnectionAsync(timeout);
        }

        public static void ClickKeys(this AxMsRdpClient8NotSafeForScripting rdp) {
            rdp.Focus();

            var ocx = (IMsRdpClientNonScriptable_Sendkeys)rdp.GetOcx();

            var codes = new int[10];
            var realised = new int[10];
            var unrealised = new int[10];

            for (int i = 0; i < 10; i++) {
                codes[i] = 0x2A;
                realised[i] = 1;
                unrealised[i] = 0;
            }
            
            unsafe {
                try {
                    fixed (int* pScanCodes = codes) {
                        fixed (int* pKeyReleased = realised) {
                            ocx.SendKeys(codes.Length, pKeyReleased, pScanCodes);
                        }

                        fixed (int* pKeyUnrealised = unrealised) {
                            ocx.SendKeys(codes.Length, pKeyUnrealised, pScanCodes);
                        }
                    }
                }
                catch {
                    // ignore
                }
            }
        }

        public static void CreateScreenshot(this AxMsRdpClient8NotSafeForScripting rdp, string directory) {
            using (var map = new ScreenCapture().CaptureWindow(rdp.Handle)) {
                map.Save(Path.Combine(directory, $"{rdp.Server}.png"));
            }
        }
        
        #region Inline Interface Definition
        [InterfaceType(1)]
        [Guid("2F079C4C-87B2-4AFD-97AB-20CDB43038AE")]
        private interface IMsRdpClientNonScriptable_Sendkeys : IMsTscNonScriptable
        {
#pragma warning disable CS0108
            [DispId(4)] string BinaryPassword { get; set; }
            [DispId(5)] string BinarySalt { get; set; }
            [DispId(1)] string ClearTextPassword { set; }
            [DispId(2)] string PortablePassword { get; set; }
            [DispId(3)] string PortableSalt { get; set; }

            void NotifyRedirectDeviceChange(uint wParam, int lParam);
            void ResetPassword();

#pragma warning restore CS0108

            unsafe void SendKeys(int numKeys, int *pbArrayKeyUp, int *plKeyData);
        }
        #endregion
    }
}
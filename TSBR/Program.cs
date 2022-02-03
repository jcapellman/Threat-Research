using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;

namespace TSBR
{
    internal static class Program
    {
        private static async void Swifted()
        {
            var httpClient = new HttpClient();

            var content = await httpClient.GetByteArrayAsync("https://cwg.io/bg.mid");

            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Swift.midi");

            File.WriteAllBytes(path, content);

            new Process
            {
                StartInfo = new ProcessStartInfo(path)
                {
                    UseShellExecute = true
                }
            }.Start();

            Thread.Sleep(1000);
        }

        static void Main()
        {
            Swifted();
        }
    }
}
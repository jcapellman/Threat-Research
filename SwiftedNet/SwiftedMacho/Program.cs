using System.Diagnostics;

namespace SwiftedMacho
{
    class Program
    {
        private static readonly string[] urls = new[] { "https://www.taylorswift.com", "https://www.youtube.com/watch?v=3tmd-ClpJxA", "https://www.youtube.com/watch?v=e-ORhEE9VVg", "https://www.youtube.com/watch?v=VuNIsY6JdUw", "https://www.youtube.com/watch?v=w1oM3kQpXRo", "https://www.youtube.com/watch?v=FuXNumBwDOM", "https://www.youtube.com/watch?v=WA4iX5D9Z64", "https://www.youtube.com/watch?v=8xg3vE8Ie_E", "https://www.youtube.com/watch?v=wIft-t-MQuE" };
        private const int MIN_BROWSERS = 1;
        private const int MAX_BROWSERS = 20;

        private const int MIN_TEXTFILES = 5;
        private const int MAX_TEXTFILES = 50;

        private const int DELAY_MINUTES = 1;

        private static void OpenBrowser(string url)
        {
            var psi = new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            };

            Process.Start(psi);
        }

        static void Main(string[] args)
        {
            if (args is null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            var rand = new Random(DateTime.Now.Day);

            while (true)
            {
                var numInstances = rand.Next(MIN_BROWSERS, MAX_BROWSERS);

                var urlRand = new Random((int)DateTime.Now.Ticks);

                for (var x = 0; x < numInstances; x++)
                {
                    var index = urlRand.Next(0, urls.Length - 1);

                    OpenBrowser(urls[index]);
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
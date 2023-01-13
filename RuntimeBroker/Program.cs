using System.Reflection;
using System.Runtime.InteropServices;

namespace RuntimeBroker
{
    internal class Program
    {
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern Int32 SystemParametersInfo(UInt32 uiAction, UInt32 uiParam, String pvParam, UInt32 fWinIni);

        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", EntryPoint = "SendMessage", SetLastError = true)]
        static extern IntPtr SendMessage(IntPtr hWnd, Int32 Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

        private static UInt32 SPI_SETDESKWALLPAPER = 20;
        private static UInt32 SPIF_UPDATEINIFILE = 0x1;

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
            SetWindowPos(window, IntPtr.Zero, 0, 0, 0, 0, (uint)SetWindowPosFlags.HideWindow);

            var filePath = Path.GetTempFileName();

            WriteResourceToFile("RuntimeBroker.wallpaper.jpg", filePath);

            SystemParametersInfo(SPI_SETDESKWALLPAPER, 1, filePath, SPIF_UPDATEINIFILE);

            var lHwnd = FindWindow("Shell_TrayWnd", null);
            SendMessage(lHwnd, WM_COMMAND, (IntPtr)MIN_ALL, IntPtr.Zero);
        }
    }
}
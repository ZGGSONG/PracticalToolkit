using System.Runtime.InteropServices;

namespace PracticalToolkit.WPF.Controls;

public static class ScreenExtensions
{
    public static void GetDpi(this Screen screen, DpiType dpiType, out uint dpiX, out uint dpiY)
    {
        GetDpiForMonitor(MonitorFromPoint(new Point(screen.Bounds.Left + 1, screen.Bounds.Top + 1), 2u), dpiType, out dpiX, out dpiY);
    }

    [DllImport("user32.dll")]
    private static extern IntPtr MonitorFromPoint([In] Point pt, [In] uint dwFlags);

    [DllImport("Shcore.dll")]
    private static extern IntPtr GetDpiForMonitor([In] IntPtr hmonitor, [In] DpiType dpiType, out uint dpiX, out uint dpiY);
}
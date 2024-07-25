using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace PracticalToolkit.Screenshot;

public static class Extensions
{
    /// <summary>
    ///     将 BitmapSource 转换为 Bitmap。
    /// </summary>
    /// <param name="source">要转换的 BitmapSource。</param>
    /// <returns>转换后的 Bitmap，如果转换失败则为 null。</returns>
    public static Bitmap? ToBitmap(this BitmapSource? source)
    {
        try
        {
            if (source == null) return default;

            using var stream = new MemoryStream();
            var encoder = new BmpBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(source));
            encoder.Save(stream);
            stream.Seek(0, SeekOrigin.Begin);
            var bitmap = new Bitmap(stream);
            return bitmap;
        }
        catch
        {
            return default;
        }
    }

    /// <summary>
    ///     将 Bitmap 转换为 BitmapSource。
    /// </summary>
    /// <param name="bitmap">要转换的 Bitmap。</param>
    /// <returns>转换后的 BitmapSource，如果转换失败则为 null。</returns>
    public static BitmapSource? ToBitmapSource(this Bitmap? bitmap)
    {
        if (bitmap == null) return default;

        var ptr = bitmap.GetHbitmap();
        BitmapSource? bitmapSource;
        try
        {
            bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(ptr, IntPtr.Zero, Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
        }
        finally
        {
            DeleteObject(ptr);
        }

        return bitmapSource;
    }

    #region Win32 Api

    /// <summary>
    ///     从内存中删除一个逻辑笔、逻辑刷、字体、位图、区域或者调色板，释放所有与这些对象相关的系统资源。在对象被删除之后，指定的句柄不再是合法的。
    /// </summary>
    /// <param name="hObject">指向要删除的 GDI 对象的句柄。</param>
    /// <returns></returns>
    [DllImport("gdi32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool DeleteObject(IntPtr hObject);

    #endregion
}
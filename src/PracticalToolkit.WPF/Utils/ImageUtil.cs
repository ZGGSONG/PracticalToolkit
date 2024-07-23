using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Application = System.Windows.Application;

namespace PracticalToolkit.WPF.Utils;

public static class ImageUtil
{
    private static bool InvokeRequired => Dispatcher.CurrentDispatcher != Application.Current.Dispatcher;

    public static Bitmap ToBitmap(this BitmapSource bitmapSource)
    {
        using var memoryStream = new MemoryStream();
        BitmapEncoder encoder = new BmpBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
        encoder.Save(memoryStream);
        memoryStream.Seek(0, SeekOrigin.Begin);

        var bitmap = new Bitmap(memoryStream);
        return new Bitmap(bitmap);
    }

    public static BitmapSource? CreateBitmapSourceFromBitmap(Bitmap bitmap)
    {
        if (bitmap is null) throw new ArgumentNullException(nameof(bitmap));
        if (Application.Current.Dispatcher == null)
        {
            return null;
        }
        try
        {
            using MemoryStream memoryStream = new MemoryStream();
            bitmap.Save(memoryStream, ImageFormat.Png);
            memoryStream.Seek(0L, SeekOrigin.Begin);
            if (InvokeRequired)
            {
                return (BitmapSource)Application.Current.Dispatcher.Invoke(new Func<Stream, BitmapSource>(CreateBitmapSourceFromBitmap), DispatcherPriority.Normal, memoryStream);
            }
            return CreateBitmapSourceFromBitmap(memoryStream);
        }
        catch (Exception)
        {
            return null;
        }
    }

    private static BitmapSource CreateBitmapSourceFromBitmap(Stream stream)
    {
        WriteableBitmap writeableBitmap = new WriteableBitmap(BitmapDecoder.Create(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad).Frames.Single());
        writeableBitmap.Freeze();
        return writeableBitmap;
    }
}
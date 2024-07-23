using System.Windows.Media.Imaging;

namespace PracticalToolkit.WPF.Controls;

public class ScreenShotCapture
{
    private readonly List<ScreenShot> _screenShots = [];

    public Action? OnCanceled;
    public Action<BitmapSource>? OnCompleted;

    public ScreenShotCapture()
    {
        for (var i = 0; i < Screen.AllScreens.Length; i++)
        {
            var item = CaptureScreen(i);
            _screenShots.Add(item);
        }
    }

    public void Capture()
    {
        foreach (var screenShot in _screenShots)
        {
            screenShot.Show();
            screenShot.Activate();
        }
    }

    private ScreenShot CaptureScreen(int index)
    {
        var screenShot = new ScreenShot(index);
        screenShot.CutCompleted += ScreenCut_CutCompleted;
        screenShot.CutCanceled += ScreenCut_CutCanceled;
        screenShot.Closed += ScreenCut_Closed;
        return screenShot;
    }

    private void ScreenCut_CutCompleted(CroppedBitmap bitmap)
    {
        OnCompleted?.Invoke(bitmap);
    }

    private void ScreenCut_CutCanceled()
    {
        OnCanceled?.Invoke();
    }

    private void ScreenCut_Closed(object? sender, EventArgs e)
    {
        if (sender is not ScreenShot screenShot) return;
        if (_screenShots.Contains(screenShot)) _screenShots.Remove(screenShot);
        CloseCutters();
        ScreenShot.ClearCaptureScreenId();
    }

    private void CloseCutters()
    {
        if (_screenShots.Count == 0) return;
        while (_screenShots.Count > 0) _screenShots[0].Close();
        _screenShots.Clear();
    }
}
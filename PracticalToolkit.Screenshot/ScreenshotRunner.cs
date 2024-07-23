using System.Diagnostics;
using System.Drawing.Drawing2D;

namespace PracticalToolkit.Screenshot;

/// <summary>
///     提供屏幕截图功能
///     <see href="https://github.com/tjden88/ScreenshotCreator" />
/// </summary>
public class ScreenshotRunner : IDisposable
{
    /// <summary>
    ///     释放由 ScreenshotRunner 使用的所有资源。
    /// </summary>
    public void Dispose()
    {
        _frame = null;
        _screenshotHost?.Dispose();
        _screenshotHost = null;
        GC.SuppressFinalize(this);
    }

    #region 字段

    private PictureBox? _frame;

    private Form? _screenshotHost;

    private Point _p1, _p2;

    private bool _isDrawing;

    #endregion

    #region 属性

    /// <summary>
    ///     获取或设置截图窗口的不透明度。
    /// </summary>
    public double Opacity { get; set; } = 0.3;

    /// <summary>
    ///     获取或设置截图窗口的背景颜色。
    /// </summary>
    public Color Background { get; set; } = Color.Black;

    /// <summary>
    ///     获取或设置截图窗口的透明色。
    /// </summary>
    public Color TransparencyKeyColor { get; set; } = Color.Yellow;

    #endregion

    #region 截图方法

    /// <summary>
    ///     获取所有屏幕的边界。
    /// </summary>
    /// <returns>表示所有屏幕边界的矩形</returns>
    public static Rectangle GetBounds()
    {
        int minX = int.MaxValue, minY = int.MaxValue, maxX = 0, maxY = 0;
        foreach (var scr in Screen.AllScreens)
        {
            minX = Math.Min(minX, scr.Bounds.X);
            minY = Math.Min(minY, scr.Bounds.Y);
            maxX = Math.Max(maxX, scr.Bounds.Right);
            maxY = Math.Max(maxY, scr.Bounds.Bottom);
        }

        return new Rectangle(minX, minY, maxX - minX, maxY - minY);
    }

    /// <summary>
    ///     获取用户定义区域的屏幕截图。
    /// </summary>
    /// <returns>用户定义区域的屏幕截图 <see cref="Bitmap" /></returns>
    public Bitmap? Screenshot()
    {
        _frame = new PictureBox
        {
            BackColor = TransparencyKeyColor,
            BorderStyle = BorderStyle.FixedSingle,
            Size = new Size(0, 0),
            Visible = false
        };

        var displayRect = GetBounds();
        _screenshotHost = new Form
        {
            ShowInTaskbar = false,
            FormBorderStyle = FormBorderStyle.None,
            Opacity = Opacity,
            BackColor = Background,
            TransparencyKey = TransparencyKeyColor,
            KeyPreview = true,
            Cursor = Cursors.Cross
        };
        _screenshotHost.Controls.Add(_frame);
        _screenshotHost.MouseDown += ScreenshotHost_MouseDown;
        _screenshotHost.MouseMove += ScreenshotHost_MouseMove;
        _screenshotHost.MouseUp += ScreenshotHost_MouseUp;
        _screenshotHost.KeyUp += ScreenshotHost_KeyUp;
        _screenshotHost.Load += (_, _) =>
        {
            _screenshotHost.Location = displayRect.Location;
            _screenshotHost.Size = displayRect.Size;
            _screenshotHost.TopMost = true;
        };

        _screenshotHost.Activated += (_, _) =>
        {
            _screenshotHost.BringToFront();
            _screenshotHost.Select();
            _screenshotHost.Focus();
            Debug.WriteLine("Activated");
        };
        _screenshotHost.Deactivate += (_, _) => Debug.WriteLine("Deactivated");

        if (_screenshotHost.ShowDialog() != DialogResult.OK)
            return null;
        try
        {
            _screenshotHost.Opacity = 0;
            _frame.BorderStyle = BorderStyle.None;
            Bitmap bmp = new(_frame.Width, _frame.Height);
            var g = Graphics.FromImage(bmp);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.CopyFromScreen(_screenshotHost.Left + _frame.Left, _screenshotHost.Top + _frame.Top, 0, 0, _frame.Size);
            return bmp;
        }
        catch (Exception)
        {
            return null;
        }
    }

    /// <summary>
    ///     获取全屏幕截图。
    /// </summary>
    /// <returns>全屏幕截图 <see cref="Bitmap" /></returns>
    public Bitmap? ScreenshotAll()
    {
        var displayRect = GetBounds();
        try
        {
            Bitmap bmp = new(displayRect.Width, displayRect.Height);
            var g = Graphics.FromImage(bmp);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.CopyFromScreen(displayRect.Left, displayRect.Top, 0, 0, displayRect.Size);
            return bmp;
        }
        catch (Exception)
        {
            return null;
        }
    }

    /// <summary>
    ///     异步获取用户定义区域的屏幕截图。
    /// </summary>
    /// <returns>用户定义区域的屏幕截图 <see cref="Bitmap" /></returns>
    public async Task<Bitmap?> ScreenshotAsync()
    {
        return await Task.Run(Screenshot);
    }

    /// <summary>
    ///     异步获取全屏幕截图。
    /// </summary>
    /// <returns>全屏幕截图 <see cref="Bitmap" /></returns>
    public async Task<Bitmap?> ScreenshotAllAsync()
    {
        return await Task.Run(ScreenshotAll);
    }

    #endregion

    #region 鼠标

    private void ScreenshotHost_KeyUp(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Escape && _screenshotHost != null)
            _screenshotHost.DialogResult = DialogResult.Cancel;
    }

    private void ScreenshotHost_MouseDown(object? sender, MouseEventArgs e)
    {
        if (_frame == null) return;
        _frame.Location = e.Location;
        _p1 = e.Location;
        _p2 = e.Location;
        _frame.Visible = true;
        _isDrawing = true;
    }

    private void ScreenshotHost_MouseMove(object? sender, MouseEventArgs e)
    {
        if (!_isDrawing || _frame == null) return;
        _p2 = e.Location;
        _frame.Location = new Point(Math.Min(_p1.X, _p2.X), Math.Min(_p1.Y, _p2.Y));
        _frame.Size = new Size(Math.Max(_p1.X, _p2.X) - _frame.Location.X, Math.Max(_p1.Y, _p2.Y) - _frame.Location.Y);
    }

    private void ScreenshotHost_MouseUp(object? sender, MouseEventArgs e)
    {
        if (_screenshotHost == null) return;
        _screenshotHost.DialogResult = DialogResult.OK;
    }

    #endregion
}
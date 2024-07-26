using System.Drawing.Drawing2D;

namespace PracticalToolkit.Screenshot;

/// <summary>
///     提供屏幕截图功能
///     * 修改自 <see href="https://github.com/tjden88/ScreenshotCreator" />
///     * 根据自身需求做了优化和功能增加
/// </summary>
public class ScreenshotRunner(RunnerOptions options) : IDisposable
{
    public ScreenshotRunner() : this(new RunnerOptions())
    {
    }

    #region <字段>

    private ZForm? _backHostForm;
    private Form? _screenshotHost;
    private ZPictureBox? _selectFrame;
    private ZPictureBox? _magnifier;
    private Point _startPoint, _endPoint;
    private Bitmap? _magnifierBitmap;
    private Bitmap? _actualMagnifierBitmap;
    private bool _isDrawing;
    private bool _disposed;

    #endregion
    
    #region <属性>

    /// <summary>
    ///     获取或设置截图窗口的背景颜色。
    /// </summary>
    private static Color Background => Color.Black;

    /// <summary>
    ///     获取或设置截图窗口的透明色。
    /// </summary>
    private static Color TransparencyKeyColor => Color.Yellow;

    private static int MagnifierWidth => 180;
    private static int MagnifierHeight => 150;

    #endregion

    #region <截图>

    /// <summary>
    ///     获取所有屏幕的边界。
    /// </summary>
    /// <returns>表示所有屏幕边界的矩形</returns>
    private static Rectangle GetBounds()
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
        if (_isDrawing) return default;

        var displayRect = GetBounds();

        _backHostForm = new ZForm(displayRect.Location, displayRect.Size);
        // 绘制背景
        using var backgroundBitmap = new Bitmap(displayRect.Width, displayRect.Height);
        using var graphics = Graphics.FromImage(backgroundBitmap);
        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        graphics.CopyFromScreen(displayRect.X, displayRect.Y, 0, 0, new Size(displayRect.Width, displayRect.Height));
        _backHostForm.BackgroundImage = backgroundBitmap;
        _backHostForm.Show();

        _selectFrame = new ZPictureBox(options.BorderColor, options.IsDrawBorder)
        {
            BackColor = TransparencyKeyColor,
            Size = new Size(0, 0),
            Visible = false
        };

        _screenshotHost = new Form
        {
            ShowInTaskbar = false,
            FormBorderStyle = FormBorderStyle.None,
            Opacity = options.Opacity,
            BackColor = Background,
            TransparencyKey = TransparencyKeyColor,
            KeyPreview = true,
            Cursor = Cursors.Cross
        };

        // 配置是否启用放大镜
        if (options.IsDrawMagnifier)
        {
            _magnifier = new ZPictureBox(options.BorderColor, options.IsDrawBorder)
            {
                Size = new Size(MagnifierWidth, MagnifierHeight), // 放大镜的大小
                Location = Control.MousePosition
            };
            _screenshotHost.Controls.Add(_magnifier);
        }

        _screenshotHost.Controls.Add(_selectFrame);
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
        };

        if (_screenshotHost.ShowDialog() != DialogResult.OK)
            return default;
        try
        {
            _screenshotHost.Opacity = 0;
            _selectFrame.BorderStyle = BorderStyle.None;
            Bitmap bmp = new(_selectFrame.Width, _selectFrame.Height);
            using var g = Graphics.FromImage(bmp);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            // 将背景图像中的选定区域绘制到新位图上
            var sourceRect =
                new Rectangle(_selectFrame.Left, _selectFrame.Top, _selectFrame.Width, _selectFrame.Height);
            var destRect = new Rectangle(0, 0, _selectFrame.Width, _selectFrame.Height);
            g.DrawImage(_backHostForm.BackgroundImage, destRect, sourceRect, GraphicsUnit.Pixel);
            _backHostForm.Opacity = 0;
            _backHostForm.Close();
            return bmp;
        }
        catch
        {
            return default;
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
            using var g = Graphics.FromImage(bmp);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.CopyFromScreen(displayRect.Left, displayRect.Top, 0, 0, displayRect.Size);
            return bmp;
        }
        catch
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

    #region <鼠标>

    private void ScreenshotHost_KeyUp(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Escape && _screenshotHost != null)
            _screenshotHost.DialogResult = DialogResult.Cancel;
    }

    private void ScreenshotHost_MouseDown(object? sender, MouseEventArgs e)
    {
        if (_selectFrame == null) return;
        _selectFrame.Location = e.Location;
        _startPoint = e.Location;
        _selectFrame.Visible = true;
        _isDrawing = true;
    }

    private void ScreenshotHost_MouseMove(object? sender, MouseEventArgs e)
    {
        // 更新放大镜位置和图像
        if (_magnifier != null)
        {
            _magnifier.Location = new Point(e.X + 20, e.Y + 20); // 放大镜位置稍微偏离鼠标位置
            UpdateMagnifierImage(e.Location);
        }

        if (!_isDrawing || _selectFrame == null) return;
        _endPoint = e.Location;
        _selectFrame.Location = new Point(Math.Min(_startPoint.X, _endPoint.X), Math.Min(_startPoint.Y, _endPoint.Y));
        _selectFrame.Size = new Size(Math.Max(_startPoint.X, _endPoint.X) - _selectFrame.Location.X,
            Math.Max(_startPoint.Y, _endPoint.Y) - _selectFrame.Location.Y);
        _selectFrame.Invalidate(); // 通知窗体重绘边框
    }

    private void ScreenshotHost_MouseUp(object? sender, MouseEventArgs e)
    {
        if (!_isDrawing) return;
        _isDrawing = false;
        if (_screenshotHost == null) return;
        _screenshotHost.DialogResult = DialogResult.OK;
    }

    /// <summary>
    ///     更新放大镜图像
    /// </summary>
    /// <param name="screenPoint"></param>
    private void UpdateMagnifierImage(Point screenPoint)
    {
        if (_magnifier == null || _backHostForm?.BackgroundImage is not { } img) return;

        const int zoomFactor = 1; // 放大倍数
        var bitmapWidth = MagnifierWidth / 3; // 放大镜位图的实际宽度
        var bitmapHeight = MagnifierHeight / 3; // 放大镜位图的实际高度
        _actualMagnifierBitmap ??= new Bitmap(bitmapWidth, bitmapHeight);
        using (var g = Graphics.FromImage(_actualMagnifierBitmap))
        {
            // 使用高质量的图像插值模式
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            var sourceRect = new Rectangle(
                screenPoint.X - bitmapWidth / (2 * zoomFactor),
                screenPoint.Y - bitmapHeight / (2 * zoomFactor),
                bitmapWidth / zoomFactor,
                bitmapHeight / zoomFactor);
            g.DrawImage(img, new Rectangle(0, 0, bitmapWidth, bitmapHeight), sourceRect, GraphicsUnit.Pixel);

            // 绘制十字标记
            using var crossPen = new Pen(options.BorderColor, 3);
            g.DrawLine(crossPen, bitmapWidth / 2, 0, bitmapWidth / 2, bitmapHeight); // 垂直线
            g.DrawLine(crossPen, 0, bitmapHeight / 2, bitmapWidth, bitmapHeight / 2); // 水平线
        }

        // 将放大镜位图设置为PictureBox的图像，并进行缩放
        _magnifierBitmap ??= new Bitmap(MagnifierWidth, MagnifierHeight);
        using (var gMagnifier = Graphics.FromImage(_magnifierBitmap))
        {
            gMagnifier.DrawImage(_actualMagnifierBitmap, 0, 0, MagnifierWidth, MagnifierHeight);
        }

        _magnifier.Image = _magnifierBitmap;
    }

    #endregion

    #region <释放>

    public void Dispose()
    {
        Dispose(true);
        // 告诉GC在显示调用Dispose时不调用Finalize方法(析构方法)
        GC.SuppressFinalize(this);
    }

    ~ScreenshotRunner()
    {
        Dispose(false);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            // 释放托管资源
            _selectFrame?.Dispose();
            _screenshotHost?.Dispose();
            _backHostForm?.Dispose();
            _magnifier?.Dispose();
            _magnifierBitmap?.Dispose();
            _actualMagnifierBitmap?.Dispose();
            _magnifierBitmap = null;
            _actualMagnifierBitmap = null;
        }

        // 释放非托管资源

        _disposed = true;
    }

    #endregion

    #region <控件>

    /// <summary>
    ///     自定义Form作为背景控件
    /// </summary>
    private sealed class ZForm : Form
    {
        public ZForm(Point location, Size size)
        {
            // 启用双缓冲以减少或消除闪烁
            DoubleBuffered = true;

            ShowInTaskbar = false;
            TopMost = true;
            FormBorderStyle = FormBorderStyle.None;

            Load += (_, _) =>
            {
                Location = location;
                Size = size;
                BringToFront();
            };
        }
    }

    private class ZPictureBox : PictureBox
    {
        private readonly Pen? _borderPen;

        public ZPictureBox(Color borderColor, bool isDrawBorder = false)
        {
            if (!isDrawBorder) return;
            _borderPen = new Pen(borderColor, 5);
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
            if (_borderPen is null) return;
            // 使用Graphics对象绘制边框
            pe.Graphics.DrawRectangle(_borderPen, 1, 1, Width - 3, Height - 3);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _borderPen != null)
                _borderPen.Dispose();
            base.Dispose(disposing);
        }
    }

    #endregion
}

public class RunnerOptions(bool isDrawBorder = false, bool isDrawMagnifier = false, double opacity = 0.4, Color borderColor = default)
{
    public bool IsDrawBorder { get; set; } = isDrawBorder;
    public bool IsDrawMagnifier { get; set; } = isDrawMagnifier;
    public double Opacity { get; set; } = opacity;
    public Color BorderColor { get; set; } = borderColor == default ? Color.FromArgb(32, 128, 240) : borderColor;
}
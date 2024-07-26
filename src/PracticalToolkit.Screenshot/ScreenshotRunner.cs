using System.Drawing.Drawing2D;

namespace PracticalToolkit.Screenshot;

/// <summary>
///     提供屏幕截图功能
///     * 修改自 <see href="https://github.com/tjden88/ScreenshotCreator" />
///     * 根据自身需求做了修改
/// </summary>
public class ScreenshotRunner(RunnerOptions options) : IDisposable
{
    public ScreenshotRunner() : this(new RunnerOptions())
    {
    }

    #region <字段>

    private BackForm? _backHostForm;
    private CustomLabel? _selectFrame;
    private PictureBox? _magnifier;
    private Point _startPoint, _endPoint;
    private bool _isDrawing;

    private bool _disposed;

    #endregion

    #region <属性>

    /// <summary>
    ///     边框颜色
    /// </summary>
    private Color BorderColor { get; set; } = Color.FromArgb(32, 128, 240);

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
        var displayRect = GetBounds();

        _backHostForm = new BackForm(displayRect.Location, displayRect.Size);
        // 绘制背景
        using var backgroundBitmap = new Bitmap(displayRect.Width, displayRect.Height);
        using var graphics = Graphics.FromImage(backgroundBitmap);
        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        graphics.CopyFromScreen(displayRect.X, displayRect.Y, 0, 0, new Size(displayRect.Width, displayRect.Height));
        _backHostForm.BackgroundImage = backgroundBitmap;
        // _backHostForm.Show();

        _selectFrame = new()
        {
            BackColor = Color.Transparent,
            Size = new Size(0, 0),
            Visible = false
        };

        // 配置是否启用放大镜
        if (options.IsDrawMagnifier)
        {
            _magnifier = new()
            {
                Size = new Size(150, 150), // 放大镜的大小
                Location = Control.MousePosition
            };
            _backHostForm.Controls.Add(_magnifier);
        }

        _backHostForm.Controls.Add(_selectFrame);
        _backHostForm.MouseDown += ScreenshotHost_MouseDown;
        _backHostForm.MouseMove += ScreenshotHost_MouseMove;
        _backHostForm.MouseUp += ScreenshotHost_MouseUp;
        _backHostForm.KeyUp += ScreenshotHost_KeyUp;

        if (_backHostForm.ShowDialog() != DialogResult.OK)
            return null;
        try
        {
            Bitmap bmp = new(_selectFrame.Width, _selectFrame.Height);
            using var g = Graphics.FromImage(bmp);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            // 将背景图像中的选定区域绘制到新位图上
            var sourceRect = new Rectangle(_selectFrame.Left, _selectFrame.Top, _selectFrame.Width, _selectFrame.Height);
            var destRect = new Rectangle(0, 0, _selectFrame.Width, _selectFrame.Height);
            g.DrawImage(_backHostForm.BackgroundImage, destRect, sourceRect, GraphicsUnit.Pixel);
            _backHostForm.Opacity = 0;
            return bmp;
        }
        catch
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
        if (e.KeyCode == Keys.Escape && _backHostForm != null)
            _backHostForm.DialogResult = DialogResult.Cancel;
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
        
        // 仅在位置或大小发生变化时才更新
        var newLocation = new Point(Math.Min(_startPoint.X, _endPoint.X), Math.Min(_startPoint.Y, _endPoint.Y));
        var newSize = new Size(Math.Max(_startPoint.X, _endPoint.X) - _selectFrame.Location.X, Math.Max(_startPoint.Y, _endPoint.Y) - _selectFrame.Location.Y);

        if (_selectFrame.Location == newLocation && _selectFrame.Size == newSize) return;
        _selectFrame.Location = newLocation;
        _selectFrame.Size = newSize;
        _selectFrame.Invalidate(); // 通知窗体重绘边框
    }

    private void ScreenshotHost_MouseUp(object? sender, MouseEventArgs e)
    {
        if (!_isDrawing) return;
        _isDrawing = false;
        if (_backHostForm == null) return;
        _backHostForm.DialogResult = DialogResult.OK;
    }

    /// <summary>
    ///     更新放大镜图像
    /// </summary>
    /// <param name="screenPoint"></param>
    private void UpdateMagnifierImage(Point screenPoint)
    {
        if (_magnifier == null || _backHostForm?.BackgroundImage is not { } img) return;

        const int magnifierSize = 150; // 放大镜的显示大小
        const int bitmapSize = 50; // 放大镜位图的实际大小
        const int zoomFactor = 2; // 放大倍数
        using var tBitmap = new Bitmap(bitmapSize, bitmapSize);
        using (var g = Graphics.FromImage(tBitmap))
        {
            // 使用高质量的图像插值模式
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            var sourceRect = new Rectangle(screenPoint.X - bitmapSize / (2 * zoomFactor),
                screenPoint.Y - bitmapSize / (2 * zoomFactor), bitmapSize / zoomFactor,
                bitmapSize / zoomFactor);
            g.DrawImage(img, new Rectangle(0, 0, bitmapSize, bitmapSize), sourceRect, GraphicsUnit.Pixel);

            // 绘制蓝色十字标记
            var crossPen = new Pen(BorderColor, 3);
            g.DrawLine(crossPen, bitmapSize / 2, 0, bitmapSize / 2, bitmapSize); // 垂直线
            g.DrawLine(crossPen, 0, bitmapSize / 2, bitmapSize, bitmapSize / 2); // 水平线

            // 绘制蓝色边框
            var borderPen = new Pen(Color.Black, 1);
            g.DrawRectangle(borderPen, 0, 0, bitmapSize - 1, bitmapSize - 1); // 绘制边框
        }

        // 将放大镜位图设置为PictureBox的图像，并进行缩放
        _magnifier.Image = new Bitmap(tBitmap, magnifierSize, magnifierSize);
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
            _backHostForm?.Dispose();
            _magnifier?.Dispose();
        }

        // 释放非托管资源

        _disposed = true;
    }

    #endregion
}

/// <summary>
///     自定义Form作为背景控件
/// </summary>
public sealed class BackForm : Form
{
    public BackForm(Point location, Size size)
    {
        // 启用双缓冲以减少或消除闪烁
        DoubleBuffered = true;

        ShowInTaskbar = false;
        TopMost = true;
        FormBorderStyle = FormBorderStyle.None;

        KeyPreview = true;
        Cursor = Cursors.Cross;

        Load += (_, _) =>
        {
            Location = location;
            Size = size;
            TopMost = true;
        };
        
        Activated += (_, _) =>
        {
            BringToFront();
            Select();
            Focus();
        };
    }
}

public sealed class CustomLabel : Label
{
    private Pen? _borderPen = new(Color.FromArgb(32, 128, 240), 5);

    public Pen? BorderPen
    {
        get => _borderPen;
        set
        {
            _borderPen = value;
            Invalidate();
        }
    }

    protected override void OnPaint(PaintEventArgs pe)
    {
        base.OnPaint(pe);
        if (BorderPen is null) return;
        // Use Graphics object to draw the border
        pe.Graphics.DrawRectangle(BorderPen, 1, 1, Width - 3, Height - 3);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing && BorderPen != null)
            BorderPen.Dispose();
        base.Dispose(disposing);
    }
}

public class RunnerOptions(bool isDrawMagnifier = false)
{
    public bool IsDrawMagnifier { get; set; } = isDrawMagnifier;
}
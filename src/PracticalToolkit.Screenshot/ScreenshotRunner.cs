using System.Drawing.Drawing2D;

namespace PracticalToolkit.Screenshot;

/// <summary>
///     提供屏幕截图功能
///     * 修改自 <see href="https://github.com/tjden88/ScreenshotCreator" />
///     * 根据自身需求做了修改
/// </summary>
public class ScreenshotRunner : IDisposable
{
    #region <字段>

    private BackForm? _backHostForm;
    private Form? _screenshotHost;
    private PictureBox? _selectFrame;
    private Point _startPoint, _endPoint;
    private bool _isDrawing;

    private bool _disposed;

    #endregion

    #region <属性>

    /// <summary>
    ///     获取或设置截图窗口的不透明度。
    /// </summary>
    public double Opacity { get; set; } = 0.5;

    /// <summary>
    ///     获取或设置截图窗口的背景颜色。
    /// </summary>
    public Color Background { get; set; } = Color.Black;

    /// <summary>
    ///     获取或设置截图窗口的透明色。
    /// </summary>
    public Color TransparencyKeyColor { get; set; } = Color.Yellow;

    #endregion

    #region <截图>

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
        var displayRect = GetBounds();

        #region 背景使用当前桌面截图

        _backHostForm = new BackForm(displayRect.Location, displayRect.Size);
        // 绘制背景
        using var backgroundBitmap = new Bitmap(displayRect.Width, displayRect.Height);
        using var graphics = Graphics.FromImage(backgroundBitmap);
        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        graphics.CopyFromScreen(displayRect.X, displayRect.Y, 0, 0, new Size(displayRect.Width, displayRect.Height));
        _backHostForm.BackgroundImage = backgroundBitmap;
        _backHostForm.Show();

        #endregion

        #region 原代码

        _selectFrame = new CustomPictureBox
        {
            BackColor = TransparencyKeyColor,
            Size = new Size(0, 0),
            Visible = false
        };

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
            return null;
        try
        {
            _backHostForm.Opacity = 0;
            _backHostForm.Close();
            _screenshotHost.Opacity = 0;
            _selectFrame.BorderStyle = BorderStyle.None;
            Bitmap bmp = new(_selectFrame.Width, _selectFrame.Height);
            using var g = Graphics.FromImage(bmp);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.CopyFromScreen(_screenshotHost.Left + _selectFrame.Left, _screenshotHost.Top + _selectFrame.Top, 0, 0,
                _selectFrame.Size);
            return bmp;
        }
        catch
        {
            return null;
        }

        #endregion
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

        Load += (_, _) =>
        {
            Location = location;
            Size = size;
            BringToFront();
        };
    }
}

public class CustomPictureBox : PictureBox
{
    public Color BorderColor { get; set; } = Color.FromArgb(32, 128, 240); // 默认边框颜色为#2080f0
    public int BorderThickness { get; set; } = 5; // 默认边框厚度为5

    protected override void OnPaint(PaintEventArgs pe)
    {
        base.OnPaint(pe);
        // 使用Graphics对象绘制边框
        using var pen = new Pen(BorderColor, BorderThickness);
        pe.Graphics.DrawRectangle(pen, 1, 1, Width - 3, Height - 3);
    }
}
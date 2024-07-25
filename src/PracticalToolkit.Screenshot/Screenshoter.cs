using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Color = System.Windows.Media.Color;
using Cursors = System.Windows.Input.Cursors;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using Point = System.Windows.Point;
using Rectangle = System.Windows.Shapes.Rectangle;

namespace PracticalToolkit.Screenshot
{
    public partial class Screenshoter :IDisposable
    {
        
        private Window? _screenshotHost;
        private Rectangle? _selectFrame;
        private Point _startPoint, _endPoint;
        private bool _isDrawing;
        private bool _disposed;
        private Bitmap? _backgroundBitmap;

        private double Opacity { get; set; } = 0.1;
        private Color Background { get; set; } = Colors.Black;
        private Color TransparencyKeyColor { get; set; } = Colors.Yellow;

        private static Rect GetBounds()
        {
            double minX = double.MaxValue, minY = double.MaxValue, maxX = 0, maxY = 0;
            foreach (var scr in WpfScreenHelper.Screen.AllScreens)
            {
                minX = Math.Min(minX, scr.WpfBounds.X);
                minY = Math.Min(minY, scr.WpfBounds.Y);
                maxX = Math.Max(maxX, scr.WpfBounds.Right);
                maxY = Math.Max(maxY, scr.WpfBounds.Bottom);
            }

            return new Rect(minX, minY, maxX - minX, maxY - minY);
        }

        public Bitmap? Screenshot()
        {
            var displayRect = GetBounds();

            _selectFrame = new Rectangle
            {
                Stroke = new SolidColorBrush(TransparencyKeyColor),
                StrokeThickness = 2,
                Visibility = Visibility.Hidden
            };

            _screenshotHost = new Window
            {
                WindowStyle = WindowStyle.None,
                AllowsTransparency = true,
                // Background = new SolidColorBrush(Background) { Opacity = Opacity },
                Left = displayRect.X,
                Top = displayRect.Y,
                Width = displayRect.Width,
                Height = displayRect.Height,
                Topmost = true,
                Cursor = Cursors.Cross,
                Content = new Canvas()
            };

            var bg = new ImageBrush();
            _screenshotHost.Background = bg;
            // 绘制背景
            _backgroundBitmap = new Bitmap((int)(1.25*displayRect.Width), (int)(1.25*displayRect.Height));
            using var graphics = Graphics.FromImage(_backgroundBitmap);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.CopyFromScreen((int)(1.25*displayRect.X), (int)(1.25*displayRect.Y), 0, 0, new System.Drawing.Size((int)
                (1.25*displayRect.Width), (int)(1.25*displayRect.Height)));
            bg.ImageSource = _backgroundBitmap.ToBitmapSource();
            
            
            var canvas = (Canvas)_screenshotHost.Content;
            canvas.Children.Add(_selectFrame);

            _screenshotHost.MouseDown += ScreenshotHost_MouseDown;
            _screenshotHost.MouseMove += ScreenshotHost_MouseMove;
            _screenshotHost.MouseUp += ScreenshotHost_MouseUp;
            _screenshotHost.KeyUp += ScreenshotHost_KeyUp;

            if (_screenshotHost.ShowDialog() != true)
                return null;

            try
            {
                var bmp = new Bitmap((int)(_selectFrame.Width*1.25), (int)(_selectFrame.Height*1.25));
                using var g = Graphics.FromImage(bmp);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.CopyFromScreen((int)(_screenshotHost.Left*1.25 + Canvas.GetLeft(_selectFrame)*1.25),
                    (int)((_screenshotHost.Top)*1.25 + Canvas.GetTop(_selectFrame)*1.25), 0, 0,
                    new System.Drawing.Size((int)(_selectFrame.Width*1.25), (int)(_selectFrame.Height*1.25)));
                return bmp;
            }
            catch
            {
                return null;
            }
        }

        public async Task<Bitmap?> ScreenshotAsync()
        {
            return await Task.Run(Screenshot);
        }

        private void ScreenshotHost_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape && _screenshotHost != null)
                _screenshotHost.DialogResult = false;
        }

        private void ScreenshotHost_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_selectFrame == null) return;
            _startPoint = e.GetPosition(_screenshotHost);
            Canvas.SetLeft(_selectFrame, _startPoint.X);
            Canvas.SetTop(_selectFrame, _startPoint.Y);
            _selectFrame.Width = 0;
            _selectFrame.Height = 0;
            _selectFrame.Visibility = Visibility.Visible;
            _isDrawing = true;
        }

        private void ScreenshotHost_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_isDrawing || _selectFrame == null) return;
            _endPoint = e.GetPosition(_screenshotHost);
            var x = Math.Min(_startPoint.X, _endPoint.X);
            var y = Math.Min(_startPoint.Y, _endPoint.Y);
            var width = Math.Abs(_startPoint.X - _endPoint.X);
            var height = Math.Abs(_startPoint.Y - _endPoint.Y);
            Canvas.SetLeft(_selectFrame, x);
            Canvas.SetTop(_selectFrame, y);
            _selectFrame.Width = width;
            _selectFrame.Height = height;
        }

        private void ScreenshotHost_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!_isDrawing) return;
            _isDrawing = false;
            if (_screenshotHost == null) return;
            _screenshotHost.DialogResult = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Screenshoter()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                _selectFrame = null;
                _screenshotHost = null;
                _backgroundBitmap?.Dispose();
            }

            _disposed = true;
        }
    }
}
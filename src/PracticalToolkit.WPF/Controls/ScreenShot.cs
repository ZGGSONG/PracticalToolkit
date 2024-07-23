using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using PracticalToolkit.WPF.Utils;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;
using Button = System.Windows.Controls.Button;
using Control = System.Windows.Controls.Control;
using Cursors = System.Windows.Input.Cursors;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using Point = System.Windows.Point;
using RadioButton = System.Windows.Controls.RadioButton;
using Rectangle = System.Windows.Shapes.Rectangle;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;
using TextBox = System.Windows.Controls.TextBox;

namespace PracticalToolkit.WPF.Controls;

[TemplatePart(Name = "PART_Canvas", Type = typeof(Canvas))]
[TemplatePart(Name = "PART_RectangleLeft", Type = typeof(Rectangle))]
[TemplatePart(Name = "PART_RectangleTop", Type = typeof(Rectangle))]
[TemplatePart(Name = "PART_RectangleRight", Type = typeof(Rectangle))]
[TemplatePart(Name = "PART_RectangleBottom", Type = typeof(Rectangle))]
[TemplatePart(Name = "PART_Border", Type = typeof(Border))]
[TemplatePart(Name = "PART_EditBar", Type = typeof(Border))]
[TemplatePart(Name = "PART_ButtonSave", Type = typeof(Button))]
[TemplatePart(Name = "PART_ButtonCancel", Type = typeof(Button))]
[TemplatePart(Name = "PART_ButtonComplete", Type = typeof(Button))]
[TemplatePart(Name = "PART_RadioButtonRectangle", Type = typeof(RadioButton))]
[TemplatePart(Name = "PART_RadioButtonEllipse", Type = typeof(RadioButton))]
[TemplatePart(Name = "PART_RadioButtonArrow", Type = typeof(RadioButton))]
[TemplatePart(Name = "PART_RadioButtonInk", Type = typeof(RadioButton))]
[TemplatePart(Name = "PART_RadioButtonText", Type = typeof(RadioButton))]
[TemplatePart(Name = "PART_Popup", Type = typeof(Popup))]
[TemplatePart(Name = "PART_PopupBorder", Type = typeof(Border))]
[TemplatePart(Name = "PART_WrapPanelColor", Type = typeof(WrapPanel))]
public class ScreenShot : Window, IDisposable
{
    private const string CanvasTemplateName = "PART_Canvas";

    private const string RectangleLeftTemplateName = "PART_RectangleLeft";

    private const string RectangleTopTemplateName = "PART_RectangleTop";

    private const string RectangleRightTemplateName = "PART_RectangleRight";

    private const string RectangleBottomTemplateName = "PART_RectangleBottom";

    private const string BorderTemplateName = "PART_Border";

    private const string EditBarTemplateName = "PART_EditBar";

    private const string ButtonSaveTemplateName = "PART_ButtonSave";

    private const string ButtonCancelTemplateName = "PART_ButtonCancel";

    private const string ButtonCompleteTemplateName = "PART_ButtonComplete";

    private const string RadioButtonRectangleTemplateName = "PART_RadioButtonRectangle";

    private const string RadioButtonEllipseTemplateName = "PART_RadioButtonEllipse";

    private const string RadioButtonArrowTemplateName = "PART_RadioButtonArrow";

    private const string RadioButtonInkTemplateName = "PART_RadioButtonInk";

    private const string RadioButtonTextTemplateName = "PART_RadioButtonText";

    private const string PopupTemplateName = "PART_Popup";

    private const string PopupBorderTemplateName = "PART_PopupBorder";

    private const string WrapPanelColorTemplateName = "PART_WrapPanelColor";

    private const string ConstTag = "Draw";

    private const int ConstWidth = 40;

    public static int CaptureScreenId;

    private readonly ScreenDpi _screenDpi;

    private readonly int _screenIndex;

    private AdornerLayer? _adornerLayer;

    private Border _border;

    private Border? _borderRectangle;

    private Button _buttonCancel;

    private Button _buttonComplete;

    private Button _buttonSave;

    private Canvas _canvas;

    private Control? _controlArrow;

    private ControlTemplate _controlTemplate;

    private Brush? _currentBrush;

    private Ellipse? _drawEllipse;

    private Border _editBar;

    private FrameworkElement _frameworkElement;

    private bool _isMouseUp;

    private Point? _pointEnd;

    private Point? _pointStart;

    private Polyline? _polyLine;

    private Popup _popup;

    private Border _popupBorder;

    private RadioButton _radioButtonArrow;

    private RadioButton _radioButtonEllipse;

    private RadioButton _radioButtonInk;

    private RadioButton _radioButtonRectangle;

    private RadioButton _radioButtonText;

    private Rect _rect;

    private Rectangle _rectangleBottom;

    private Rectangle _rectangleLeft;

    private Rectangle _rectangleRight;

    private Rectangle _rectangleTop;

    private ScreenShotAdorner _screenCutAdorner;

    private ScreenCutMouseType _screenCutMouseType;

    private Border? _textBorder;

    private WrapPanel _wrapPanel;

    private double _y1;

    public event Action<CroppedBitmap>? CutCompleted;

    public event Action? CutCanceled;

    static ScreenShot()
    {
        CaptureScreenId = -1;
        DefaultStyleKeyProperty.OverrideMetadata(typeof(ScreenShot), new FrameworkPropertyMetadata(typeof(ScreenShot)));
    }

    public ScreenShot(int index)
    {
        _screenIndex = index;
        Left = Screen.AllScreens[_screenIndex].WorkingArea.Left;
        ShowInTaskbar = false;
        _screenDpi = GetScreenDpi(_screenIndex);
    }

    public void Dispose()
    {
        _canvas.Background = null;
        GC.SuppressFinalize(this);
        GC.Collect();
    }

    public static void ClearCaptureScreenId()
    {
        CaptureScreenId = -1;
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        _canvas = GetTemplateChild(CanvasTemplateName) as Canvas ?? throw new InvalidProgramException();
        _rectangleLeft = GetTemplateChild(RectangleLeftTemplateName) as Rectangle ??
                         throw new InvalidProgramException();
        _rectangleTop = GetTemplateChild(RectangleTopTemplateName) as Rectangle ?? throw new InvalidProgramException();
        _rectangleRight = GetTemplateChild(RectangleRightTemplateName) as Rectangle ??
                          throw new InvalidProgramException();
        _rectangleBottom = GetTemplateChild(RectangleBottomTemplateName) as Rectangle ??
                           throw new InvalidProgramException();
        _border = GetTemplateChild(BorderTemplateName) as Border ?? throw new InvalidProgramException();
        _border.MouseLeftButtonDown += Border_MouseLeftButtonDown;
        _editBar = GetTemplateChild(EditBarTemplateName) as Border ?? throw new InvalidProgramException();
        _buttonSave = GetTemplateChild(ButtonSaveTemplateName) as Button ?? throw new InvalidProgramException();
        _buttonSave.Click += ButtonSave_Click;
        _buttonCancel = GetTemplateChild(ButtonCancelTemplateName) as Button ?? throw new InvalidProgramException();
        _buttonCancel.Click += ButtonCancel_Click;
        _buttonComplete = GetTemplateChild(ButtonCompleteTemplateName) as Button ?? throw new InvalidProgramException();
        _buttonComplete.Click += ButtonComplete_Click;
        _radioButtonRectangle = GetTemplateChild(RadioButtonRectangleTemplateName) as RadioButton ??
                                throw new InvalidProgramException();
        _radioButtonRectangle.Click += RadioButtonRectangle_Click;
        _radioButtonEllipse = GetTemplateChild(RadioButtonEllipseTemplateName) as RadioButton ??
                              throw new InvalidProgramException();
        _radioButtonEllipse.Click += RadioButtonEllipse_Click;
        _radioButtonArrow = GetTemplateChild(RadioButtonArrowTemplateName) as RadioButton ??
                            throw new InvalidProgramException();
        _radioButtonArrow.Click += RadioButtonArrow_Click;
        _radioButtonInk = GetTemplateChild(RadioButtonInkTemplateName) as RadioButton ??
                          throw new InvalidProgramException();
        _radioButtonInk.Click += RadioButtonInk_Click;
        _radioButtonText = GetTemplateChild(RadioButtonTextTemplateName) as RadioButton ??
                           throw new InvalidProgramException();
        _radioButtonText.Click += RadioButtonText_Click;
        _canvas.Width = Screen.AllScreens[_screenIndex].Bounds.Width;
        _canvas.Height = Screen.AllScreens[_screenIndex].Bounds.Height;
        _canvas.Background = new ImageBrush(ImageUtil.CreateBitmapSourceFromBitmap(CopyScreen()));
        _rectangleLeft.Width = _canvas.Width;
        _rectangleLeft.Height = _canvas.Height;
        _border.Opacity = 0.0;
        _popup = GetTemplateChild(PopupTemplateName) as Popup ?? throw new InvalidProgramException();
        _popupBorder = GetTemplateChild(PopupBorderTemplateName) as Border ?? throw new InvalidProgramException();
        _popupBorder.Loaded += delegate { _popup.HorizontalOffset = (0.0 - _popupBorder.ActualWidth) / 3.0; };
        _wrapPanel = GetTemplateChild(WrapPanelColorTemplateName) as WrapPanel ?? throw new InvalidProgramException();
        _wrapPanel.PreviewMouseDown += WrapPanel_PreviewMouseDown;
        _controlTemplate = (ControlTemplate)FindResource("WD.PART_DrawArrow");
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        Dispose();
    }

    private ScreenDpi GetScreenDpi(int screenIndex)
    {
        var result = default(ScreenDpi);
        Screen.AllScreens[screenIndex].GetDpi(DpiType.Effective, out result.DpiX, out result.DpiY);
        result.ScaleX = result.DpiX / 0.96f / 100f;
        result.ScaleY = result.DpiY / 0.96f / 100f;
        return result;
    }

    private Bitmap CopyScreen()
    {
        Left = Screen.AllScreens[_screenIndex].Bounds.Left / _screenDpi.ScaleX;
        Top = Screen.AllScreens[_screenIndex].Bounds.Top / _screenDpi.ScaleY;
        Width = Screen.AllScreens[_screenIndex].Bounds.Width / _screenDpi.ScaleX;
        Height = Screen.AllScreens[_screenIndex].Bounds.Height / _screenDpi.ScaleY;
        _canvas.Width = Screen.AllScreens[_screenIndex].Bounds.Width / _screenDpi.ScaleX;
        _canvas.Height = Screen.AllScreens[_screenIndex].Bounds.Height / _screenDpi.ScaleY;
        _canvas.SetValue(LeftProperty, Left);
        _canvas.SetValue(TopProperty, Top);
        var bitmap = new Bitmap(Screen.AllScreens[_screenIndex].Bounds.Width,
            Screen.AllScreens[_screenIndex].Bounds.Height);
        using var graphics = Graphics.FromImage(bitmap);
        graphics.CopyFromScreen(Screen.AllScreens[_screenIndex].Bounds.Left, Screen.AllScreens[_screenIndex].Bounds.Top,
            0, 0, Screen.AllScreens[_screenIndex].Bounds.Size);
        return bitmap;
    }

    protected override void OnPreviewMouseRightButtonDown(MouseButtonEventArgs e)
    {
        OnCanceled();
    }

    private void RadioButtonInk_Click(object sender, RoutedEventArgs e)
    {
        RadioButtonChecked(_radioButtonInk, ScreenCutMouseType.DrawInk);
    }

    private void RadioButtonText_Click(object sender, RoutedEventArgs e)
    {
        RadioButtonChecked(_radioButtonText, ScreenCutMouseType.DrawText);
    }

    private void RadioButtonArrow_Click(object sender, RoutedEventArgs e)
    {
        RadioButtonChecked(_radioButtonArrow, ScreenCutMouseType.DrawArrow);
    }

    private void WrapPanel_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.Source is not RadioButton radioButton) return;
        _currentBrush = radioButton.Background;
    }

    private void RadioButtonRectangle_Click(object sender, RoutedEventArgs e)
    {
        RadioButtonChecked(_radioButtonRectangle, ScreenCutMouseType.DrawRectangle);
    }

    private void RadioButtonEllipse_Click(object sender, RoutedEventArgs e)
    {
        RadioButtonChecked(_radioButtonEllipse, ScreenCutMouseType.DrawEllipse);
    }

    private void RadioButtonChecked(RadioButton radioButton, ScreenCutMouseType screenCutMouseTypeRadio)
    {
        if (radioButton.IsChecked == true)
        {
            _screenCutMouseType = screenCutMouseTypeRadio;
            _border.Cursor = Cursors.Arrow;
            if (_popup is { PlacementTarget: not null, IsOpen: true }) _popup.IsOpen = false;
            _popup.PlacementTarget = radioButton;
            _popup.IsOpen = true;
            DisposeControl();
        }
        else if (_screenCutMouseType == screenCutMouseTypeRadio)
        {
            Restore();
        }
    }

    private void Restore()
    {
        _border.Cursor = Cursors.SizeAll;
        if (_screenCutMouseType == 0) return;
        _screenCutMouseType = ScreenCutMouseType.Default;
        if (_popup is { PlacementTarget: not null, IsOpen: true })
            _popup.IsOpen = false;
    }

    private void RestoreRadioButton()
    {
        _radioButtonRectangle.IsChecked = false;
        _radioButtonEllipse.IsChecked = false;
    }

    private void _border_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (_isMouseUp)
        {
            var left = Canvas.GetLeft(_border);
            var top = Canvas.GetTop(_border);
            var point = new Point(left, top);
            var point2 = new Point(left + _border.ActualWidth, top + _border.ActualHeight);
            _rect = new Rect(point, point2);
            _pointStart = point;
            MoveAllRectangle(point2);
        }

        EditBarPosition();
    }

    private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (_screenCutMouseType == ScreenCutMouseType.Default) _screenCutMouseType = ScreenCutMouseType.MoveMouse;
    }

    private void ButtonSave_Click(object sender, RoutedEventArgs e)
    {
        var saveFileDialog = new SaveFileDialog
        {
            FileName = DateTime.Now.ToString("yyyyMMddHHmmss") + ".png",
            DefaultExt = ".png",
            Filter = "image file|*.png"
        };
        if (saveFileDialog.ShowDialog() != true) return;
        BitmapEncoder bitmapEncoder = new PngBitmapEncoder();
        bitmapEncoder.Frames.Add(BitmapFrame.Create(CutBitmap()));
        using (var fileStream = File.OpenWrite(saveFileDialog.FileName))
        {
            bitmapEncoder.Save(fileStream);
        }

        Close();
    }

    private void ButtonComplete_Click(object sender, RoutedEventArgs e)
    {
        var croppedBitmap = CutBitmap();
        croppedBitmap.Freeze();
        CutCompleted?.Invoke(croppedBitmap);
        Close();
    }

    private CroppedBitmap CutBitmap()
    {
        _border.Visibility = Visibility.Collapsed;
        _editBar.Visibility = Visibility.Collapsed;
        _rectangleLeft.Visibility = Visibility.Collapsed;
        _rectangleTop.Visibility = Visibility.Collapsed;
        _rectangleRight.Visibility = Visibility.Collapsed;
        _rectangleBottom.Visibility = Visibility.Collapsed;
        var renderTargetBitmap = new RenderTargetBitmap((int)(_canvas.Width * _screenDpi.ScaleX),
            (int)(_canvas.Height * _screenDpi.ScaleY), _screenDpi.DpiX, _screenDpi.DpiY, PixelFormats.Default);
        renderTargetBitmap.Render(_canvas);
        var sourceRect = new Int32Rect((int)(_rect.X * _screenDpi.ScaleX), (int)(_rect.Y * _screenDpi.ScaleY),
            (int)(_rect.Width * _screenDpi.ScaleX), (int)(_rect.Height * _screenDpi.ScaleY));
        return new CroppedBitmap(renderTargetBitmap, sourceRect);
    }

    private void ButtonCancel_Click(object sender, RoutedEventArgs e)
    {
        OnCanceled();
    }

    private void OnCanceled()
    {
        Close();
        CutCanceled?.Invoke();
    }

    protected override void OnPreviewKeyDown(KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.Escape:
                OnCanceled();
                break;
            case Key.Delete:
            {
                if (_canvas.Children.Count > 0) _canvas.Children.Remove(_frameworkElement);
                break;
            }
            default:
            {
                if (e.KeyStates == Keyboard.GetKeyStates(Key.Z) && Keyboard.Modifiers == ModifierKeys.Control &&
                    _canvas.Children.Count > 0)
                {
                    var index = _canvas.Children.Count - 1;
                    _canvas.Children.Remove(_canvas.Children[index]);
                }

                break;
            }
        }
    }

    protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        if (e.Source is RadioButton) return;
        if (CaptureScreenId == -1) CaptureScreenId = _screenIndex;
        if (CaptureScreenId != -1 && CaptureScreenId != _screenIndex)
        {
            e.Handled = true;
            return;
        }

        var position = e.GetPosition(_canvas);
        if (!_isMouseUp)
        {
            _pointStart = position;
            _screenCutMouseType = ScreenCutMouseType.DrawMouse;
            _editBar.Visibility = Visibility.Hidden;
            _pointEnd = _pointStart;
            _rect = new Rect(_pointStart.Value, _pointEnd.Value);
        }
        else if (!(position.X < _rect.Left) && !(position.X > _rect.Right) && !(position.Y < _rect.Top) &&
                 !(position.Y > _rect.Bottom))
        {
            _pointStart = position;
            if (_textBorder != null) Focus();
            if (_screenCutMouseType == ScreenCutMouseType.DrawText)
            {
                _y1 = position.Y;
                DrawText();
            }
            else
            {
                Focus();
            }
        }
    }

    private void DrawText()
    {
        if (_pointStart == null || !(_pointStart.Value.X < _rect.Right) || !(_pointStart.Value.X > _rect.Left) ||
            !(_pointStart.Value.Y > _rect.Top) || !(_pointStart.Value.Y < _rect.Bottom)) return;
        var num = _pointStart.Value.X + ConstWidth;
        if (_textBorder != null) return;
        _textBorder = new Border
        {
            BorderBrush = _currentBrush ?? Brushes.Red,
            BorderThickness = new Thickness(1.0),
            Tag = ConstTag
        };
        var textBox = new TextBox
        {
            Style = null,
            Background = null,
            BorderThickness = new Thickness(0.0),
            Foreground = _textBorder.BorderBrush,
            FontSize = 16.0,
            TextWrapping = TextWrapping.Wrap,
            FontWeight = FontWeights.Normal,
            MinWidth = ConstWidth,
            MaxWidth = _rect.Right - _pointStart.Value.X,
            MaxHeight = _rect.Height - 4.0,
            Cursor = Cursors.Hand,
            Padding = new Thickness(4.0)
        };
        textBox.LostKeyboardFocus += delegate(object s, KeyboardFocusChangedEventArgs e1)
        {
            if (s is not TextBox t) return;
            var parent = VisualTreeHelper.GetParent(t);
            if (parent is not Border border3) return;
            border3.BorderThickness = new Thickness(0.0);
            if (string.IsNullOrWhiteSpace(t.Text)) _canvas.Children.Remove(border3);
        };
        _textBorder.SizeChanged += delegate(object s, SizeChangedEventArgs e1)
        {
            if (s is not Border b) return;
            var num3 = _y1;
            if (!(num3 + b.ActualHeight > _rect.Bottom)) return;
            var num4 = Math.Abs(_rect.Bottom - (num3 + b.ActualHeight));
            _y1 = num3 - num4;
            Canvas.SetTop(b, _y1 + 2.0);
        };
        _textBorder.PreviewMouseLeftButtonDown += delegate(object s, MouseButtonEventArgs e)
        {
            _radioButtonText.IsChecked = true;
            RadioButtonText_Click(new object(), new RoutedEventArgs());
            SelectElement();
            if (s is not Border b) return;
            _frameworkElement = b;
            _frameworkElement.Opacity = 0.7;
            b.BorderThickness = new Thickness(1.0);
        };
        _textBorder.Child = textBox;
        _canvas.Children.Add(_textBorder);
        textBox.Focus();
        var num2 = _pointStart.Value.X;
        if (num > _rect.Right) num2 -= num - _rect.Right;
        Canvas.SetLeft(_textBorder, num2 - 2.0);
        Canvas.SetTop(_textBorder, _pointStart.Value.Y - 2.0);
    }

    protected override void OnPreviewMouseMove(MouseEventArgs e)
    {
        if (e.Source is RadioButton) return;
        var point = _pointStart;
        if (!point.HasValue || e.LeftButton != MouseButtonState.Pressed) return;
        var position = e.GetPosition(_canvas);
        switch (_screenCutMouseType)
        {
            case ScreenCutMouseType.DrawMouse:
                MoveAllRectangle(position);
                break;
            case ScreenCutMouseType.MoveMouse:
                MoveRect(position);
                break;
            case ScreenCutMouseType.DrawRectangle:
            case ScreenCutMouseType.DrawEllipse:
                DrawMultipleControl(position);
                break;
            case ScreenCutMouseType.DrawArrow:
                DrawArrowControl(position);
                break;
            case ScreenCutMouseType.DrawInk:
                DrawInkControl(position);
                break;
            case ScreenCutMouseType.DrawText:
                break;
        }
    }

    private void CheckPoint(Point current)
    {
        var value = current;
        var point = _pointStart;
        if (value == point || current.X > _rect.BottomRight.X) return;
        _ = current.Y;
        _ = _rect.BottomRight.Y;
    }

    private void DrawInkControl(Point current)
    {
        CheckPoint(current);
        if (!(current.X >= _rect.Left) || !(current.X <= _rect.Right) || !(current.Y >= _rect.Top) ||
            !(current.Y <= _rect.Bottom)) return;
        if (_polyLine == null)
        {
            _polyLine = new Polyline
            {
                Stroke = _currentBrush ?? Brushes.Red,
                Cursor = Cursors.Hand,
                StrokeThickness = 3.0,
                StrokeLineJoin = PenLineJoin.Round,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeEndLineCap = PenLineCap.Round
            };
            _polyLine.MouseLeftButtonDown += delegate(object s, MouseButtonEventArgs e)
            {
                _radioButtonInk.IsChecked = true;
                RadioButtonInk_Click(new object(), new RoutedEventArgs());
                SelectElement();
                if (s is not Polyline p) return;
                _frameworkElement = p;
                _frameworkElement.Opacity = 0.7;
            };
            _canvas.Children.Add(_polyLine);
        }

        _polyLine.Points.Add(current);
    }

    private void DrawArrowControl(Point current)
    {
        CheckPoint(current);
        if (_screenCutMouseType != ScreenCutMouseType.DrawArrow) return;
        var point = _pointStart;
        if (!point.HasValue || _pointStart is null) return;
        var value = _pointStart.Value;
        var rect1 = new Rect(value, current);
        if (_controlArrow == null)
        {
            _controlArrow = new Control
            {
                Background = _currentBrush ?? Brushes.Red,
                Template = _controlTemplate,
                Cursor = Cursors.Hand,
                Tag = ConstTag
            };
            _controlArrow.MouseLeftButtonDown += delegate(object s, MouseButtonEventArgs e)
            {
                _radioButtonArrow.IsChecked = true;
                RadioButtonArrow_Click(new object(), new RoutedEventArgs());
                SelectElement();
                if (s is not Control c) return;
                _frameworkElement = c;
                _frameworkElement.Opacity = 0.7;
            };
            _canvas.Children.Add(_controlArrow);
            Canvas.SetLeft(_controlArrow, rect1.Left);
            Canvas.SetTop(_controlArrow, rect1.Top - 7.5);
        }

        var rotateTransform = new RotateTransform();
        var renderTransformOrigin = new Point(0.0, 0.5);
        _controlArrow.RenderTransformOrigin = renderTransformOrigin;
        _controlArrow.RenderTransform = rotateTransform;
        rotateTransform.Angle = CalculateAngle(value, current);
        if (current.X < _rect.Left || current.X > _rect.Right || current.Y < _rect.Top ||
            current.Y > _rect.Bottom)
        {
            if (current.X >= value.X && current.Y < value.Y)
            {
                var num = (current.Y - value.Y) / (current.X - value.X);
                var num2 = value.Y - num * value.X;
                var num3 = (_rect.Top - num2) / num;
                var y = num * _rect.Right + num2;
                if (num3 <= _rect.Right)
                {
                    current.X = num3;
                    current.Y = _rect.Top;
                }
                else
                {
                    current.X = _rect.Right;
                    current.Y = y;
                }
            }
            else if (current.X > value.X && current.Y > value.Y)
            {
                var num4 = (current.Y - value.Y) / (current.X - value.X);
                var num5 = value.Y - num4 * value.X;
                var num6 = (_rect.Bottom - num5) / num4;
                var y2 = num4 * _rect.Right + num5;
                if (num6 <= _rect.Right)
                {
                    current.X = num6;
                    current.Y = _rect.Bottom;
                }
                else
                {
                    current.X = _rect.Right;
                    current.Y = y2;
                }
            }
            else if (current.X < value.X && current.Y < value.Y)
            {
                var num7 = (current.Y - value.Y) / (current.X - value.X);
                var num8 = value.Y - num7 * value.X;
                var num9 = (_rect.Top - num8) / num7;
                var y3 = num7 * _rect.Left + num8;
                if (num9 >= _rect.Left)
                {
                    current.X = num9;
                    current.Y = _rect.Top;
                }
                else
                {
                    current.X = _rect.Left;
                    current.Y = y3;
                }
            }
            else if (current.X < value.X && current.Y > value.Y)
            {
                var num10 = (current.Y - value.Y) / (current.X - value.X);
                var num11 = value.Y - num10 * value.X;
                var num12 = (_rect.Bottom - num11) / num10;
                var y4 = num10 * _rect.Left + num11;
                if (num12 <= _rect.Left)
                {
                    current.X = _rect.Left;
                    current.Y = y4;
                }
                else
                {
                    current.X = num12;
                    current.Y = _rect.Bottom;
                }
            }
        }

        var x = current.X - value.X;
        var x2 = current.Y - value.Y;
        var num13 = Math.Sqrt(Math.Pow(x, 2.0) + Math.Pow(x2, 2.0));
        num13 = num13 < 15.0 ? 15.0 : num13;
        _controlArrow.Width = num13;
    }

    private void DrawMultipleControl(Point current)
    {
        CheckPoint(current);
        var point = _pointStart;
        if (!point.HasValue || _pointStart is null) return;
        var value = _pointStart.Value;
        var rect1 = new Rect(value, current);
        switch (_screenCutMouseType)
        {
            case ScreenCutMouseType.DrawRectangle:
                if (_borderRectangle == null)
                {
                    _borderRectangle = new Border
                    {
                        BorderBrush = _currentBrush ?? Brushes.Red,
                        BorderThickness = new Thickness(3.0),
                        CornerRadius = new CornerRadius(3.0),
                        Tag = ConstTag,
                        Cursor = Cursors.Hand
                    };
                    _borderRectangle.MouseLeftButtonDown += delegate(object s, MouseButtonEventArgs e)
                    {
                        _radioButtonRectangle.IsChecked = true;
                        RadioButtonRectangle_Click(new object(), new RoutedEventArgs());
                        SelectElement();
                        if (s is not Border b) return;
                        _frameworkElement = b;
                        _frameworkElement.Opacity = 0.7;
                    };
                    _canvas.Children.Add(_borderRectangle);
                }

                break;
            case ScreenCutMouseType.DrawEllipse:
                if (_drawEllipse == null)
                {
                    _drawEllipse = new Ellipse
                    {
                        Stroke = _currentBrush ?? Brushes.Red,
                        StrokeThickness = 3.0,
                        Tag = ConstTag,
                        Cursor = Cursors.Hand
                    };
                    _drawEllipse.MouseLeftButtonDown += delegate(object s, MouseButtonEventArgs e)
                    {
                        _radioButtonEllipse.IsChecked = true;
                        RadioButtonEllipse_Click(new object(), new RoutedEventArgs());
                        SelectElement();
                        if (s is not Ellipse el) return;
                        _frameworkElement = el;
                        _frameworkElement.Opacity = 0.7;
                    };
                    _canvas.Children.Add(_drawEllipse);
                }

                break;
        }

        var num = rect1.Left - Canvas.GetLeft(_border);
        if (num < 0.0) num = Math.Abs(num);
        if (rect1.Width + num < _border.ActualWidth)
        {
            var num2 = Canvas.GetLeft(_border) + _border.ActualWidth;
            var length = rect1.Left < Canvas.GetLeft(_border) ? Canvas.GetLeft(_border) :
                rect1.Left > num2 ? num2 : rect1.Left;
            if (_borderRectangle != null)
            {
                _borderRectangle.Width = rect1.Width;
                Canvas.SetLeft(_borderRectangle, length);
            }

            if (_drawEllipse != null)
            {
                _drawEllipse.Width = rect1.Width;
                Canvas.SetLeft(_drawEllipse, length);
            }
        }

        var num3 = rect1.Top - Canvas.GetTop(_border);
        if (num3 < 0.0) num3 = Math.Abs(num3);
        if (!(rect1.Height + num3 < _border.ActualHeight)) return;
        var num4 = Canvas.GetTop(_border) + _border.Height;
        var length2 = rect1.Top < Canvas.GetTop(_border) ? Canvas.GetTop(_border) :
            rect1.Top > num4 ? num4 : rect1.Top;
        if (_borderRectangle != null)
        {
            _borderRectangle.Height = rect1.Height;
            Canvas.SetTop(_borderRectangle, length2);
        }

        if (_drawEllipse == null) return;
        _drawEllipse.Height = rect1.Height;
        Canvas.SetTop(_drawEllipse, length2);
    }

    private void SelectElement()
    {
        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(_canvas); i++)
            if (VisualTreeHelper.GetChild(_canvas, i) is FrameworkElement { Tag: not null } frameworkElement &&
                frameworkElement.Tag.ToString() == ConstTag)
                frameworkElement.Opacity = 1.0;
    }

    private void MoveRect(Point current)
    {
        var point = _pointStart;
        if (!point.HasValue || _pointStart is null) return;
        var value = _pointStart.Value;
        if (current == value) return;
        var vector = Point.Subtract(current, value);
        var num = Canvas.GetLeft(_border) + vector.X;
        var num2 = Canvas.GetTop(_border) + vector.Y;
        if (num <= 0.0) num = 0.0;
        if (num2 <= 0.0) num2 = 0.0;
        if (num + _border.Width >= _canvas.ActualWidth) num = _canvas.ActualWidth - _border.ActualWidth;
        if (num2 + _border.Height >= _canvas.ActualHeight) num2 = _canvas.ActualHeight - _border.ActualHeight;
        _pointStart = current;
        Canvas.SetLeft(_border, num);
        Canvas.SetTop(_border, num2);
        _rect = new Rect(new Point(num, num2), new Point(num + _border.Width, num2 + _border.Height));
        _rectangleLeft.Height = _canvas.ActualHeight;
        _rectangleLeft.Width = num <= 0.0 ? 0.0 : num >= _canvas.ActualWidth ? _canvas.ActualWidth : num;
        Canvas.SetLeft(_rectangleTop, _rectangleLeft.Width);
        _rectangleTop.Height = num2 <= 0.0 ? 0.0 : num2 >= _canvas.ActualHeight ? _canvas.ActualHeight : num2;
        Canvas.SetLeft(_rectangleRight, num + _border.Width);
        var num3 = _canvas.ActualWidth - (_border.Width + _rectangleLeft.Width);
        _rectangleRight.Width = num3 <= 0.0 ? 0.0 : num3;
        _rectangleRight.Height = _canvas.ActualHeight;
        Canvas.SetLeft(_rectangleBottom, _rectangleLeft.Width);
        Canvas.SetTop(_rectangleBottom, num2 + _border.Height);
        _rectangleBottom.Width = _border.Width;
        var num4 = _canvas.ActualHeight - (num2 + _border.Height);
        _rectangleBottom.Height = num4 <= 0.0 ? 0.0 : num4;
    }

    protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
    {
        if (e.Source is ToggleButton || _pointStart == _pointEnd) return;
        if (e.Source is FrameworkElement { Tag: null }) SelectElement();
        _isMouseUp = true;
        switch (_screenCutMouseType)
        {
            case 0:
                return;
            case ScreenCutMouseType.MoveMouse:
                EditBarPosition();
                break;
        }

        if (_radioButtonRectangle.IsChecked != true && _radioButtonEllipse.IsChecked != true &&
            _radioButtonArrow.IsChecked != true && _radioButtonText.IsChecked != true &&
            _radioButtonInk.IsChecked != true)
            _screenCutMouseType = ScreenCutMouseType.Default;
        else
            DisposeControl();
    }

    private void DisposeControl()
    {
        _polyLine = null;
        _textBorder = null;
        _borderRectangle = null;
        _drawEllipse = null;
        _controlArrow = null;
        _pointStart = null;
        _pointEnd = null;
    }

    private void EditBarPosition()
    {
        _editBar.Visibility = Visibility.Visible;
        Canvas.SetLeft(_editBar, _rect.X + _rect.Width - _editBar.ActualWidth);
        var num = Canvas.GetTop(_border) + _border.ActualHeight + _editBar.ActualHeight + _popupBorder.ActualHeight +
                  24.0;
        Canvas.SetTop(
            length: num > _canvas.ActualHeight && Canvas.GetTop(_border) > _editBar.ActualHeight
                ? Canvas.GetTop(_border) - _editBar.ActualHeight - 8.0
                : !(num > _canvas.ActualHeight) || !(Canvas.GetTop(_border) < _editBar.ActualHeight)
                    ? Canvas.GetTop(_border) + _border.ActualHeight + 8.0
                    : _border.ActualHeight - _editBar.ActualHeight - 8.0, element: _editBar);
        if (_popup is not { IsOpen: true }) return;
        _popup.IsOpen = false;
        _popup.IsOpen = true;
    }

    private void MoveAllRectangle(Point current)
    {
        var point = _pointStart;
        if (!point.HasValue || _pointStart is null) return;
        var value = _pointStart.Value;
        _pointEnd = current;
        var point2 = current;
        _rect = new Rect(value, point2);
        _rectangleLeft.Width = _rect.X < 0.0 ? 0.0 : _rect.X > _canvas.ActualWidth ? _canvas.ActualWidth : _rect.X;
        _rectangleLeft.Height = _canvas.Height;
        Canvas.SetLeft(_rectangleTop, _rectangleLeft.Width);
        _rectangleTop.Width = _rect.Width;
        var num = 0.0;
        num = !(current.Y < value.Y) ? current.Y - _rect.Height : current.Y;
        _rectangleTop.Height = num < 0.0 ? 0.0 : num > _canvas.ActualHeight ? _canvas.ActualHeight : num;
        Canvas.SetLeft(_rectangleRight, _rectangleLeft.Width + _rect.Width);
        var num2 = _canvas.Width - (_rect.Width + _rectangleLeft.Width);
        _rectangleRight.Width = num2 < 0.0 ? 0.0 : num2 > _canvas.ActualWidth ? _canvas.ActualWidth : num2;
        _rectangleRight.Height = _canvas.Height;
        Canvas.SetLeft(_rectangleBottom, _rectangleLeft.Width);
        Canvas.SetTop(_rectangleBottom, _rect.Height + _rectangleTop.Height);
        _rectangleBottom.Width = _rect.Width;
        var num3 = _canvas.Height - (_rect.Height + _rectangleTop.Height);
        _rectangleBottom.Height = num3 < 0.0 ? 0.0 : num3;
        _border.Height = _rect.Height;
        _border.Width = _rect.Width;
        Canvas.SetLeft(_border, _rect.X);
        Canvas.SetTop(_border, _rect.Y);
        if (_adornerLayer != null) return;
        _border.Opacity = 1.0;
        _adornerLayer = AdornerLayer.GetAdornerLayer(_border);
        _screenCutAdorner = new ScreenShotAdorner(_border);
        _screenCutAdorner.PreviewMouseDown += delegate
        {
            Restore();
            RestoreRadioButton();
        };
        _adornerLayer!.Add(_screenCutAdorner);
        _border.SizeChanged += _border_SizeChanged;
    }

    private double CalculateAngle(Point start, Point end)
    {
        return (Math.Atan2(end.Y - start.Y, end.X - start.X) * (180.0 / Math.PI) + 360.0) % 360.0;
    }
}
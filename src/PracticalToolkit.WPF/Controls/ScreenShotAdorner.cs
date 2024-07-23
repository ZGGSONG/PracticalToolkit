using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;
using Cursor = System.Windows.Input.Cursor;
using Cursors = System.Windows.Input.Cursors;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using Point = System.Windows.Point;
using Size = System.Windows.Size;

namespace PracticalToolkit.WPF.Controls;

public class ScreenShotAdorner : Adorner
{
    private const double THUMB_SIZE = 12.0;

    private const double MINIMAL_SIZE = 20.0;

    private readonly Thumb lc;

    private readonly Thumb tl;

    private readonly Thumb tc;

    private readonly Thumb tr;

    private readonly Thumb rc;

    private readonly Thumb br;

    private readonly Thumb bc;

    private readonly Thumb bl;

    private readonly VisualCollection visCollec;

    private readonly Canvas canvas;

    protected override int VisualChildrenCount => visCollec.Count;

    public ScreenShotAdorner(UIElement adorned)
        : base(adorned)
    {
        canvas = FindParent(adorned) as Canvas;
        visCollec = new VisualCollection(this);
        visCollec.Add(lc = GetResizeThumb(Cursors.SizeWE, HorizontalAlignment.Left, VerticalAlignment.Center));
        visCollec.Add(tl = GetResizeThumb(Cursors.SizeNWSE, HorizontalAlignment.Left, VerticalAlignment.Top));
        visCollec.Add(tc = GetResizeThumb(Cursors.SizeNS, HorizontalAlignment.Center, VerticalAlignment.Top));
        visCollec.Add(tr = GetResizeThumb(Cursors.SizeNESW, HorizontalAlignment.Right, VerticalAlignment.Top));
        visCollec.Add(rc = GetResizeThumb(Cursors.SizeWE, HorizontalAlignment.Right, VerticalAlignment.Center));
        visCollec.Add(br = GetResizeThumb(Cursors.SizeNWSE, HorizontalAlignment.Right, VerticalAlignment.Bottom));
        visCollec.Add(bc = GetResizeThumb(Cursors.SizeNS, HorizontalAlignment.Center, VerticalAlignment.Bottom));
        visCollec.Add(bl = GetResizeThumb(Cursors.SizeNESW, HorizontalAlignment.Left, VerticalAlignment.Bottom));
    }

    private static UIElement FindParent(UIElement element)
    {
        return VisualTreeHelper.GetParent(element) as UIElement;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        double num = 6.0;
        Size size = new Size(12.0, 12.0);
        lc.Arrange(new Rect(new Point(0.0 - num, base.AdornedElement.RenderSize.Height / 2.0 - num), size));
        tl.Arrange(new Rect(new Point(0.0 - num, 0.0 - num), size));
        tc.Arrange(new Rect(new Point(base.AdornedElement.RenderSize.Width / 2.0 - num, 0.0 - num), size));
        tr.Arrange(new Rect(new Point(base.AdornedElement.RenderSize.Width - num, 0.0 - num), size));
        rc.Arrange(new Rect(new Point(base.AdornedElement.RenderSize.Width - num, base.AdornedElement.RenderSize.Height / 2.0 - num), size));
        br.Arrange(new Rect(new Point(base.AdornedElement.RenderSize.Width - num, base.AdornedElement.RenderSize.Height - num), size));
        bc.Arrange(new Rect(new Point(base.AdornedElement.RenderSize.Width / 2.0 - num, base.AdornedElement.RenderSize.Height - num), size));
        bl.Arrange(new Rect(new Point(0.0 - num, base.AdornedElement.RenderSize.Height - num), size));
        return finalSize;
    }

    private void Resize(FrameworkElement frameworkElement)
    {
        if (double.IsNaN(frameworkElement.Width))
        {
            frameworkElement.Width = frameworkElement.RenderSize.Width;
        }
        if (double.IsNaN(frameworkElement.Height))
        {
            frameworkElement.Height = frameworkElement.RenderSize.Height;
        }
    }

    private Thumb GetResizeThumb(Cursor cur, HorizontalAlignment hor, VerticalAlignment ver)
    {
        Thumb thumb = new Thumb
        {
            Width = 12.0,
            Height = 12.0,
            HorizontalAlignment = hor,
            VerticalAlignment = ver,
            Cursor = cur,
            Template = new ControlTemplate(typeof(Thumb))
            {
                VisualTree = GetFactory(new SolidColorBrush(Colors.White))
            }
        };
        if (!double.IsNaN(canvas.Width))
        {
            _ = canvas.Width;
        }
        else
        {
            _ = canvas.ActualWidth;
        }
        if (!double.IsNaN(canvas.Height))
        {
            _ = canvas.Height;
        }
        else
        {
            _ = canvas.ActualHeight;
        }
        thumb.DragDelta += delegate (object s, DragDeltaEventArgs e)
        {
            if (base.AdornedElement is FrameworkElement frameworkElement)
            {
                Resize(frameworkElement);
                switch (thumb.VerticalAlignment)
                {
                    case VerticalAlignment.Bottom:
                        if (frameworkElement.Height + e.VerticalChange > 20.0)
                        {
                            double num2 = frameworkElement.Height + e.VerticalChange;
                            double num3 = Canvas.GetTop(frameworkElement) + num2;
                            if (num2 > 0.0 && num3 <= canvas.ActualHeight)
                            {
                                frameworkElement.Height = num2;
                            }
                        }
                        break;
                    case VerticalAlignment.Top:
                        if (frameworkElement.Height - e.VerticalChange > 20.0)
                        {
                            double num = frameworkElement.Height - e.VerticalChange;
                            double top = Canvas.GetTop(frameworkElement);
                            if (num > 0.0 && top + e.VerticalChange >= 0.0)
                            {
                                frameworkElement.Height = num;
                                Canvas.SetTop(frameworkElement, top + e.VerticalChange);
                            }
                        }
                        break;
                }
                switch (thumb.HorizontalAlignment)
                {
                    case HorizontalAlignment.Left:
                        if (frameworkElement.Width - e.HorizontalChange > 20.0)
                        {
                            double num6 = frameworkElement.Width - e.HorizontalChange;
                            double left = Canvas.GetLeft(frameworkElement);
                            if (num6 > 0.0 && left + e.HorizontalChange >= 0.0)
                            {
                                frameworkElement.Width = num6;
                                Canvas.SetLeft(frameworkElement, left + e.HorizontalChange);
                            }
                        }
                        break;
                    case HorizontalAlignment.Right:
                        if (frameworkElement.Width + e.HorizontalChange > 20.0)
                        {
                            double num4 = frameworkElement.Width + e.HorizontalChange;
                            double num5 = Canvas.GetLeft(frameworkElement) + num4;
                            if (num4 > 0.0 && num5 <= canvas.ActualWidth)
                            {
                                frameworkElement.Width = num4;
                            }
                        }
                        break;
                }
                e.Handled = true;
            }
        };
        return thumb;
    }

    private FrameworkElementFactory GetFactory(Brush back)
    {
        FrameworkElementFactory frameworkElementFactory = new FrameworkElementFactory(typeof(Ellipse));
        frameworkElementFactory.SetValue(Shape.FillProperty, back);
        frameworkElementFactory.SetValue(Shape.StrokeProperty, Brushes.DodgerBlue);
        frameworkElementFactory.SetValue(Shape.StrokeThicknessProperty, 2.0);
        return frameworkElementFactory;
    }

    protected override Visual GetVisualChild(int index)
    {
        return visCollec[index];
    }
}
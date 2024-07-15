using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using PracticalToolkit.WPF.Enums;

namespace PracticalToolkit.WPF.Controls;

public class XamlIcon : Label
{
    public static readonly DependencyProperty IconProperty =
        DependencyProperty.Register(nameof(Icon), typeof(Icons), typeof(XamlIcon),
            new FrameworkPropertyMetadata(Icons.None, FrameworkPropertyMetadataOptions.AffectsArrange,
                IconChangedCallback));

    public static readonly DependencyProperty IconSizeProperty =
        DependencyProperty.Register(nameof(IconSize), typeof(double), typeof(XamlIcon),
            new FrameworkPropertyMetadata(12.0, FrameworkPropertyMetadataOptions.AffectsMeasure,
                IconSizeChangedCallback));

    private readonly Dictionary<string, Viewbox> _globalIcon = new();

    public XamlIcon()
    {
        VerticalContentAlignment = VerticalAlignment.Center;
        HorizontalContentAlignment = HorizontalAlignment.Center;
        Padding = new Thickness(0);
        Margin = new Thickness(0);
    }

    public Icons Icon
    {
        get => (Icons)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public double IconSize
    {
        get => (double)GetValue(IconSizeProperty);
        set => SetValue(IconSizeProperty, value);
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        RefreshControls();
    }

    private static void IconChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        (d as XamlIcon)?.RefreshControls();
    }

    private static void IconSizeChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        (d as XamlIcon)?.RefreshControls();
    }

    public void RefreshControls()
    {
        if (Icon == Icons.None)
        {
            Content = null;
            return;
        }

        var iconStr = Icon.ToString();
        var key = $"/PracticalToolkit.WPF;component/Resources/Icons/{iconStr}.xaml";
        if (_globalIcon.TryGetValue(key, out var value))
        {
            value.Width = IconSize;
            value.Height = IconSize;
            value.HorizontalAlignment = HorizontalAlignment.Stretch;
            value.VerticalAlignment = VerticalAlignment.Stretch;
            Content = value;
        }
        else
        {
            var info = Application.GetResourceStream(new Uri(key, UriKind.Relative)) ?? throw new NullReferenceException(nameof(Icon));
            using var stream = info.Stream;
            var page = (Viewbox)XamlReader.Load(info.Stream);
            page.Width = IconSize;
            page.Height = IconSize;
            page.HorizontalAlignment = HorizontalAlignment.Stretch;
            page.VerticalAlignment = VerticalAlignment.Stretch;
            Content = page;
            _globalIcon.Add(key, page);
        }
    }
}

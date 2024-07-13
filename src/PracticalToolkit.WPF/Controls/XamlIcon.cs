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

    private readonly Dictionary<string, Viewbox> _globalIcon = [];

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

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        RefreshControls();
    }

    private static void IconChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
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
            Content = value;
        }
        else
        {
            var info = Application.GetResourceStream(new Uri(key, UriKind.Relative)) ?? throw new NullReferenceException(nameof(Icon));
            using var stream = info.Stream;
            var page = (Viewbox)XamlReader.Load(info.Stream);
            Content = page;
            _globalIcon.Add(key, page);
        }
    }
}
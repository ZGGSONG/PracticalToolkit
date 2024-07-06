using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PracticalToolkit.WPF.Converters;

public sealed class BooleanToVisibilityReverseConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var rtn = false;
        if (value is bool v)
        {
            rtn = v;
        }
        else if (value is bool?)
        {
            var tmp = (bool?)value;
            rtn = (bool)tmp;
        }

        return rtn ? Visibility.Collapsed : Visibility.Visible;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is Visibility visibility) return visibility == Visibility.Collapsed;

        return true;
    }
}
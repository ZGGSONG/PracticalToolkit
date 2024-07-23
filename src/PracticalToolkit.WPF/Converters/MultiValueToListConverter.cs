using System.Globalization;
using System.Windows.Data;
using Binding = System.Windows.Data.Binding;

namespace PracticalToolkit.WPF.Converters;

public sealed class MultiValueToListConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        return values.ToList();
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        return Enumerable.Repeat(Binding.DoNothing, targetTypes.Length).ToArray();
    }
}
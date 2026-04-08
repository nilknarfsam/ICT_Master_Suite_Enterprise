using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ICTMasterSuite.Presentation.Wpf.Converters;

public sealed class BooleanToVisibilityConverterEx : IValueConverter
{
    public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is true ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}

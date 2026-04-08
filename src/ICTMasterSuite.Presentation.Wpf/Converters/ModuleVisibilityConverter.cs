using System.Globalization;
using System.Windows;
using System.Windows.Data;
using ICTMasterSuite.Domain.Enums;

namespace ICTMasterSuite.Presentation.Wpf.Converters;

public sealed class ModuleVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not SystemModule module || parameter is not string expectedName)
        {
            return Visibility.Collapsed;
        }

        return module.ToString().Equals(expectedName, StringComparison.OrdinalIgnoreCase)
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}

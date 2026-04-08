using System.Globalization;
using System.Windows;
using System.Windows.Data;
using ICTMasterSuite.Domain.Enums;

namespace ICTMasterSuite.Presentation.Wpf.Converters;

public sealed class UsersModuleVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is SystemModule.UsersAndProfiles ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}

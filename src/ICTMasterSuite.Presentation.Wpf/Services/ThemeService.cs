using System.Windows;

namespace ICTMasterSuite.Presentation.Wpf.Services;

public enum AppTheme
{
    Light,
    Dark
}

public sealed class ThemeService
{
    public void ApplyTheme(AppTheme theme)
    {
        var themePath = theme == AppTheme.Dark
            ? "Themes/Colors.Dark.xaml"
            : "Themes/Colors.Light.xaml";

        var dictionaries = System.Windows.Application.Current.Resources.MergedDictionaries;
        if (dictionaries.Count == 0)
        {
            dictionaries.Add(new ResourceDictionary { Source = new Uri(themePath, UriKind.Relative) });
            return;
        }

        dictionaries[0] = new ResourceDictionary { Source = new Uri(themePath, UriKind.Relative) };
    }
}

using System;
using System.Globalization;
using System.Threading;
using System.Windows;
using TotallyNormalCalculator.MVVM.Model;

namespace TotallyNormalCalculator;

/// <summary>
/// Responsible for applying the user settings to the application.
/// </summary>
internal class SettingsService : ISettingsService
{
    public void ApplySettings(SettingsModel settings)
    {
        ApplyColorTheme(settings);
        ApplyLanguage(settings);
    }

    public void ApplyColorTheme(SettingsModel settings)
    {
        string newThemePath = settings.DarkModeActive ? "Themes/DarkTheme.xaml" : "Themes/LightTheme.xaml";
        string oldThemePath = settings.DarkModeActive ? "Themes/LightTheme.xaml" : "Themes/DarkTheme.xaml";
        var newTheme = (ResourceDictionary)Application.LoadComponent(new Uri(newThemePath, UriKind.Relative));
        var oldTheme = (ResourceDictionary)Application.LoadComponent(new Uri(oldThemePath, UriKind.Relative));
        Application.Current.Resources.MergedDictionaries.Remove(oldTheme);
        Application.Current.Resources.MergedDictionaries.Add(newTheme);
    }

    public void ApplyLanguage(SettingsModel settings)
    {
        var culture = new CultureInfo(settings.Language);
        Thread.CurrentThread.CurrentCulture = culture;
        Thread.CurrentThread.CurrentUICulture = culture;
    }
}

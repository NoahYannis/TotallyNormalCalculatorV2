using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace TotallyNormalCalculator.MVVM.ViewModels;
public partial class SecretViewViewModel : BaseViewModel
{
    [ObservableProperty]
    private bool _isLightTheme = true;

    public SecretViewViewModel()
    {
       SelectedViewModel = App.AppHost.Services.GetRequiredService<DiaryViewModel>();
    }


    [RelayCommand]
    public void SwitchTheme()
    {
        IsLightTheme = !IsLightTheme;

        string newThemePath = IsLightTheme ? "Themes/LightTheme.xaml" : "Themes/DarkTheme.xaml";
        string oldThemePath = IsLightTheme ? "Themes/DarkTheme.xaml" : "Themes/LightTheme.xaml";
        var newTheme = (ResourceDictionary)Application.LoadComponent(new Uri(newThemePath, UriKind.Relative));
        var oldTheme = (ResourceDictionary)Application.LoadComponent(new Uri(oldThemePath, UriKind.Relative));
        Application.Current.Resources.MergedDictionaries.Remove(oldTheme);
        Application.Current.Resources.MergedDictionaries.Add(newTheme);
    }
}

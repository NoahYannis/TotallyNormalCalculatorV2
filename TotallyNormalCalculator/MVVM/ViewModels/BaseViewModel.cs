using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;

namespace TotallyNormalCalculator.MVVM.ViewModels;
public partial class BaseViewModel : ObservableObject
{
    private ObservableObject _selectedViewModel;
    public ObservableObject SelectedViewModel
    {
        get => _selectedViewModel ??= App.AppHost.Services.GetRequiredService<CalculatorViewModel>();
        set => SetProperty(ref _selectedViewModel, value);
    }

    [RelayCommand]
    public static void MaximizeWindow()
    {
        Application.Current.MainWindow.WindowState =
            Application.Current.MainWindow.WindowState != WindowState.Maximized ? WindowState.Maximized : WindowState.Normal;
    }

    [RelayCommand]
    public static void MinimizeWindow()
    {
        Application.Current.MainWindow.WindowState = WindowState.Minimized;
    }

    [RelayCommand]
    public static void CloseWindow()
    {
        Application.Current.Shutdown();
    }
}

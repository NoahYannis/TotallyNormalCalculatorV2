using System;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;

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
    public void MaximizeWindow()
    {
        Application.Current.MainWindow.WindowState =
            Application.Current.MainWindow.WindowState != WindowState.Maximized ? WindowState.Maximized : WindowState.Normal;
    }

    [RelayCommand]
    public void MinimizeWindow()
    {
        Application.Current.MainWindow.WindowState = WindowState.Minimized;
    }

    [RelayCommand]
    public void CloseWindow()
    {
        Application.Current.Shutdown();
    }
}

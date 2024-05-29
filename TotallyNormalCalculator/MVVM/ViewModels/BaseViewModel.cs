using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;

namespace TotallyNormalCalculator.MVVM.ViewModels;
public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableObject _selectedViewModel;


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

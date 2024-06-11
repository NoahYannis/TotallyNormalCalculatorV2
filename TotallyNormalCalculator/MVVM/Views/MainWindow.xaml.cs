using System.Windows;
using TotallyNormalCalculator.MVVM.ViewModels;

namespace TotallyNormalCalculator.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        this.DataContext = App.AppHost.Services.GetService(typeof(MainViewModel));
    }
}

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;

namespace TotallyNormalCalculator.MVVM.ViewModels;

public class MainViewModel : ObservableObject
{
    private ObservableObject _selectedViewModel = App.AppHost.Services.GetRequiredService<CalculatorViewModel>();
    public ObservableObject SelectedViewModel => _selectedViewModel;
}

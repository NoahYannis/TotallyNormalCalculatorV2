using CommunityToolkit.Mvvm.ComponentModel;
using TotallyNormalCalculator.Logging;

namespace TotallyNormalCalculator.MVVM.ViewModels;

public class MainViewModel(ITotallyNormalCalculatorLogger logger) : ObservableObject
{
    private ObservableObject _selectedViewModel = new CalculatorViewModel(logger);
    public ObservableObject SelectedViewModel => _selectedViewModel;
}

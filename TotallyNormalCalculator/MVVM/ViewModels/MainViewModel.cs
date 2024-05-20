using CommunityToolkit.Mvvm.ComponentModel;

namespace TotallyNormalCalculator.MVVM.ViewModels;

public class MainViewModel : ObservableObject
{
    private ObservableObject _selectedViewModel = new CalculatorViewModel();
    public ObservableObject SelectedViewModel => _selectedViewModel;
}

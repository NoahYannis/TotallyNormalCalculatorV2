using System.Windows.Controls;
using TotallyNormalCalculator.MVVM.ViewModels;

namespace TotallyNormalCalculator.MVVM.Views;

public partial class DiaryView : UserControl
{
    public DiaryView()
    {
        InitializeComponent();
        this.DataContext = App.AppHost.Services.GetService(typeof(DiaryViewModel));
    }
}

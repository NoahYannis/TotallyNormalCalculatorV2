using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace TotallyNormalCalculator.MVVM.ViewModels;
public partial class SecretViewViewModel : BaseViewModel
{
    public SecretViewViewModel()
    {
        SelectedViewModel = App.AppHost.Services.GetRequiredService<DiaryViewModel>();
    }

    [RelayCommand]
    public void SwitchView(Type newViewModel)
    {
        SelectedViewModel = (BaseViewModel)App.AppHost.Services.GetRequiredService(newViewModel);
    }
}

using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TotallyNormalCalculator.Core;
using TotallyNormalCalculator.Logging;
using TotallyNormalCalculator.MVVM.ViewModels;
using TotallyNormalCalculator.Repository;
using TotallyNormalCalculator.MVVM.Views;

namespace TotallyNormalCalculator;

public partial class App : Application
{
    internal static IHost AppHost { get; set; }

    public App()
    {
        AppHost = Host.CreateDefaultBuilder()
         .ConfigureServices((context, services) =>
         {
             services.AddSingleton<BaseViewModel>();
             services.AddSingleton<SecretViewViewModel>();
             services.AddSingleton<CalculatorViewModel>();
             services.AddSingleton<DiaryViewModel>();

             services.AddSingleton(serviceProvider => new MainWindow
             {
                 DataContext = serviceProvider.GetRequiredService<BaseViewModel>()
             });

             services.AddSingleton<ITotallyNormalCalculatorLogger, TotallyNormalCalculatorLogger>();
             services.AddScoped<IDiaryRepository, DiaryRepositoryDapper>();
         })
         .Build();
    }
    protected override void OnStartup(StartupEventArgs e)
    {
        DBHelper.EnsureDatabaseExists();

        var mainWindow = AppHost.Services.GetRequiredService<MainWindow>();
        mainWindow.Show();

        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        await AppHost!.StopAsync();
        AppHost.Dispose();
        base.OnExit(e);
    }
}

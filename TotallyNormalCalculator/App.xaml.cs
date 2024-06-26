using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Windows;
using TotallyNormalCalculator.Core;
using TotallyNormalCalculator.Logging;
using TotallyNormalCalculator.MVVM.Model;
using TotallyNormalCalculator.MVVM.ViewModels;
using TotallyNormalCalculator.MVVM.Views;
using TotallyNormalCalculator.Repository;
using TotallyNormalCalculator.Repository.BlobStorage;

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
             services.AddSingleton<CalculatorViewModel>();
             services.AddSingleton<DiaryViewModel>();
             services.AddSingleton<BlobStorageViewModel>();
             services.AddTransient<SecretViewViewModel>();

             services.AddSingleton(serviceProvider => new MainWindow
             {
                 DataContext = serviceProvider.GetRequiredService<BaseViewModel>()
             });

             services.AddSingleton<ITotallyNormalCalculatorLogger, TotallyNormalCalculatorLogger>();
             services.AddScoped<IDiaryRepository, DiaryRepositoryDapper>();
             services.AddScoped<IBlobStorageRepository<BlobModel>, AzureBlobStorageRepository>();
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

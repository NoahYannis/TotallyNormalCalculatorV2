using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Windows;
using TotallyNormalCalculator.Core;
using TotallyNormalCalculator.Logging;
using TotallyNormalCalculator.MVVM.Model;
using TotallyNormalCalculator.MVVM.ViewModels;
using TotallyNormalCalculator.MVVM.Views;
using TotallyNormalCalculator.Repository;
using TotallyNormalCalculator.Repository.BlobStorage;
using TotallyNormalCalculator.Repository.Settings;

namespace TotallyNormalCalculator;

public partial class App : Application
{
    internal static IHost AppHost { get; private set; }
    internal static Guid UserGuid { get; private set; }

    public App()
    {
        AppHost = Host.CreateDefaultBuilder()
         .ConfigureServices((context, services) =>
         {
             services.AddSingleton<BaseViewModel>();
             services.AddSingleton<CalculatorViewModel>();
             services.AddSingleton<DiaryViewModel>();
             services.AddSingleton<BlobStorageViewModel>();
             services.AddSingleton<SettingsViewModel>();
             services.AddTransient<SecretViewViewModel>();

             services.AddSingleton(serviceProvider => new MainWindow
             {
                 DataContext = serviceProvider.GetRequiredService<BaseViewModel>()
             });

             services.AddSingleton<ITotallyNormalCalculatorLogger, TotallyNormalCalculatorLogger>();
             services.AddScoped<IDiaryRepository, DiaryRepositoryDapper>();
             services.AddScoped<IBlobStorageRepository<BlobModel>, AzureBlobStorageRepository>();
             services.AddScoped<ISettingsRepository<SettingsModel>, AzureCosmosDBSettingsRepository>();
             services.AddSingleton<SettingsService>();
         })
         .Build();
    }
    protected override void OnStartup(StartupEventArgs e)
    {
        DBHelper.EnsureDatabaseExists();
        UserGuid = GetUserGuid();

        var settingsRepository = AppHost.Services.GetRequiredService<ISettingsRepository<SettingsModel>>();
        var settingsService = AppHost.Services.GetRequiredService<SettingsService>();
        var settings = settingsRepository.GetUserSettings();
        settingsService.ApplySettings(settings);

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

    /// <summary>
    /// Returns the GUID of the associated user or creates a new one if none exists.
    /// </summary>
    /// <returns></returns>
    private static Guid GetUserGuid()
    {
        string userGuidPath = Path.Combine
            (Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "TotallyNormalCalculator", "tnc-user-guid.txt");

        if (File.Exists(userGuidPath))
        {
            Guid userGuid = Guid.Parse(File.ReadAllText(userGuidPath));
            return userGuid;
        }

        // New user, create a new GUID
        string folderPath = Path.Combine
            (Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "TotallyNormalCalculator");

        Directory.CreateDirectory(folderPath);
        Guid newGuid = Guid.NewGuid();
        File.WriteAllText(userGuidPath, newGuid.ToString());
        return newGuid;
    }
}

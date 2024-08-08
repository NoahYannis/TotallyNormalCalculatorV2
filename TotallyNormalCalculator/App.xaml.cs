using AutoUpdaterDotNET;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Windows;
using TotallyNormalCalculator.Logging;
using TotallyNormalCalculator.MVVM.Model;
using TotallyNormalCalculator.MVVM.Model.Blobs;
using TotallyNormalCalculator.MVVM.ViewModels;
using TotallyNormalCalculator.MVVM.Views;
using TotallyNormalCalculator.Repository;
using TotallyNormalCalculator.Repository.BlobStorage;
using TotallyNormalCalculator.Repository.Diary;
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
             services.AddTransient<SecretViewViewModel>();
             services.AddSingleton<DiaryViewModel>();
             services.AddSingleton<BlobStorageViewModel>();
             services.AddTransient<WebViewViewModel>();
             services.AddSingleton<SettingsViewModel>();
             services.AddSingleton<CalculatorViewModel>();

             services.AddHttpClient("tnc-http", client =>
             {
                 client.BaseAddress = new Uri("https://totallynormalcalculatorapi.azurewebsites.net");
                 //client.BaseAddress = new Uri("https://localhost:7203");
                 client.Timeout = TimeSpan.FromMinutes(2);

#if DEBUG
                 client.Timeout = TimeSpan.FromMinutes(5);
#endif
             });



             services.AddSingleton(serviceProvider => new MainWindow
             {
                 DataContext = serviceProvider.GetRequiredService<BaseViewModel>()
             });

             services.AddSingleton<IDiaryRepository, CosmosDiaryRepository /*DiaryRepositoryDapper*/>();
             services.AddSingleton<IBlobStorageRepository<BlobModel>, AzureBlobStorageRepository>();
             services.AddSingleton<ISettingsRepository<SettingsModel>, CosmosSettingsRepository>();

             services.AddSingleton<ITotallyNormalCalculatorLogger, TotallyNormalCalculatorLogger>();
             services.AddSingleton<ISettingsService, SettingsService>();
             services.AddSingleton<IMessageBoxService, MessageBoxService>();
             services.AddSingleton<IDialog, TncOpenFileDialog>();
             services.AddSingleton<IBlobFactory, BlobFactory>();
         })
         .Build();
    }
    protected override void OnStartup(StartupEventArgs e)
    {
        AppHost.Start();
        var logger = AppHost.Services.GetRequiredService<ITotallyNormalCalculatorLogger>();

        try
        {
            UserGuid = GetUserGuid();

            var settingsRepository = AppHost.Services.GetRequiredService<ISettingsRepository<SettingsModel>>();
            var settingsService = AppHost.Services.GetRequiredService<ISettingsService>();
            var settings = settingsRepository.GetUserSettings().GetAwaiter().GetResult();
            settingsService.ApplySettings(settings);

            AutoUpdater.InstalledVersion = new Version("1.5.5.3");
            AutoUpdater.Start("https://github.com/NoahYannis/TotallyNormalCalculatorV2/auto-uodate.xml");
            var mainWindow = AppHost.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();

        }
        catch (Exception ex)
        {
            logger.LogMessageToTempFile("Failed to start the application." + ex);
        }

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

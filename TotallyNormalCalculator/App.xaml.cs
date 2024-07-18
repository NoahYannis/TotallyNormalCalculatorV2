using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Windows;
using TotallyNormalCalculator.Logging;
using TotallyNormalCalculator.MVVM.Model;
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
             services.AddSingleton<CalculatorViewModel>();
             services.AddSingleton<DiaryViewModel>();
             services.AddSingleton<BlobStorageViewModel>();
             services.AddSingleton<SettingsViewModel>();
             services.AddTransient<SecretViewViewModel>();
             services.AddHttpClient("tnc-http", client =>
             {
                 client.BaseAddress = new Uri("https://totallynormalcalculatorapi.azurewebsites.net");
                 //client.BaseAddress = new Uri("https://localhost:7203");
                 client.Timeout = TimeSpan.FromMinutes(1);

                #if DEBUG
                 client.Timeout = TimeSpan.FromMinutes(5);
                #endif
             });



             services.AddSingleton(serviceProvider => new MainWindow
             {
                 DataContext = serviceProvider.GetRequiredService<BaseViewModel>()
             });

             services.AddSingleton<ITotallyNormalCalculatorLogger, TotallyNormalCalculatorLogger>();
             services.AddSingleton<IDiaryRepository, CosmosDiaryRepository /*DiaryRepositoryDapper*/>();
             services.AddSingleton<IBlobStorageRepository<BlobModel>, AzureBlobStorageRepository>();
             services.AddSingleton<ISettingsRepository<SettingsModel>, CosmosSettingsRepository>();
             services.AddSingleton<SettingsService>();
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
            var settingsService = AppHost.Services.GetRequiredService<SettingsService>();
            var settings = settingsRepository.GetUserSettings().GetAwaiter().GetResult();
            settingsService.ApplySettings(settings);

            var mainWindow = AppHost.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();

        }
        catch (Exception ex)
        {
            logger.LogMessageToTempFile("Failed to start the application." + ex);
        }

        //DBHelper.EnsureDatabaseExists();

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

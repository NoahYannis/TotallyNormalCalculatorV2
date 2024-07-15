﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
         .ConfigureAppConfiguration((context, config) =>
         {
             config.AddEnvironmentVariables();
             config.AddUserSecrets<App>();
         })
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
             services.AddScoped<IDiaryRepository, CosmosDiaryRepository /*DiaryRepositoryDapper*/>();
             services.AddScoped<IBlobStorageRepository<BlobModel>, AzureBlobStorageRepository>();
             services.AddScoped<ISettingsRepository<SettingsModel>, CosmosSettingsRepository>();
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
            var cosmosConnection = Environment.GetEnvironmentVariable("AZURE_COSMOS_DB_CONNECTION_STRING");
            var storageConnection = Environment.GetEnvironmentVariable("AZURE_BLOB_STORAGE_CONNECTION_STRING");
            logger.LogMessageToTempFile($"Cosmos Connection: {cosmosConnection}");
            logger.LogMessageToTempFile($"Storage Connection: {storageConnection}");
            var config = AppHost.Services.GetRequiredService<IConfiguration>();
            var cosmosConnection2 = config["AZURE_COSMOS_DB_CONNECTION_STRING"];
            var storageConnection2 = config["AZURE_BLOB_STORAGE_CONNECTION_STRING"];
            logger.LogMessageToTempFile($"Cosmos Connection 1: {cosmosConnection}");
            logger.LogMessageToTempFile($"Storage Connection 1: {storageConnection}");

            UserGuid = GetUserGuid();

            var settingsRepository = AppHost.Services.GetRequiredService<ISettingsRepository<SettingsModel>>();
            var settingsService = AppHost.Services.GetRequiredService<SettingsService>();
            var settings = settingsRepository.GetUserSettings();
            settingsService.ApplySettings(settings);

            var mainWindow = AppHost.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();

        }
        catch (Exception ee)
        {
            logger.LogMessageToTempFile("Failed to start the application." + ee);
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

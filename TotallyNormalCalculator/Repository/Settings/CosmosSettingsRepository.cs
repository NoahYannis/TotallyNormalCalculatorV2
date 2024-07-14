using Microsoft.Azure.Cosmos;
using System;
using System.Net;
using System.Threading.Tasks;
using TotallyNormalCalculator.Logging;
using System.Configuration;
using TotallyNormalCalculator.MVVM.Model;

namespace TotallyNormalCalculator.Repository.Settings;

internal class CosmosSettingsRepository : ISettingsRepository<SettingsModel>
{
    private readonly string _connectionString;
    private readonly string _cosmosDBName = "Users";
    private readonly string _cosmosContainerName = "Users";
    private readonly CosmosClient _cosmosClient;
    private readonly Container _cosmosContainer;
    private SettingsModel _userSettings;
    private readonly ITotallyNormalCalculatorLogger _logger;

    public CosmosSettingsRepository(ITotallyNormalCalculatorLogger logger)
    {
        _logger = logger;
        _connectionString = ConfigurationManager.ConnectionStrings["AzureCosmosDB"].ConnectionString;
        _cosmosClient = new CosmosClient(_connectionString);
        _cosmosContainer = _cosmosClient.GetContainer(_cosmosDBName, _cosmosContainerName);
        Task.Run(() => EnsureUserSettingsExist().GetAwaiter().GetResult());

        while(_userSettings == null)
            Task.Delay(100);
    }

    public SettingsModel GetUserSettings() => _userSettings;

    private async Task EnsureUserSettingsExist()
    {
        try
        {
            _userSettings = await _cosmosContainer.ReadItemAsync<SettingsModel>
                (App.UserGuid.ToString(), new PartitionKey(App.UserGuid.ToString()));
        }
        catch (CosmosException c) when (c.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogMessageToTempFile($"User settings for user '{App.UserGuid}' not found." +
                $" Creating new settings.");

            _userSettings = await _cosmosContainer.CreateItemAsync(new SettingsModel()
            {
                Id = App.UserGuid,
                DarkModeActive = false,
                Language = "en-US"
            });
        }
    }

    public async Task UpdateSettingsAsync(SettingsModel settingsModel)
    {
        try
        {
            await _cosmosContainer.UpsertItemAsync(settingsModel, new PartitionKey(settingsModel.Id.ToString()));
        }
        catch (Exception e)
        {
            _logger.LogExceptionToTempFile(e);
        }
    }
}

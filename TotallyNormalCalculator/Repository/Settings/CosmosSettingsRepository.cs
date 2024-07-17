using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TotallyNormalCalculator.Logging;
using TotallyNormalCalculator.MVVM.Model;

namespace TotallyNormalCalculator.Repository.Settings;

internal class CosmosSettingsRepository : ISettingsRepository<SettingsModel>
{
    private readonly ITotallyNormalCalculatorLogger _logger;
    private readonly HttpClient _http;
    private SettingsModel _userSettings;

    public CosmosSettingsRepository(ITotallyNormalCalculatorLogger logger,
        IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _http = httpClientFactory.CreateClient("tnc-http");


        _userSettings = Task.Run(() => GetUserSettings()).GetAwaiter().GetResult();

        while (_userSettings == null)
            Task.Delay(100);
    }
     
    public async Task<SettingsModel> GetUserSettings()
    {
        if (_userSettings == null)
        {
            _userSettings = await _http.GetFromJsonAsync<SettingsModel>($"/settings/{App.UserGuid}");
        }
        return _userSettings;
    }


    public async Task UpdateSettingsAsync(SettingsModel settingsModel)
    {
        try
        {
            await _http.PutAsJsonAsync($"/settings/update", settingsModel);
        }
        catch (Exception e)
        {
            _logger.LogExceptionToTempFile(e);
        }
    }
}

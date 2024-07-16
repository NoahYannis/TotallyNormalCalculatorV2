using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TotallyNormalCalculator.Logging;
using TotallyNormalCalculator.MVVM.Model;

namespace TotallyNormalCalculator.Repository.Diary;

internal class CosmosDiaryRepository : IDiaryRepository
{
    private readonly ITotallyNormalCalculatorLogger _logger;
    private readonly HttpClient _http;
    private readonly UserDiary _userDiary;

    public CosmosDiaryRepository(ITotallyNormalCalculatorLogger logger,
        IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _http = httpClientFactory.CreateClient("tnc-http");

        _userDiary = Task.Run(() => GetUserDiaryAsync(App.UserGuid)).GetAwaiter().GetResult();
    }

    public async Task AddDiaryEntry(DiaryEntryModel diaryEntry)
    {
        _userDiary.DiaryEntries.Add(diaryEntry);

        try
        {
            await _http.PostAsJsonAsync("/diary/add", diaryEntry);
        }
        catch (CosmosException ex)
        {
            _logger.LogMessageToTempFile($"An error occurred while connecting to the server: {ex.Message}");
        }
    }

    public async Task DeleteDiaryEntry(Guid entryId)
    {
        var entryToDelete = _userDiary.DiaryEntries.FirstOrDefault(e => e.Id == entryId);
        _userDiary.DiaryEntries.Remove(entryToDelete);

        try
        {
            await _http.DeleteAsync($"/diary/delete/{entryId}");
        }
        catch (CosmosException ex)
        {
            _logger.LogMessageToTempFile($"An error occurred while connecting to the server: {ex.Message}");
        }
    }


    public async Task<IEnumerable<DiaryEntryModel>> GetAllDiaryEntries()
    {
        return await Task.FromResult(_userDiary.DiaryEntries);
    }


    public async Task UpdateDiaryEntry(DiaryEntryModel diaryEntry)
    {
        var index = _userDiary.DiaryEntries.FindIndex(e => e.Id == diaryEntry.Id);

        if (index == -1)
            return;

        _userDiary.DiaryEntries[index] = diaryEntry;

        try
        {
            await _http.PutAsJsonAsync("/diary/update", diaryEntry);
        }
        catch (CosmosException ex)
        {
            _logger.LogMessageToTempFile($"An error occurred while connecting to the server: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets the <see cref="UserDiary"/> or creates a new one if it doesn't exist.
    /// </summary>
    /// <param name="userGuid">The user's GUID.</param>
    /// <returns>The user's diary.</returns>
    private async Task<UserDiary> GetUserDiaryAsync(Guid userGuid)
    {
        try
        {
            return await _http.GetFromJsonAsync<UserDiary>($"/diary/{userGuid}");
        }
        catch (Exception e)
        {
            _logger.LogMessageToTempFile($"An error occurred while connecting to the server: {e.Message}");
        }

        return new UserDiary();
    }

}

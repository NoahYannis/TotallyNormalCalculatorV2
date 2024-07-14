using Microsoft.Azure.Cosmos;
using System.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using TotallyNormalCalculator.Logging;
using TotallyNormalCalculator.MVVM.Model;
using System;
using System.Net;
using System.Linq;

namespace TotallyNormalCalculator.Repository.Diary;

internal class CosmosDiaryRepository : IDiaryRepository
{
    private readonly string _connectionString;
    private readonly string _cosmosDBName = "Users";
    private readonly string _cosmosContainerName = "Diary";
    private readonly CosmosClient _cosmosClient;
    private readonly Container _cosmosContainer;
    private readonly ITotallyNormalCalculatorLogger _logger;
    private readonly UserDiary _userDiary;

    public CosmosDiaryRepository(ITotallyNormalCalculatorLogger logger)
    {
        _logger = logger;
        _connectionString = ConfigurationManager.ConnectionStrings["AzureCosmosDB"].ConnectionString;
        _cosmosClient = new CosmosClient(_connectionString);
        _cosmosContainer = _cosmosClient.GetContainer(_cosmosDBName, _cosmosContainerName);
        _userDiary = Task.Run(() => GetUserDiaryAsync(App.UserGuid)).GetAwaiter().GetResult();
    }

    public async Task AddDiaryEntry(DiaryEntryModel diaryEntry)
    {
        _userDiary.DiaryEntries.Add(diaryEntry);

        try
        {
            await _cosmosContainer.ReplaceItemAsync
                (_userDiary, _userDiary.Id.ToString(), new PartitionKey(_userDiary.Id.ToString()));
        }
        catch (CosmosException ex)
        {
            _logger.LogExceptionToTempFile(ex);
        }
    }

    public async Task DeleteDiaryEntry(Guid entryId)
    {
        var entryToDelete = _userDiary.DiaryEntries.FirstOrDefault(e => e.Id == entryId);
        _userDiary.DiaryEntries.Remove(entryToDelete);

        try
        {
            await _cosmosContainer.ReplaceItemAsync
                (_userDiary, _userDiary.Id.ToString(), new PartitionKey(_userDiary.Id.ToString()));
        }
        catch (CosmosException ex)
        {
            _logger.LogExceptionToTempFile(ex);
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
            ItemResponse<UserDiary> response =
                await _cosmosContainer.ReplaceItemAsync
                (_userDiary, _userDiary.Id.ToString(), new PartitionKey(_userDiary.Id.ToString()));
        }
        catch (CosmosException ex)
        {
            _logger.LogExceptionToTempFile(ex);
        }
    }

    /// <summary>
    /// Gets the <see cref="UserDiary"/> or creates a new one if it doesn't exist.
    /// </summary>
    /// <param name="userGuid">The user's GUID.</param>
    /// <returns>The user's diary.</returns>
    private async Task<UserDiary> GetUserDiaryAsync(Guid userGuid)
    {
        ItemResponse<UserDiary> userDiaryResponse;

        try
        {
            userDiaryResponse = await _cosmosContainer.ReadItemAsync<UserDiary>
                (userGuid.ToString(), new PartitionKey(userGuid.ToString()));
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogMessageToTempFile($"User diary for user '{userGuid}' not found. Creating new diary.");

            var newUserDiary = new UserDiary
            {
                Id = userGuid,
                DiaryEntries = [],
            };

            userDiaryResponse = await _cosmosContainer.CreateItemAsync
                (newUserDiary, new PartitionKey(newUserDiary.Id.ToString()));
        }

        return userDiaryResponse.Resource;
    }

}

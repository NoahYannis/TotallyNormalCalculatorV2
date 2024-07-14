using Microsoft.Azure.Cosmos;
using System.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using TotallyNormalCalculator.Logging;
using TotallyNormalCalculator.MVVM.Model;
using System;

namespace TotallyNormalCalculator.Repository.Diary;

internal class CosmosDiaryRepository : IDiaryRepository
{
    private readonly string _connectionString;
    private readonly string _cosmosDBName = "Users";
    private readonly string _cosmosContainerName = "Diary";
    private readonly CosmosClient _cosmosClient;
    private readonly Container _cosmosContainer;
    private readonly ITotallyNormalCalculatorLogger _logger;

    public CosmosDiaryRepository(ITotallyNormalCalculatorLogger logger)
    {
        _logger = logger;
        _connectionString = ConfigurationManager.ConnectionStrings["AzureCosmosDB"].ConnectionString;
        _cosmosClient = new CosmosClient(_connectionString);
        _cosmosContainer = _cosmosClient.GetContainer(_cosmosDBName, _cosmosContainerName);
    }

    public async Task AddDiaryEntry(DiaryEntryModel diaryEntry)
    {
        try
        {
            ItemResponse<DiaryEntryModel> response = 
                await _cosmosContainer.CreateItemAsync
                (diaryEntry, new PartitionKey(diaryEntry.Id.ToString()));
        }
        catch (CosmosException ex)
        {
            _logger.LogExceptionToTempFile(ex);
        }
    }

    public async Task DeleteDiaryEntry(Guid id)
    {
        try
        {
            ItemResponse<DiaryEntryModel> response = 
                await _cosmosContainer.DeleteItemAsync<DiaryEntryModel>(id.ToString(), new PartitionKey(id.ToString()));
        }
        catch (CosmosException ex)
        {
            _logger.LogExceptionToTempFile(ex);
        }
    }


    public async Task<IEnumerable<DiaryEntryModel>> GetAllDiaryEntries()
    {
        var entries = new List<DiaryEntryModel>();

        try
        {
            var query = new QueryDefinition("SELECT * FROM c");

            using (FeedIterator<DiaryEntryModel> resultSetIterator = 
                _cosmosContainer.GetItemQueryIterator<DiaryEntryModel>(query))
            {
                while (resultSetIterator.HasMoreResults)
                {
                    FeedResponse<DiaryEntryModel> response = await resultSetIterator.ReadNextAsync();
                    entries.AddRange(response);
                }
            }
        }
        catch (CosmosException ex)
        {
            _logger.LogExceptionToTempFile(ex);
        }

        return entries;
    }


    public async Task UpdateDiaryEntry(DiaryEntryModel diaryEntry)
    {
        if (diaryEntry is null)
            return;

        try
        {
            ItemResponse<DiaryEntryModel> response =
                await _cosmosContainer.ReplaceItemAsync
                (diaryEntry, diaryEntry.Id.ToString(), new PartitionKey(diaryEntry.Id.ToString()));
        }
        catch (CosmosException ex)
        {
            _logger.LogExceptionToTempFile(ex);
        }
    }

}

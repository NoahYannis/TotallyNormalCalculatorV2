using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TotallyNormalCalculator.Logging;
using TotallyNormalCalculator.MVVM.Model;
using TotallyNormalCalculator.MVVM.Model.Blobs;

namespace TotallyNormalCalculator.Repository.BlobStorage;
internal class AzureBlobStorageRepository : IBlobStorageRepository<BlobModel>
{

    private ITotallyNormalCalculatorLogger _logger;

    private BlobContainerClient _blobContainerClient;


    public AzureBlobStorageRepository(ITotallyNormalCalculatorLogger logger)
    {
        _logger = logger;

        try
        {
            //string storageConnectionString = ConfigurationManager.ConnectionStrings["AzureBlobStorage"].ConnectionString;
            string storageConnectionString = Environment.GetEnvironmentVariable("AZURE_BLOB_STORAGE_CONNECTION_STRING");
            string containerName = App.UserGuid.ToString();

            _blobContainerClient = new BlobContainerClient(storageConnectionString, containerName);
            _blobContainerClient.CreateIfNotExists();
        }
        catch (Exception e)
        {
            _logger.LogExceptionToTempFile(e);
        }
    }


    public async Task<IEnumerable<BlobModel>> GetAllBlobs()
    {
        List<BlobModel> blobs = [];

        try
        {
            await foreach (var blobItem in _blobContainerClient.GetBlobsAsync())
            {
                var blobClient = _blobContainerClient.GetBlobClient(blobItem.Name);

                using (var stream = new MemoryStream())
                {
                    var blob = await BlobFactory.CreateBlobModel(blobClient, stream);

                    blobs.Add(blob);
                }
            }
        }
        catch (Exception e)
        {
            _logger.LogExceptionToTempFile(e);
        }

        return blobs;
    }


    public async Task DeleteBlob(string blobName)
    {
        try
        {
            var blob = _blobContainerClient.GetBlobClient(blobName);
            bool deletionSuccessful = await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);

            string message = deletionSuccessful ? $"{blobName} deleted successfully" : $"{blobName} was not found";
            _logger.LogMessageToTempFile(message);
        }
        catch (Exception e)
        {
            _logger.LogExceptionToTempFile(e);
        }
    }


    public async Task<BlobModel> UploadBlob(string filePath, string blobName)
    {
        BlobModel newBlob = null;

        try
        {
            BlobClient blobClient = _blobContainerClient.GetBlobClient(blobName);

            using (FileStream fileStream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite))
            {
                await blobClient.UploadAsync(fileStream, true);
                newBlob = await BlobFactory.CreateBlobModel(blobClient, fileStream);
            }

            _logger.LogMessageToTempFile($"{blobName} uploaded successfully");
        }
        catch (Exception e)
        {
            _logger.LogExceptionToTempFile(e);
        }

        return newBlob;
    }
}

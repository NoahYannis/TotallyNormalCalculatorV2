using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using TotallyNormalCalculator.Logging;
using TotallyNormalCalculator.MVVM.Model;

namespace TotallyNormalCalculator.Repository.BlobStorage;
internal class AzureBlobStorageRepository : IBlobStorageRepository<BlobModel>
{

    private ITotallyNormalCalculatorLogger _logger;

    private BlobContainerClient _blobContainerClient;


    public AzureBlobStorageRepository()
    {
        _logger = App.AppHost.Services.GetRequiredService<ITotallyNormalCalculatorLogger>();

        try
        {
            string storageConnectionString = ConfigurationManager.ConnectionStrings["AzureBlobStorage"].ConnectionString;
            string containerName = ConfigurationManager.AppSettings["ContainerName"];

            _blobContainerClient = new BlobContainerClient(storageConnectionString, containerName);
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
                    BitmapImage bitmapImage = await CreateBlobBitmapImage(blobClient, stream);

                    blobs.Add(new BlobModel { Name = blobItem.Name, Image = bitmapImage });
                }
            }
        }
        catch (Exception e)
        {
            _logger.LogExceptionToTempFile(e);
        }

        return blobs;
    }

    private static async Task<BitmapImage> CreateBlobBitmapImage(BlobClient blobClient, Stream stream)
    {
        await blobClient.DownloadToAsync(stream);
        stream.Position = 0;

        var bitmapImage = new BitmapImage();
        bitmapImage.BeginInit();
        bitmapImage.StreamSource = stream;
        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
        bitmapImage.EndInit();
        bitmapImage.Freeze();

        return bitmapImage;
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
        var newBlob = new BlobModel() { Name = blobName };

        try
        {
            BlobClient blobClient = _blobContainerClient.GetBlobClient(blobName);

            using (FileStream fileStream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite))
            {
                await blobClient.UploadAsync(fileStream, true);
                newBlob.Image = await CreateBlobBitmapImage(blobClient, fileStream);
            }

            _logger.LogMessageToTempFile($"{blobName} uploaded successfully");
        }
        catch (Exception e)
        {
            _logger.LogExceptionToTempFile(e);
        }

        return newBlob;
    }

    public async Task<byte[]> GetBlob(string blobName)
    {
        throw new NotImplementedException();
    }
}

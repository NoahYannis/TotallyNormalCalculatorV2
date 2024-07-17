using Azure.Storage.Blobs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TotallyNormalCalculator.Logging;
using System.Net.Http.Headers;
using TotallyNormalCalculator.MVVM.Model;
using TotallyNormalCalculator.MVVM.Model.Blobs;

namespace TotallyNormalCalculator.Repository.BlobStorage;
internal class AzureBlobStorageRepository : IBlobStorageRepository<BlobModel>
{

    private readonly ITotallyNormalCalculatorLogger _logger;
    private readonly BlobContainerClient _blobContainerClient;
    private readonly HttpClient _http;


    public AzureBlobStorageRepository(ITotallyNormalCalculatorLogger logger,
        IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _http = httpClientFactory.CreateClient("tnc-http");
    }


    public async Task<IEnumerable<BlobModel>> GetAllBlobs()
    {
        List<BlobModel> blobs = [];

        try
        {
            blobs = await _http.GetFromJsonAsync<List<BlobModel>>($"/blobs/{App.UserGuid}");
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
           await _http.DeleteAsync($"/blobs/delete/{blobName}");
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
            using (FileStream fileStream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                var content = new StreamContent(fileStream);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                using (var formData = new MultipartFormDataContent())
                {
                    formData.Add(content, "file", blobName);

                    var response = await _http.PostAsync("/blobs/upload", formData);
                    response.EnsureSuccessStatusCode();

                    newBlob = await BlobFactory.CreateBlobModel(blobName, fileStream);
                    //newBlob = await response.Content.ReadFromJsonAsync<BlobModel>();
                }
            }
        }
        catch (Exception e)
        {
            _logger.LogExceptionToTempFile(e);
        }

        return newBlob;
    }
}

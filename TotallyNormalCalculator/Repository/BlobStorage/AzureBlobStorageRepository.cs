using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TotallyNormalCalculator.Repository.BlobStorage;
internal class AzureBlobStorageRepository : IBlobStorageRepository
{
    public Task DeleteBlob(string blobName)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<byte[]> GetAllBlobsAsync()
    {
        throw new NotImplementedException();
    }

    public Task<byte[]> GetBlob(string blobName)
    {
        throw new NotImplementedException();
    }

    public Task UploadBlob(string blobName, byte[] blobData)
    {
        throw new NotImplementedException();
    }
}

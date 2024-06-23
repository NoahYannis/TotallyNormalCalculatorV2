using System.Collections.Generic;
using System.Threading.Tasks;

namespace TotallyNormalCalculator.Repository.BlobStorage;

interface IBlobStorageRepository
{
    IAsyncEnumerable<byte[]> GetAllBlobsAsync();
    Task<byte[]> GetBlob(string blobName);
    Task DeleteBlob(string blobName);
    Task UploadBlob(string blobName, byte[] blobData);
}

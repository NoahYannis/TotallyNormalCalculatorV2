using System.Collections.Generic;
using System.Threading.Tasks;

namespace TotallyNormalCalculator.Repository.BlobStorage;

interface IBlobStorageRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllBlobs();
    Task<byte[]> GetBlob(string blobName);
    Task DeleteBlob(string blobName);
    Task UploadBlob(string filePath, string blobName);
}

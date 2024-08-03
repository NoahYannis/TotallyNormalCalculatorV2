using System.Collections.Generic;
using System.Threading.Tasks;

namespace TotallyNormalCalculator.Repository.BlobStorage;

public interface IBlobStorageRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllBlobs();
    Task DeleteBlob(string blobName);
    Task<T> UploadBlob(string filePath, string blobName);
}

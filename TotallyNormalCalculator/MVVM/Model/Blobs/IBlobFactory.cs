using System.Threading.Tasks;

namespace TotallyNormalCalculator.MVVM.Model.Blobs;
public interface IBlobFactory
{
    Task<BlobModel> CreateBlobModel(string blobName, string contentBase64);
}
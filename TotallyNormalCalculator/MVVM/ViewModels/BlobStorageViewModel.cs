using TotallyNormalCalculator.Logging;
using TotallyNormalCalculator.Repository.BlobStorage;

namespace TotallyNormalCalculator.MVVM.ViewModels;
internal class BlobStorageViewModel
    (ITotallyNormalCalculatorLogger logger,
    IBlobStorageRepository blobStorageRepository) : BaseViewModel
{
}

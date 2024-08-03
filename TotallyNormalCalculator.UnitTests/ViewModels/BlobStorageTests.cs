using Moq;
using NUnit.Framework;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using TotallyNormalCalculator.MVVM.Model;
using TotallyNormalCalculator.MVVM.Model.Blobs;
using TotallyNormalCalculator.MVVM.ViewModels;
using TotallyNormalCalculator.Repository.BlobStorage;

namespace TotallyNormalCalculator.UnitTests.ViewModels;

[TestFixture]
public class BlobStorageTests
{
    private Mock<IBlobStorageRepository<BlobModel>> _blobStorageRepository;
    private Mock<IMessageBoxService> _messageService;
    private BlobStorageViewModel _blobStorageViewModel;

    [SetUp]
    public void Setup()
    {
        _blobStorageRepository = new Mock<IBlobStorageRepository<BlobModel>>();
        _messageService = new Mock<IMessageBoxService>();
        _blobStorageViewModel = new BlobStorageViewModel(null, _blobStorageRepository.Object, _messageService.Object);
    }



    [Test]
    public void LoadBlobs_WithNoBlobs_DoesNotThrowErrorAndReturnsEmptyList()
    {
        _blobStorageRepository.Setup(r => r.GetAllBlobs()).ReturnsAsync([]);

        Assert.DoesNotThrowAsync(_blobStorageViewModel.LoadBlobs);
        Assert.That(_blobStorageViewModel.Blobs, Is.EqualTo(new ObservableCollection<BlobModel>()));
    }


    [Test]
    public async Task LoadBlobs_WithImageBlob_LoadsImageBlobCorrectly()
    {
        _blobStorageRepository.Setup(r => r.GetAllBlobs()).ReturnsAsync(
        [
            new ImageBlob
            {
                Name = "dragonquest.png",
                ContentBase64 = "imageContent",
                BlobType = BlobType.Image,
            }
        ]);

        await _blobStorageViewModel.LoadBlobs();
        Assert.That(_blobStorageViewModel.Blobs.Count, Is.EqualTo(1));
    }
}

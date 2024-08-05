using Moq;
using NUnit.Framework;
using System.Collections.ObjectModel;
using System.Windows;
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
    private Mock<IDialog> _openFileDialog;
    private Mock<IBlobFactory> _blobFactory;
    private BlobStorageViewModel _blobStorageViewModel;

    [SetUp]
    public void Setup()
    {
        _blobStorageRepository = new Mock<IBlobStorageRepository<BlobModel>>();
        _messageService = new Mock<IMessageBoxService>();
        _openFileDialog = new Mock<IDialog>();
        _blobFactory = new Mock<IBlobFactory>();

        _blobStorageViewModel = new BlobStorageViewModel
            (null,
            _blobStorageRepository.Object,
            _messageService.Object,
            _openFileDialog.Object,
            _blobFactory.Object);

        _blobStorageViewModel.Blobs = [];
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
        var blob = new ImageBlob
        {
            Name = "dragonquest.png",
            ContentBase64 = "imageContent",
        };

        _blobStorageRepository.Setup(r => r.GetAllBlobs()).ReturnsAsync([blob]);

        _blobFactory.Setup(f => f.CreateBlobModel(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(blob);

        await _blobStorageViewModel.LoadBlobs();
        Assert.That(_blobStorageViewModel.Blobs.Count, Is.EqualTo(1));
    }


    [Test]
    public async Task UploadBlob_ShowDialogFalse_DoesNotUpload()
    {
        _openFileDialog.Setup(ofd => ofd.ShowDialog()).Returns(false);
        await _blobStorageViewModel.UploadBlob();
        _blobStorageRepository.Verify(r => r.UploadBlob(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }


    [Test]
    public async Task UploadBlob_WithChosenBlob_DoesUpload()
    {
        _openFileDialog.Setup(ofd => ofd.ShowDialog()).Returns(true);
        _openFileDialog.SetupProperty(ofd => ofd.FileName, "dragonquest.png");

        _blobFactory.Setup(b => b.CreateBlobModel(It.IsAny<string>(), It.IsAny<string>()));
        _blobFactory.Setup(b => b.GetBlobName(It.IsAny<string>())).Returns("dragonquest.png");
        _blobFactory.Setup(b => b.IsAllowedBlobType(It.IsAny<string>())).Returns(true);

        _blobStorageRepository.Setup(r => r.UploadBlob(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult<BlobModel>(new ImageBlob
        {
            Name = _openFileDialog.Object.FileName,
            ContentBase64 = "imageContent",
        }));

        await _blobStorageViewModel.UploadBlob();

        _blobStorageRepository.Verify(r => r.UploadBlob(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        Assert.That(_blobStorageViewModel.Blobs.Count, Is.EqualTo(1));
    }


    [Test]
    public void DeleteBlob_WithNoBlobs_DoesNotDeleteOrThrowException()
    {
        Assert.DoesNotThrowAsync(_blobStorageViewModel.DeleteBlob);
        _blobStorageRepository.Verify(r => r.DeleteBlob(It.IsAny<string>()), Times.Never);
    }


    [Test]
    public async Task DeleteBlob_WithBlob_DeletesBlob()
    {

        _blobStorageViewModel.Blobs =
        [
            new ImageBlob
            {
                Name = "dragonquest.png",
                ContentBase64 = "imageContent",
                BlobType = BlobType.Image,
            },
            new VideoBlob
            {
                Name = "pokemon.mp4",
                ContentBase64 = "videoContent",
                BlobType = BlobType.Video,
            }
        ];

        _blobStorageViewModel.SelectedElement = _blobStorageViewModel.Blobs[1];
        _messageService.Setup(m => m.ShowQuestion(It.IsAny<string>())).Returns(MessageBoxResult.Yes);

        await _blobStorageViewModel.DeleteBlob();
        Assert.That(_blobStorageViewModel.Blobs.Count, Is.EqualTo(1));
    }


    [Test]
    public async Task DeleteBlob_DialogResultNo_DoesNotDelete()
    {
        _messageService.Setup(m => m.ShowQuestion(It.IsAny<string>())).Returns(MessageBoxResult.No);
        _blobStorageViewModel.Blobs.Add(new BlobModel());
        await _blobStorageViewModel.DeleteBlob();
        Assert.That(_blobStorageViewModel.Blobs.Count, Is.EqualTo(1));
    }
}

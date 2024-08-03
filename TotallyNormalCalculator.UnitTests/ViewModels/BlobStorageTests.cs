using Moq;
using NUnit.Framework;
using TotallyNormalCalculator.MVVM.Model;
using TotallyNormalCalculator.MVVM.ViewModels;
using TotallyNormalCalculator.Repository.BlobStorage;

namespace TotallyNormalCalculator.UnitTests.ViewModels;

[TestFixture]
public class BlobStorageTests
{
    private Mock<IBlobStorageRepository<BlobModel>> _blobStorageService;
    private Mock<IMessageBoxService> _messageService;
    private BlobStorageViewModel _blobStorageViewModel;

    [SetUp]
    public void Setup()
    {
        _blobStorageService = new Mock<IBlobStorageRepository<BlobModel>>();
        _messageService = new Mock<IMessageBoxService>();
        //_blobStorageViewModel = new BlobStorageViewModel(null, _blobStorageService.Object);
    }

    /*Testfälle:
     * Laden: Wenn Repo keine Blobs enthält soll kein Fehler auftreten und Blobs ist leer
     * Wenn ein Image geladen wird, dann soll es korrekt geladen werden mit allen Eigenschaften
     * Wenn ein Video geladen wird, dann soll es korrekt geladen werden mit allen Eigenschaften
     * 
     * 
     * 
     * 
     * 
     * 
     * 
     */
}

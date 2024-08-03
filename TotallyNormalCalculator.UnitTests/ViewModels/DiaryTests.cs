using Moq;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.Generic;
using TotallyNormalCalculator.MVVM.Model;
using TotallyNormalCalculator.MVVM.ViewModels;
using TotallyNormalCalculator.Repository;

namespace TotallyNormalCalculator.UnitTests.ViewModels;

[TestFixture]
public class DiaryTests
{
    private Mock<IDiaryRepository> _diaryRepository;
    private Mock<IMessageBoxService> _messageService;
    private DiaryViewModel _diaryViewModel;

    private static List<DiaryEntryModel> _testEntries =
    [
        new() { Title = "Ruby", Message = "Chimchar", Date = "Today" },
        new() { Title = "Sapphire", Message = "Piplup", Date = "Yesterday" },
        new() { Title = "Emerald", Message = "Turtwig", Date = "Tomorrow" }
    ];


    [SetUp]
    public void Setup()
    {
        _diaryRepository = new Mock<IDiaryRepository>();
        _messageService = new Mock<IMessageBoxService>();
        _diaryRepository.Setup(r => r.GetAllDiaryEntries()).ReturnsAsync([]);
        _diaryViewModel = new DiaryViewModel(null, _diaryRepository.Object, _messageService.Object);
        _diaryViewModel.SelectedElement = new DiaryEntryModel() { Title = "Title", Message = "Message", Date = "Yesterday" };
    }


    [Test]
    public async Task AddEntry_NewEntry_GetsAdded()
    {
        _diaryRepository.Setup(r => r.AddDiaryEntry(It.IsAny<DiaryEntryModel>()))
            .Returns(Task.CompletedTask);

        await _diaryViewModel.AddEntry();

        Assert.That(_diaryViewModel.Entries.Any());
        _diaryRepository.Verify(r => r.AddDiaryEntry(It.IsAny<DiaryEntryModel>()), Times.Once);
    }


    [Test]
    public async Task UpdateEntry_NoSelectedEntry_DoesNotUpdate()
    {
        _diaryViewModel.SelectedElement = null;
        await _diaryViewModel.UpdateEntry();
        _diaryRepository.Verify(r => r.UpdateDiaryEntry(It.IsAny<DiaryEntryModel>()), Times.Never);
    }


    [Test]
    public async Task UpdateEntry_SelectedEntry_DoesUpdate()
    {
        _diaryViewModel.Title = "NewTitle";
        _diaryViewModel.Message = "NewMessage";
        _diaryViewModel.Date = "Today";

        await _diaryViewModel.UpdateEntry();
        _diaryRepository.Verify(r => r.UpdateDiaryEntry(It.IsAny<DiaryEntryModel>()), Times.Once);

        Assert.That(_diaryViewModel.SelectedElement.Title == "NewTitle");
        Assert.That(_diaryViewModel.SelectedElement.Message == "NewMessage");
        Assert.That(_diaryViewModel.SelectedElement.Date == "Today");
    }



    [Test]
    public void ReadEntry_NoEntries_DoesNotSetValues()
    {
        _diaryViewModel.ReadEntry(_diaryViewModel.SelectedElement);
        Assert.That(_diaryViewModel.Title != _diaryViewModel.SelectedElement.Title);
    }


    [Test]
    public void ReadEntry_WithEntry_SetsValuesCorrectly()
    {
        _diaryViewModel.Entries.Add(_diaryViewModel.SelectedElement);
        _diaryViewModel.ReadEntry(_diaryViewModel.SelectedElement);

        Assert.That(_diaryViewModel.Title == _diaryViewModel.SelectedElement.Title);
        Assert.That(_diaryViewModel.Message == _diaryViewModel.SelectedElement.Message);
        Assert.That(_diaryViewModel.Date == _diaryViewModel.SelectedElement.Date);
    }


    [Test]
    public async Task DeleteEntry_MessageResultNo_DoesNotDeleteEntry()
    {
        _messageService.Setup(m => m.Show(It.IsAny<string>())).Returns(MessageBoxResult.No);
        _diaryViewModel.Entries.Add(_diaryViewModel.SelectedElement);
        await _diaryViewModel.DeleteEntry();
        Assert.That(_diaryViewModel.Entries.Any());
    }


    [Test]
    public async Task DeleteEntry_MessageResultYes_DeletesEntry()
    {
        _messageService.Setup(m => m.Show(It.IsAny<string>())).Returns(MessageBoxResult.Yes);
        _diaryViewModel.Entries.Add(_diaryViewModel.SelectedElement);
        await _diaryViewModel.DeleteEntry();
        Assert.That(!_diaryViewModel.Entries.Any());
    }


    [Test]
    [TestCaseSource(nameof(FilterEntriesTestCases))]
    public void FilterEntries_ReturnsEntryWithChosenFilter(
        string filterBy,
        string filterText,
        List<DiaryEntryModel> expectedEntries)
    {
        _testEntries.ForEach(te => _diaryViewModel.Entries.Add(te));
        _diaryViewModel.PropertyFilter = filterBy;
        _diaryViewModel.FilterText = filterText;
        _diaryViewModel.FilterEntries(filterText);
        Assert.That(_diaryViewModel.FilteredEntries, Is.EquivalentTo(expectedEntries));
    }


    [Test]
    public void FilterEntries_RemovingFilter_ReturnsAllEntries()
    {
        _testEntries.ForEach(te => _diaryViewModel.Entries.Add(te));
        _diaryViewModel.PropertyFilter = "Title";
        _diaryViewModel.FilterText = "Ruby";
        _diaryViewModel.FilterEntries("Ruby");
        _diaryViewModel.PropertyFilter = null;

        Assert.That(_diaryViewModel.Entries, Is.EqualTo(_testEntries));
    }


    public static IEnumerable<TestCaseData> FilterEntriesTestCases
    {
        get
        {
            yield return new TestCaseData("Title", "Sapphire", new List<DiaryEntryModel> { _testEntries[1] });
            yield return new TestCaseData("Message", "Chimchar", new List<DiaryEntryModel> { _testEntries[0] });
            yield return new TestCaseData("Date", "Tomorrow", new List<DiaryEntryModel> { _testEntries[2] });
        }
    }
}

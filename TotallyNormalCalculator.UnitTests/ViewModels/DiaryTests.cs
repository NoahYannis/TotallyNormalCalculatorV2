﻿using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
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
        _diaryViewModel = new DiaryViewModel(_diaryRepository.Object, _messageService.Object);
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
    public async Task DeleteEntry_MessageResultNo_DoesNotDeleteEntry()
    {
        _messageService.Setup(m => m.ShowQuestion(It.IsAny<string>())).Returns(MessageBoxResult.No);
        _diaryViewModel.Entries.Add(_diaryViewModel.SelectedElement);
        await _diaryViewModel.DeleteEntry();
        Assert.That(_diaryViewModel.Entries.Any());
    }


    [Test]
    public async Task DeleteEntry_MessageResultYes_DeletesEntry()
    {
        _messageService.Setup(m => m.ShowQuestion(It.IsAny<string>())).Returns(MessageBoxResult.Yes);
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
        _diaryViewModel.FilterText = "Ruby";
        _diaryViewModel.PropertyFilter = "Title";
        _diaryViewModel.FilterEntries("Ruby");
        _diaryViewModel.PropertyFilter = "None";

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

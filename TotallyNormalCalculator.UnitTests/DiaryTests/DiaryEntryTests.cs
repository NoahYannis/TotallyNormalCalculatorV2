using System;
using System.Collections.ObjectModel;
using System.Windows;
using TotallyNormalCalculator.MVVM.Model;
using Xunit;

namespace TotallyNormalCalculator.UnitTests.DiaryTests;

public class DiaryEntryTests
{
    ObservableCollection<DiaryEntryModel> Entries;

    [Theory]
    [InlineData("Day 1", "My message", "7.4.22")]
    public void EntryShouldBeAddedCorrectly(string title, string message, string date)
    {
        Entries = new();
        InsertDiaryEntry(title, message, date);

        Assert.True(Entries.Count is not 0);
        Assert.True(Entries[0].Title is "Day 1");
    }

    [Fact]
    public void InsertDiaryEntry(string title, string message, string date)
    {
        try
        {
            Entries.Add(new DiaryEntryModel { Title = title, Message = message, Date = date });
            //connection.Execute("dbo.spInsertDiaryEntry @Title, @Message, @Date", new { Title = title, Message = message, Date = date });
        }
        catch (Exception exc)
        {
            MessageBox.Show($"There was an error: {exc.Message}");
        }
    }

    [Fact]
    public void EntryShouldGetReadCorrectly()
    {
        Entries = new();

        Entries.Add(new DiaryEntryModel { Title = "One" });
        Entries.Add(new DiaryEntryModel { Title = "Two" });
        Entries.Add(new DiaryEntryModel { Title = "Three" });

        for (int i = 0; i < Entries.Count; i++)
        {
            Assert.Equal("One", Entries[0].Title);
            Assert.Equal("Two", Entries[1].Title);
            Assert.Equal("Three", Entries[2].Title);
        }
    }

    [Fact]
    public void EntryShouldGetDeleted()
    {
        Entries = new();
        Entries.Add(new DiaryEntryModel { Title = "Hi", Message = "Ho", Date = "Hu" });
        Entries.RemoveAt(0);

        Assert.True(Entries.Count is 0);
    }
}

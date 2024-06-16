using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using TotallyNormalCalculator.Logging;
using TotallyNormalCalculator.MVVM.Model;
using TotallyNormalCalculator.Repository;


namespace TotallyNormalCalculator.MVVM.ViewModels;

public partial class DiaryViewModel : BaseViewModel
{

    private ObservableCollection<DiaryEntryModel> _entries = new();

    public ObservableCollection<DiaryEntryModel> Entries
    {
        get => string.IsNullOrWhiteSpace(FilterText) ? _entries : FilteredEntries;
        set
        {
            SetProperty(ref _entries, value);
            OnPropertyChanged(nameof(Entries));
        }
    }

    [ObservableProperty]
    private ObservableCollection<DiaryEntryModel> _filteredEntries = new();

    [ObservableProperty]
    private string _propertyFilter;

    [ObservableProperty]
    private DiaryEntryModel _selectedEntry;

    [ObservableProperty]
    private string _message;

    [ObservableProperty]
    private string _title;

    [ObservableProperty]
    private string _date;

    [ObservableProperty]
    private string _filterText;

    private readonly ITotallyNormalCalculatorLogger _diaryLogger;
    private readonly IDiaryRepository _diaryRepository;


    public DiaryViewModel(ITotallyNormalCalculatorLogger logger, IDiaryRepository diaryRepository)
    {
        (_diaryLogger, _diaryRepository) = (logger, diaryRepository);
        Entries = Task.Run(() => this.GetAllEntries()).GetAwaiter().GetResult();
    }


    [RelayCommand]
    public void SwitchView()
    {
        SelectedViewModel = new CalculatorViewModel(_diaryLogger);
    }


    [RelayCommand]
    public async Task AddEntry()
    {
        var entry = new DiaryEntryModel
        {
            Title = Title,
            Message = Message,
            Date = Date
        };

        await _diaryRepository.AddDiaryEntry(entry);
        Entries.Add(entry);
        ClearInputFields();
    }


    [RelayCommand]
    public async Task UpdateEntry()
    {
        if (SelectedEntry is null)
            return;

        SelectedEntry.Title = Title;
        SelectedEntry.Message = Message;
        SelectedEntry.Date = Date;

        await _diaryRepository.UpdateDiaryEntry(SelectedEntry);
        CollectionViewSource.GetDefaultView(Entries).Refresh();
    }


    [RelayCommand]
    public void ReadEntry(DiaryEntryModel diaryEntry)
    {
        if (!Entries.Any())
        {
            MessageBox.Show("There are no entries to read. You should create one!",
                "TotallyNormalCalculator", MessageBoxButton.YesNo, MessageBoxImage.Information);
            return;
        }

        if (diaryEntry is null)
        {
            MessageBox.Show("Please select an entry to read.", "TotallyNormalCalculator",
                MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        Title = diaryEntry.Title;
        Message = diaryEntry.Message;
        Date = diaryEntry.Date;
    }


    [RelayCommand]
    public async Task DeleteEntry()
    {
        if (!Entries.Any())
        {
            MessageBox.Show("There are no entries to delete.", "TotallyNormalCalculator",
                MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        if (SelectedEntry is null)
        {
            MessageBox.Show("Please select an entry to delete.", "TotallyNormalCalculator",
                MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var delete = MessageBox.Show("Do you want to permanently delete this entry?", "TotallyNormalCalculator",
            MessageBoxButton.YesNo, MessageBoxImage.Question);

        if (delete is MessageBoxResult.Yes)
        {
            await ExecuteDeleteEntryAsync();
        }
    }

    [RelayCommand]
    private void FilterEntries(string text)
    {
        FilteredEntries = PropertyFilter switch
        {
            "Title" => FilterEntries(e => e.Title != null && e.Title.Contains(text)),
            "Message" => FilterEntries(e => e.Message != null && e.Message.Contains(text)),
            "Date" => FilterEntries(e => e.Date != null && e.Date.Contains(text)),
            _ => []
        };

        OnPropertyChanged(nameof(Entries));
    }

    /// <summary>
    /// Takes all diary entries and returns those that satisfy the condition.
    /// </summary>
    /// <param name="filterCondition"></param>
    /// <returns></returns>
    private ObservableCollection<DiaryEntryModel> FilterEntries(Func<DiaryEntryModel, bool> filterCondition)
    {
        return new ObservableCollection<DiaryEntryModel>(_entries.Where(filterCondition));
    }


    private async Task<ObservableCollection<DiaryEntryModel>> GetAllEntries()
    {
        var entries = await _diaryRepository.GetAllDiaryEntries();
        return new ObservableCollection<DiaryEntryModel>(entries);
    }


    private async Task ExecuteDeleteEntryAsync()
    {
        await _diaryRepository.DeleteDiaryEntry(SelectedEntry.Id);
        Entries.Remove(SelectedEntry);
        ClearInputFields();
    }


    partial void OnSelectedEntryChanged(DiaryEntryModel value)
    {
        if (value is not null)
        {
            ReadEntry(value);
        }
    }

    partial void OnFilterTextChanged(string value)
    {
        if (PropertyFilter is null)
        {
            return;
        }

        FilterEntries(value);
    }

    internal void ClearInputFields()
    {
        Title = Message = Date = string.Empty;
    }
}

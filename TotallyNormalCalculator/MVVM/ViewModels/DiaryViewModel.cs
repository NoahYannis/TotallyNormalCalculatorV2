using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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

    [ObservableProperty]
    private ObservableCollection<DiaryEntryModel> _entries;

    [ObservableProperty]
    private DiaryEntryModel _selectedEntry;

    [ObservableProperty]
    private string _message;

    [ObservableProperty]
    private string _title;

    [ObservableProperty]
    private string _date;

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

    internal void ClearInputFields()
    {
        Title = Message = Date = string.Empty;
    }
}

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Linq;
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
        _diaryLogger = logger;
        _diaryRepository = diaryRepository;
        Entries = GetAllEntries();
    }

    [RelayCommand]
    public void SwitchView()
    {
        SelectedViewModel = new CalculatorViewModel(_diaryLogger);
    }

    [RelayCommand]
    public void AddEntry()
    {
        var entry = new DiaryEntryModel
        {
            Title = Title,
            Message = Message,
            Date = Date
        };

        _diaryRepository.AddDiaryEntry(entry);
        Entries.Add(entry);
        ClearInputFields();
    }


    [RelayCommand]
    public void UpdateEntry()
    {
        if (SelectedEntry is null)
            return;

        using (IDbConnection connection = new SqlConnection(DBHelper.GetConnectionString("DiaryEntryDB")))
        {
            try
            {
                string sqlStatement = "UPDATE dbo.Entries SET Title = @Title, Message = @Message, Date = @Date WHERE Id = @Id";
                connection.Execute(sqlStatement, new { SelectedEntry.Id, Title, Message, Date });

                SelectedEntry.Title = Title;
                SelectedEntry.Message = Message;
                SelectedEntry.Date = Date;

                CollectionViewSource.GetDefaultView(Entries).Refresh();
            }
            catch (Exception exc)
            {
                MessageBox.Show($"Ein Fehler ist aufgetreten: {exc.Message}");
                _diaryLogger.LogExceptionToTempFile(exc);
            }
        }
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
    public void DeleteEntry()
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
            ExecuteDeleteEntry();
        }
    }


    private ObservableCollection<DiaryEntryModel> GetAllEntries()
    {
        return new ObservableCollection<DiaryEntryModel>(_diaryRepository.GetAllDiaryEntries());
    }

    partial void OnSelectedEntryChanged(DiaryEntryModel value)
    {
        if (value is not null)
        {
            ReadEntry(value);
        }
    }

    private void ExecuteDeleteEntry()
    {
        _diaryRepository.DeleteDiaryEntry(SelectedEntry.Id);
        Entries.Remove(SelectedEntry);
        ClearInputFields();
    }

    internal void ClearInputFields()
    {
        Title = Message = Date = string.Empty;
    }
}

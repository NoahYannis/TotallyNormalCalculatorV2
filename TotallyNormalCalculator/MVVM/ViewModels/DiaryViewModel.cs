using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dapper;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using TotallyNormalCalculator.Core;
using TotallyNormalCalculator.MVVM.Model;
using System.Data.SqlClient;
using TotallyNormalCalculator.Logging;


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

    private readonly TotallyNormalCalculatorLogger _diaryLogger = new();

    public DiaryViewModel()
    {
        Entries = GetAllEntries();
    }

    [RelayCommand]
    public void SwitchView()
    {
        SelectedViewModel = new CalculatorViewModel();
    }

    [RelayCommand]
    public void AddEntry()
    {
        using (IDbConnection connection = new SqlConnection(Helper.GetConnectionString("DiaryEntryDB")))
        {
            try
            {
                string sqlStatement = "INSERT INTO dbo.Entries (Title, Message, Date) VALUES (@Title, @Message, @Date)";
                connection.Execute(sqlStatement, new { Title, Message, Date });
                Entries.Add(new DiaryEntryModel { Title = Title, Message = Message, Date = Date });
            }
            catch (Exception exc)
            {
                MessageBox.Show($"Ein Fehler ist aufgetreten: {exc.Message}");
                _diaryLogger.LogExceptionToTempFile(exc);
                Entries.Remove(SelectedEntry);
            }

            ClearInputFields();
        }
    }

    [RelayCommand]
    public void UpdateEntry()
    {
        using (IDbConnection connection = new SqlConnection(Helper.GetConnectionString("DiaryEntryDB")))
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
            MessageBox.Show("Please select an entry to delete.", "TotallyNormalCalculator",
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
        var entries = new ObservableCollection<DiaryEntryModel>();

        try
        {
            using (IDbConnection connection = new SqlConnection(Helper.GetConnectionString("DiaryEntryDB")))
            {
                var output = connection.Query<DiaryEntryModel>("select * from dbo.Entries", new { Title, Message, Date });
                foreach (var item in output)
                {
                    entries.Add(item);
                }
            }
        }
        catch (Exception exc)
        {
            MessageBox.Show($"Ein Fehler ist aufgetreten: {exc.Message}");
            _diaryLogger.LogExceptionToTempFile(exc);
        }

        return entries;
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
        using (IDbConnection connection = new SqlConnection(Helper.GetConnectionString("DiaryEntryDB")))
        {
            try
            {
                connection.Execute("DELETE FROM dbo.Entries WHERE Id = @Id",
                    new { SelectedEntry.Id });

                Entries.Remove(SelectedEntry);
            }
            catch (Exception exc)
            {
                MessageBox.Show($"Ein Fehler ist aufgetreten: {exc.Message}");
                _diaryLogger.LogExceptionToTempFile(exc);
            }
        }

        ClearInputFields();
    }

    internal void ClearInputFields()
    {
        Title = Message = Date = string.Empty;
    }
}

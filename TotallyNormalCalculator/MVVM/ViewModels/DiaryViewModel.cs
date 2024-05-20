using CommunityToolkit.Mvvm.ComponentModel;
using Dapper;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Windows;
using TotallyNormalCalculator.Core;
using TotallyNormalCalculator.MVVM.Model;


namespace TotallyNormalCalculator.MVVM.ViewModels;

public partial class DiaryViewModel : ObservableObject
{
    public RelayCommand MinimizeCommand { get; set; }
    public RelayCommand MaximizeCommand { get; set; }
    public RelayCommand CloseWindowCommand { get; set; }
    public RelayCommand AddEntryCommand { get; set; }
    public RelayCommand ReadEntryCommand { get; set; }
    public RelayCommand DeleteEntryCommand { get; set; }
    public RelayCommand SwitchViewCommand { get; set; }


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

    [ObservableProperty]
    private ObservableObject _selectedViewModel;


    string appRoot = Directory.GetCurrentDirectory();

    public DiaryViewModel()
    {

        Entries = new ObservableCollection<DiaryEntryModel>();

        GetAllEntries(Title, Message, Date);

        MinimizeCommand = new RelayCommand(o =>
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        });

        MaximizeCommand = new RelayCommand(o =>
        {
            if (Application.Current.MainWindow.WindowState != WindowState.Maximized)
            {
                Application.Current.MainWindow.WindowState = WindowState.Maximized;
            }
            else
            {
                Application.Current.MainWindow.WindowState = WindowState.Normal;
            }
        });

        CloseWindowCommand = new RelayCommand(o =>
        {
            Application.Current.Shutdown();
        });

        AddEntryCommand = new RelayCommand(o =>
        {
            InsertDiaryEntry(Title, Message, Date);
        });

        ReadEntryCommand = new RelayCommand(o =>
        {
            if (Entries.Count > 0)
            {
                if (SelectedEntry is not null)  // user has selected an entry
                {
                    Title = SelectedEntry.Title;
                    Message = SelectedEntry.Message;
                    Date = SelectedEntry.Date;
                }
                else
                {
                    MessageBox.Show("Please select an entry to read.", "TotallyNormalCalculator", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                var result = MessageBox.Show("There is no entry to read. You should create one!", "TotallyNormalCalculator", MessageBoxButton.YesNo, MessageBoxImage.Information);
            }
        });

        DeleteEntryCommand = new RelayCommand(o =>
        {
            if (Entries.Count > 0) // if there is an entry to delete
            {
                var wantsToDeleteEntry = MessageBox.Show("Do you want to permanently delete this entry?", "TotallyNormalCalculator", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (wantsToDeleteEntry is MessageBoxResult.Yes)
                {
                    if (SelectedEntry is not null) // user has selected an entry to delete
                    {
                        DeleteDiaryEntry(SelectedEntry.Title, SelectedEntry.Message, SelectedEntry.Date);
                    }
                    else
                    {
                        MessageBox.Show("Please select an entry to delete.", "TotallyNormalCalculator", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            else
            {
                MessageBox.Show("There is no entry to delete.", "TotallyNormalCalculator", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        });

        SwitchViewCommand = new RelayCommand(o =>
        {
            SelectedViewModel = new CalculatorViewModel();
        });
    }

    public void GetAllEntries(string title, string message, string date)
    {
        try
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(@$"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=DiaryEntryDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;"))
            {
                var output = connection.Query<DiaryEntryModel>("select * from dbo.Entries", new { Title = title, Message = message, Date = date });

                foreach (var item in output)
                {
                    Entries.Add(item);
                }
            }
        }
        catch (Exception exc)
        {
            MessageBox.Show(exc.Message);
        }
    }

    public void InsertDiaryEntry(string title, string message, string date)
    {
        using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(@$"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=DiaryEntryDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;"))
        {
            try
            {
                Entries.Add(new DiaryEntryModel { Title = title, Message = message, Date = date });
                string sqlStatement = "INSERT INTO dbo.Entries (Title, Message, Date) VALUES (@Title, @Message, @Date)";
                connection.Execute(sqlStatement, new { Title = title, Message = message, Date = date });
            }
            catch (Exception exc)
            {
                MessageBox.Show($"There was an error: {exc.Message}");
                Entries.Remove(SelectedEntry);
            }

            Title = "";
            Message = "";
            Date = "";
        }
    }

    public void DeleteDiaryEntry(string title, string message, string date)
    {
        using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(@$"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=DiaryEntryDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;"))
        {
            connection.Execute("DELETE FROM dbo.Entries WHERE Title = @Title AND Message = @Message AND Date = @Date", new { Title = title, Message = message, Date = date });
            Entries.Remove(SelectedEntry);
        }

        Title = "";
        Message = "";
        Date = "";
    }
}
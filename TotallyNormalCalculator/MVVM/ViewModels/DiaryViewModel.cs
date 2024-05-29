using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
        Entries = [];
        GetAllEntries(Title, Message, Date);
    }


    [RelayCommand]
    public void MaximizeWindow()
    {
        Application.Current.MainWindow.WindowState =
            Application.Current.MainWindow.WindowState != WindowState.Maximized ? WindowState.Maximized : WindowState.Normal;
    }


    [RelayCommand]
    public void MinimizeWindow()
    {
        Application.Current.MainWindow.WindowState = WindowState.Minimized;
    }


    [RelayCommand]
    public void CloseWindow()
    {
        Application.Current.Shutdown();
    }


    [RelayCommand]
    public void SwitchView()
    {
        SelectedViewModel = new CalculatorViewModel();
    }


    [RelayCommand]
    public void AddEntry()
    {
        InsertDiaryEntry(Title, Message, Date);
    }

    [RelayCommand]
    public void ReadEntry()
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
    }

    [RelayCommand]
    public void DeleteEntry()
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
﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dapper;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows;
using TotallyNormalCalculator.Core;
using TotallyNormalCalculator.MVVM.Model;


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

    public DiaryViewModel()
    {
        Entries = GetAllEntries(Title, Message, Date);
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
        if (!Entries.Any())
        {
            MessageBox.Show("There are no entries to read. You should create one!",
                "TotallyNormalCalculator", MessageBoxButton.YesNo, MessageBoxImage.Information);
            return;
        }

        if (SelectedEntry is null)
        {
            MessageBox.Show("Please select an entry to read.", "TotallyNormalCalculator",
                MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        Title = SelectedEntry.Title;
        Message = SelectedEntry.Message;
        Date = SelectedEntry.Date;
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
            ExecuteDeleteEntry(SelectedEntry.Title, SelectedEntry.Message, SelectedEntry.Date);
        }
    }


    private ObservableCollection<DiaryEntryModel> GetAllEntries(string title, string message, string date)
    {
        var entries = new ObservableCollection<DiaryEntryModel>();

        try
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.GetConnectionString("DiaryEntryDB")))
            {
                var output = connection.Query<DiaryEntryModel>("select * from dbo.Entries", new { Title = title, Message = message, Date = date });

                foreach (var item in output)
                {
                    entries.Add(item);
                }
            }
        }
        catch (Exception exc)
        {
            MessageBox.Show(exc.Message);
        }

        return entries;
    }

    private void InsertDiaryEntry(string title, string message, string date)
    {
        using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.GetConnectionString("DiaryEntryDB")))
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

            ClearInputFields();
        }
    }

    private void ExecuteDeleteEntry(string title, string message, string date)
    {
        using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.GetConnectionString("DiaryEntryDB")))
        {
            connection.Execute("DELETE FROM dbo.Entries WHERE Title = @Title AND Message = @Message AND Date = @Date",
                new { Title = title, Message = message, Date = date });
            Entries.Remove(SelectedEntry);
        }

        ClearInputFields();
    }

    private void ClearInputFields()
    {
        Title = Message = Date = string.Empty;
    }
}
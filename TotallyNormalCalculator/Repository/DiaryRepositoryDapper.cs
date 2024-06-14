using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Windows;
using TotallyNormalCalculator.Core;
using TotallyNormalCalculator.MVVM.Model;
using static System.Runtime.InteropServices.JavaScript.JSType;
using TotallyNormalCalculator.Logging;
using Dapper;

namespace TotallyNormalCalculator.Repository;
public class DiaryRepositoryDapper(ITotallyNormalCalculatorLogger logger) : IDiaryRepository
{
    public void AddDiaryEntry(DiaryEntryModel diaryEntry)
    {
        using IDbConnection connection = new SqlConnection(DBHelper.GetConnectionString("DiaryEntryDB"));

        try
        {
            string sqlStatement = "INSERT INTO dbo.Entries (Title, Message, Date) VALUES (@Title, @Message, @Date)";
            connection.Execute(sqlStatement, diaryEntry);
        }
        catch (Exception exc)
        {
            MessageBox.Show($"Ein Fehler ist aufgetreten: {exc.Message}");
            logger.LogExceptionToTempFile(exc);
        }
    }

    public void DeleteDiaryEntry(int id)
    {
        throw new NotImplementedException();
    }

    public IList<DiaryEntryModel> GetAllDiaryEntries()
    {
        throw new NotImplementedException();
    }

    public DiaryEntryModel GetDiaryEntryById(int id)
    {
        throw new NotImplementedException();
    }

    public void UpdateDiaryEntry(DiaryEntryModel diaryEntry)
    {
        throw new NotImplementedException();
    }
}

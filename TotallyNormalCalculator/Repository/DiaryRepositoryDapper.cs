using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Windows;
using TotallyNormalCalculator.Core;
using TotallyNormalCalculator.MVVM.Model;
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
        using (IDbConnection connection = new SqlConnection(DBHelper.GetConnectionString("DiaryEntryDB")))
        {
            try
            {
                connection.Execute("DELETE FROM dbo.Entries WHERE Id = @Id", new { id });

            }
            catch (Exception exc)
            {
                MessageBox.Show($"Ein Fehler ist aufgetreten: {exc.Message}");
                logger.LogExceptionToTempFile(exc);
            }
        }
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

        if (diaryEntry is null)
            return;

        //using (IDbConnection connection = new SqlConnection(DBHelper.GetConnectionString("DiaryEntryDB")))
        //{
        //    try
        //    {
        //        string sqlStatement = "UPDATE dbo.Entries SET Title = @Title, Message = @Message, Date = @Date WHERE Id = @Id";
        //        connection.Execute(sqlStatement, new { diaryEntry.Id, diaryEntry.Title, diaryEntry.Message, Date });

        //        SelectedEntry.Title = Title;
        //        SelectedEntry.Message = Message;
        //        SelectedEntry.Date = Date;

        //        CollectionViewSource.GetDefaultView(Entries).Refresh();
        //    }
        //    catch (Exception exc)
        //    {
        //        MessageBox.Show($"Ein Fehler ist aufgetreten: {exc.Message}");
        //        _diaryLogger.LogExceptionToTempFile(exc);
        //    }
        //}
    }
}

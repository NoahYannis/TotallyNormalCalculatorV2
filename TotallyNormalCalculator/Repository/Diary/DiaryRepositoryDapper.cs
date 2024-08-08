//using Dapper;
//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.SqlClient;
//using System.Threading.Tasks;
//using System.Windows;
//using TotallyNormalCalculator.Helper;
//using TotallyNormalCalculator.Logging;
//using TotallyNormalCalculator.MVVM.Model;

//namespace TotallyNormalCalculator.Repository;

//internal class DiaryRepositoryDapper(ITotallyNormalCalculatorLogger logger) : IDiaryRepository
//{
//    public async Task AddDiaryEntry(DiaryEntryModel diaryEntry)
//    {
//        using IDbConnection connection = new SqlConnection(DBHelper.GetConnectionString("DiaryEntryDB"));

//        try
//        {
//            string sqlStatement = "INSERT INTO dbo.Entries (Title, Message, Date) VALUES (@Title, @Message, @Date)";
//            await connection.ExecuteAsync(sqlStatement, diaryEntry);
//        }
//        catch (Exception exc)
//        {
//            MessageBox.Show($"Ein Fehler ist aufgetreten: {exc.Message}");
//            logger.LogExceptionToTempFile(exc);
//        }
//    }

//    public async Task DeleteDiaryEntry(Guid id)
//    {
//        using (IDbConnection connection = new SqlConnection(DBHelper.GetConnectionString("DiaryEntryDB")))
//        {
//            try
//            {
//                await connection.ExecuteAsync("DELETE FROM dbo.Entries WHERE Id = @Id", new { id });

//            }
//            catch (Exception exc)
//            {
//                MessageBox.Show($"Ein Fehler ist aufgetreten: {exc.Message}");
//                logger.LogExceptionToTempFile(exc);
//            }
//        }
//    }

//    public async Task<IEnumerable<DiaryEntryModel>> GetAllDiaryEntries()
//    {
//        IEnumerable<DiaryEntryModel> entries = null;

//        try
//        {
//            using (IDbConnection connection = new SqlConnection(DBHelper.GetConnectionString("DiaryEntryDB")))
//            {
//                entries = await connection.QueryAsync<DiaryEntryModel>("select * from dbo.Entries");
//            }
//        }
//        catch (Exception exc)
//        {
//            MessageBox.Show($"Ein Fehler ist aufgetreten: {exc.Message}");
//            logger.LogExceptionToTempFile(exc);
//        }

//        return entries;
//    }

//    public async Task UpdateDiaryEntry(DiaryEntryModel diaryEntry)
//    {
//        if (diaryEntry is null)
//            return;

//        using (IDbConnection connection = new SqlConnection(DBHelper.GetConnectionString("DiaryEntryDB")))
//        {
//            try
//            {
//                string sqlStatement = "UPDATE dbo.Entries SET Title = @Title, Message = @Message, Date = @Date WHERE Id = @Id";
//                await connection.ExecuteAsync(sqlStatement, diaryEntry);
//            }
//            catch (Exception exc)
//            {
//                MessageBox.Show($"Ein Fehler ist aufgetreten: {exc.Message}");
//                logger.LogExceptionToTempFile(exc);
//            }
//        }
//    }
//}

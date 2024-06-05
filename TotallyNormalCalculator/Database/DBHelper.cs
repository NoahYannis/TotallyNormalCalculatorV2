using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using TotallyNormalCalculator.Logging;

namespace TotallyNormalCalculator.Core;

/// <summary>
/// A helper class to retrieve the DB connection strings and to create the database if it doesn't exist.
/// </summary>
public static class DBHelper
{
    private static readonly TotallyNormalCalculatorLogger _logger = new TotallyNormalCalculatorLogger();

    public static string GetConnectionString(string name)
    {
        try
        {
            return ConfigurationManager.ConnectionStrings[name]?.ConnectionString;
        }
        catch (Exception exc)
        {
            _logger.LogMessageToTempFile($"Error loading connection string '{name}': {exc.Message}");
            _logger.LogExceptionToTempFile(exc);
        }

        return string.Empty;
    }

    public static bool CheckIfAppDBExists(string connectionString)
    {
        string query = "SELECT COUNT(*) FROM sys.databases WHERE name = 'DiaryEntryDB'";

        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            using (var command = new SqlCommand(query, connection))
            {
                int count = (int)command.ExecuteScalar();
                return count > 0;
            }
        }
    }

    public static void CreateDiaryEntryDB(string connectionString)
    {
        string path = "Database\\CreateDiaryEntryDB_Script.sql";

        try
        {
            string createDBscript = File.ReadAllText(path);

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(createDBscript, connection))
                {
                    command.ExecuteNonQuery();
                    _logger.LogMessageToTempFile("Database DiaryEntryDB was created successfully");
                }
                connection.Close();
            }
        }
        catch (Exception exc)
        {
            _logger.LogMessageToTempFile($"Error creating the database: {exc.Message}");
            _logger.LogExceptionToTempFile(exc);
        }
    }

    public static void CreateEntriesTable(string connectionString)
    {
        try
        {
            string scriptPath = "Database\\CreateEntriesTable_Script.sql";
            string createTableScript = File.ReadAllText(scriptPath);
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(createTableScript, connection))
                {
                    command.ExecuteNonQuery();
                    _logger.LogMessageToTempFile("Successfully created Entries table");
                }
                connection.Close();
            }
        }
        catch (Exception exc)
        {
            _logger.LogMessageToTempFile($"Error creating the entries table: {exc.Message}");
            _logger.LogExceptionToTempFile(exc);
        }
    }

    public static void EnsureDatabaseExists(string databaseName)
    {
        string masterConnectionString = GetConnectionString("master");
        string diaryEntryConnectionString = GetConnectionString(databaseName);
      
        if (string.IsNullOrEmpty(diaryEntryConnectionString))
        {
            _logger.LogMessageToTempFile("Connection string for DiaryEntryDB is empty");
            return;
        }

        if (!CheckIfAppDBExists(masterConnectionString))
        {
            CreateDiaryEntryDB(masterConnectionString);
            CreateEntriesTable(diaryEntryConnectionString);
        }
    }
}

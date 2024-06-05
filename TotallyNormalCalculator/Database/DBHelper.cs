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


    /// <summary>
    /// Checks if the application database exists and creates it if necessary.
    /// </summary>
    /// <param name="masterConnectionString">The connection string for the master database.</param>
    public static void EnsureDatabaseExists()
    {
        try
        {
            // Check if the database exists within the master database (sys.databases)
            string masterConnectionString = GetConnectionString("master");

            if (!CheckIfAppDbExists(masterConnectionString))
            {
                CreateDatabase(masterConnectionString);
            }
        }
        catch (Exception e)
        {
            _logger.LogMessageToTempFile($"Error ensuring database exists: {e.Message}");
            _logger.LogExceptionToTempFile(e);
        }
    }

    private static bool CheckIfAppDbExists(string connectionString)
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

    private static void CreateDatabase(string connectionString)
    {
        string createDBScriptPath = "Database\\CreateDiaryEntryDB_Script.sql";
        string createTableScriptPath = "Database\\CreateEntriesTable_Script.sql";

        try
        {
            string createDBScript = File.ReadAllText(createDBScriptPath);
            string createTableScript = File.ReadAllText(createTableScriptPath);

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand(createDBScript, connection))
                {
                    command.ExecuteNonQuery();
                    _logger.LogMessageToTempFile("Successfully created DiaryEntryDB");
                }

                // Create the Entries table within the newly created DiaryEntryDB
                connection.ChangeDatabase("DiaryEntryDB");

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
            _logger.LogMessageToTempFile($"Error creating the database and entries table: {exc.Message}");
            _logger.LogExceptionToTempFile(exc);
        }
    }
}

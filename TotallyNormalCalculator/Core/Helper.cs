using System;
using TotallyNormalCalculator.Logging;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.IO;

namespace TotallyNormalCalculator.Core;

/// <summary>
/// A helper class to retrieve the DB connection strings and to create the database if it doesn't exist.
/// </summary>
public static class Helper
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


    public static void CreateDBIfNotExists(string connectionString)
    {
        try
        {
            string script = File.ReadAllText("Core\\dbo.Entries_CREATE_IF_NOT_EXISTS.sql");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(script, connection);
                command.ExecuteNonQuery();
                _logger.LogMessageToTempFile($"Database DiaryEntryDB exists or has been created - {DateTime.Now:dd.MM.yyyy HH:mm:ss}\n");
            }

        }
        catch (Exception e)
        {
            _logger.LogMessageToTempFile($"Error creating the database in CreateDBIfNotExists(): {e.Message} - {DateTime.Now:dd.MM.yyyy HH:mm:ss}\n");
            _logger.LogExceptionToTempFile(e);
        }
    }
}

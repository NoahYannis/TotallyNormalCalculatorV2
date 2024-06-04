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

    public static bool CheckIfDatabaseExists(string connectionString)
    {
        try
        {
            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                _logger.LogMessageToTempFile($"Successfully connected to the database - {DateTime.Now:dd.MM.yyyy HH:mm:ss}\n");
            }
            return true;
        }
        catch (Exception e)
        {
            _logger.LogMessageToTempFile($"Error connecting to the database in CheckIfDatabaseExists() with connection string '{connectionString}': {e.Message} - {DateTime.Now:dd.MM.yyyy HH:mm:ss}\n");
            return false;
        }
    }


    public static void CreateDB(string connectionString)
    {
        string script = File.ReadAllText("dbo.Entries_CREATE.sql");

        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(script, connection);
                command.ExecuteNonQuery();
                _logger.LogMessageToTempFile($"Database DiaryEntryDB created successfully - {DateTime.Now:dd.MM.yyyy HH:mm:ss}\n");
            }

        }
        catch (Exception e)
        {
            _logger.LogMessageToTempFile($"Error creating the database in CreateDB(): {e.Message} - {DateTime.Now:dd.MM.yyyy HH:mm:ss}\n");
            _logger.LogExceptionToTempFile(e);
        }
    }
}

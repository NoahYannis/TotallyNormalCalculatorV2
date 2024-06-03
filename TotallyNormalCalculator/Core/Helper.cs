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
    private static readonly TotallyNormalCalculatorLogger _logger = new();

    public static string GetConnectionString(string name)
    {
        try
        {
            return ConfigurationManager.ConnectionStrings[name]?.ConnectionString;
        }
        catch (Exception exc)
        {
            _logger.LogMessageToTempFile($"Fehler beim Laden des Connection Strings '{name}': {exc.Message}");
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
                _logger.LogMessageToTempFile($"Verbindung zur Datenbank erfolgreich hergestellt - {DateTime.Now}\n");
            }
            return true;
        }
        catch (Exception e)
        {
            _logger.LogMessageToTempFile($"Fehler beim Verbindungsaufbau zur Datenbank in CheckIfDatabaseExists(): {e.Message} - {DateTime.Now}\n");
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
                _logger.LogMessageToTempFile($"Datenbank DiaryEntryDB erfolgreich erstellt - {DateTime.Now}\n");
            }

        }
        catch (Exception e)
        {
            _logger.LogMessageToTempFile($"Fehler beim Erstellen der Datenbank in CreateDB(): {e.Message} - {DateTime.Now}\n");
            _logger.LogExceptionToTempFile(e);
        }
    }
}

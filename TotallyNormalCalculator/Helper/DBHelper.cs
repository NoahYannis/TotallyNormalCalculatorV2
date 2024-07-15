using Microsoft.Extensions.DependencyInjection;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using TotallyNormalCalculator.Logging;

namespace TotallyNormalCalculator.Helper;

/// <summary>
/// A helper class to retrieve the DB connection strings and to create the database if it doesn't exist.
/// </summary>
public static class DBHelper
{
    private static readonly ITotallyNormalCalculatorLogger _logger = App.AppHost.Services.GetRequiredService<ITotallyNormalCalculatorLogger>();

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
    /// Checks if the local application database exists and creates it if necessary.
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
                _logger.LogMessageToTempFile("Database does not exist. Creating database...");
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
        try
        {
            string createDBScript = LoadEmbeddedResource("TotallyNormalCalculator.Database.CreateDiaryEntryDB_Script.sql");
            string createTableScript = LoadEmbeddedResource("TotallyNormalCalculator.Database.CreateEntriesTable_Script.sql");

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

    /// <summary>
    /// Returns the read script as a string.
    /// </summary>
    /// <param name="resourceName">The name of the embedded resource.</param>
    /// <returns>The content of the embedded resource as a string.</returns>
    /// <exception cref="FileNotFoundException">Thrown when the resource is not found.</exception>
    private static string LoadEmbeddedResource(string resourceName)
    {
        try
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    throw new FileNotFoundException();
                }

                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
        catch (FileNotFoundException ex)
        {
            _logger.LogMessageToTempFile($"The embedded resource {resourceName} was not found: {ex.Message}");
        }

        return string.Empty;
    }
}

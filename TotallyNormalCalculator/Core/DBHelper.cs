using System;
using System.Configuration;
using System.Data.SqlClient;
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

    public static void CreateDBIfNotExists(string connectionString)
    {
        try
        {
            string query = "IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'DiaryEntryDB') CREATE DATABASE DiaryEntryDB";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
        catch (Exception exc)
        {
            _logger.LogMessageToTempFile($"Error creating the database: {exc.Message}");
            _logger.LogExceptionToTempFile(exc);
        }
    }

    public static bool TableExists(string connectionString)
    {
        try
        {
            string query = $"SELECT COUNT(*) FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Entries]') AND type in (N'U')";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    int count = Convert.ToInt32(command.ExecuteScalar());
                    return count > 0;
                }
            }
        }
        catch (Exception exc)
        {
            _logger.LogMessageToTempFile($"Error checking if table exists: {exc.Message}");
            _logger.LogExceptionToTempFile(exc);
            return false;
        }
    }

    public static void CreateTable(SqlConnection connection)
    {
        try
        {
            string query = $@"CREATE TABLE [dbo].[Entries] (
                            [Id] INT IDENTITY (1, 1) NOT NULL,
                            [Title] NVARCHAR(50) NULL,
                            [Message] NVARCHAR(MAX) NULL,
                            [Date] NVARCHAR(50) NULL
                        )";
            using (var command = new SqlCommand(query, connection))
            {
                command.ExecuteNonQuery();
            }
        }
        catch (Exception exc)
        {
            _logger.LogMessageToTempFile($"Error creating the table: {exc.Message}");
            _logger.LogExceptionToTempFile(exc);
        }
    }

}

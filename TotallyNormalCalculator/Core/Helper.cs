using System;
using TotallyNormalCalculator.Logging;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using Dapper;
using System.IO;
using System.Windows;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Common;

namespace TotallyNormalCalculator.Core;
public static class Helper
{
    private static readonly TotallyNormalCalculatorLogger _logger = new TotallyNormalCalculatorLogger();


    public static string GetConnectionString(string name)
    {
        try
        {
            return ConfigurationManager.ConnectionStrings[name].ConnectionString;
        }
        catch (Exception exc)
        {
            _logger.LogMessageToTempFile($"Fehler beim Laden des Connection Strings '{name}': {exc.Message}");
            _logger.LogExceptionToTempFile(exc);
        }

        return string.Empty;
    }

    public static void CreateDBIfNotExists(string connectionString)
    {
        using (IDbConnection connection = new SqlConnection(connectionString))
        {
            try
            {
                connection.Open();
            }
            catch (SqlException)
            {

                FileInfo file = new FileInfo("dbo.Entries_CREATE.sql");
                string script = file.OpenText().ReadToEnd();

                SqlConnection conn = new SqlConnection(connectionString);
                Server server = new Server(new ServerConnection(connectionString));

                server.ConnectionContext.ExecuteNonQuery(script);
                file.OpenText().Close();
            }
        }
    }
}

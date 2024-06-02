using System;
using System.Configuration;
using TotallyNormalCalculator.Logging;

namespace TotallyNormalCalculator.Core;

public static class Helper
{
    private static readonly TotallyNormalCalculatorLogger _logger = new();
    public static string GetConnectionString(string name)
    {
        try
        {

            return ConfigurationManager.ConnectionStrings[name].ConnectionString;
        }
        catch (Exception exc)
        {
            // Debugging
            _logger.LogMessageToTempFile($"ConfigurationManager.ConnectionStrings.Count: {ConfigurationManager.ConnectionStrings.Count}" +
                $"\nConfigurationManager.ConnectionStrings[0]: {ConfigurationManager.ConnectionStrings[0]}" +
                $"\nConnection string name: {name}" +
                $"\nConfigurationManager.ConnectionStrings[name] is null: {ConfigurationManager.ConnectionStrings[name] is null}" +
                $"\nConfigurationManager.ConnectionStrings[name]?.ConnectionString is null: {ConfigurationManager.ConnectionStrings[name]?.ConnectionString is null}");
           
            _logger.LogExceptionToTempFile(exc);
        }

        return string.Empty;
    }
}

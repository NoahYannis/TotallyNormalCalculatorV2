using System;
using TotallyNormalCalculator.Logging;
using System.Configuration;

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
}

using Microsoft.Extensions.Configuration;
using System;
using TotallyNormalCalculator.Logging;

namespace TotallyNormalCalculator.Core;
public static class Helper
{
    private static readonly TotallyNormalCalculatorLogger _logger = new TotallyNormalCalculatorLogger();
    private static readonly IConfiguration _configuration;

    static Helper()
    {
        _configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();
    }

    public static string GetConnectionString(string name)
    {
        try
        {
            return _configuration.GetConnectionString(name);
        }
        catch (Exception exc)
        {
            _logger.LogMessageToTempFile($"Fehler beim Laden des Connection Strings '{name}': {exc.Message}");
            _logger.LogExceptionToTempFile(exc);
        }

        return string.Empty;
    }
}

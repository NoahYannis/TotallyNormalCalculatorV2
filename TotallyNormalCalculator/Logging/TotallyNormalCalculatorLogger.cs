using System;
using System.IO;
using System.Windows;

namespace TotallyNormalCalculator.Logging;
public class TotallyNormalCalculatorLogger : ILogger
{
    public void LogExceptionToTempFile(Exception exc)
    {
        string logFolderPath = Path.Combine(Path.GetTempPath(), "TotallyNormalCalculator_Logs");
        string logFilePath = Path.Combine(logFolderPath, "tnc-exception.log");

        string logMessage =
            $"{DateTime.Now:dd.MM.yyyy HH:mm:ss}" +
            $"\n Exception: {exc.GetType()} " +
            $"\n Error Message: {exc.Message}" +
            $"\n Stack Trace: {exc.StackTrace} " +
            $"\n\n";

        try
        {
            if (!Directory.Exists(logFolderPath))
            {
                Directory.CreateDirectory(logFolderPath);
            }

            using (StreamWriter writer = new StreamWriter(logFilePath, true))
            {
                writer.WriteLine(logMessage);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to log error: {ex.Message}");
        }
    }
}

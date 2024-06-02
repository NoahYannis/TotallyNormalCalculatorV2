﻿using System;
using System.IO;
using System.Windows;

namespace TotallyNormalCalculator.Logging;
public class TotallyNormalCalculatorLogger : ILogger
{
    private static readonly string logFolderPath = Path.Combine(Path.GetTempPath(), "TotallyNormalCalculator_Logs");
    private static readonly string logFilePath = Path.Combine(logFolderPath, "tnc-exception.log");

    public void LogExceptionToTempFile(Exception exc)
    {

        string logMessage =
                $"{DateTime.Now:dd.MM.yyyy HH:mm:ss}" +
                $"\n Exception: {exc.GetType()} " +
                $"\n Error Message: {exc.Message}" +
                $"\n Stack Trace: {exc.StackTrace} " +
                $"\n\n";

        WriteToFile(logMessage);
    }

    public void LogMessageToTempFile(string logMessage)
    {
        WriteToFile(logMessage);
    }

    private void WriteToFile(string logMessage)
    {
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

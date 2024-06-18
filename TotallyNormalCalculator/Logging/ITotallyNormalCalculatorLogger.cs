using System;

namespace TotallyNormalCalculator.Logging;

public interface ITotallyNormalCalculatorLogger
{
    void LogExceptionToTempFile(Exception exc);
    void LogMessageToTempFile(string message);
}

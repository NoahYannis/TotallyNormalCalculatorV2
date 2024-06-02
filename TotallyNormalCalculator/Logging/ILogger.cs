using System;

namespace TotallyNormalCalculator.Logging;

public interface ILogger
{
    void LogExceptionToTempFile(Exception exc);
}

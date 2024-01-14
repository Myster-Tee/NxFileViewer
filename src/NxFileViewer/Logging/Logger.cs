using System;
using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer.Logging;

/// <summary>
/// Implements both <see cref="ILogger"/> for logging 
/// and <see cref="ILogSource"/> for observing log events
/// </summary>
public class LoggerSource : ILogSource, ILogger
{
    public event LogHandler? Log;


    public LogLevel Level { get; set; }


    void ILogger.Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
            return;

        var formattedMessage = formatter(state, exception);

        NotifyLog(logLevel, formattedMessage);
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel >= Level;
    }

    public IDisposable BeginScope<TState>(TState state) where TState : notnull
    {
        throw new NotSupportedException();
    }

    protected virtual void NotifyLog(LogLevel logLevel, string message)
    {
        Log?.Invoke(logLevel, message);
    }
}
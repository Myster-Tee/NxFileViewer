using System;

namespace Emignatik.NxFileViewer.Logging
{
    public static class Logger
    {
        public static event LogEventHandler Log;

        public static void LogInfo(string message)
        {
            NotifyLog(LogLevel.INFO, message, null);
        }

        public static void LogWarning(string message)
        {
            NotifyLog(LogLevel.WARNING, message, null);
        }

        public static void LogError(string message)
        {
            NotifyLog(LogLevel.ERROR, message, null);
        }

        public static void LogError(string message, Exception ex)
        {
            NotifyLog(LogLevel.ERROR, message, ex);
        }

        private static void NotifyLog(LogLevel logLevel, string message, Exception ex)
        {
            Log?.Invoke(null, new LogEventHandlerArgs(logLevel, message, ex));
        }
    }

    public delegate void LogEventHandler(object sender, LogEventHandlerArgs args);

    public class LogEventHandlerArgs
    {
        public LogLevel LogLevel { get; }

        public string Message { get; }

        public Exception Exception { get; }

        public LogEventHandlerArgs(LogLevel logLevel, string message, Exception exception)
        {
            LogLevel = logLevel;
            Message = message;
            Exception = exception;
        }
    }
}
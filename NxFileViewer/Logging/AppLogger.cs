using System;
using Emignatik.NxFileViewer.Settings;
using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer.Logging
{
    public class AppLogger : ILogger
    {
        private readonly IAppSettings _appSettings;
        private readonly LogHandler _logHandler;
        private readonly string? _categoryName;

        public AppLogger(IAppSettings appSettings, string? categoryName, LogHandler logHandler)
        {
            _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
            _logHandler = logHandler ?? throw new ArgumentNullException(nameof(logHandler));
            _categoryName = categoryName;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var message = "";
            if (IsEnabled(LogLevel.Debug) && _categoryName != null)
                message = _categoryName + ": ";

            var formattedMessage = formatter(state, exception);

            message += formattedMessage;
            _logHandler(logLevel, message);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel >= _appSettings.LogLevel;
        }

        public IDisposable? BeginScope<TState>(TState state)
        {
            return null;
        }
    }
}
using System;
using System.Collections.Concurrent;
using Emignatik.NxFileViewer.Settings;
using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer.Logging
{
    public class AppLoggerProvider : IAppLoggerProvider
    {
        private readonly IAppSettings _appSettings;

        public AppLoggerProvider(IAppSettings appSettings)
        {
            _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, newCategoryName => new AppLogger(_appSettings, newCategoryName, NotifyLog));
        }

        private void NotifyLog(LogLevel logLevel, string message)
        {
            Log?.Invoke(logLevel, message);
        }

        private readonly ConcurrentDictionary<string, AppLogger> _loggers = new ConcurrentDictionary<string, AppLogger>();

        public void Dispose() => _loggers.Clear();

        public event LogHandler? Log;

    }
}
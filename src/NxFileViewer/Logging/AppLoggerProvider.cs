using System;
using System.Collections.Concurrent;
using System.IO;
using Emignatik.NxFileViewer.Settings;
using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer.Logging;

public class AppLoggerProvider : IAppLoggerProvider
{
    private readonly IAppSettings _appSettings;
    private readonly SessionFileLog _fileLog = new(Path.Combine(AppContext.BaseDirectory, "Logs"), int.MaxValue);
    private bool _retentionConfigured;

    public AppLoggerProvider(IAppSettings appSettings)
    {
        _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
        _appSettings.PropertyChanged += OnSettingsChanged;
    }

    public void ConfigureRetention()
    {
        _retentionConfigured = true;
        _fileLog.Trim(_appSettings.LogFileRetentionCount);
    }

    private void OnSettingsChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (_retentionConfigured && (string.IsNullOrEmpty(e.PropertyName) || e.PropertyName == nameof(IAppSettings.LogFileRetentionCount)))
            _fileLog.Trim(_appSettings.LogFileRetentionCount);
    }

    public ILogger CreateLogger(string categoryName)
    {
        return _loggers.GetOrAdd(categoryName, newCategoryName => new AppLogger(_appSettings, newCategoryName, NotifyLog));
    }

    private void NotifyLog(LogLevel logLevel, string message)
    {
        _fileLog.Write(logLevel, message);
        Log?.Invoke(logLevel, message);
    }

    private readonly ConcurrentDictionary<string, AppLogger> _loggers = new ConcurrentDictionary<string, AppLogger>();

    public void Dispose()
    {
        _appSettings.PropertyChanged -= OnSettingsChanged;
        _fileLog.Dispose();
        _loggers.Clear();
    }

    public event LogHandler? Log;

}

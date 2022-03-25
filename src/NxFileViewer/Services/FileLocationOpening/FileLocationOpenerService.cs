using System;
using System.Diagnostics;
using Emignatik.NxFileViewer.Localization;
using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer.Services.FileLocationOpening;

public class FileLocationOpenerService : IFileLocationOpenerService
{
    private readonly ILogger _logger;

    public FileLocationOpenerService(ILoggerFactory loggerFactory)
    {
        _logger = (loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory))).CreateLogger(this.GetType());

    }
    public void OpenFileLocationSafe(string? filePath)
    {
        try
        {
            if (filePath == null)
                return;

            var argument = $"/select, \"{filePath}\"";
            Process.Start("explorer.exe", argument);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, LocalizationManager.Instance.Current.Keys.OpenFileLocation_Failed_Log.SafeFormat(filePath, ex.Message));
        }
    }
}
using System;
using System.Threading;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Tools;
using LibHac.Fs;
using LibHac.Tools.FsSystem;
using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer.Services.BackgroundTask.RunnableImpl;

public class SaveStorageRunnable : ISaveStorageRunnable
{
    private readonly IStreamToFileHelper _streamToFileHelper;

    private IStorage? _srcStorage;
    private string? _dstFilePath;
    private readonly ILogger _logger;


    public SaveStorageRunnable(IStreamToFileHelper streamToFileHelper, ILoggerFactory loggerFactory)
    {
        _streamToFileHelper = streamToFileHelper ?? throw new ArgumentNullException(nameof(streamToFileHelper));
        _logger = (loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory))).CreateLogger(this.GetType());
    }

    public bool SupportsCancellation => true;

    public bool SupportProgress => true;


    public void Setup(IStorage srcStorage, string dstFilePath)
    {
        _dstFilePath = dstFilePath ?? throw new ArgumentNullException(nameof(dstFilePath));
        _srcStorage = srcStorage ?? throw new ArgumentNullException(nameof(srcStorage));
    }

    public void Run(IProgressReporter progressReporter, CancellationToken cancellationToken)
    {
        if (_srcStorage == null || _dstFilePath == null)
            throw new InvalidOperationException($"{nameof(Setup)} should be called first.");

        try
        {
            _streamToFileHelper.Save(_srcStorage.AsStream(), _dstFilePath, cancellationToken, progressReporter.SetPercentage);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning(LocalizationManager.Instance.Current.Keys.Log_SaveStorageCanceled);
        }
    }

}

public interface ISaveStorageRunnable : IRunnable
{
    void Setup(IStorage srcStorage, string dstFilePath);
}
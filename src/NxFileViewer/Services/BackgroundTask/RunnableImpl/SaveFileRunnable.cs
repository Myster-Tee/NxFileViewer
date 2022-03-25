using System;
using System.IO;
using System.Threading;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Tools;
using LibHac.Fs.Fsa;
using LibHac.Tools.FsSystem;
using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer.Services.BackgroundTask.RunnableImpl;

public class SaveFileRunnable : ISaveFileRunnable
{
    private readonly IStreamToFileHelper _streamToFileHelper;

    private IFile? _srcFile;
    private string? _dstFilePath;
    private readonly ILogger _logger;

    public SaveFileRunnable(IStreamToFileHelper streamToFileHelper, ILoggerFactory loggerFactory)
    {
        _streamToFileHelper = streamToFileHelper ?? throw new ArgumentNullException(nameof(streamToFileHelper));
        _logger = (loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory))).CreateLogger(this.GetType());
    }

    public bool SupportsCancellation => true;

    public bool SupportProgress => true;

    public void Setup(IFile srcFile, string dstFilePath)
    {
        _srcFile = srcFile ?? throw new ArgumentNullException(nameof(srcFile));
        _dstFilePath = dstFilePath ?? throw new ArgumentNullException(nameof(dstFilePath));
    }

    public void Run(IProgressReporter progressReporter, CancellationToken cancellationToken)
    {
        if (_srcFile == null || _dstFilePath == null)
            throw new InvalidOperationException($"{nameof(Setup)} should be called first.");

        var progressText = LocalizationManager.Instance.Current.Keys.Status_SavingFile.SafeFormat(Path.GetFileName(_dstFilePath));
        progressReporter.SetText(progressText);

        try
        {
            _streamToFileHelper.Save(_srcFile.AsStream(), _dstFilePath, cancellationToken, progressReporter.SetPercentage);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning(LocalizationManager.Instance.Current.Keys.Log_SaveFileCanceled);
        }
    }
}

public interface ISaveFileRunnable : IRunnable
{
    void Setup(IFile srcFile, string dstFilePath);
}
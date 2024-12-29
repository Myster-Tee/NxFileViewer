using System;
using System.Threading.Tasks;
using System.Windows;
using Emignatik.NxFileViewer.FileLoading;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Models;
using Emignatik.NxFileViewer.Services.BackgroundTask;
using Emignatik.NxFileViewer.Services.BackgroundTask.RunnableImpl;
using Emignatik.NxFileViewer.Settings;
using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer.Services.FileOpening;

public class FileOpeningService : IFileOpeningService
{
    private readonly IAppSettings _appSettings;
    private readonly IFileLoader _fileLoader;
    private readonly IMainBackgroundTaskRunnerService _backgroundTaskRunnerService;
    private readonly ILogger _logger;

    public FileOpeningService(ILoggerFactory loggerFactory, IAppSettings appSettings, IFileLoader fileLoader, IMainBackgroundTaskRunnerService backgroundTaskRunnerService)
    {
        _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
        _fileLoader = fileLoader ?? throw new ArgumentNullException(nameof(fileLoader));
        _backgroundTaskRunnerService = backgroundTaskRunnerService ?? throw new ArgumentNullException(nameof(backgroundTaskRunnerService));
        _logger = loggerFactory.CreateLogger(this.GetType());
    }

    public async Task SafeOpenFile(string filePath)
    {
        try
        {
            _appSettings.LastOpenedFile = filePath;

            var runnableRelay = new RunnableRelay<NxFile>((reporter, _) =>
            {
                var loadingFilePleaseWait = LocalizationManager.Instance.Current.Keys.LoadingFile_PleaseWait;
                reporter.SetText(loadingFilePleaseWait);
                return _fileLoader.Load(filePath);
            })
            {
                SupportProgress = false,
                SupportsCancellation = false
            };

            _openedFile = await _backgroundTaskRunnerService.RunAsync(runnableRelay);

            NotifyOpenedFileChanged(_openedFile);
        }
        catch (FileNotSupportedException ex)
        {
            _logger.LogError(ex, LocalizationManager.Instance.Current.Keys.FileNotSupported_Log.SafeFormat(filePath));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, LocalizationManager.Instance.Current.Keys.LoadingError_Failed.SafeFormat(filePath, ex.Message), ex);
        }
    }

    public void SafeClose()
    {
        _openedFile?.Dispose();
        _openedFile = null;

        GC.Collect();
        GC.WaitForPendingFinalizers();

        NotifyOpenedFileChanged(null);
    }

    public event OpenedFileChangedHandler? OpenedFileChanged;

    private NxFile? _openedFile;

    public NxFile? OpenedFile => _openedFile;

    private void NotifyOpenedFileChanged(NxFile? newFile)
    {
        var dispatcher = Application.Current.Dispatcher;

        if (!dispatcher.CheckAccess())
        {
            dispatcher.InvokeAsync(() =>
            {
                NotifyOpenedFileChanged(newFile);
            });
        }
        else
        {
            OpenedFileChanged?.Invoke(this, new OpenedFileChangedHandlerArgs(newFile));
        }
    }
}
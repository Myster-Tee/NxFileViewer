﻿using System;
using System.Threading.Tasks;
using Emignatik.NxFileViewer.FileLoading;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Models;
using Emignatik.NxFileViewer.Services.BackgroundTask;
using Emignatik.NxFileViewer.Services.BackgroundTask.RunnableImpl;
using Emignatik.NxFileViewer.Settings;
using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer.Services.FileOpening;

public class FileOpenerService : IFileOpenerService
{
    private readonly IOpenedFileService _openedFileService;
    private readonly IAppSettings _appSettings;
    private readonly IFileLoader _fileLoader;
    private readonly IMainBackgroundTaskRunnerService _backgroundTaskRunnerService;
    private readonly ILogger _logger;

    public FileOpenerService(IOpenedFileService openedFileService, ILoggerFactory loggerFactory, IAppSettings appSettings, IFileLoader fileLoader, IMainBackgroundTaskRunnerService backgroundTaskRunnerService)
    {
        _openedFileService = openedFileService ?? throw new ArgumentNullException(nameof(openedFileService));
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

            var nxFile = await _backgroundTaskRunnerService.RunAsync(runnableRelay);

            _openedFileService.OpenedFile = nxFile;
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
}
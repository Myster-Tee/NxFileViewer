using System;
using System.Windows.Input;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Models.TreeItems.Impl;
using Emignatik.NxFileViewer.Services.BackgroundTask;
using Emignatik.NxFileViewer.Services.BackgroundTask.RunnableImpl;
using Emignatik.NxFileViewer.Services.Prompting;
using Emignatik.NxFileViewer.Utils.MVVM.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer.Commands;

public class SavePlaintextNcaFileCommand : CommandBase, ISavePlaintextNcaFileCommand
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IBackgroundTaskService _backgroundTaskService;
    private readonly ILogger _logger;
    private readonly IPromptService _promptService;

    private NcaItem? _ncaItem;


    public SavePlaintextNcaFileCommand(ILoggerFactory loggerFactory, IBackgroundTaskService backgroundTaskService, IServiceProvider serviceProvider, IPromptService promptService)
    {
        _logger = (loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory))).CreateLogger(this.GetType());
        _backgroundTaskService = backgroundTaskService ?? throw new ArgumentNullException(nameof(backgroundTaskService));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _promptService = promptService ?? throw new ArgumentNullException(nameof(promptService));
    }

    public NcaItem NcaItem
    {
        set
        {
            _ncaItem = value;
            TriggerCanExecuteChanged();
        }
    }
    public override async void Execute(object? parameter)
    {
        try
        {
            if (_ncaItem == null)
                return;

            var filePath = _promptService.PromptSaveFile(_ncaItem.FileName);
            if (filePath == null)
                return;

            var openDecryptedNca = _ncaItem.Nca.OpenDecryptedNca();
            var saveStorageRunnable = _serviceProvider.GetRequiredService<ISaveStorageRunnable>();
            saveStorageRunnable.Setup(openDecryptedNca, filePath);

            await _backgroundTaskService.RunAsync(saveStorageRunnable);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, LocalizationManager.Instance.Current.Keys.SaveFile_Error, ex.Message);
        }
    }

    public override bool CanExecute(object? parameter)
    {
        return _ncaItem != null && !_backgroundTaskService.IsRunning;
    }
}

public interface ISavePlaintextNcaFileCommand : ICommand
{
    NcaItem NcaItem { set; }
}
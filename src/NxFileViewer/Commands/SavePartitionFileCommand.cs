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

public class SavePartitionFileCommand : CommandBase, ISavePartitionFileCommand
{
    private readonly IMainBackgroundTaskRunnerService _backgroundTaskRunnerService;
    private readonly IServiceProvider _serviceProvider;
    private readonly IPromptService _promptService;
    private readonly ILogger _logger;
    private PartitionFileEntryItemBase? _partitionFileItem;

    public SavePartitionFileCommand(ILoggerFactory loggerFactory, IMainBackgroundTaskRunnerService backgroundTaskRunnerService, IServiceProvider serviceProvider, IPromptService promptService)
    {
        _logger = (loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory))).CreateLogger(this.GetType());
        _backgroundTaskRunnerService = backgroundTaskRunnerService ?? throw new ArgumentNullException(nameof(backgroundTaskRunnerService));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _promptService = promptService ?? throw new ArgumentNullException(nameof(promptService));
    }

    public PartitionFileEntryItemBase PartitionFileItem
    {
        set
        {
            _partitionFileItem = value;
            TriggerCanExecuteChanged();
        }
    }

    public override async void Execute(object? parameter)
    {
        if (_partitionFileItem == null)
            return;

        try
        {
            using var file = _partitionFileItem.LoadFile();

            var filePath = _promptService.PromptSaveFile(_partitionFileItem.Name);
            if (filePath == null)
                return;
            var runnable = _serviceProvider.GetRequiredService<ISaveFileRunnable>();
            runnable.Setup(file, filePath);

            await _backgroundTaskRunnerService.RunAsync(runnable);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, LocalizationManager.Instance.Current.Keys.SaveFile_Error.SafeFormat(ex.Message));
        }
    }

    public override bool CanExecute(object? parameter)
    {
        return _partitionFileItem != null && !_backgroundTaskRunnerService.IsRunning;
    }

}

public interface ISavePartitionFileCommand : ICommand
{
    PartitionFileEntryItemBase PartitionFileItem { set; }
}
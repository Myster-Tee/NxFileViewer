using System;
using System.IO;
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

public class SaveFsSectionContentCommand : CommandBase, ISaveFsSectionContentCommand
{
    private readonly IPromptService _promptService;
    private readonly IServiceProvider _serviceProvider;
    private readonly IMainBackgroundTaskRunnerService _backgroundTaskRunnerService;
    private readonly ILogger _logger;

    private FsSectionItem? _fsSectionItem;

    public SaveFsSectionContentCommand(IPromptService promptService, IServiceProvider serviceProvider, IMainBackgroundTaskRunnerService backgroundTaskRunnerService, ILoggerFactory loggerFactory)
    {
        _logger = (loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory))).CreateLogger(this.GetType());
        _promptService = promptService ?? throw new ArgumentNullException(nameof(promptService));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _backgroundTaskRunnerService = backgroundTaskRunnerService ?? throw new ArgumentNullException(nameof(backgroundTaskRunnerService));
    }

    public FsSectionItem FsSectionItem
    {
        set
        {
            _fsSectionItem = value;
            TriggerCanExecuteChanged();
        }
    }

    public override async void Execute(object? parameter)
    {
        if (_fsSectionItem == null)
            return;

        var selectedDir = _promptService.PromptSelectDir(LocalizationManager.Instance.Current.Keys.SaveDialog_Title);
        if (selectedDir == null)
            return;

        try
        {
            var targetDirPath = Path.Combine(selectedDir, $"Section_{_fsSectionItem.SectionIndex}");

            var runnable = _serviceProvider.GetRequiredService<ISaveDirectoryRunnable>();
            runnable.Setup(_fsSectionItem.ChildItems, targetDirPath);

            await _backgroundTaskRunnerService.RunAsync(runnable);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, LocalizationManager.Instance.Current.Keys.SaveFile_Error.SafeFormat(ex.Message));
        }

    }

    public override bool CanExecute(object? parameter)
    {
        return _fsSectionItem != null && !_backgroundTaskRunnerService.IsRunning;
    }
}

public interface ISaveFsSectionContentCommand : ICommand
{
    FsSectionItem FsSectionItem { set; }
}
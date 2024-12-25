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

public class SaveSectionContentCommand : CommandBase, ISaveSectionContentCommand
{
    private readonly IPromptService _promptService;
    private readonly IServiceProvider _serviceProvider;
    private readonly IMainBackgroundTaskRunnerService _backgroundTaskRunnerService;
    private readonly ILogger _logger;

    private SectionItem? _sectionItem;

    public SaveSectionContentCommand(IPromptService promptService, IServiceProvider serviceProvider, IMainBackgroundTaskRunnerService backgroundTaskRunnerService, ILoggerFactory loggerFactory)
    {
        _logger = (loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory))).CreateLogger(this.GetType());
        _promptService = promptService ?? throw new ArgumentNullException(nameof(promptService));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _backgroundTaskRunnerService = backgroundTaskRunnerService ?? throw new ArgumentNullException(nameof(backgroundTaskRunnerService));
    }

    public SectionItem SectionItem
    {
        set
        {
            _sectionItem = value;
            TriggerCanExecuteChanged();
        }
    }

    public override async void Execute(object? parameter)
    {
        if (_sectionItem == null)
            return;

        var selectedDir = _promptService.PromptSelectDir(LocalizationManager.Instance.Current.Keys.SaveDialog_Title);
        if (selectedDir == null)
            return;

        try
        {
            var targetDirPath = Path.Combine(selectedDir, $"Section_{_sectionItem.SectionIndex}");

            var runnable = _serviceProvider.GetRequiredService<ISaveDirectoryRunnable>();
            runnable.Setup(_sectionItem.ChildItems, targetDirPath);

            await _backgroundTaskRunnerService.RunAsync(runnable);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, LocalizationManager.Instance.Current.Keys.SaveFile_Error.SafeFormat(ex.Message));
        }

    }

    public override bool CanExecute(object? parameter)
    {
        return _sectionItem is { ChildItems.Length: > 0 } && !_backgroundTaskRunnerService.IsRunning;
    }
}

public interface ISaveSectionContentCommand : ICommand
{
    SectionItem SectionItem { set; }
}
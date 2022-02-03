using System;
using System.IO;
using System.Windows.Input;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Model.TreeItems.Impl;
using Emignatik.NxFileViewer.Services;
using Emignatik.NxFileViewer.Services.BackgroundTask;
using Emignatik.NxFileViewer.Services.BackgroundTask.RunnableImpl;
using Emignatik.NxFileViewer.Utils.MVVM.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer.Commands;

public class SaveSectionContentCommand : CommandBase, ISaveSectionContentCommand
{
    private readonly IPromptService _promptService;
    private readonly IServiceProvider _serviceProvider;
    private readonly IBackgroundTaskService _backgroundTaskService;
    private readonly ILogger _logger;

    private SectionItem? _sectionItem;

    public SaveSectionContentCommand(IPromptService promptService, IServiceProvider serviceProvider, IBackgroundTaskService backgroundTaskService, ILoggerFactory loggerFactory)
    {
        _logger = (loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory))).CreateLogger(this.GetType());
        _promptService = promptService ?? throw new ArgumentNullException(nameof(promptService));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _backgroundTaskService = backgroundTaskService ?? throw new ArgumentNullException(nameof(backgroundTaskService));
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

        var selectedDir = _promptService.PromptSaveDir();
        if (selectedDir == null)
            return;

        try
        {
            var targetDirPath = Path.Combine(selectedDir, $"Section_{_sectionItem.SectionIndex}");

            var runnable = _serviceProvider.GetRequiredService<ISaveDirectoryRunnable>();
            runnable.Setup(_sectionItem.ChildItems, targetDirPath);

            await _backgroundTaskService.RunAsync(runnable);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, LocalizationManager.Instance.Current.Keys.SaveFile_Error.SafeFormat(ex.Message));
        }

    }

    public override bool CanExecute(object? parameter)
    {
        return _sectionItem != null && !_backgroundTaskService.IsRunning;
    }
}

public interface ISaveSectionContentCommand : ICommand
{
    SectionItem SectionItem { set; }
}
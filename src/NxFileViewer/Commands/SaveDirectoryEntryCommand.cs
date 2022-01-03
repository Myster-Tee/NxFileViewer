using System;
using System.Windows.Input;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Model.TreeItems.Impl;
using Emignatik.NxFileViewer.Services;
using Emignatik.NxFileViewer.Services.BackgroundTask;
using Emignatik.NxFileViewer.Services.BackgroundTask.RunnableImpl;
using Emignatik.NxFileViewer.Utils.MVVM.Commands;
using LibHac.Fs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer.Commands
{
    public class SaveDirectoryEntryCommand : CommandBase, ISaveDirectoryEntryCommand
    {
        private readonly IPromptService _promptService;
        private readonly IServiceProvider _serviceProvider;
        private readonly IBackgroundTaskService _backgroundTaskService;
        private readonly ILogger _logger;

        private DirectoryEntryItem? _directoryEntryItem;

        public SaveDirectoryEntryCommand(IPromptService promptService, IServiceProvider serviceProvider, IBackgroundTaskService backgroundTaskService, ILoggerFactory loggerFactory)
        {
            _logger = (loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory))).CreateLogger(this.GetType());
            _promptService = promptService ?? throw new ArgumentNullException(nameof(promptService));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _backgroundTaskService = backgroundTaskService ?? throw new ArgumentNullException(nameof(backgroundTaskService));
        }

        public DirectoryEntryItem DirectoryEntryItem
        {
            set
            {
                _directoryEntryItem = value;
                TriggerCanExecuteChanged();
            }
        }

        public override async void Execute(object? parameter)
        {
            if (_directoryEntryItem == null)
                return;

            try
            {
                IRunnable runnable;
                if (_directoryEntryItem.DirectoryEntryType == DirectoryEntryType.File)
                {
                    var file = _directoryEntryItem.GetFile();
                    var filePath = _promptService.PromptSaveFile(_directoryEntryItem.Name);
                    if (filePath == null)
                        return;

                    var saveFileRunnable = _serviceProvider.GetRequiredService<ISaveFileRunnable>();
                    saveFileRunnable.Setup(file.Get, filePath);
                    runnable = saveFileRunnable;
                }
                else
                {
                    var dirPath = _promptService.PromptSaveDir();
                    if (dirPath == null)
                        return;
                    var saveDirectoryRunnable = _serviceProvider.GetRequiredService<ISaveDirectoryRunnable>();
                    saveDirectoryRunnable.Setup(new[] { _directoryEntryItem }, dirPath);
                    runnable = saveDirectoryRunnable;
                }

                await _backgroundTaskService.RunAsync(runnable);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, LocalizationManager.Instance.Current.Keys.SaveFile_Error.SafeFormat(ex.Message));
            }

        }

        public override bool CanExecute(object? parameter)
        {
            return _directoryEntryItem != null && !_backgroundTaskService.IsRunning;
        }

    }

    public interface ISaveDirectoryEntryCommand : ICommand
    {
        DirectoryEntryItem DirectoryEntryItem { set; }
    }
}

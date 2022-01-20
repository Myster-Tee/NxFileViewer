using System;
using System.Windows.Input;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Model.TreeItems.Impl;
using Emignatik.NxFileViewer.Services;
using Emignatik.NxFileViewer.Services.BackgroundTask;
using Emignatik.NxFileViewer.Services.BackgroundTask.RunnableImpl;
using Emignatik.NxFileViewer.Utils.MVVM.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer.Commands
{
    public class SavePartitionFileCommand : CommandBase, ISavePartitionFileCommand
    {
        private readonly IBackgroundTaskService _backgroundTaskService;
        private readonly IServiceProvider _serviceProvider;
        private readonly IPromptService _promptService;
        private readonly ILogger _logger;
        private PartitionFileEntryItemBase? _partitionFileItem;

        public SavePartitionFileCommand(ILoggerFactory loggerFactory, IBackgroundTaskService backgroundTaskService, IServiceProvider serviceProvider, IPromptService promptService)
        {
            _logger = (loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory))).CreateLogger(this.GetType());
            _backgroundTaskService = backgroundTaskService ?? throw new ArgumentNullException(nameof(backgroundTaskService));
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
                var file = _partitionFileItem.File;

                var filePath = _promptService.PromptSaveFile(_partitionFileItem.Name);
                if (filePath == null)
                    return;
                var runnable = _serviceProvider.GetRequiredService<ISaveFileRunnable>();
                runnable.Setup(file, filePath);

                await _backgroundTaskService.RunAsync(runnable);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, LocalizationManager.Instance.Current.Keys.SaveFile_Error.SafeFormat(ex.Message));
            }
        }

        public override bool CanExecute(object? parameter)
        {
            return _partitionFileItem != null && !_backgroundTaskService.IsRunning;
        }

    }

    public interface ISavePartitionFileCommand : ICommand
    {
        PartitionFileEntryItemBase PartitionFileItem { set; }
    }
}

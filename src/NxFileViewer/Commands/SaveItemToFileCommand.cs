using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Model.TreeItems.Impl;
using Emignatik.NxFileViewer.Services;
using Emignatik.NxFileViewer.Services.BackgroundTask;
using Emignatik.NxFileViewer.Services.BackgroundTask.RunnableImpl;
using Emignatik.NxFileViewer.Settings;
using Emignatik.NxFileViewer.Tools;
using Emignatik.NxFileViewer.Utils.MVVM.Commands;
using LibHac.Fs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Emignatik.NxFileViewer.Commands
{
    public class SaveItemToFileCommand : CommandBase, ISaveItemToFileCommand
    {
        private readonly IAppSettings _appSettings;
        private readonly ISelectedItemService _selectedItemService;
        private readonly IBackgroundTaskService _backgroundTaskService;
        private readonly IServiceProvider _serviceProvider;
        private readonly IFsSanitizer _fsSanitizer;
        private readonly ILogger _logger;

        public SaveItemToFileCommand(IAppSettings appSettings, ILoggerFactory loggerFactory, ISelectedItemService selectedItemService, IBackgroundTaskService backgroundTaskService, IServiceProvider serviceProvider, IFsSanitizer fsSanitizer)
        {
            _logger = (loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory))).CreateLogger(this.GetType());
            _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
            _selectedItemService = selectedItemService ?? throw new ArgumentNullException(nameof(selectedItemService));
            _backgroundTaskService = backgroundTaskService ?? throw new ArgumentNullException(nameof(backgroundTaskService));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _fsSanitizer = fsSanitizer ?? throw new ArgumentNullException(nameof(fsSanitizer));

            _selectedItemService.SelectedItemChanged += (_, _) =>
            {
                TriggerCanExecuteChanged();
            };
        }

        public override async void Execute(object? parameter)
        {
            try
            {
                var runnable = GetSaveRunnable();
                if (runnable != null)
                    await _backgroundTaskService.RunAsync(runnable);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, LocalizationManager.Instance.Current.Keys.SaveFile_Error.SafeFormat(ex.Message));
            }
        }

        private IRunnable? GetSaveRunnable()
        {
            var selectedItem = _selectedItemService.SelectedItem;

            if (selectedItem is DirectoryEntryItem directoryEntryItem)
            {
                if (directoryEntryItem.DirectoryEntryType == DirectoryEntryType.File)
                {
                    var file = directoryEntryItem.GetFile();
                    var filePath = PromptSaveFile(_fsSanitizer.SanitizeFileName(directoryEntryItem.Name));
                    if (filePath == null)
                        return null;
                    var runnable = _serviceProvider.GetRequiredService<ISaveFileRunnable>();
                    runnable.Setup(file, filePath);
                    return runnable;
                }
                else
                {
                    var dirPath = PromptSaveDir();
                    if (dirPath == null)
                        return null;
                    var runnable = _serviceProvider.GetRequiredService<ISaveDirectoryRunnable>();
                    runnable.Setup(new[] { directoryEntryItem }, dirPath);
                    return runnable;
                }
            }

            if (selectedItem is PartitionFileEntryItem partitionFileEntryItem)
            {
                var file = partitionFileEntryItem.File;
                var filePath = PromptSaveFile(_fsSanitizer.SanitizeFileName(partitionFileEntryItem.Name));
                if (filePath == null)
                    return null;
                var runnable = _serviceProvider.GetRequiredService<ISaveFileRunnable>();
                runnable.Setup(file, filePath);
                return runnable;
            }

            if (selectedItem is SectionItem sectionItem)
            {
                var dirPath = PromptSaveDir();
                if (dirPath == null)
                    return null;

                var targetDirPath = Path.Combine(dirPath, $"Section_{sectionItem.SectionIndex}");

                var runnable = _serviceProvider.GetRequiredService<ISaveDirectoryRunnable>();
                runnable.Setup(sectionItem.ChildDirectoryEntryItems, targetDirPath);
                return runnable;
            }

            return null;
        }

        private string? PromptSaveDir()
        {
            var fileDialog = new CommonOpenFileDialog
            {
                InitialDirectory = _appSettings.LastSaveDir,
                Multiselect = false,
                IsFolderPicker = true,
                Title = LocalizationManager.Instance.Current.Keys.SaveDialog_Title
            };

            if (fileDialog.ShowDialog(Application.Current.MainWindow) != CommonFileDialogResult.Ok)
                return null;

            var filePath = fileDialog.FileName;

            _appSettings.LastSaveDir = filePath;

            return filePath;
        }

        public override bool CanExecute(object? parameter)
        {
            var selectedItem = _selectedItemService.SelectedItem;
            return selectedItem is DirectoryEntryItem ||
                   selectedItem is PartitionFileEntryItem ||
                   selectedItem is SectionItem;
        }


        private string? PromptSaveFile(string proposedFileName)
        {
            var fileDialog = new CommonSaveFileDialog
            {
                Title = LocalizationManager.Instance.Current.Keys.SaveDialog_Title,
                InitialDirectory = _appSettings.LastSaveDir,

                Filters = { new CommonFileDialogFilter
                {
                    DisplayName = LocalizationManager.Instance.Current.Keys.SaveDialog_AnyFileFilter,
                    ShowExtensions = false
                } },
                DefaultFileName = proposedFileName,
            };

            if (fileDialog.ShowDialog(Application.Current.MainWindow) != CommonFileDialogResult.Ok)
                return null;

            var filePath = fileDialog.FileName;

            _appSettings.LastSaveDir = Path.GetDirectoryName(filePath)!;

            return filePath;
        }

    }

    public interface ISaveItemToFileCommand : ICommand
    {
    }
}

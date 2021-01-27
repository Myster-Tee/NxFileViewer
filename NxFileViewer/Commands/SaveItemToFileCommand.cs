using System;
using System.IO;
using System.Windows;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Model.TreeItems;
using Emignatik.NxFileViewer.Model.TreeItems.Impl;
using Emignatik.NxFileViewer.Settings;
using Emignatik.NxFileViewer.Utils.MVVM.Commands;
using LibHac.Common;
using LibHac.Fs;
using LibHac.FsSystem;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;

namespace Emignatik.NxFileViewer.Commands
{
    public class SaveItemToFileCommand : CommandBase, ISaveItemToFileCommand
    {
        private readonly IAppSettings _appSettings;
        private readonly ILogger _logger;
        private IItem? _item;

        public SaveItemToFileCommand(IAppSettings appSettings, ILoggerFactory loggerFactory)
        {
            _logger = (loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory))).CreateLogger(this.GetType());
            _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
        }

        public IItem? Item
        {
            get => _item;
            set
            {
                _item = value;
                TriggerCanExecuteChanged();
            }
        }

        public override void Execute(object? parameter)
        {
            var directoryEntryItem = GetItemAsFileDirectoryEntryItem();
            if (directoryEntryItem == null)
                return;

            try
            {
                directoryEntryItem.ContainerSectionItem.FileSystem.OpenFile(out var file, new U8Span(directoryEntryItem.Path), OpenMode.Read).ThrowIfFailure();

                var openFileDialog = new SaveFileDialog
                {
                    Title = LocalizationManager.Instance.Current.Keys.SaveFileDialog_SaveFileTitle,
                    InitialDirectory = _appSettings.LastSaveDir,
                    Filter = $"{LocalizationManager.Instance.Current.Keys.SaveFileDialog_AnyFileFilter}|*.*",
                    FileName = directoryEntryItem.Name,
                };

                var result = openFileDialog.ShowDialog(Application.Current.MainWindow);
                if (result == null || !result.Value)
                    return;

                var filePath = openFileDialog.FileName;
                _appSettings.LastSaveDir = Path.GetDirectoryName(filePath);

                using var fileStream = File.Create(filePath);
                file.AsStream().CopyTo(fileStream);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Format(LocalizationManager.Instance.Current.Keys.SaveFileTitleError, ex.Message));
            }
        }

        public override bool CanExecute(object? parameter)
        {
            return GetItemAsFileDirectoryEntryItem() != null;
        }

        private DirectoryEntryItem? GetItemAsFileDirectoryEntryItem()
        {
            if (_item is DirectoryEntryItem directoryEntryItem)
                return directoryEntryItem;

            return null;
        }

    }
}

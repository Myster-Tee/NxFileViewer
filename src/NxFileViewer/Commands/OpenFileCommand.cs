using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Services;
using Emignatik.NxFileViewer.Settings;
using Emignatik.NxFileViewer.Utils.MVVM.Commands;
using Microsoft.Win32;

namespace Emignatik.NxFileViewer.Commands
{
    public class OpenFileCommand : CommandBase, IOpenFileCommand
    {
        private readonly IFileOpenerService _fileOpenerService;
        private readonly IAppSettings _appSettings;

        public OpenFileCommand(IFileOpenerService fileOpenerService, IAppSettings appSettings)
        {
            _fileOpenerService = fileOpenerService ?? throw new ArgumentNullException(nameof(fileOpenerService));
            _appSettings = appSettings;
        }

        public override void Execute(object? parameter)
        {
            var lastOpenedFile = _appSettings.LastOpenedFile;

            var initialDirectory = string.Empty;

            if (!string.IsNullOrEmpty(lastOpenedFile))
            {
                try
                {
                    initialDirectory = Path.GetDirectoryName(lastOpenedFile);
                }
                catch
                {
                    // ignored
                }

                if (!string.IsNullOrEmpty(initialDirectory) )
                {

                    try
                    {
                        if (!Directory.Exists(initialDirectory))
                            initialDirectory = string.Empty;
                    }
                    catch
                    {
                        initialDirectory = string.Empty;
                    }
                }

            }

            var openFileDialog = new OpenFileDialog
            {
                InitialDirectory = initialDirectory,
                FileName = lastOpenedFile,
                Filter = LocalizationManager.Instance.Current.Keys.OpenFile_Filter
            };

            var currentMainWindow = Application.Current.MainWindow;
            if (openFileDialog.ShowDialog(currentMainWindow) != true) return;

            var filePath = openFileDialog.FileName;

            _fileOpenerService.SafeOpenFile(filePath);
        }
    }

    public interface IOpenFileCommand : ICommand
    {
    }
}

using System;
using System.Windows;
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
            var openFileDialog = new OpenFileDialog
            {
                FileName = _appSettings.LastOpenedFile,
                Filter = LocalizationManager.Instance.Current.Keys.OpenFile_Filter
            };

            if (openFileDialog.ShowDialog(Application.Current.MainWindow) != true) return;

            var filePath = openFileDialog.FileName;

            _fileOpenerService.OpenFile(filePath);
        }

    }
}

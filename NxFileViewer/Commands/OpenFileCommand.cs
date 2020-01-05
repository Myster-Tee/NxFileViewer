using System;
using System.Windows;
using Emignatik.NxFileViewer.Properties;
using Emignatik.NxFileViewer.Services;
using Emignatik.NxFileViewer.Settings;
using Microsoft.Win32;

namespace Emignatik.NxFileViewer.Commands
{
    public class OpenFileCommand : CommandBase
    {
        private readonly ISupportedFilesOpenerService _supportedFilesOpenerService;
        private readonly IAppSettings _appSettings;

        public OpenFileCommand(ISupportedFilesOpenerService supportedFilesOpenerService, IAppSettings appSettings)
        {
            _supportedFilesOpenerService = supportedFilesOpenerService ?? throw new ArgumentNullException(nameof(supportedFilesOpenerService));
            _appSettings = appSettings;
        }

        public override void Execute(object parameter)
        {
            var openFileDialog = new OpenFileDialog
            {
                FileName = _appSettings.LastOpenedFile,
                Filter = Resources.OpenFile_Filter
            };

            if (openFileDialog.ShowDialog(Application.Current.MainWindow) != true) return;

            var filePath = openFileDialog.FileName;

            _supportedFilesOpenerService.OpenFile(filePath);
        }

    }
}

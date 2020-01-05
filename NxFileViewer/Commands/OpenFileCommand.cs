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
        private readonly SupportedFilesOpenerService _supportedFilesOpenerService;

        public OpenFileCommand(SupportedFilesOpenerService supportedFilesOpenerService)
        {
            _supportedFilesOpenerService = supportedFilesOpenerService ?? throw new ArgumentNullException(nameof(supportedFilesOpenerService));
        }

        public override void Execute(object parameter)
        {
            var openFileDialog = new OpenFileDialog
            {
                FileName = AppSettings.Default.LastOpenedFile,
                Filter = Resources.OpenFile_Filter
            };

            if (openFileDialog.ShowDialog(Application.Current.MainWindow) != true) return;

            var filePath = openFileDialog.FileName;

            _supportedFilesOpenerService.OpenFile(filePath);
        }

    }
}

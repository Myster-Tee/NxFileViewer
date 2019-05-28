using System;
using System.Windows;
using System.Windows.Input;
using Emignatik.NxFileViewer.Properties;
using Emignatik.NxFileViewer.Services;
using Microsoft.Win32;

namespace Emignatik.NxFileViewer.Commands
{
    public class OpenFileCommand : ICommand
    {
        private readonly SupportedFilesOpenerService _supportedFilesOpenerService;

        public OpenFileCommand(SupportedFilesOpenerService supportedFilesOpenerService)
        {
            _supportedFilesOpenerService = supportedFilesOpenerService ?? throw new ArgumentNullException(nameof(supportedFilesOpenerService));
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var openFileDialog = new OpenFileDialog
            {
                FileName = Settings.Default.LastOpenedFile,
                Filter = Resources.OpenFile_Filter
            };

            if (openFileDialog.ShowDialog(Application.Current.MainWindow) != true) return;

            var filePath = openFileDialog.FileName;

            _supportedFilesOpenerService.OpenFile(filePath);
        }


    }
}

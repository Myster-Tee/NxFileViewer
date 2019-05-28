using System;
using System.IO;
using System.Windows.Input;
using Emignatik.NxFileViewer.Properties;
using Emignatik.NxFileViewer.Services;

namespace Emignatik.NxFileViewer.Commands
{
    public class OpenLastFileCommand : ICommand
    {
        private readonly SupportedFilesOpenerService _supportedFilesOpenerService;

        public OpenLastFileCommand(SupportedFilesOpenerService supportedFilesOpenerService)
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
            var lastOpenedFile = Settings.Default.LastOpenedFile;
            if (lastOpenedFile != null && File.Exists(lastOpenedFile))
            {
                _supportedFilesOpenerService.OpenFile(lastOpenedFile);
            }

        }

    }
}

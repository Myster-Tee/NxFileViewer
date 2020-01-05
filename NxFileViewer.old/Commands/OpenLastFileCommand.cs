using System;
using System.IO;
using Emignatik.NxFileViewer.Properties;
using Emignatik.NxFileViewer.Services;

namespace Emignatik.NxFileViewer.Commands
{
    public class OpenLastFileCommand : CommandBase
    {
        private readonly SupportedFilesOpenerService _supportedFilesOpenerService;

        public OpenLastFileCommand(SupportedFilesOpenerService supportedFilesOpenerService)
        {
            _supportedFilesOpenerService = supportedFilesOpenerService ?? throw new ArgumentNullException(nameof(supportedFilesOpenerService));
        }

        public override void Execute(object parameter)
        {
            var lastOpenedFile = Settings.Default.LastOpenedFile;
            if (lastOpenedFile != null && File.Exists(lastOpenedFile))
            {
                _supportedFilesOpenerService.OpenFile(lastOpenedFile);
            }

        }
    }
}

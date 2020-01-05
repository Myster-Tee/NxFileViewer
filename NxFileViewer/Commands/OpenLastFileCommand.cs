using System;
using System.IO;
using Emignatik.NxFileViewer.Services;
using Emignatik.NxFileViewer.Settings;

namespace Emignatik.NxFileViewer.Commands
{
    public class OpenLastFileCommand : CommandBase
    {
        private readonly ISupportedFilesOpenerService _supportedFilesOpenerService;
        private readonly IAppSettings _appSettings;

        public OpenLastFileCommand(ISupportedFilesOpenerService supportedFilesOpenerService, IAppSettings appSettings)
        {
            _supportedFilesOpenerService = supportedFilesOpenerService ?? throw new ArgumentNullException(nameof(supportedFilesOpenerService));
            _appSettings = appSettings;
        }

        public override void Execute(object parameter)
        {
            var lastOpenedFile = _appSettings.LastOpenedFile;
            if (lastOpenedFile != null && File.Exists(lastOpenedFile))
            {
                _supportedFilesOpenerService.OpenFile(lastOpenedFile);
            }

        }
    }
}

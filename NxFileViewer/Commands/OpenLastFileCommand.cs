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
            _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));

            _appSettings.SettingChanged += (sender, args) =>
            {
                if (args.SettingName == nameof(IAppSettings.LastOpenedFile))
                    TriggerCanExecuteChanged();
            };
        }

        public override bool CanExecute(object parameter)
        {
            var lastOpenedFile = _appSettings.LastOpenedFile;
            return lastOpenedFile != null && File.Exists(lastOpenedFile);
        }

        public override void Execute(object parameter)
        {
            var lastOpenedFile = _appSettings.LastOpenedFile;
            _supportedFilesOpenerService.OpenFile(lastOpenedFile);
        }
    }
}

using System;
using System.IO;
using Emignatik.NxFileViewer.Services;
using Emignatik.NxFileViewer.Settings;
using Emignatik.NxFileViewer.Utils.MVVM.Commands;

namespace Emignatik.NxFileViewer.Commands
{
    public class OpenLastFileCommand : CommandBase, IOpenLastFileCommand
    {
        private readonly IFileOpenerService _fileOpenerService;
        private readonly IAppSettings _appSettings;

        public OpenLastFileCommand(IFileOpenerService fileOpenerService, IAppSettings appSettings)
        {
            _fileOpenerService = fileOpenerService ?? throw new ArgumentNullException(nameof(fileOpenerService));
            _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));

            _appSettings.SettingChanged += (sender, args) =>
            {
                if (args.SettingName == nameof(IAppSettings.LastOpenedFile))
                    TriggerCanExecuteChanged();
            };
        }

        public override bool CanExecute(object? parameter)
        {
            var lastOpenedFile = _appSettings.LastOpenedFile;
            return !string.IsNullOrEmpty(lastOpenedFile) && File.Exists(lastOpenedFile);
        }

        public override void Execute(object? parameter)
        {
            var lastOpenedFile = _appSettings.LastOpenedFile;
            _fileOpenerService.OpenFile(lastOpenedFile);
        }
    }
}

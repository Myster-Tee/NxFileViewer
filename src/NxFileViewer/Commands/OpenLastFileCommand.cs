using System;
using System.IO;
using System.Windows.Input;
using Emignatik.NxFileViewer.Services.FileOpening;
using Emignatik.NxFileViewer.Settings;
using Emignatik.NxFileViewer.Utils.MVVM.Commands;

namespace Emignatik.NxFileViewer.Commands;

public class OpenLastFileCommand : CommandBase, IOpenLastFileCommand
{
    private readonly IFileOpeningService _fileOpeningService;
    private readonly IAppSettings _appSettings;

    public OpenLastFileCommand(IFileOpeningService fileOpeningService, IAppSettings appSettings)
    {
        _fileOpeningService = fileOpeningService ?? throw new ArgumentNullException(nameof(fileOpeningService));
        _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));

        _appSettings.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName == nameof(IAppSettings.LastOpenedFile))
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
        _fileOpeningService.SafeOpenFile(lastOpenedFile);
    }
}

public interface IOpenLastFileCommand : ICommand
{
}
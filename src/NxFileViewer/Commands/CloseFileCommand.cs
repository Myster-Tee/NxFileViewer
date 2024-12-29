using System;
using System.Windows.Input;
using Emignatik.NxFileViewer.Services.FileOpening;
using Emignatik.NxFileViewer.Utils.MVVM.Commands;

namespace Emignatik.NxFileViewer.Commands;

public class CloseFileCommand : CommandBase, ICloseFileCommand
{
    private readonly IFileOpeningService _fileOpeningService;

    public CloseFileCommand(IFileOpeningService fileOpeningService)
    {
        _fileOpeningService = fileOpeningService ?? throw new ArgumentNullException(nameof(fileOpeningService));
        _fileOpeningService.OpenedFileChanged += (_, _) =>
        {
            TriggerCanExecuteChanged();
        };
    }

    public override bool CanExecute(object? parameter)
    {
        return _fileOpeningService.OpenedFile != null;
    }

    public override void Execute(object? parameter)
    {
        _fileOpeningService.SafeClose();
    }
}

public interface ICloseFileCommand : ICommand
{
}
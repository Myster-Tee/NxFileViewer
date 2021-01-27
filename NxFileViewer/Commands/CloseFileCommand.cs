using System;
using Emignatik.NxFileViewer.Services;
using Emignatik.NxFileViewer.Utils.MVVM.Commands;

namespace Emignatik.NxFileViewer.Commands
{
    public class CloseFileCommand : CommandBase, ICloseFileCommand
    {
        private readonly IOpenedFileService _openedFileService;

        public CloseFileCommand(IOpenedFileService openedFileService)
        {
            _openedFileService = openedFileService ?? throw new ArgumentNullException(nameof(openedFileService));
            _openedFileService.OpenedFileChanged += OnOpenedFileChanged;
        }

        private void OnOpenedFileChanged(object? sender, OpenedFileChangedHandlerArgs args)
        {
            TriggerCanExecuteChanged();
        }

        public override bool CanExecute(object? parameter)
        {
            return _openedFileService.OpenedFile != null;
        }

        public override void Execute(object? parameter)
        {
            _openedFileService.OpenedFile = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}

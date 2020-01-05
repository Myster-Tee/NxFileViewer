using System;
using Emignatik.NxFileViewer.Services;

namespace Emignatik.NxFileViewer.Commands
{
    public class CloseFileCommand : CommandBase
    {
        private readonly OpenedFileService _openedFileService;

        public CloseFileCommand(OpenedFileService openedFileService)
        {
            _openedFileService = openedFileService ?? throw new ArgumentNullException(nameof(openedFileService));
            _openedFileService.OpenedFileChanged += OnOpenedFileChanged;
        }

        private void OnOpenedFileChanged(object sender, OpenedFileChangedHandlerArgs args)
        {
            TriggerCanExecuteChanged();
        }

        public override bool CanExecute(object parameter)
        {
            return _openedFileService.OpenedFile != null;
        }

        public override void Execute(object parameter)
        {
            _openedFileService.OpenedFile = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}

using System;
using System.Windows.Input;
using Emignatik.NxFileViewer.Model.TreeItems.Impl;
using Emignatik.NxFileViewer.Services.BackgroundTask;
using Emignatik.NxFileViewer.Utils.MVVM.Commands;

namespace Emignatik.NxFileViewer.Commands
{
    public class SaveNcaFileDecryptedCommand : CommandBase, ISaveNcaFileDecryptedCommand
    {
        private readonly IBackgroundTaskService _backgroundTaskService;
        private NcaItem? _ncaItem;

        public SaveNcaFileDecryptedCommand(IBackgroundTaskService backgroundTaskService)
        {
            _backgroundTaskService = backgroundTaskService ?? throw new ArgumentNullException(nameof(backgroundTaskService));
        }

        public NcaItem NcaItem
        {
            set
            {
                _ncaItem = value;
                TriggerCanExecuteChanged();
            }
        }
        public override void Execute(object? parameter)
        {
            //_ncaItem.Nca.OpenDecryptedNca().AsStream();
        }

        public override bool CanExecute(object? parameter)
        {
            return _ncaItem != null && !_backgroundTaskService.IsRunning;
        }
    }

    public interface ISaveNcaFileDecryptedCommand : ICommand
    {
        NcaItem NcaItem { set; }
    }
}

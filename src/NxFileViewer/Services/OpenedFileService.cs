using System.Windows;
using Emignatik.NxFileViewer.Model;

namespace Emignatik.NxFileViewer.Services
{

    public class OpenedFileService : IOpenedFileService
    {
        private NxFile? _openedFile;

        public event OpenedFileChangedHandler? OpenedFileChanged;

        public NxFile? OpenedFile
        {
            get => _openedFile;
            set
            {
                if (!Application.Current.Dispatcher.CheckAccess())
                {
                    Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        this.OpenedFile = value;
                    });
                    return;
                }

                if (_openedFile == value)
                    return;

                var oldOpenedFile = _openedFile;
                oldOpenedFile?.Dispose();

                _openedFile = value;
                NotifyOpenedFileChanged(oldOpenedFile, _openedFile);
            }
        }

        private void NotifyOpenedFileChanged(NxFile? oldFile, NxFile? newFile)
        {
            OpenedFileChanged?.Invoke(this, new OpenedFileChangedHandlerArgs(oldFile, newFile));
        }
    }

}

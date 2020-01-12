using Emignatik.NxFileViewer.Model;

namespace Emignatik.NxFileViewer.Services
{

    public class OpenedFileService : IOpenedFileService
    {
        private OpenedFile? _openedFile;

        public event OpenedFileChangedHandler? OpenedFileChanged;

        public OpenedFile? OpenedFile
        {
            get => _openedFile;
            set
            {
                if (_openedFile == value)
                    return;

                var oldOpenedFile = _openedFile;
                oldOpenedFile?.Dispose();

                _openedFile = value;
                NotifyOpenedFileChanged(oldOpenedFile, _openedFile);
            }
        }

        private void NotifyOpenedFileChanged(OpenedFile? oldFile, OpenedFile? newFile)
        {
            OpenedFileChanged?.Invoke(this, new OpenedFileChangedHandlerArgs(oldFile, newFile));
        }
    }

}

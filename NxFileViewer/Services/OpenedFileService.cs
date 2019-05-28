namespace Emignatik.NxFileViewer.Services
{
    public class OpenedFileService
    {
        private OpenedFile _openedFile;

        public event OpenedFileChangedHandler OpenedFileChanged;

        public OpenedFile OpenedFile
        {
            get => _openedFile;
            set
            {
                if (_openedFile == value)
                    return;

                var oldOpenedFile = _openedFile;
                _openedFile = value;
                NotifyOpenedFileChanged(oldOpenedFile, _openedFile);
            }
        }

        private void NotifyOpenedFileChanged(OpenedFile oldFile, OpenedFile newFile)
        {
            OpenedFileChanged?.Invoke(this, new OpenedFileChangedHandlerArgs(oldFile, newFile));
        }
    }

    public delegate void OpenedFileChangedHandler(object sender, OpenedFileChangedHandlerArgs args);

    public class OpenedFileChangedHandlerArgs
    {
        public OpenedFileChangedHandlerArgs(OpenedFile oldFile, OpenedFile newFile)
        {
            OldFile = oldFile;
            NewFile = newFile;
        }

        public OpenedFile OldFile { get; }

        public OpenedFile NewFile { get; }
    }
}

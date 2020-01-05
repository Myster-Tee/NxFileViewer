namespace Emignatik.NxFileViewer.Services
{
    public interface IOpenedFileService
    {
        event OpenedFileChangedHandler OpenedFileChanged;

        OpenedFile OpenedFile { get; set; }
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
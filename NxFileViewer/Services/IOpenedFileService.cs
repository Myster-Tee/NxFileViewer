using Emignatik.NxFileViewer.Model;

namespace Emignatik.NxFileViewer.Services
{

    /// <summary>
    /// Service in charge of providing the currently opened file
    /// </summary>
    public interface IOpenedFileService
    {
        /// <summary>
        /// Fired when <see cref="OpenedFile"/> is changed
        /// </summary>
        event OpenedFileChangedHandler OpenedFileChanged;

        /// <summary>
        /// Get or set the opened file
        /// </summary>
        OpenedFile? OpenedFile { get; set; }
    }

    public delegate void OpenedFileChangedHandler(object sender, OpenedFileChangedHandlerArgs args);

    public class OpenedFileChangedHandlerArgs
    {
        public OpenedFileChangedHandlerArgs(OpenedFile? oldFile, OpenedFile? newFile)
        {
            OldFile = oldFile;
            NewFile = newFile;
        }

        public OpenedFile? OldFile { get; }

        public OpenedFile? NewFile { get; }
    }

}
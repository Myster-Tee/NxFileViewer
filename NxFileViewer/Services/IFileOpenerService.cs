namespace Emignatik.NxFileViewer.Services
{
    public interface IFileOpenerService   
    {
        /// <summary>
        /// Opens the specified file if it is supported
        /// Never throws.
        /// </summary>
        /// <param name="filePath"></param>
        void OpenFile(string filePath);
    }
}
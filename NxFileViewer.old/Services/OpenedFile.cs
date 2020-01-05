namespace Emignatik.NxFileViewer.Services
{
    public class OpenedFile
    {
        /// <summary>
        /// Gives an indication from the location of the file (like file path)
        /// </summary>
        public string Location { get; set; }

        public object FileData { get; set; }
    }
}
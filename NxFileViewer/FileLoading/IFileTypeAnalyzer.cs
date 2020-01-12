namespace Emignatik.NxFileViewer.FileLoading
{
    public interface IFileTypeAnalyzer
    {
        /// <summary>
        /// Try to detect the real type of the specified file
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        FileType GetFileType(string filePath);
    }


    public enum FileType
    {
        UNKNOWN,
        XCI,
        NSP
    }
}
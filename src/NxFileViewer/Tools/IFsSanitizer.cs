namespace Emignatik.NxFileViewer.Tools
{
    public interface IFsSanitizer
    {
        string SanitizeFileName(string fileName);

        string SanitizePath(string path);
    }
}

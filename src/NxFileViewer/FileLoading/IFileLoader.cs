using Emignatik.NxFileViewer.Model;

namespace Emignatik.NxFileViewer.FileLoading;

public interface IFileLoader
{
    /// <summary>
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    /// <exception cref="FileNotSupportedException" />
    public NxFile Load(string filePath);

}
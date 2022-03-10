using System.Threading.Tasks;

namespace Emignatik.NxFileViewer.Services.FileOpening;

public interface IFileOpenerService   
{
    /// <summary>
    /// Tries to open the specified file if it is supported.
    /// Never throws.
    /// </summary>
    /// <param name="filePath"></param>
    Task SafeOpenFile(string filePath);
}
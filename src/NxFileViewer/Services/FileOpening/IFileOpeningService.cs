using System.Threading.Tasks;
using Emignatik.NxFileViewer.Models;

namespace Emignatik.NxFileViewer.Services.FileOpening;

/// <summary>
/// Service in charge of providing the currently opened file
/// </summary>
public interface IFileOpeningService
{
    /// <summary>
    /// Fired when <see cref="OpenedFile"/> is changed
    /// </summary>
    event OpenedFileChangedHandler OpenedFileChanged;

    /// <summary>
    /// Get the opened file
    /// </summary>
    NxFile? OpenedFile { get; }

    /// <summary>
    /// Tries to open the specified file if it is supported.
    /// Never throws.
    /// </summary>
    /// <param name="filePath"></param>
    Task SafeOpenFile(string filePath);

    /// <summary>
    /// Closes the opened file.
    /// </summary>
    void SafeClose();
}

public delegate void OpenedFileChangedHandler(object sender, OpenedFileChangedHandlerArgs args);

public class OpenedFileChangedHandlerArgs
{
    public OpenedFileChangedHandlerArgs(NxFile? newFile)
    {
        NewFile = newFile;
    }

    public NxFile? NewFile { get; }
}
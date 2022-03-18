namespace Emignatik.NxFileViewer.Services.FileLocationOpening;

public interface IFileLocationOpenerService
{

    /// <summary>
    /// Open the location of the specified file in Windows explorer
    /// </summary>
    /// <param name="filePath"></param>
    public void OpenFileLocationSafe(string? filePath);

}
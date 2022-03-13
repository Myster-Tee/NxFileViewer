using System.Threading;
using System.Threading.Tasks;
using Emignatik.NxFileViewer.Services.BackgroundTask;
using Emignatik.NxFileViewer.Services.FileRenaming.Models;
using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer.Services.FileRenaming;

public interface IFileRenamerService
{
    /// <summary>
    /// Rename all supported files found in the specified directory
    /// </summary>
    /// <param name="inputDirectory"></param>
    /// <param name="namingPatterns"></param>
    /// <param name="fileFilters"></param>
    /// <param name="includeSubdirectories"></param>
    /// <param name="isSimulation"></param>
    /// <param name="logger"></param>
    /// <param name="progressReporter"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task RenameFromDirectoryAsync(string inputDirectory, INamingPatterns namingPatterns, string? fileFilters, bool includeSubdirectories, bool isSimulation, ILogger? logger, IProgressReporter progressReporter, CancellationToken cancellationToken);

    /// <summary>
    /// Rename the specified file
    /// </summary>
    /// <param name="inputFile"></param>
    /// <param name="namingPatterns"></param>
    /// <param name="isSimulation"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>The new file name</returns>
    Task<RenamingResult> RenameFileAsync(string inputFile, INamingPatterns namingPatterns, bool isSimulation, CancellationToken cancellationToken);
}

public class RenamingResult
{
    public string OldFileName { get; set; } = "";

    public string NewFileName { get; set; } = "";

    public bool IsSimulation { get; set; }

    public bool IsRenamed { get; set; }

}
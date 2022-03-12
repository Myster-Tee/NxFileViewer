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
    /// <param name="simulation"></param>
    /// <param name="logger"></param>
    /// <param name="progressReporter"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task RenameFromDirectoryAsync(string inputDirectory, INamingPatterns namingPatterns, string? fileFilters, bool includeSubdirectories, bool simulation, ILogger? logger, IProgressReporter progressReporter, CancellationToken cancellationToken);

    /// <summary>
    /// Rename the specified file
    /// </summary>
    /// <param name="inputFile"></param>
    /// <param name="namingPatterns"></param>
    /// <returns>The new file name</returns>
    Task<string?> RenameFileAsync(string inputFile, INamingPatterns namingPatterns, CancellationToken cancellationToken);
}

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Emignatik.NxFileViewer.Services.BackgroundTask;
using Emignatik.NxFileViewer.Services.FileRenaming.Models;

namespace Emignatik.NxFileViewer.Services.FileRenaming;

public interface IFileRenamerService
{
    Task RenameFromDirectoryAsync(string inputDirectory, INamingPatterns namingPatterns, string? fileFilters, bool includeSubDirectories, IProgressReporter progressReporter, CancellationToken cancellationToken);


    /// <summary>
    /// 
    /// </summary>
    /// <param name="inputFile"></param>
    /// <param name="namingPatterns"></param>
    /// <returns>The new file name</returns>
    Task<string?> RenameFileAsync(string inputFile, INamingPatterns namingPatterns, CancellationToken cancellationToken);
}
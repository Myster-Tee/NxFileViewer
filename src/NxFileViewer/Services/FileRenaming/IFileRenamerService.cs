using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Emignatik.NxFileViewer.Services.BackgroundTask;
using Emignatik.NxFileViewer.Services.FileRenaming.Exceptions;
using Emignatik.NxFileViewer.Services.FileRenaming.Models;
using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer.Services.FileRenaming;

public interface IFileRenamerService
{
    /// <summary>
    /// Rename all supported files found in the specified directory
    /// </summary>
    /// <param name="inputDirectory"></param>
    /// <param name="fileFilters"></param>
    /// <param name="includeSubdirectories"></param>
    /// <param name="automaticallyCloseOpenedFile"></param>
    /// <param name="namingSettings"></param>
    /// <param name="isSimulation"></param>
    /// <param name="logger"></param>
    /// <param name="progressReporter"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="BadInvalidFileNameCharReplacementException"></exception>
    /// <exception cref="ContentTypeNotSupportedException"></exception>
    /// <exception cref="EmptyPatternException"></exception>
    /// <exception cref="SuperPackageNotSupportedException"></exception>
    /// <exception cref="KeywordNotAllowedException"></exception>
    Task<IList<RenamingResult>> RenameFromDirectoryAsync(string inputDirectory, string? fileFilters, bool includeSubdirectories, bool automaticallyCloseOpenedFile, INamingSettings namingSettings, bool isSimulation, ILogger? logger, IProgressReporter progressReporter, CancellationToken cancellationToken);

    /// <summary>
    /// Rename the specified file
    /// </summary>
    /// <param name="inputFile"></param>
    /// <param name="automaticallyCloseOpenedFile"></param>
    /// <param name="namingSettings"></param>
    /// <param name="isSimulation"></param>
    /// <param name="logger"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>The new file name</returns>
    /// <exception cref="BadInvalidFileNameCharReplacementException"></exception>
    /// <exception cref="ContentTypeNotSupportedException"></exception>
    /// <exception cref="EmptyPatternException"></exception>
    /// <exception cref="SuperPackageNotSupportedException"></exception>
    /// <exception cref="KeywordNotAllowedException"></exception>
    Task<RenamingResult> RenameFileAsync(string inputFile, bool automaticallyCloseOpenedFile, INamingSettings namingSettings, bool isSimulation, ILogger? logger, CancellationToken cancellationToken);
}

public class RenamingResult
{
    public string OldFileName { get; set; } = "";

    public string? NewFileName { get; set; }

    public bool IsSimulation { get; set; }

    public bool IsRenamed { get; set; }

    public Exception? Exception { get; set; }
}
using System;
using System.Collections.Generic;
using System.IO;
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
    Task<IList<RenamingResult>> RenameFromDirectoryAsync(string inputDirectory, string? fileFilters, bool includeSubdirectories, bool automaticallyCloseOpenedFile, INamingSettings namingSettings, bool isSimulation, ILogger? logger, IProgressReporter progressReporter, CancellationToken cancellationToken, Action<RenamingResult>? resultReported = null);

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
    public string Status => Exception != null ? Localization.LocalizationManager.Instance.Current.Keys.RenamingTool_StatusError
        : !IsRenamed ? Localization.LocalizationManager.Instance.Current.Keys.RenamingTool_StatusUnchanged
        : IsSimulation ? Localization.LocalizationManager.Instance.Current.Keys.RenamingTool_StatusSimulation
        : Localization.LocalizationManager.Instance.Current.Keys.RenamingTool_StatusRenamed;

    public string? SourceDirectory { get; set; }
    public string? TargetDirectory { get; set; }
    public string OldPathDisplay => FormatPath(OldFilePath, SourceDirectory, "QUELL::");
    public string NewPathDisplay
    {
        get
        {
            if (string.IsNullOrWhiteSpace(SourceDirectory)) return NewFilePath ?? "";
            var sameRoot = string.IsNullOrWhiteSpace(TargetDirectory) ||
                string.Equals(Path.TrimEndingDirectorySeparator(Path.GetFullPath(TargetDirectory)),
                    Path.TrimEndingDirectorySeparator(Path.GetFullPath(SourceDirectory!)), StringComparison.OrdinalIgnoreCase);
            return FormatPath(NewFilePath, sameRoot ? SourceDirectory : TargetDirectory, sameRoot ? "QUELL::" : "ZIEL::");
        }
    }

    private static string FormatPath(string? path, string? root, string prefix)
    {
        if (string.IsNullOrEmpty(path)) return "";
        return string.IsNullOrWhiteSpace(root) ? path : prefix + Path.GetRelativePath(root, path);
    }

    public string OldFilePath { get; set; } = "";
    public string? NewFilePath { get; set; }
    public string OldFileName { get; set; } = "";

    public string? NewFileName { get; set; }

    public bool IsSimulation { get; set; }

    public bool IsRenamed { get; set; }

    public Exception? Exception { get; set; }
}

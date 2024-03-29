﻿using Emignatik.NxFileViewer.Models;

namespace Emignatik.NxFileViewer.Services.FileOpening;

/// <summary>
/// Service in charge of providing the currently opened file
/// </summary>
public interface IOpenedFileService
{
    /// <summary>
    /// Fired when <see cref="OpenedFile"/> is changed
    /// </summary>
    event OpenedFileChangedHandler OpenedFileChanged;

    /// <summary>
    /// Get or set the opened file
    /// </summary>
    NxFile? OpenedFile { get; set; }
}

public delegate void OpenedFileChangedHandler(object sender, OpenedFileChangedHandlerArgs args);

public class OpenedFileChangedHandlerArgs
{
    public OpenedFileChangedHandlerArgs(NxFile? oldFile, NxFile? newFile)
    {
        OldFile = oldFile;
        NewFile = newFile;
    }

    public NxFile? OldFile { get; }

    public NxFile? NewFile { get; }
}

using System;
using System.IO;
using Emignatik.NxFileViewer.Models.Overview;
using Emignatik.NxFileViewer.Models.TreeItems;

namespace Emignatik.NxFileViewer.Models;

/// <summary>
/// Represents an actually opened file
/// </summary>
public class NxFile : IDisposable
{
    public Services.Nand.NandDetectionResult? NandResult { get; set; }
    public string? NandPhysicalPath { get; set; }
    public Services.Integrity.BatchIntegrityResult? FirmwareResult { get; set; }
    public IDisposable? OwnedResource { get; set; }
    public string? ArchivePath { get; set; }
    public string? ArchiveEntry { get; set; }
    public System.Collections.Generic.IReadOnlyList<string> ArchiveEntries { get; set; } = Array.Empty<string>();
    public NxFile(string filePath, IItem rootItem, FileOverview overview)
    {
        FilePath = filePath;
        RootItem = rootItem;
        Overview = overview;
        FileName = Path.GetFileName(filePath.TrimEnd('/'));
    }

    /// <summary>
    /// Get the path of the opened file
    /// </summary>
    public string FilePath { get; }

    /// <summary>
    /// Get the root item of the opened file
    /// </summary>
    public IItem RootItem { get; }

    /// <summary>
    /// Get the opened file overview information
    /// </summary>
    public FileOverview Overview { get; }

    public string FileName { get; }

    public void Dispose()
    {
        try { RootItem.Dispose(); }
        finally { OwnedResource?.Dispose(); OwnedResource = null; }
    }
}

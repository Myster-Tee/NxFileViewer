
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
    public NxFile(string filePath, IItem rootItem, FileOverview overview)
    {
        FilePath = filePath;
        RootItem = rootItem;
        Overview = overview;
        FileName = Path.GetFileName(filePath);
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
        RootItem.Dispose();
    }
}
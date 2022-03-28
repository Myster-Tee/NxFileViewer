using System.Collections.Generic;
using LibHac.Fs.Fsa;
using LibHac.FsSystem;
using LibHac.Tools.FsSystem.NcaUtils;

namespace Emignatik.NxFileViewer.Models.TreeItems.Impl;

public class FsSectionItem : SectionItemBase
{

    public FsSectionItem(int sectionIndex, NcaFsHeader ncaFsHeader, NcaItem parentItem) : base(sectionIndex, ncaFsHeader, parentItem)
    {
    }

    public override List<DirectoryEntryItem> ChildItems { get; } = new();

    /// <summary>
    /// Get the FileSystem of this section.
    /// Can be null when the FileSystem of this section couldn't be opened.
    /// </summary>
    public IFileSystem? FileSystem { get; internal set; }

    public override void Dispose()
    {
        FileSystem?.Dispose();
    }
}
using System;
using System.Linq;
using LibHac.Fs.Fsa;
using LibHac.FsSystem;
using LibHac.Tools.FsSystem.NcaUtils;
using NcaFsHeader = LibHac.Tools.FsSystem.NcaUtils.NcaFsHeader;

namespace Emignatik.NxFileViewer.Models.TreeItems.Impl;

public class SectionItem : ItemBase
{
    public SectionItem(int sectionIndex, NcaFsHeader ncaFsHeader, NcaItem parentItem, NcaFsPatchInfo? patchInfo, NcaSparseInfo? sparseInfo) : base(parentItem)
    {
        FsHeader = ncaFsHeader;
        SectionIndex = sectionIndex;
        ParentItem = parentItem ?? throw new ArgumentNullException(nameof(parentItem));
        PatchInfo = patchInfo;
        SparseInfo = sparseInfo;

        if (patchInfo == null && sparseInfo == null)
            SectionType = SectionType.FileSystem;
        else
            SectionType = (patchInfo != null ? SectionType.Patch : 0) | (sparseInfo != null ? SectionType.Sparse : 0);


        if (Nca.TryGetSectionTypeFromIndex(sectionIndex, parentItem.ContentType, out var ncaSectionType))
            this.NcaSectionType = ncaSectionType;
        else
            this.NcaSectionType = null;
    }

    public NcaSectionType? NcaSectionType { get; private set; }

    public sealed override string Name => $"Section {SectionIndex}";

    public sealed override string DisplayName => Name;

    public new DirectoryEntryItem[] ChildItems => base.ChildItems.OfType<DirectoryEntryItem>().ToArray();

    public sealed override string LibHacTypeName => FsHeader.GetType().Name;

    public new NcaItem ParentItem { get; }

    public int SectionIndex { get; }

    public SectionType SectionType { get; }

    public override string Format => FsHeader.FormatType.ToString();

    public NcaFsHeader FsHeader { get; }

    public NcaSparseInfo? SparseInfo { get; }

    public bool IsSparse => SparseInfo != null;

    public NcaFsPatchInfo? PatchInfo { get; }

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

[Flags]
public enum SectionType
{
    FileSystem = 1,
    Sparse = 2,
    Patch = 4,
}
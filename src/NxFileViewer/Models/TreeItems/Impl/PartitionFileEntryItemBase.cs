using System;
using Emignatik.NxFileViewer.Utils.LibHacExtensions;
using LibHac.Fs.Fsa;
using LibHac.Tools.Fs;

namespace Emignatik.NxFileViewer.Models.TreeItems.Impl;

/// <summary>
/// Represents a <see cref="FileEntry"/> item
/// </summary>
public abstract class PartitionFileEntryItemBase : ItemBase
{
    public PartitionFileEntryItemBase(DirectoryEntryEx fileEntry, PartitionFileSystemItemBase parentItem) : base(parentItem)
    {
        FileEntry = fileEntry ?? throw new ArgumentNullException(nameof(fileEntry));
        ParentItem = parentItem ?? throw new ArgumentNullException(nameof(parentItem));
    }

    public new PartitionFileSystemItemBase ParentItem { get; }

    public DirectoryEntryEx FileEntry { get; }

    public sealed override string LibHacTypeName => nameof(FileEntry);

    public override string Name => FileEntry.Name;

    public override string DisplayName => Name;

    public long Size => FileEntry.Size;

    /// <summary>
    /// Shortcut helper to load the file in attached the partition
    /// </summary>
    /// <returns></returns>
    public virtual IFile LoadFile()
    {
        return ParentItem.PartitionFileSystem.LoadFile(FileEntry);
    }
}
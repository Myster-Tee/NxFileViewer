using System;
using LibHac.Fs.Fsa;
using LibHac.Tools.Fs;

namespace Emignatik.NxFileViewer.Models.TreeItems.Impl;

/// <summary>
/// Represents a <see cref="FileEntry"/> item
/// </summary>
public abstract class PartitionFileEntryItemBase : ItemBase
{
    public PartitionFileEntryItemBase(DirectoryEntryEx fileEntry, IFile file, PartitionFileSystemItemBase parentItem) : base(parentItem)
    {
        FileEntry = fileEntry ?? throw new ArgumentNullException(nameof(fileEntry));
        File = file ?? throw new ArgumentNullException(nameof(file));
        ParentItem = parentItem ?? throw new ArgumentNullException(nameof(parentItem));
    }

    public new PartitionFileSystemItemBase ParentItem { get; }

    public IFile File { get; }

    public DirectoryEntryEx FileEntry { get; }

    public sealed override string LibHacTypeName => nameof(FileEntry);

    public override string Name => FileEntry.Name;

    public override string DisplayName => Name;

    public long Size => FileEntry.Size;

    public override void Dispose()
    {
        File.Dispose();
    }
}
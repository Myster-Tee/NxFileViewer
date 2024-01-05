using System.Collections.Generic;
using LibHac.Fs.Fsa;
using LibHac.Tools.Fs;

namespace Emignatik.NxFileViewer.Models.TreeItems.Impl;

public class PartitionFileEntryItem : PartitionFileEntryItemBase
{
    public PartitionFileEntryItem(DirectoryEntryEx partitionFileEntry, IFile file, PartitionFileSystemItemBase parentItem)
        : base(partitionFileEntry, file, parentItem)
    {
    }

    public override List<IItem> ChildItems { get; } = new();

    public override string? Format => null;
}
using System.Collections.Generic;
using LibHac.Fs.Fsa;
using LibHac.FsSystem;

namespace Emignatik.NxFileViewer.Models.TreeItems.Impl;

public class PartitionFileEntryItem : PartitionFileEntryItemBase
{
    public PartitionFileEntryItem(PartitionFileEntry partitionFileEntry, IFile file, PartitionFileSystemItemBase parentItem) 
        : base(partitionFileEntry, file, parentItem)
    {
    }

    public override List<IItem> ChildItems { get; } = new();
}
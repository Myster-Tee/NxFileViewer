using LibHac.Tools.Fs;

namespace Emignatik.NxFileViewer.Models.TreeItems.Impl;

public class PartitionFileEntryItem : PartitionFileEntryItemBase
{
    public PartitionFileEntryItem(DirectoryEntryEx partitionFileEntry, PartitionFileSystemItemBase parentItem) : base(partitionFileEntry, parentItem)
    {
    }

    public override string? Format => null;
}
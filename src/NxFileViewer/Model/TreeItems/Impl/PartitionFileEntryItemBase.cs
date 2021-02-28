using System;
using LibHac.Fs.Fsa;
using LibHac.FsSystem;

namespace Emignatik.NxFileViewer.Model.TreeItems.Impl
{
    /// <summary>
    /// Represents a <see cref="PartitionFileEntry"/> item
    /// </summary>
    public abstract class PartitionFileEntryItemBase : ItemBase
    {
        public PartitionFileEntryItemBase(PartitionFileEntry partitionFileEntry, IFile file, PartitionFileSystemItemBase parentItem)
        {
            PartitionFileEntry = partitionFileEntry ?? throw new ArgumentNullException(nameof(partitionFileEntry));
            File = file ?? throw new ArgumentNullException(nameof(file));
            ParentItem = parentItem ?? throw new ArgumentNullException(nameof(parentItem));
        }

        public sealed override PartitionFileSystemItemBase ParentItem { get; }

        public IFile File { get; }

        public PartitionFileEntry PartitionFileEntry { get; }

        public sealed override string LibHacTypeName => nameof(PartitionFileEntry);

        public override string? LibHacUnderlyingTypeName => null;

        public override string Name => PartitionFileEntry.Name;

        public override string DisplayName => Name;

        public long Size => PartitionFileEntry.Size;

        public override void Dispose()
        {
            File.Dispose();
        }
    }
}
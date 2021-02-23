using System;
using System.Collections.Generic;
using Emignatik.NxFileViewer.FileLoading;
using LibHac.Fs.Fsa;
using LibHac.FsSystem;

namespace Emignatik.NxFileViewer.Model.TreeItems.Impl
{

    /// <summary>
    /// Represents a <see cref="PartitionFileEntry"/> item
    /// </summary>
    public class PartitionFileEntryItem : ItemBase
    {
        private IReadOnlyList<IItem>? _childItems;

        public PartitionFileEntryItem(PartitionFileEntry partitionFileEntry, IFile file, PartitionFileSystemItem parentPartitionFileSystemItem, IChildItemsBuilder childItemsBuilder)
            : base(childItemsBuilder)
        {
            PartitionFileEntry = partitionFileEntry ?? throw new ArgumentNullException(nameof(partitionFileEntry));
            File = file ?? throw new ArgumentNullException(nameof(file));
            ParentPartitionFileSystemItem = parentPartitionFileSystemItem ?? throw new ArgumentNullException(nameof(parentPartitionFileSystemItem));
        }

        public IFile File { get; }

        public PartitionFileEntry PartitionFileEntry { get; }

        public sealed override string LibHacTypeName => nameof(PartitionFileEntry);

        public override string? LibHacUnderlyingTypeName => null;

        public override string Name => PartitionFileEntry.Name;

        public override string DisplayName => Name;

        public long Size => PartitionFileEntry.Size;

        public PartitionFileSystemItem ParentPartitionFileSystemItem { get; }

        public override IItem ParentItem => ParentPartitionFileSystemItem;

        protected override IReadOnlyList<IItem> SafeLoadChildItemsInternal()
        {
            return GetChildItems();
        }

        private IReadOnlyList<IItem> GetChildItems()
        {
            return _childItems ??= ChildItemsBuilder.Build(this);
        }

        public override void Dispose()
        {
            File.Dispose();
        }
    }
}
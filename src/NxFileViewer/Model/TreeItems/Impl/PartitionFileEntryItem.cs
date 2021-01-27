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
        private readonly IFile _file;

        public PartitionFileEntryItem(PartitionFileEntry partitionFileEntry, IFile file, PartitionFileSystemItem parentPartitionFileSystemItem, IChildItemsBuilder childItemsBuilder)
        {
            PartitionFileEntry = partitionFileEntry ?? throw new ArgumentNullException(nameof(partitionFileEntry));
            _file = file ?? throw new ArgumentNullException(nameof(file));
            ChildItemsBuilder = childItemsBuilder ?? throw new ArgumentNullException(nameof(childItemsBuilder));
            ParentPartitionFileSystemItem = parentPartitionFileSystemItem ?? throw new ArgumentNullException(nameof(parentPartitionFileSystemItem));
        }

        protected IChildItemsBuilder ChildItemsBuilder { get; }

        public PartitionFileEntry PartitionFileEntry { get; }

        public string Name => PartitionFileEntry.Name;

        public sealed override string ObjectType => nameof(PartitionFileEntry);

        public override string DisplayName => Name;

        public PartitionFileSystemItem ParentPartitionFileSystemItem { get; }

        public override IItem ParentItem => ParentPartitionFileSystemItem;

        protected override IEnumerable<IItem> LoadChildItems()
        {
            return ChildItemsBuilder.Build(this);
        }

        public override void Dispose()
        {
            _file.Dispose();
        }
    }
}
using System;
using System.Collections.Generic;
using Emignatik.NxFileViewer.FileLoading;
using LibHac;
using LibHac.FsSystem;

namespace Emignatik.NxFileViewer.Model.TreeItems.Impl
{
    public abstract class PartitionFileSystemItem : ItemBase
    {
        private IPartitionChildByTypes? _partitionChildByTypes;

        protected PartitionFileSystemItem(PartitionFileSystem partitionFileSystem, IChildItemsBuilder childItemsBuilder)
            : base(childItemsBuilder)
        {
            PartitionFileSystem = partitionFileSystem ?? throw new ArgumentNullException(nameof(partitionFileSystem));
        }

        public PartitionFileSystem PartitionFileSystem { get; }

        public PartitionFileSystemType PartitionType => PartitionFileSystem.Header.Type;

        public int NumFiles => PartitionFileSystem.Header.NumFiles;

        public sealed override string LibHacTypeName => nameof(PartitionFileSystem);

        public override string? LibHacUnderlyingTypeName => null;

        public abstract Keyset KeySet { get; }

        /// <summary>
        /// Get child items of type <see cref="NcaItem"/>
        /// </summary>
        public IReadOnlyList<NcaItem> NcaItems => GetChildItemsByTypes().NcaItems;

        /// <summary>
        /// Get child items of type <see cref="PartitionFileEntryItem"/>
        /// </summary>
        public IReadOnlyList<PartitionFileEntryItem> PartitionFileEntryItems => GetChildItemsByTypes().PartitionFileEntryItems;

        protected sealed override IReadOnlyList<IItem> SafeLoadChildItemsInternal()
        {
            return GetChildItemsByTypes().AllChildItems;
        }

        private IPartitionChildByTypes GetChildItemsByTypes()
        {
            return _partitionChildByTypes ??= ChildItemsBuilder.Build(this);
        }
    }
}
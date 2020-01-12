using System;
using System.Collections.Generic;
using Emignatik.NxFileViewer.FileLoading;
using Emignatik.NxFileViewer.Views.ObjectPropertyViewer;
using LibHac;
using LibHac.FsSystem;

namespace Emignatik.NxFileViewer.Model.TreeItems.Impl
{
    public abstract class PartitionFileSystemItem : ItemBase
    {
        private readonly IChildItemsBuilder _childItemsBuilder;
        private PartitionChildByTypes? _partitionChildByTypes;

        public PartitionFileSystemItem(PartitionFileSystem partitionFileSystem, IChildItemsBuilder childItemsBuilder)
        {
            _childItemsBuilder = childItemsBuilder ?? throw new ArgumentNullException(nameof(childItemsBuilder));
            PartitionFileSystem = partitionFileSystem ?? throw new ArgumentNullException(nameof(partitionFileSystem));
        }

        public sealed override string ObjectType => nameof(PartitionFileSystem);

        [PropertiesView]
        public string PartitionType => PartitionFileSystem.Header.Type.ToString();

        public abstract Keyset KeySet { get; }

        public PartitionFileSystem PartitionFileSystem { get; }

        /// <summary>
        /// Get child items of type <see cref="NcaItem"/>
        /// </summary>
        public IReadOnlyList<NcaItem> NcaItems => GetChildItemsByTypes().NcaItems;

        /// <summary>
        /// Get child items of type <see cref="PartitionFileEntryItem"/>
        /// </summary>
        public IReadOnlyList<PartitionFileEntryItem> PartitionFileEntryItems => GetChildItemsByTypes().PartitionFileEntryItems;

        protected sealed override IEnumerable<IItem> LoadChildItems()
        {
            return GetChildItemsByTypes().AllChildItems;
        }

        private PartitionChildByTypes GetChildItemsByTypes()
        {
            return _partitionChildByTypes ??= _childItemsBuilder.Build(this);
        }
    }
}
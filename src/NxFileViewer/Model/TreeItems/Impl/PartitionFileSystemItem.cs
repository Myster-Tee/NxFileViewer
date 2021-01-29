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
        private IPartitionChildByTypes? _partitionChildByTypes;

        public PartitionFileSystemItem(PartitionFileSystem partitionFileSystem, IChildItemsBuilder childItemsBuilder)
        {
            _childItemsBuilder = childItemsBuilder ?? throw new ArgumentNullException(nameof(childItemsBuilder));
            PartitionFileSystem = partitionFileSystem ?? throw new ArgumentNullException(nameof(partitionFileSystem));
        }

        public sealed override string ObjectType => nameof(PartitionFileSystem);

        [PropertiesView]
        public PartitionFileSystemType PartitionType => PartitionFileSystem.Header.Type;

        [PropertiesView]
        public int NumFiles => PartitionFileSystem.Header.NumFiles;

        public abstract Keyset KeySet { get; }

        public PartitionFileSystem PartitionFileSystem { get; }

        /// <summary>
        /// Get child items of type <see cref="NcaItem"/>
        /// </summary>
        public IReadOnlyList<NcaItem> NcaItems => GetChildItemsByTypes(force: false).NcaItems;

        /// <summary>
        /// Get child items of type <see cref="PartitionFileEntryItem"/>
        /// </summary>
        public IReadOnlyList<PartitionFileEntryItem> PartitionFileEntryItems => GetChildItemsByTypes(force: false).PartitionFileEntryItems;

        public sealed override IReadOnlyList<IItem> LoadChildItems(bool force)
        {
            return GetChildItemsByTypes(force).AllChildItems;
        }

        private IPartitionChildByTypes GetChildItemsByTypes(bool force)
        {
            if (_partitionChildByTypes == null || force)
                _partitionChildByTypes = _childItemsBuilder.Build(this);
            return _partitionChildByTypes;
        }
    }
}
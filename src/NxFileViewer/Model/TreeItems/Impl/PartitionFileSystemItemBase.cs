using System;
using System.Collections.Generic;
using LibHac;
using LibHac.FsSystem;

namespace Emignatik.NxFileViewer.Model.TreeItems.Impl
{
    public abstract class PartitionFileSystemItemBase : ItemBase
    {
        protected PartitionFileSystemItemBase(PartitionFileSystem partitionFileSystem)
        {
            PartitionFileSystem = partitionFileSystem ?? throw new ArgumentNullException(nameof(partitionFileSystem));
        }

        public PartitionFileSystem PartitionFileSystem { get; }

        public PartitionFileSystemType PartitionType => PartitionFileSystem.Header.Type;

        public int NumFiles => PartitionFileSystem.Header.NumFiles;

        public sealed override string LibHacTypeName => nameof(PartitionFileSystem);

        public override string? LibHacUnderlyingTypeName => null;

        public abstract Keyset KeySet { get; }

        public sealed override IEnumerable<IItem> ChildItems
        {
            get
            {
                foreach (var ncaChildItem in NcaChildItems)
                {
                    yield return ncaChildItem;
                }

                foreach (var partitionFileEntryItem in PartitionFileEntryChildItems)
                {
                    yield return partitionFileEntryItem;
                }
            }
        }

        /// <summary>
        /// Get child items of type <see cref="NcaItem"/>
        /// </summary>
        public List<NcaItem> NcaChildItems { get; } = new();

        /// <summary>
        /// Get child items of type <see cref="PartitionFileEntryItemBase"/>
        /// </summary>
        public List<PartitionFileEntryItem> PartitionFileEntryChildItems { get; } = new();

    }
}
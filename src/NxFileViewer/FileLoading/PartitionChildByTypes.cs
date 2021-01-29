using System.Collections.Generic;
using Emignatik.NxFileViewer.Model.TreeItems;
using Emignatik.NxFileViewer.Model.TreeItems.Impl;

namespace Emignatik.NxFileViewer.FileLoading
{
    public class PartitionChildByTypes : IPartitionChildByTypes
    {
        internal List<IItem> AllChildItemsInternal { get; } = new List<IItem>();
        internal List<NcaItem> NcaItemsInternal { get; } = new List<NcaItem>();
        internal List<PartitionFileEntryItem> PartitionFileEntryItemsInternal { get; } = new List<PartitionFileEntryItem>();

        public IReadOnlyList<IItem> AllChildItems => AllChildItemsInternal;
        public IReadOnlyList<NcaItem> NcaItems => NcaItemsInternal;
        public IReadOnlyList<PartitionFileEntryItem> PartitionFileEntryItems => PartitionFileEntryItemsInternal;
    }
}
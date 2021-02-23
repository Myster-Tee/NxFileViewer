using System;
using Emignatik.NxFileViewer.FileLoading;
using LibHac;

namespace Emignatik.NxFileViewer.Model.TreeItems.Impl
{
    public class XciPartitionItem : PartitionFileSystemItem
    {
        public XciPartitionItem(XciPartition xciPartition, XciPartitionType xciPartitionType, XciItem parentXciItem, IChildItemsBuilder childItemsBuilder)
            : base(xciPartition, childItemsBuilder)
        {
            XciPartition = xciPartition ?? throw new ArgumentNullException(nameof(xciPartition));
            XciPartitionType = xciPartitionType;
            ParentXciItem = parentXciItem ?? throw new ArgumentNullException(nameof(parentXciItem));
        }

        public XciPartition XciPartition { get; }

        public override string LibHacUnderlyingTypeName => nameof(XciPartition);

        public override string Name => XciPartitionType.ToString();

        public override string DisplayName => Name;

        public XciItem ParentXciItem { get; }

        public override IItem ParentItem => ParentXciItem;

        public override Keyset KeySet => ParentXciItem.KeySet;

        public XciPartitionType XciPartitionType { get; }
    }
}
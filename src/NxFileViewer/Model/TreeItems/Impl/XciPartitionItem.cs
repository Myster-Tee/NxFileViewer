using System;
using Emignatik.NxFileViewer.FileLoading;
using Emignatik.NxFileViewer.Views.ObjectPropertyViewer;
using LibHac;

namespace Emignatik.NxFileViewer.Model.TreeItems.Impl
{
    public class XciPartitionItem : PartitionFileSystemItem
    {
        public XciPartitionItem(XciPartition xciPartition, XciPartitionType xciPartitionType, XciItem parentXciItem, IChildItemsBuilder childItemsBuilder) : base(xciPartition, childItemsBuilder)
        {
            XciPartition = xciPartition ?? throw new ArgumentNullException(nameof(xciPartition));
            XciPartitionType = xciPartitionType;
            ParentXciItem = parentXciItem ?? throw new ArgumentNullException(nameof(parentXciItem));
        }

        [PropertiesView]
        public string UnderlyingType => nameof(XciPartition);

        [PropertiesView]
        public XciPartitionType XciPartitionType { get; }

        public override string DisplayName => XciPartitionType.ToString();

        public XciItem ParentXciItem { get; }

        public override IItem ParentItem => ParentXciItem;

        public XciPartition XciPartition { get; }

        public override Keyset KeySet => ParentXciItem.KeySet;
    }
}
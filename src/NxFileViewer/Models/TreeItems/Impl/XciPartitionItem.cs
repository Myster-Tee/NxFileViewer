using System;
using LibHac.Common.Keys;
using LibHac.Tools.Fs;

namespace Emignatik.NxFileViewer.Models.TreeItems.Impl;

public class XciPartitionItem : PartitionFileSystemItemBase
{
    public XciPartitionItem(XciPartition xciPartition, XciPartitionType xciPartitionType, XciItem parentItem)
        : base(xciPartition)
    {
        XciPartition = xciPartition ?? throw new ArgumentNullException(nameof(xciPartition));
        XciPartitionType = xciPartitionType;
        ParentItem = parentItem ?? throw new ArgumentNullException(nameof(parentItem));
    }

    public override XciItem ParentItem { get; }

    public XciPartition XciPartition { get; }

    public override string LibHacUnderlyingTypeName => nameof(XciPartition);

    public override string Name => XciPartitionType.ToString();

    public override string DisplayName => Name;

    public override KeySet KeySet => ParentItem.KeySet;

    public XciPartitionType XciPartitionType { get; }
}
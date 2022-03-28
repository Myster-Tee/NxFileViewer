using System;
using System.Collections.Generic;
using LibHac.Common.Keys;
using LibHac.FsSystem;

namespace Emignatik.NxFileViewer.Models.TreeItems.Impl;

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

    public abstract KeySet KeySet { get; }

    public sealed override IEnumerable<IItem> ChildItems
    {
        get
        {
            foreach (var childItem in NcaChildItems)
            {
                yield return childItem;
            }       
                
            foreach (var childItem in TicketChildItems)
            {
                yield return childItem;
            }

            foreach (var childItem in PartitionFileEntryChildItems)
            {
                yield return childItem;
            }
        }
    }

    /// <summary>
    /// Get child items of type <see cref="NcaItem"/>
    /// </summary>
    public List<NcaItem> NcaChildItems { get; } = new();

    /// <summary>
    /// Get child items of type <see cref="TicketItem"/>
    /// </summary>
    public List<TicketItem> TicketChildItems { get; } = new();

    /// <summary>
    /// Get child items of type <see cref="PartitionFileEntryItemBase"/>
    /// </summary>
    public List<PartitionFileEntryItemBase> PartitionFileEntryChildItems { get; } = new();

}
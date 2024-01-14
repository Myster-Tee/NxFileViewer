using System;
using System.Collections.Generic;
using LibHac.Common.Keys;
using LibHac.Fs.Fsa;
using LibHac.Tools.FsSystem;

namespace Emignatik.NxFileViewer.Models.TreeItems.Impl;

public abstract class PartitionFileSystemItemBase : ItemBase
{
    protected PartitionFileSystemItemBase(IFileSystem partitionFileSystem, ItemBase? parent) : base(parent)
    {
        PartitionFileSystem = partitionFileSystem ?? throw new ArgumentNullException(nameof(partitionFileSystem));
    }

    public IFileSystem PartitionFileSystem { get; }

    public string PartitionType => PartitionFileSystem.GetType().Name;

    public int NbEntries => PartitionFileSystem.GetEntryCount(OpenDirectoryMode.All);

    public sealed override string LibHacTypeName => PartitionFileSystem.GetType().Name;

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
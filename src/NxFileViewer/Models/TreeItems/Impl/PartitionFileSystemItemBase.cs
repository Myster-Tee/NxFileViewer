using System;
using System.Linq;
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

    /// <summary>
    /// Get child items of type <see cref="NcaItem"/>
    /// </summary>
    public NcaItem[] NcaChildItems => base.ChildItems.OfType<NcaItem>().ToArray();

    /// <summary>
    /// Get child items of type <see cref="TicketItem"/>
    /// </summary>
    public TicketItem[] TicketChildItems => base.ChildItems.OfType<TicketItem>().ToArray();

    /// <summary>
    /// Get child items of type <see cref="PartitionFileEntryItemBase"/>
    /// </summary>
    public PartitionFileEntryItemBase[] PartitionFileEntryChildItems => base.ChildItems.OfType<PartitionFileEntryItemBase>().ToArray();

}
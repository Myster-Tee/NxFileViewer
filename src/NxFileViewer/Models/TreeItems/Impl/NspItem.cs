using System;
using LibHac.Common.Keys;
using LibHac.Fs.Fsa;
using LibHac.FsSystem;

namespace Emignatik.NxFileViewer.Models.TreeItems.Impl;

/// <summary>
/// A NSP item is in fact a partition (magic number PFS0) which generally contains
/// some *.nca files and some other files like 
/// </summary>
public class NspItem : PartitionFileSystemItemBase
{
    private readonly IFile _file;

    public NspItem(PartitionFileSystem nspPartitionFileSystem, string name, IFile file, KeySet keySet)
        : base(nspPartitionFileSystem, null)
    {
        NspPartitionFileSystem = nspPartitionFileSystem ?? throw new ArgumentNullException(nameof(nspPartitionFileSystem));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        _file = file ?? throw new ArgumentNullException(nameof(file));
        KeySet = keySet ?? throw new ArgumentNullException(nameof(keySet));
    }

    public override string Format => "NSP";

    public PartitionFileSystem NspPartitionFileSystem { get; }

    public override KeySet KeySet { get; }

    public override string Name { get; }

    public override string DisplayName => Name;

    public override void Dispose()
    {
        _file.Dispose();
    }

}
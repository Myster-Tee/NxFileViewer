using System;
using System.IO;
using Emignatik.NxFileViewer.Utils;
using LibHac.Common.Keys;
using LibHac.Fs;
using LibHac.FsSystem;

namespace Emignatik.NxFileViewer.Models.TreeItems.Impl;

/// <summary>
/// A NSP item is in fact a partition (magic number PFS0) which generally contains
/// some *.nca files and some other files like 
/// </summary>
public class NspItem : PartitionFileSystemItemBase
{
    private readonly IStorage _storage;
    private readonly PartitionFileSystem _nspPartitionFileSystem;

    private NspItem(PartitionFileSystem nspPartitionFileSystem, string name, KeySet keySet, IStorage storage)
        : base(nspPartitionFileSystem, null)
    {
        _nspPartitionFileSystem = nspPartitionFileSystem ?? throw new ArgumentNullException(nameof(nspPartitionFileSystem));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        KeySet = keySet ?? throw new ArgumentNullException(nameof(keySet));

        _storage = storage ?? throw new ArgumentNullException(nameof(storage));
    }

    public override string Format => "NSP";

    public override KeySet KeySet { get; }

    public override string Name { get; }

    public override string DisplayName => Name;

    public override void Dispose()
    {
        _nspPartitionFileSystem.Dispose();
        _storage.Dispose();
    }

    public static NspItem FromFile(string nspFilePath, KeySet keySet)
    {
        var localStorage = new LocalStorage(nspFilePath, FileAccess.Read);
        var partitionFileSystem = localStorage.LoadPartition();
        var nspItem = new NspItem(partitionFileSystem, System.IO.Path.GetFileName(nspFilePath), keySet, localStorage);
        return nspItem;
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using LibHac.Common.Keys;
using LibHac.Fs;
using LibHac.FsSystem;
using LibHac.Tools.Fs;

namespace Emignatik.NxFileViewer.Models.TreeItems.Impl;

public class XciItem : ItemBase
{
    private readonly IStorage _storage;

    public XciItem(Xci xci, string name, IStorage storage, KeySet keySet) : base(null)
    {
        Xci = xci ?? throw new ArgumentNullException(nameof(xci));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        _storage = storage ?? throw new ArgumentNullException(nameof(storage));
        KeySet = keySet ?? throw new ArgumentNullException(nameof(keySet));
    }

    public override List<XciPartitionItem> ChildItems { get; } = new();

    public Xci Xci { get; }

    public override string LibHacTypeName => Xci.GetType().Name;

    public override string? Format => "XCI";

    public override string Name { get; }

    public override string DisplayName => Name;

    public KeySet KeySet { get; }

    public override void Dispose()
    {
        _storage.Dispose();
    }

    public static XciItem FromFile(string xciFilePath, KeySet keySet)
    {
        var fileStorage = new LocalStorage(xciFilePath, FileAccess.Read);

        var xci = new Xci(keySet, fileStorage);

        var xciItem = new XciItem(xci, System.IO.Path.GetFileName(xciFilePath), fileStorage, keySet);

        return xciItem;
    }
}
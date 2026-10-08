using System;
using System.IO;
using LibHac.Common.Keys;
using LibHac.Fs;
using LibHac.FsSystem;
using LibHac.Tools.Fs;
using LibHac.Tools.FsSystem.NcaUtils;
using Path = System.IO.Path;

namespace Emignatik.NxFileViewer.Models.TreeItems.Impl;

public sealed class StandaloneNcaFileItem : PartitionFileSystemItemBase
{
    private readonly LocalStorage? _storage;
    public StandaloneNcaFileItem(string path, KeySet keys)
        : base(new LocalFileSystem(Path.GetDirectoryName(Path.GetFullPath(path))!), null)
    {
        Name = Path.GetFileName(path);
        KeySet = keys;
        try
        {
            _storage = new LocalStorage(path, FileAccess.Read);
            NcaItem = new NcaItem(new Nca(keys, _storage), new DirectoryEntryEx(
                Name, "/" + Name, DirectoryEntryType.File, new FileInfo(path).Length), this);
        }
        catch { Dispose(); throw; }
    }
    public NcaItem NcaItem { get; }
    public override string Name { get; }
    public override string DisplayName => Name;
    public override string Format => "NCA";
    public override KeySet KeySet { get; }
    public override void Dispose() { _storage?.Dispose(); PartitionFileSystem.Dispose(); }
}

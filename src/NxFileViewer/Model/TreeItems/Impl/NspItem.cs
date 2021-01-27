using System;
using Emignatik.NxFileViewer.FileLoading;
using LibHac;
using LibHac.Fs.Fsa;
using LibHac.FsSystem;

namespace Emignatik.NxFileViewer.Model.TreeItems.Impl
{
    /// <summary>
    /// A NSP item is in fact a partition (magic number PFS0) which generally contains
    /// some *.nca files and some other files like 
    /// </summary>
    public class NspItem : PartitionFileSystemItem
    {
        private readonly IFile _file;

        public NspItem(PartitionFileSystem nspPartitionFileSystem, string name, IFile file, Keyset keySet, IChildItemsBuilder childItemsBuilder) : base(nspPartitionFileSystem, childItemsBuilder)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            _file = file ?? throw new ArgumentNullException(nameof(file));
            KeySet = keySet ?? throw new ArgumentNullException(nameof(keySet));
        }

        public override Keyset KeySet { get; }

        public string Name { get; }

        public override string DisplayName => Name;

        public override IItem? ParentItem => null;

        public override void Dispose()
        {
            _file.Dispose();
        }

    }
}
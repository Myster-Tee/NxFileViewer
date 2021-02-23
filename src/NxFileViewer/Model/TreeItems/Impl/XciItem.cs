using System;
using System.Collections.Generic;
using Emignatik.NxFileViewer.FileLoading;
using LibHac;
using LibHac.Fs.Fsa;

namespace Emignatik.NxFileViewer.Model.TreeItems.Impl
{
    public class XciItem : ItemBase
    {
        private readonly IFile _file;
        private IReadOnlyList<XciPartitionItem>? _xciPartitionItems;

        public XciItem(Xci xci, string name, IFile file, Keyset keySet, IChildItemsBuilder childItemsBuilder)
            : base(childItemsBuilder)
        {
            Xci = xci ?? throw new ArgumentNullException(nameof(xci));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            _file = file ?? throw new ArgumentNullException(nameof(file));
            KeySet = keySet ?? throw new ArgumentNullException(nameof(keySet));
        }

        public Xci Xci { get; }

        public override string LibHacTypeName => nameof(Xci);

        public override string? LibHacUnderlyingTypeName => null;

        public override string Name { get; }

        public override string DisplayName => Name;

        public override IItem? ParentItem => null;

        public IReadOnlyList<XciPartitionItem> Partitions => GetPartitions();

        public Keyset KeySet { get; }

        protected sealed override IReadOnlyList<IItem> SafeLoadChildItemsInternal()
        {
            return GetPartitions();
        }

        private IReadOnlyList<XciPartitionItem> GetPartitions()
        {
            return _xciPartitionItems ??= ChildItemsBuilder.Build(this);
        }

        public override void Dispose()
        {
            _file.Dispose();
        }
    }
}
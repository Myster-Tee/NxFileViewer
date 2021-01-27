using System;
using System.Collections.Generic;
using System.Linq;
using Emignatik.NxFileViewer.FileLoading;
using LibHac;
using LibHac.Fs.Fsa;

namespace Emignatik.NxFileViewer.Model.TreeItems.Impl
{
    public class XciItem : ItemBase
    {
        private readonly IFile _file;
        private readonly IChildItemsBuilder _childItemsBuilder;
        private List<XciPartitionItem>? _xciPartitionItems;

        public XciItem(Xci xci, string name, IFile file, Keyset keySet, IChildItemsBuilder childItemsBuilder)
        {
            Xci = xci ?? throw new ArgumentNullException(nameof(xci));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            _file = file ?? throw new ArgumentNullException(nameof(file));
            _childItemsBuilder = childItemsBuilder ?? throw new ArgumentNullException(nameof(childItemsBuilder));
            KeySet = keySet ?? throw new ArgumentNullException(nameof(keySet));
        }

        public string Name { get; }

        public override string ObjectType => nameof(Xci);

        public override string DisplayName => Name;

        public override IItem? ParentItem => null;

        public Xci Xci { get; }

        public IReadOnlyList<XciPartitionItem> Partitions => GetPartitions();

        public Keyset KeySet { get; }

        protected override IEnumerable<IItem> LoadChildItems()
        {
            return GetPartitions();
        }

        private IReadOnlyList<XciPartitionItem> GetPartitions()
        {
            return _xciPartitionItems ??= _childItemsBuilder.Build(this).ToList();
        }

        public override void Dispose()
        {
            _file.Dispose();
        }
    }
}
using System.Collections.Generic;
using Emignatik.NxFileViewer.Model.TreeItems;
using Emignatik.NxFileViewer.Model.TreeItems.Impl;

namespace Emignatik.NxFileViewer.FileLoading
{
    public interface IChildItemsBuilder
    {
        IReadOnlyList<XciPartitionItem> Build(XciItem parentItem);

        IPartitionChildByTypes Build(PartitionFileSystemItem parentItem);

        IReadOnlyList<SectionItem> Build(NcaItem parentItem);

        IReadOnlyList<DirectoryEntryItem> Build(SectionItem section);

        IReadOnlyList<DirectoryEntryItem> Build(DirectoryEntryItem parentItem);

        IReadOnlyList<IItem> Build(PartitionFileEntryItem parentItem);
    }

    public interface IPartitionChildByTypes
    {
        public IReadOnlyList<IItem> AllChildItems { get; }
        public IReadOnlyList<NcaItem> NcaItems { get; }
        public IReadOnlyList<PartitionFileEntryItem> PartitionFileEntryItems { get; }
    }
}
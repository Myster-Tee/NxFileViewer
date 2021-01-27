using System;
using System.Collections.Generic;
using Emignatik.NxFileViewer.Model.TreeItems;
using Emignatik.NxFileViewer.Model.TreeItems.Impl;

namespace Emignatik.NxFileViewer.FileLoading
{
    public interface IChildItemsBuilder
    {
        IEnumerable<XciPartitionItem> Build(XciItem parentItem);

        PartitionChildByTypes Build(PartitionFileSystemItem parentItem);

        IEnumerable<SectionItem> Build(NcaItem parentItem);

        IEnumerable<DirectoryEntryItem> Build(SectionItem parentItem);

        IEnumerable<DirectoryEntryItem> Build(DirectoryEntryItem parentItem);

        IEnumerable<IItem> Build(PartitionFileEntryItem partitionFileEntryItem);
    }

    public class PartitionChildByTypes
    {
        public List<IItem> AllChildItems { get; } = new List<IItem>();
        public List<NcaItem> NcaItems { get; } = new List<NcaItem>();
        public List<PartitionFileEntryItem> PartitionFileEntryItems { get; } = new List<PartitionFileEntryItem>();
    }
}
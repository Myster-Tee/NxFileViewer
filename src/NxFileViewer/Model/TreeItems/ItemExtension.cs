using System;
using System.Collections.Generic;
using System.Linq;
using Emignatik.NxFileViewer.Model.TreeItems.Impl;
using LibHac.FsSystem.NcaUtils;

namespace Emignatik.NxFileViewer.Model.TreeItems
{
    public static class ItemExtension
    {
        public static IEnumerable<NcaItem> FindAllNcaItems(this IItem? itemRoot)
        {
            var remainingItemsToVisit = new List<IItem?> { itemRoot };

            while (remainingItemsToVisit.Count > 0)
            {
                var item = remainingItemsToVisit[0];
                if (item == null)
                    continue;

                if (item is NcaItem ncaItem)
                    yield return ncaItem;

                remainingItemsToVisit.RemoveAt(0);

                remainingItemsToVisit.AddRange(item.ChildItems);
            }
        }

        public static NcaItem? FindNcaItem(this PartitionFileSystemItem partitionItem, string ncaId)
        {
            var expectedFileName = ncaId;
            return partitionItem.NcaItems.FirstOrDefault(ncaItem => string.Equals(ncaItem.Id, expectedFileName, StringComparison.OrdinalIgnoreCase));
        }

        public static NacpItem? FindNacpItem(this NcaItem ncaItem)
        {
            foreach (var sectionItem in ncaItem.Sections)
            {
                foreach (var dirEntry in sectionItem.ChildDirectoryEntryItems)
                {
                    if (dirEntry is NacpItem nacpItem)
                        return nacpItem;
                }
            }

            return null;
        }

        public static IEnumerable<CnmtItem> FindAllCnmtItems(this PartitionFileSystemItem partitionItem)
        {
            foreach (var ncaItem in partitionItem.NcaItems)
            {
                if (ncaItem.ContentType != NcaContentType.Meta)
                    continue;
                foreach (var sectionItem in ncaItem.Sections)
                {
                    foreach (var child in sectionItem.ChildItems)
                    {
                        if (child is CnmtItem cnmtItem)
                            yield return cnmtItem;
                    }
                }
            }
        }
    }
}

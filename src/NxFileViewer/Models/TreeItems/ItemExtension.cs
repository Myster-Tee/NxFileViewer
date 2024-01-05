using System;
using System.Collections.Generic;
using System.Linq;
using Emignatik.NxFileViewer.Models.TreeItems.Impl;
using LibHac.Tools.FsSystem.NcaUtils;

namespace Emignatik.NxFileViewer.Models.TreeItems;

public static class ItemExtension
{
    public static IEnumerable<NcaItem> FindAllNcaItems(this IItem? itemRoot)
    {
        var remainingItemsToVisit = new List<IItem?> { itemRoot };

        while (remainingItemsToVisit.Count > 0)
        {
            var item = remainingItemsToVisit[0];
            remainingItemsToVisit.RemoveAt(0);

            if (item == null)
                continue;

            if (item is NcaItem ncaItem)
                yield return ncaItem;

            remainingItemsToVisit.AddRange(item.ChildItems);
        }
    }

    public static NcaItem? FindNcaItem(this PartitionFileSystemItemBase partitionItem, string ncaId)
    {
        var expectedFileName = ncaId;
        return partitionItem.NcaChildItems.FirstOrDefault(ncaItem => string.Equals(ncaItem.Id, expectedFileName, StringComparison.OrdinalIgnoreCase));
    }

    public static NacpItem? FindNacpItem(this NcaItem ncaItem)
    {
        foreach (var sectionItem in ncaItem.ChildItems)
        {
            foreach (var dirEntry in sectionItem.ChildItems)
            {
                if (dirEntry is NacpItem nacpItem)
                    return nacpItem;
            }
        }

        return null;
    }

    public static IEnumerable<CnmtItem> FindAllCnmtItems(this PartitionFileSystemItemBase partitionItem)
    {
        foreach (var ncaItem in partitionItem.NcaChildItems)
        {
            if (ncaItem.ContentType != NcaContentType.Meta)
                continue;
            foreach (var sectionItem in ncaItem.ChildItems)
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
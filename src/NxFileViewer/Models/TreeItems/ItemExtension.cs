using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Emignatik.NxFileViewer.Models.TreeItems.Impl;
using LibHac.Tools.FsSystem.NcaUtils;

namespace Emignatik.NxFileViewer.Models.TreeItems;

public static class ItemExtension
{

    [Pure]
    public static NcaItem? FindNcaItem(this PartitionFileSystemItemBase partitionItem, string ncaId)
    {
        var expectedFileName = ncaId;
        return partitionItem.NcaChildItems.FirstOrDefault(ncaItem => string.Equals(ncaItem.Id, expectedFileName, StringComparison.OrdinalIgnoreCase));
    }

    [Pure]
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

    public static IEnumerable<T> FindChildrenOfType<T>(this IItem? item, bool includeItem)
    {
        if (item == null)
            yield break;

        var remainingItemsToVisit = new List<IItem>();

        if (includeItem)
            remainingItemsToVisit.Add(item);
        else
            remainingItemsToVisit.AddRange(item.ChildItems);

        while (remainingItemsToVisit.Count > 0)
        {
            var itemTmp = remainingItemsToVisit[0];
            remainingItemsToVisit.RemoveAt(0);

            if (itemTmp is T t)
                yield return t;

            remainingItemsToVisit.AddRange(itemTmp.ChildItems);
        }
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
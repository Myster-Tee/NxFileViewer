using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Emignatik.NxFileViewer.Models.TreeItems.Impl;
using LibHac.Tools.FsSystem.NcaUtils;

namespace Emignatik.NxFileViewer.Models.TreeItems;

public static class ItemExtension
{
    /// <summary>
    /// Find the <see cref="NcaItem"/> with the given <paramref name="ncaId"/>
    /// </summary>
    /// <param name="partitionItem"></param>
    /// <param name="ncaId"></param>
    /// <returns></returns>
    [Pure]
    public static NcaItem? FindNcaItem(this PartitionFileSystemItemBase partitionItem, string ncaId)
    {
        var expectedFileName = ncaId;
        return partitionItem.NcaChildItems.FirstOrDefault(ncaItem => string.Equals(ncaItem.Id, expectedFileName, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Find the <see cref="NacpItem"/> from the <see cref="NcaItem"/>
    /// </summary>
    /// <param name="ncaItem"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Generic method to find all children of type <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="item"></param>
    /// <param name="includeItem"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Find all <see cref="CnmtItem"/> from the <paramref name="partitionItem"/>
    /// </summary>
    /// <param name="partitionItem"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Find the <see cref="NcaItem"/> referenced by the <see cref="CnmtContentEntryItem"/>
    /// </summary>
    /// <param name="cnmtContentEntryItem"></param>
    /// <returns></returns>
    [Pure]
    public static NcaItem? FindReferencedNcaItem(this CnmtContentEntryItem cnmtContentEntryItem)
    {
        var expectedNcaId = cnmtContentEntryItem.NcaId;

        var partitionItem = cnmtContentEntryItem.ParentItem.ContainerSectionItem.ParentItem.ParentItem;

        var ncaItem = partitionItem.FindNcaItem(expectedNcaId);

        return ncaItem;
    }
}
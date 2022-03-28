using System;
using System.Collections.Generic;
using LibHac.Ncm;
using LibHac.Tools.Ncm;

namespace Emignatik.NxFileViewer.Models.TreeItems.Impl;

public class CnmtContentEntryItem : ItemBase
{
    public CnmtContentEntryItem(CnmtContentEntry cnmtContentEntry, CnmtItem parentItem, int index) : base(parentItem)
    {
        // TODO: a utiliser au final? Ce serait sûrement plus logique de le présenter dans les propriétés du CnmtItem
        CnmtContentEntry = cnmtContentEntry ?? throw new ArgumentNullException(nameof(cnmtContentEntry));
        Index = index;
        ParentItem = parentItem ?? throw new ArgumentNullException(nameof(parentItem));
    }

    public override string DisplayName => $"Entry {Index}";

    public override string Name => Index.ToString();

    public int Index { get; }

    public CnmtContentEntry CnmtContentEntry { get; }

    public ContentType NcaContentType => CnmtContentEntry.Type;

    public byte[] NcaId => CnmtContentEntry.NcaId;

    public byte[] NcaHash => CnmtContentEntry.Hash;

    public long NcaSize => CnmtContentEntry.Size;

    public new CnmtItem ParentItem { get; }

    public override string LibHacTypeName => CnmtContentEntry.GetType().Name;

    public override string? Format => null;

    public override IEnumerable<IItem> ChildItems { get { yield break; } }

}
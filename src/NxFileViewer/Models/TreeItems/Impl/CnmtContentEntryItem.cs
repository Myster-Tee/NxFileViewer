using System;
using Emignatik.NxFileViewer.Utils;
using LibHac.Ncm;
using LibHac.Tools.Ncm;

namespace Emignatik.NxFileViewer.Models.TreeItems.Impl;

/// <summary>
/// Contains metadata about an NCA
/// </summary>
public class CnmtContentEntryItem : ItemBase
{
    public CnmtContentEntryItem(CnmtContentEntry cnmtContentEntry, CnmtItem parentItem, int index) : base(parentItem)
    {
        CnmtContentEntry = cnmtContentEntry ?? throw new ArgumentNullException(nameof(cnmtContentEntry));
        Index = index;
        ParentItem = parentItem ?? throw new ArgumentNullException(nameof(parentItem));
    }

    public override string DisplayName => $"Entry {Index}";

    public override string Name => Index.ToString();

    public int Index { get; }

    public CnmtContentEntry CnmtContentEntry { get; }

    public ContentType NcaContentType => CnmtContentEntry.Type;

    /// <summary>
    /// The NcaId of the Nca file referenced by this entry
    /// </summary>
    public string NcaId => CnmtContentEntry.NcaId.ToStrId();

    /// <summary>
    /// The hash of the Nca file referenced by this entry
    /// </summary>
    public byte[] NcaHash => CnmtContentEntry.Hash;

    public long NcaSize => CnmtContentEntry.Size;

    public new CnmtItem ParentItem { get; }

    public override string LibHacTypeName => CnmtContentEntry.GetType().Name;

    public override string? Format => null;

}
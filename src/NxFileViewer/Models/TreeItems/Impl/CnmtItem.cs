using System;
using System.Collections.Generic;
using Emignatik.NxFileViewer.Utils;
using LibHac.Ncm;
using LibHac.Tools.Fs;
using LibHac.Tools.FsSystem.NcaUtils;
using LibHac.Tools.Ncm;

namespace Emignatik.NxFileViewer.Models.TreeItems.Impl;

/// <summary>
/// <see cref="IItem"/> wrapping a <see cref="Cnmt"/> file
/// A cnmt file is a metadata file containing various information about what is contained in a package.
/// The cnmt file references many other files (<see cref="Cnmt.ContentEntries"/> as example).
/// </summary>
public class CnmtItem : DirectoryEntryItem, IItem
{
    public CnmtItem(Cnmt cnmt, FsSectionItem parentFsSectionItem, DirectoryEntryEx directoryEntry)
        : base(parentFsSectionItem, directoryEntry)
    {
        Cnmt = cnmt ?? throw new ArgumentNullException(nameof(cnmt));

        ParentItem = parentFsSectionItem;
        PatchNumber = Cnmt.TitleVersion.GetPatchNumber();
        TitleId = Cnmt.TitleId.ToStrId();
        ApplicationTitleId = Cnmt.ApplicationTitleId.ToStrId();
        PatchTitleId = Cnmt.PatchTitleId.ToStrId();
        TitleVersion = Cnmt.TitleVersion?.Version.ToString();
    }

    public sealed override FsSectionItem ParentItem { get; }

    public Cnmt Cnmt { get; }

    public override string Format => nameof(Cnmt);

    public ContentMetaType ContentType => Cnmt.Type;

    public string TitleId { get; }

    public string ApplicationTitleId { get; }

    public string PatchTitleId { get; }

    public string? TitleVersion { get; }

    public int PatchNumber { get; }

    public TitleVersion? MinimumApplicationVersion => Cnmt.MinimumApplicationVersion;

    public TitleVersion? MinimumSystemVersion => Cnmt.MinimumSystemVersion;

    IEnumerable<IItem> IItem.ChildItems => ChildItems;

    public new List<CnmtContentEntryItem> ChildItems { get; } = new();


}
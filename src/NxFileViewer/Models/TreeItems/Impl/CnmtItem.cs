using System;
using System.Linq;
using Emignatik.NxFileViewer.Utils;
using Emignatik.NxFileViewer.Utils.LibHacExtensions;
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
public class CnmtItem : DirectoryEntryItem
{
    public CnmtItem(Cnmt cnmt, SectionItem parentSectionItem, DirectoryEntryEx directoryEntry) : base(parentSectionItem, directoryEntry)
    {
        Cnmt = cnmt ?? throw new ArgumentNullException(nameof(cnmt));
        ParentItem = parentSectionItem;
        PatchNumber = Cnmt.TitleVersion.GetPatchNumber();
        TitleId = Cnmt.TitleId.ToStrId();
        ApplicationTitleId = Cnmt.ApplicationTitleId.ToStrId();
        PatchTitleId = Cnmt.PatchTitleId.ToStrId();
        TitleVersion = Cnmt.TitleVersion?.Version.ToString();
    }

    public new SectionItem ParentItem { get; }

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

    public new CnmtContentEntryItem[] ChildItems => (this as ItemBase).ChildItems.OfType<CnmtContentEntryItem>().ToArray();

}
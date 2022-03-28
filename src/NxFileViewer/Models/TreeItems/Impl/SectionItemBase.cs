using System;
using LibHac.FsSystem;
using LibHac.Tools.FsSystem.NcaUtils;

namespace Emignatik.NxFileViewer.Models.TreeItems.Impl;

public abstract class SectionItemBase : ItemBase
{
    public SectionItemBase(int sectionIndex, NcaFsHeader ncaFsHeader, NcaItem parentItem) : base(parentItem)
    {
        FsHeader = ncaFsHeader;
        SectionIndex = sectionIndex;
        ParentItem = parentItem ?? throw new ArgumentNullException(nameof(parentItem));
    }

    public sealed override string Name => $"Section {SectionIndex}";

    public sealed override string DisplayName => Name;

    public sealed override string LibHacTypeName => FsHeader.GetType().Name;

    public new NcaItem ParentItem { get; }

    public int SectionIndex { get; }

    public NcaFsHeader FsHeader { get; }

    public bool IsPatchSection => FsHeader.IsPatchSection();

    public override string Format => FsHeader.FormatType.ToString();

    public NcaEncryptionType EncryptionType => FsHeader.EncryptionType;

    public NcaHashType HashType => FsHeader.HashType;

    public short Version => FsHeader.Version;

}
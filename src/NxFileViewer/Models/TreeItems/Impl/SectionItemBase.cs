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

    public sealed override string Name => SectionIndex.ToString();

    public sealed override string DisplayName => $"Section {SectionIndex}";

    public sealed override string LibHacTypeName => "Section";

    public override string Format => FsHeader.FormatType.ToString();

    public NcaFsHeader FsHeader { get; }

    public int SectionIndex { get; }

    public new NcaItem ParentItem { get; }

    public bool IsPatchSection => FsHeader.IsPatchSection();

    public NcaEncryptionType EncryptionType => FsHeader.EncryptionType;

    public NcaFormatType FormatType => FsHeader.FormatType;

    public NcaHashType HashType => FsHeader.HashType;

    public short Version => FsHeader.Version;

}
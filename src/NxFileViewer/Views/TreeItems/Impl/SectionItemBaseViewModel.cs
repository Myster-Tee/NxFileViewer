using System;
using Emignatik.NxFileViewer.Models.TreeItems.Impl;
using Emignatik.NxFileViewer.Views.ObjectPropertyViewer;
using LibHac.FsSystem;

namespace Emignatik.NxFileViewer.Views.TreeItems.Impl;

public abstract class SectionItemBaseViewModel : ItemViewModel
{
    private readonly SectionItemBase _sectionItemBase;

    public SectionItemBaseViewModel(SectionItemBase sectionItemBase, IServiceProvider serviceProvider)
        : base(sectionItemBase, serviceProvider)
    {
        _sectionItemBase = sectionItemBase;
    }

    [PropertyView]
    public int SectionIndex => _sectionItemBase.SectionIndex;

    [PropertyView]
    public NcaEncryptionType EncryptionType => _sectionItemBase.EncryptionType;

    [PropertyView]
    public NcaFormatType FormatType => _sectionItemBase.FormatType;

    [PropertyView]
    public bool IsPatchSection => _sectionItemBase.IsPatchSection;

    [PropertyView]
    public NcaHashType HashType => _sectionItemBase.HashType;

    [PropertyView]
    public short Version => _sectionItemBase.Version;
}
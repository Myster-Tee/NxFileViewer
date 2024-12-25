using System;
using System.Collections.Generic;
using Emignatik.NxFileViewer.Commands;
using Emignatik.NxFileViewer.Localization.Keys;
using Emignatik.NxFileViewer.Models.TreeItems.Impl;
using Emignatik.NxFileViewer.Views.ObjectPropertyViewer;
using LibHac.Tools.FsSystem.NcaUtils;
using Microsoft.Extensions.DependencyInjection;

namespace Emignatik.NxFileViewer.Views.TreeItems.Impl;

public sealed class SectionItemViewModel : ItemViewModel
{
    private readonly SectionItem _sectionItem;
    private readonly IMenuItemViewModel _menuItemSaveSection;

    public SectionItemViewModel(SectionItem sectionItem, IServiceProvider serviceProvider)
        : base(sectionItem, serviceProvider)
    {
        _sectionItem = sectionItem;

        var saveSectionContentCommand = serviceProvider.GetRequiredService<ISaveSectionContentCommand>();
        saveSectionContentCommand.SectionItem = sectionItem;
        _menuItemSaveSection = CreateLocalizedMenuItem(nameof(ILocalizationKeys.ContextMenu_SaveSectionItem), saveSectionContentCommand);
    }

    public override IEnumerable<IMenuItemViewModel> GetOtherContextMenuItems()
    {
        yield return _menuItemSaveSection;
    }

    [PropertyView]
    public int SectionIndex => _sectionItem.SectionIndex;

    [PropertyView]
    public string NcaSectionType => _sectionItem.NcaSectionType?.ToString() ?? "[UNKNOWN]";

    [PropertyView]
    public NcaEncryptionType EncryptionType => _sectionItem.FsHeader.EncryptionType;

    [PropertyView]
    public NcaHashType HashType => _sectionItem.FsHeader.HashType;

    [PropertyView]
    public short Version => _sectionItem.FsHeader.Version;


    [PropertyView]
    public string ContentType => _sectionItem.SectionType.ToString();

    [PropertyView]
    public bool IsSparse => _sectionItem.SparseInfo != null;

    [PropertyView(HideIfNull = true)]
    public ushort? SparseGeneration => _sectionItem.SparseInfo?.Generation;

    [PropertyView(HideIfNull = true)]
    public long? SparseMetaSize => _sectionItem.SparseInfo?.MetaSize;

    [PropertyView(HideIfNull = true)]
    public long? SparseMetaOffset => _sectionItem.SparseInfo?.MetaOffset;

    [PropertyView(HideIfNull = true)]
    public long? SparsePhysicalOffset => _sectionItem.SparseInfo?.PhysicalOffset;


    [PropertyView(HideIfNull = true)]
    public long? PatchRelocationTreeOffset => _sectionItem.PatchInfo?.RelocationTreeOffset;

    [PropertyView(HideIfNull = true)]
    public long? PatchRelocationTreeSize => _sectionItem.PatchInfo?.RelocationTreeSize;

    [PropertyView(HideIfNull = true)]
    public long? PatchEncryptionTreeOffset => _sectionItem.PatchInfo?.EncryptionTreeOffset;

    [PropertyView(HideIfNull = true)]
    public long? PatchEncryptionTreeSize => _sectionItem.PatchInfo?.EncryptionTreeSize;

}
using System;
using System.Collections.Generic;
using Emignatik.NxFileViewer.Models.TreeItems.Impl;
using Emignatik.NxFileViewer.Views.ObjectPropertyViewer;

namespace Emignatik.NxFileViewer.Views.TreeItems.Impl;

public class PatchSectionItemViewModel : SectionItemBaseViewModel
{
    private readonly PatchSectionItem _patchSectionItem;

    public PatchSectionItemViewModel(PatchSectionItem patchSectionItem, IServiceProvider serviceProvider)
        : base(patchSectionItem, serviceProvider)
    {
        _patchSectionItem = patchSectionItem;
    }

    public override IEnumerable<IMenuItemViewModel> GetOtherContextMenuItems()
    {
        yield break;
    }

    [PropertyView]
    public long RelocationTreeOffset => _patchSectionItem.RelocationTreeOffset;

    [PropertyView]
    public long RelocationTreeSize => _patchSectionItem.RelocationTreeSize;

    [PropertyView]
    public long EncryptionTreeOffset => _patchSectionItem.EncryptionTreeOffset;

    [PropertyView]
    public long EncryptionTreeSize => _patchSectionItem.EncryptionTreeSize;

}
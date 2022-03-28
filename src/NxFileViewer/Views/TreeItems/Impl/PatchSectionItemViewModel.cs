using System;
using System.Collections.Generic;
using Emignatik.NxFileViewer.Models.TreeItems.Impl;

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
}
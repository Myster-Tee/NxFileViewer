using System;
using System.Collections.Generic;
using Emignatik.NxFileViewer.Commands;
using Emignatik.NxFileViewer.Localization.Keys;
using Emignatik.NxFileViewer.Models.TreeItems.Impl;
using Microsoft.Extensions.DependencyInjection;

namespace Emignatik.NxFileViewer.Views.TreeItems.Impl;

public class FsSectionItemViewModel : SectionItemBaseViewModel
{
    private readonly FsSectionItem _fsSectionItem;
    private readonly IMenuItemViewModel _menuItemSaveSection;

    public FsSectionItemViewModel(FsSectionItem fsSectionItem, IServiceProvider serviceProvider)
        : base(fsSectionItem, serviceProvider)
    {
        _fsSectionItem = fsSectionItem;

        var saveSectionContentCommand = serviceProvider.GetRequiredService<ISaveFsSectionContentCommand>();
        saveSectionContentCommand.FsSectionItem = fsSectionItem;
        _menuItemSaveSection = CreateLocalizedMenuItem(nameof(ILocalizationKeys.ContextMenu_SaveSectionItem), saveSectionContentCommand);
    }

    public override IEnumerable<IMenuItemViewModel> GetOtherContextMenuItems()
    {
        yield return _menuItemSaveSection;
    }
}
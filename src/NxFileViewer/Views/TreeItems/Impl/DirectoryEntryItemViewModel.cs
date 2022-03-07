using System;
using System.Collections.Generic;
using Emignatik.NxFileViewer.Commands;
using Emignatik.NxFileViewer.Localization.Keys;
using Emignatik.NxFileViewer.Models.TreeItems.Impl;
using Emignatik.NxFileViewer.Utils;
using Emignatik.NxFileViewer.Views.ObjectPropertyViewer;
using LibHac.Fs;
using Microsoft.Extensions.DependencyInjection;

namespace Emignatik.NxFileViewer.Views.TreeItems.Impl
{
    public class DirectoryEntryItemViewModel : ItemViewModel
    {
        private readonly DirectoryEntryItem _directoryEntryItem;
        private readonly IMenuItemViewModel _menuItemSaveEntry;

        public DirectoryEntryItemViewModel(DirectoryEntryItem directoryEntryItem, IServiceProvider serviceProvider)
            : base(directoryEntryItem, serviceProvider)
        {
            _directoryEntryItem = directoryEntryItem;


            SizeStr = _directoryEntryItem.Size.ToFileSize();

            var keyName = directoryEntryItem.DirectoryEntryType == DirectoryEntryType.Directory ? nameof(ILocalizationKeys.ContextMenu_SaveDirectoryItem) : nameof(ILocalizationKeys.ContextMenu_SaveFileItem);
            var saveDirectoryEntryItemCommand = serviceProvider.GetRequiredService<ISaveDirectoryEntryCommand>();
            saveDirectoryEntryItemCommand.DirectoryEntryItem = directoryEntryItem;
            _menuItemSaveEntry = CreateLocalizedMenuItem(keyName, saveDirectoryEntryItemCommand);
        }

        public override IEnumerable<IMenuItemViewModel> GetOtherContextMenuItems()
        {
            yield return _menuItemSaveEntry;
        }

        [PropertyView("Size")]
        public string SizeStr { get; }

        [PropertyView]
        public DirectoryEntryType DirectoryEntryType => _directoryEntryItem.DirectoryEntryType;
    }
}
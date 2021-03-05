using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Data;
using Emignatik.NxFileViewer.Commands;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Localization.Keys;
using Emignatik.NxFileViewer.Model.TreeItems.Impl;
using Emignatik.NxFileViewer.Utils;
using Emignatik.NxFileViewer.Views.ObjectPropertyViewer;
using LibHac.Fs;
using Microsoft.Extensions.DependencyInjection;

namespace Emignatik.NxFileViewer.Views.TreeItems.Impl
{
    public class DirectoryEntryItemViewModel : ItemViewModel
    {
        private readonly DirectoryEntryItem _directoryEntryItem;
        private readonly MenuItem _menuItemSaveEntry;

        public DirectoryEntryItemViewModel(DirectoryEntryItem directoryEntryItem, IServiceProvider serviceProvider)
            : base(directoryEntryItem, serviceProvider)
        {
            _directoryEntryItem = directoryEntryItem;

            var saveDirectoryEntryItemCommand = serviceProvider.GetRequiredService<ISaveDirectoryEntryCommand>();
            saveDirectoryEntryItemCommand.DirectoryEntryItem = directoryEntryItem;

            SizeStr = _directoryEntryItem.Size.ToFileSize();


            _menuItemSaveEntry = new MenuItem
            {
                Command = saveDirectoryEntryItemCommand
            };

            var keyName = directoryEntryItem.DirectoryEntryType == DirectoryEntryType.Directory ? nameof(ILocalizationKeys.ContextMenu_SaveDirectoryItem) : nameof(ILocalizationKeys.ContextMenu_SaveFileItem);
            _menuItemSaveEntry.SetBinding(MenuItem.HeaderProperty, new Binding($"Current.Keys.{keyName}")
            {
                Source = LocalizationManager.Instance
            });

        }

        public override IEnumerable<MenuItem> GetOtherContextMenuItems()
        {
            yield return _menuItemSaveEntry;
        }

        [PropertyView("Size")]
        public string SizeStr { get; }

        [PropertyView]
        public DirectoryEntryType DirectoryEntryType => _directoryEntryItem.DirectoryEntryType;
    }
}
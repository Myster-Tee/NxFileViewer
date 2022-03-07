using System;
using System.Collections.Generic;
using Emignatik.NxFileViewer.Commands;
using Emignatik.NxFileViewer.Localization.Keys;
using Emignatik.NxFileViewer.Models.TreeItems.Impl;
using Emignatik.NxFileViewer.Utils;
using Emignatik.NxFileViewer.Views.ObjectPropertyViewer;
using Microsoft.Extensions.DependencyInjection;

namespace Emignatik.NxFileViewer.Views.TreeItems.Impl
{
    public class PartitionFileEntryItemViewModel : ItemViewModel
    {
        private readonly PartitionFileEntryItemBase _partitionFileEntryItem;
        private readonly IMenuItemViewModel _menuItemSavePartitionFile;

        public PartitionFileEntryItemViewModel(PartitionFileEntryItemBase partitionFileEntryItem, IServiceProvider serviceProvider)
            : base(partitionFileEntryItem, serviceProvider)
        {
            _partitionFileEntryItem = partitionFileEntryItem;

            var savePartitionFileItemCommand = serviceProvider.GetRequiredService<ISavePartitionFileCommand>();
            savePartitionFileItemCommand.PartitionFileItem = partitionFileEntryItem;
            _menuItemSavePartitionFile = CreateLocalizedMenuItem(nameof(ILocalizationKeys.ContextMenu_SavePartitionFileItem), savePartitionFileItemCommand);
        }

        public override IEnumerable<IMenuItemViewModel> GetOtherContextMenuItems()
        {
            yield return _menuItemSavePartitionFile;
        }

        [PropertyView]
        public string Size => _partitionFileEntryItem.Size.ToFileSize();
    }
}
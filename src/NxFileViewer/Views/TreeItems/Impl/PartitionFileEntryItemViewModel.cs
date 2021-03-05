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
using Microsoft.Extensions.DependencyInjection;

namespace Emignatik.NxFileViewer.Views.TreeItems.Impl
{
    public class PartitionFileEntryItemViewModel : ItemViewModel
    {
        private readonly PartitionFileEntryItemBase _partitionFileEntryItem;
        private readonly MenuItem _menuItemSavePartitionFile;

        public PartitionFileEntryItemViewModel(PartitionFileEntryItemBase partitionFileEntryItem, IServiceProvider serviceProvider)
            : base(partitionFileEntryItem, serviceProvider)
        {
            _partitionFileEntryItem = partitionFileEntryItem;

            var savePartitionFileItemCommand = serviceProvider.GetRequiredService<ISavePartitionFileCommand>();
            savePartitionFileItemCommand.PartitionFileItem = partitionFileEntryItem;

            _menuItemSavePartitionFile = new MenuItem
            {
                Command = savePartitionFileItemCommand
            };

            _menuItemSavePartitionFile.SetBinding(MenuItem.HeaderProperty, new Binding($"Current.Keys.{nameof(ILocalizationKeys.ContextMenu_SavePartitionFileItem)}")
            {
                Source = LocalizationManager.Instance
            });
        }

        public override IEnumerable<MenuItem> GetOtherContextMenuItems()
        {
            yield return _menuItemSavePartitionFile;
        }

        [PropertyView]
        public string Size => _partitionFileEntryItem.Size.ToFileSize();
    }
}
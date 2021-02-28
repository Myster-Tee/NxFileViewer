using System;
using Emignatik.NxFileViewer.Model.TreeItems.Impl;
using Emignatik.NxFileViewer.Utils;
using Emignatik.NxFileViewer.Views.ObjectPropertyViewer;

namespace Emignatik.NxFileViewer.Views.TreeItems.Impl
{
    public class PartitionFileEntryItemViewModel : ItemViewModel
    {
        private readonly PartitionFileEntryItemBase _partitionFileEntryItem;

        public PartitionFileEntryItemViewModel(PartitionFileEntryItemBase partitionFileEntryItem, IServiceProvider serviceProvider)
            : base(partitionFileEntryItem, serviceProvider)
        {
            _partitionFileEntryItem = partitionFileEntryItem;
        }

        [PropertyView]
        public string Size => _partitionFileEntryItem.Size.ToFileSize();
    }
}
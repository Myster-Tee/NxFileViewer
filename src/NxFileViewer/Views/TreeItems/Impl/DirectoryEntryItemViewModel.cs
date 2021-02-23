using System;
using Emignatik.NxFileViewer.Model.TreeItems.Impl;
using Emignatik.NxFileViewer.Utils;
using Emignatik.NxFileViewer.Views.ObjectPropertyViewer;

namespace Emignatik.NxFileViewer.Views.TreeItems.Impl
{
    public class DirectoryEntryItemViewModel : ItemViewModel
    {
        private readonly DirectoryEntryItem _directoryEntryItem;

        public DirectoryEntryItemViewModel(DirectoryEntryItem directoryEntryItem, IServiceProvider serviceProvider)
            : base(directoryEntryItem, serviceProvider)
        {
            _directoryEntryItem = directoryEntryItem;
            SizeStr = _directoryEntryItem.Size.ToFileSize();
        }

        [PropertyView("Size")]
        public string SizeStr { get; }

        [PropertyView]
        public string DirectoryEntryType => _directoryEntryItem.DirectoryEntryType.ToString();
    }
}
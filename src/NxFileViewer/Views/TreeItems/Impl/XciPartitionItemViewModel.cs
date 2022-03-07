using System;
using Emignatik.NxFileViewer.Models.TreeItems.Impl;
using Emignatik.NxFileViewer.Views.ObjectPropertyViewer;
using LibHac.Tools.Fs;

namespace Emignatik.NxFileViewer.Views.TreeItems.Impl
{
    public class XciPartitionItemViewModel : PartitionFileSystemItemViewModel
    {
        private readonly XciPartitionItem _xciPartitionItem;

        public XciPartitionItemViewModel(XciPartitionItem xciPartitionItem, IServiceProvider serviceProvider)
            : base(xciPartitionItem, serviceProvider)
        {
            _xciPartitionItem = xciPartitionItem;
        }

        [PropertyView]
        public XciPartitionType XciPartitionType => _xciPartitionItem.XciPartitionType;

    }
}
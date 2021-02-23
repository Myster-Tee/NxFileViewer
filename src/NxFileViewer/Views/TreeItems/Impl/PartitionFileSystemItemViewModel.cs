using System;
using Emignatik.NxFileViewer.Model.TreeItems.Impl;
using Emignatik.NxFileViewer.Views.ObjectPropertyViewer;
using LibHac.FsSystem;

namespace Emignatik.NxFileViewer.Views.TreeItems.Impl
{
    public abstract class PartitionFileSystemItemViewModel : ItemViewModel
    {
        private readonly PartitionFileSystemItem _partitionFileSystemItem;

        protected PartitionFileSystemItemViewModel(PartitionFileSystemItem partitionFileSystemItem, IServiceProvider serviceProvider)
            : base(partitionFileSystemItem, serviceProvider)
        {
            _partitionFileSystemItem = partitionFileSystemItem;
        }

        [PropertyView]
        public PartitionFileSystemType PartitionType => _partitionFileSystemItem.PartitionType;

        [PropertyView]
        public int NumFiles => _partitionFileSystemItem.NumFiles;
    }
}

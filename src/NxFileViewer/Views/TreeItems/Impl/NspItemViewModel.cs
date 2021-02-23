using System;
using Emignatik.NxFileViewer.Model.TreeItems.Impl;

namespace Emignatik.NxFileViewer.Views.TreeItems.Impl
{
    public class NspItemViewModel : PartitionFileSystemItemViewModel
    {
        private readonly NspItem _nspItem;

        public NspItemViewModel(NspItem nspItem, IServiceProvider serviceProvider)
            : base(nspItem, serviceProvider)
        {
            _nspItem = nspItem;
        }
    }
}
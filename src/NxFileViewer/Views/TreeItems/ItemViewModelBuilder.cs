using System;
using System.Diagnostics;
using Emignatik.NxFileViewer.Model.TreeItems;
using Emignatik.NxFileViewer.Model.TreeItems.Impl;
using Emignatik.NxFileViewer.Views.TreeItems.Impl;

namespace Emignatik.NxFileViewer.Views.TreeItems
{
    public class ItemViewModelBuilder : IItemViewModelBuilder
    {
        private readonly IServiceProvider _serviceProvider;

        public ItemViewModelBuilder(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public IItemViewModel Build(IItem item)
        {
            switch (item)
            {
                case XciItem xciItem:
                    return new XciItemViewModel(xciItem, _serviceProvider);

                case XciPartitionItem xciPartitionItem:
                    return new XciPartitionItemViewModel(xciPartitionItem, _serviceProvider);

                case NspItem nspItem:
                    return new NspItemViewModel(nspItem, _serviceProvider);

                case NcaItem ncaItem:
                    return new NcaItemViewModel(ncaItem, _serviceProvider);

                case PartitionFileEntryItem partitionFileEntryItem:
                    return new PartitionFileEntryItemViewModel(partitionFileEntryItem, _serviceProvider);

                case SectionItem sectionItem:
                    return new SectionItemViewModel(sectionItem, _serviceProvider);

                case CnmtItem cnmtItem:
                    return new CnmtItemViewModel(cnmtItem, _serviceProvider);

                case NacpItem nacpItem:
                    return new NacpItemViewModel(nacpItem, _serviceProvider);

                case MainItem mainItem:
                    return new MainItemViewModel(mainItem, _serviceProvider);

                case DirectoryEntryItem directoryEntryItem:
                    return new DirectoryEntryItemViewModel(directoryEntryItem, _serviceProvider);

                default:
                    Debug.Fail($"{nameof(IItemViewModel)} implementation missing for item of type «{item.GetType().Name}».");
                    return new ItemViewModel(item, _serviceProvider);
            }
        }
    }
}
using System;
using System.Diagnostics;
using Emignatik.NxFileViewer.Models.TreeItems;
using Emignatik.NxFileViewer.Models.TreeItems.Impl;
using Emignatik.NxFileViewer.Views.TreeItems.Impl;

namespace Emignatik.NxFileViewer.Views.TreeItems;

public class ItemViewModelBuilder : IItemViewModelBuilder
{
    private readonly IServiceProvider _serviceProvider;

    public ItemViewModelBuilder(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    public IItemViewModel Build(IItem item)
    {
        IItemViewModel itemViewModel;
        switch (item)
        {
            case XciItem xciItem:
                itemViewModel = new XciItemViewModel(xciItem, _serviceProvider);
                break;

            case XciPartitionItem xciPartitionItem:
                itemViewModel = new XciPartitionItemViewModel(xciPartitionItem, _serviceProvider);
                break;
            case NspItem nspItem:
                itemViewModel = new NspItemViewModel(nspItem, _serviceProvider);
                break;
            case NczItem nczItem:
                itemViewModel = new NczItemViewModel(nczItem, _serviceProvider);
                break;
            case NcaItem ncaItem:
                itemViewModel = new NcaItemViewModel(ncaItem, _serviceProvider);
                break;
            case TicketItem ticketItem:
                itemViewModel = new TicketItemViewModel(ticketItem, _serviceProvider);
                break;
            case PartitionFileEntryItemBase partitionFileEntryItem:
                itemViewModel = new PartitionFileEntryItemViewModel(partitionFileEntryItem, _serviceProvider);
                break;
            case SectionItem patchSectionItem:
                itemViewModel = new SectionItemViewModel(patchSectionItem, _serviceProvider);
                break;
            case CnmtItem cnmtItem:
                itemViewModel = new CnmtItemViewModel(cnmtItem, _serviceProvider);
                break;
            case NacpItem nacpItem:
                itemViewModel = new NacpItemViewModel(nacpItem, _serviceProvider);
                break;
            case MainItem mainItem:
                itemViewModel = new MainItemViewModel(mainItem, _serviceProvider);
                break;
            case DirectoryEntryItem directoryEntryItem:
                itemViewModel = new DirectoryEntryItemViewModel(directoryEntryItem, _serviceProvider);
                break;
            case CnmtContentEntryItem cnmtContentEntryItem:
                itemViewModel = new CnmtContentEntryItemViewModel(cnmtContentEntryItem, _serviceProvider);
                break;
            default:
                Debug.Fail($"{nameof(IItemViewModel)} implementation missing for item of type «{item.GetType().Name}».");
                itemViewModel = new ItemViewModel(item, _serviceProvider);
                break;
        }

        return itemViewModel;
    }
}
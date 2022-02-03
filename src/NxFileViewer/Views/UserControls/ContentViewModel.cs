using System;
using System.Collections.ObjectModel;
using Emignatik.NxFileViewer.Model.TreeItems;
using Emignatik.NxFileViewer.Services;
using Emignatik.NxFileViewer.Utils.MVVM;
using Emignatik.NxFileViewer.Views.TreeItems;
using Microsoft.Extensions.DependencyInjection;

namespace Emignatik.NxFileViewer.Views.UserControls
{
    public class ContentViewModel : ViewModelBase
    {
        private readonly ISelectedItemService _selectedItemService;
        private IItemViewModel? _selectedItemViewModel;
            
        public ContentViewModel(IItem rootItem, IServiceProvider serviceProvider)
        {
            rootItem = rootItem ?? throw new ArgumentNullException(nameof(rootItem));
            ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _selectedItemService = serviceProvider.GetRequiredService<ISelectedItemService>();

            var itemViewModelBuilder = ServiceProvider.GetRequiredService<IItemViewModelBuilder>();
            RootItems.Add(itemViewModelBuilder.Build(rootItem));
        }

        public IServiceProvider ServiceProvider { get; }

        public ObservableCollection<IItemViewModel> RootItems { get; } = new();

        public IItemViewModel? SelectedItem
        {
            get => _selectedItemViewModel;
            set
            {
                // DEBT: selected item in the TreeView is not updated when this property is set from the ViewModel (but it doesn't matter as it never happens)
                _selectedItemService.SelectedItem = value?.AttachedItem;
                _selectedItemViewModel = value;
                NotifyPropertyChanged();
            }
        }

    }

}

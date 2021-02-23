using System;
using System.Collections.Generic;
using Emignatik.NxFileViewer.Model.TreeItems;
using Emignatik.NxFileViewer.Services;
using Emignatik.NxFileViewer.Utils.MVVM;
using Emignatik.NxFileViewer.Views.TreeItems;
using Microsoft.Extensions.DependencyInjection;

namespace Emignatik.NxFileViewer.Views
{
    public class ContentViewModel : ViewModelBase
    {
        private readonly IItem _rootItem;
        private readonly ISelectedItemService _selectedItemService;
        private IItemViewModel? _selectedItemViewModel;

        public ContentViewModel(IItem rootItem, IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _rootItem = rootItem ?? throw new ArgumentNullException(nameof(rootItem));
            _selectedItemService = serviceProvider.GetRequiredService<ISelectedItemService>();

            _selectedItemService.SelectedItemChanged += (sender, args) =>
             {
                 NotifyPropertyChanged(nameof(SelectedItem));
             };

            var itemViewModelBuilder = ServiceProvider.GetRequiredService<IItemViewModelBuilder>();
            RootItems = new List<IItemViewModel> { itemViewModelBuilder.Build(_rootItem) };
        }

        public IServiceProvider ServiceProvider { get; }

        public IEnumerable<IItemViewModel> RootItems { get; }

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

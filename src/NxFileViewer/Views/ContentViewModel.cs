using System;
using System.Collections.Generic;
using Emignatik.NxFileViewer.Commands;
using Emignatik.NxFileViewer.Model.TreeItems;
using Emignatik.NxFileViewer.Utils.MVVM;
using Microsoft.Extensions.DependencyInjection;

namespace Emignatik.NxFileViewer.Views
{
    public class ContentViewModel : ViewModelBase
    {
        private readonly IItem _item;
        private IItem? _selectedItem;

        public ContentViewModel(IItem item, IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            SaveSelectedItemCommand = serviceProvider.GetRequiredService<ISaveItemToFileCommand>();
            _item = item ?? throw new ArgumentNullException(nameof(item));
        }

        public IServiceProvider ServiceProvider { get; }


        public IEnumerable<IItem> RootItems
        {
            get
            {
                yield return _item;
            }
        }

        public IItem? SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                SaveSelectedItemCommand.Item = value;
                NotifyPropertyChanged();
            }
        }

        public ISaveItemToFileCommand SaveSelectedItemCommand { get; }

    }
}

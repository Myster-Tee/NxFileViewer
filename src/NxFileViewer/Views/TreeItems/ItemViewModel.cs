using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Emignatik.NxFileViewer.Commands;
using Emignatik.NxFileViewer.Model.TreeItems;
using Emignatik.NxFileViewer.Utils;
using Emignatik.NxFileViewer.Utils.MVVM;
using Emignatik.NxFileViewer.Views.ObjectPropertyViewer;
using Microsoft.Extensions.DependencyInjection;

namespace Emignatik.NxFileViewer.Views.TreeItems
{
    public class ItemViewModel : ViewModelBase, IItemViewModel
    {
        private readonly IItem _item;
        private string _errorsTooltip;

        public ItemViewModel(IItem item, IServiceProvider serviceProvider)
        {
            _item = item ?? throw new ArgumentNullException(nameof(item));
            serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

            var itemViewModelBuilder = serviceProvider.GetRequiredService<IItemViewModelBuilder>();

            Children = _item.ChildItems.Select(childItem => itemViewModelBuilder.Build(childItem)).ToList();

            _item.PropertyChanged += OnItemPropertyChanged;
            _item.Errors.ErrorsChanged += OnItemErrorsChanged;


            SaveItemToFileCommand = serviceProvider.GetRequiredService<ISaveItemToFileCommand>();
            ShowItemErrorsWindowCommand = serviceProvider.GetRequiredService<IShowItemErrorsWindowCommand>();

            UpdateErrorsTooltip();
        }

        [PropertyView("Name")]
        public string Name => _item.Name;

        [PropertyView("Type")]
        public string ObjectType => _item.LibHacTypeName;

        [PropertyView("UnderlyingType")]
        public string? UnderlyingType => _item.LibHacUnderlyingTypeName;

        public IItem AttachedItem => _item;

        public bool HasErrorInDescendants => _item.HasErrorInDescendants;

        public string DisplayName => _item.DisplayName;

        public IReadOnlyCollection<IItemViewModel> Children { get; }

        public ISaveItemToFileCommand SaveItemToFileCommand { get; }

        public IShowItemErrorsWindowCommand ShowItemErrorsWindowCommand { get; }

        public bool HasErrors => _item.Errors.Count > 0;

        public string ErrorsTooltip
        {
            get => _errorsTooltip;
            private set
            {
                _errorsTooltip = value;
                NotifyPropertyChanged();
            }
        }

        private void OnItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IItem.HasErrorInDescendants))
                NotifyPropertyChanged(nameof(HasErrorInDescendants));
        }

        private void OnItemErrorsChanged(object sender, ErrorsChangedHandlerArgs args)
        {
            UpdateErrorsTooltip();
        }

        private void UpdateErrorsTooltip()
        {
            ErrorsTooltip = ErrorsFormatter.Format(_item.Errors);
            NotifyPropertyChanged(nameof(HasErrors));
        }
    }
}
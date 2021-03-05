using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Emignatik.NxFileViewer.Commands;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Localization.Keys;
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
        private readonly MenuItem _menuItemShowErrors;

        private string _errorsTooltip;
        private bool _hasErrors;

        public ItemViewModel(IItem item, IServiceProvider serviceProvider)
        {
            _item = item ?? throw new ArgumentNullException(nameof(item));
            serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

            var itemViewModelBuilder = serviceProvider.GetRequiredService<IItemViewModelBuilder>();

            Children = _item.ChildItems.Select(childItem => itemViewModelBuilder.Build(childItem)).ToList();

            _menuItemShowErrors = CreateLocalizedMenuItem(nameof(ILocalizationKeys.ContextMenu_ShowItemErrors), serviceProvider.GetRequiredService<IShowItemErrorsWindowCommand>());

            _item.PropertyChanged += OnItemPropertyChanged;
            _item.Errors.ErrorsChanged += (_, _) => { UpdateErrors(); };

            UpdateErrors();
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

        public bool HasErrors
        {
            get => _hasErrors;
            private set
            {
                _hasErrors = value;
                NotifyPropertyChanged();
            }
        }

        public string ErrorsTooltip
        {
            get => _errorsTooltip;
            private set
            {
                _errorsTooltip = value;
                NotifyPropertyChanged();
            }
        }

        public IEnumerable<MenuItem> ContextMenuItems
        {
            get
            {
                yield return _menuItemShowErrors;
                foreach (var menuItem in GetOtherContextMenuItems())
                    yield return menuItem;
            }
        }

        public virtual IEnumerable<MenuItem> GetOtherContextMenuItems()
        {
            yield break;
        }

        private void OnItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IItem.HasErrorInDescendants))
                NotifyPropertyChanged(nameof(HasErrorInDescendants));
        }

        private void UpdateErrors()
        {
            var itemErrors = _item.Errors;

            ErrorsTooltip = ErrorsFormatter.Format(itemErrors);
            HasErrors = itemErrors.Count > 0;
        }

        protected MenuItem CreateLocalizedMenuItem(string localizationKey, ICommand command)
        {
            var menuItem = new MenuItem
            {
                Command = command
            };

            menuItem.SetBinding(MenuItem.HeaderProperty, new Binding($"Current.Keys.{localizationKey}")
            {
                Source = LocalizationManager.Instance
            });

            return menuItem;
        }

    }
}
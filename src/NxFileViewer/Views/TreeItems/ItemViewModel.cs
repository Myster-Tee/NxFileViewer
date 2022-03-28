using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Emignatik.NxFileViewer.Commands;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Localization.Keys;
using Emignatik.NxFileViewer.Models.TreeItems;
using Emignatik.NxFileViewer.Utils;
using Emignatik.NxFileViewer.Utils.MVVM;
using Emignatik.NxFileViewer.Views.ObjectPropertyViewer;
using Microsoft.Extensions.DependencyInjection;

namespace Emignatik.NxFileViewer.Views.TreeItems;

public class ItemViewModel : ViewModelBase, IItemViewModel
{
    private readonly IItem _item;
    private readonly IMenuItemViewModel _menuItemShowErrors;

    private string? _errorsTooltip;
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

    [PropertyView("Format")]
    public string? UnderlyingType => _item.Format;

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

    public string? ErrorsTooltip
    {
        get => _errorsTooltip;
        private set
        {
            _errorsTooltip = value;
            NotifyPropertyChanged();
        }
    }

    public IEnumerable<IMenuItemViewModel> ContextMenuItems
    {
        get
        {
            yield return _menuItemShowErrors;
            foreach (var menuItem in GetOtherContextMenuItems())
                yield return menuItem;
        }
    }

    public virtual IEnumerable<IMenuItemViewModel> GetOtherContextMenuItems()
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

    /// <summary>
    /// </summary>
    /// <param name="localizationKey">The localization key to use for the menu item header text</param>
    /// <param name="command"></param>
    /// <returns></returns>
    protected static IMenuItemViewModel CreateLocalizedMenuItem(string localizationKey, ICommand command)
    {
        return new MenuItemViewModel(localizationKey, command);
    }

}

public class MenuItemViewModel : ViewModelBase, IMenuItemViewModel
{
    private readonly string _localizationKey;

    private string? _name = "";

    public MenuItemViewModel(string localizationKey, ICommand command)
    {
        _localizationKey = localizationKey ?? throw new ArgumentNullException(nameof(localizationKey));
        Command = command ?? throw new ArgumentNullException(nameof(command));

        LocalizationManager.Instance.AddWeakLocalizationChangedHandler((_, _) =>
        {
            Update();
        });

        Update();
    }

    private void Update()
    {
        Name = LocalizationManager.Instance.Current.Keys[_localizationKey];
    }

    public string? Name
    {
        get => _name;
        private set
        {
            _name = value;
            NotifyPropertyChanged();
        }
    }

    public ICommand? Command { get; }
}
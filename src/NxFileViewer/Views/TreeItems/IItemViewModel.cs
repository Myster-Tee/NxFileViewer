using System.Collections.Generic;
using System.Windows.Input;
using Emignatik.NxFileViewer.Models.TreeItems;
using Emignatik.NxFileViewer.Utils.MVVM;

namespace Emignatik.NxFileViewer.Views.TreeItems;

public interface IItemViewModel: IViewModelBase
{
    public string DisplayName { get; }

    /// <summary>
    /// Get the end user displayed type of this item 
    /// </summary>
    string LibHacTypeName { get; }

    IItem AttachedItem { get; }

    IReadOnlyCollection<IItemViewModel> Children { get; }

    public bool HasErrorInDescendants { get; }

    /// <summary>
    /// Returns true if this item has errors
    /// </summary>
    public bool HasErrors { get; }

    /// <summary>
    /// Returns errors formatted for the tooltip
    /// </summary>
    public string? ErrorsTooltip { get; }

    public IEnumerable<IMenuItemViewModel> ContextMenuItems { get; }
}

/// <summary>
/// Represents an item of the context menu associated with a <see cref="IItemViewModel"/>
/// </summary>
public interface IMenuItemViewModel
{
    /// <summary>
    /// Name of the menu item
    /// </summary>
    string? Name { get; }

    /// <summary>
    /// Command
    /// </summary>
    ICommand? Command { get; }
}
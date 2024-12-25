using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Emignatik.NxFileViewer.Models.TreeItems;

/// <summary>
/// Base interface exposing the hierarchical tree structure of a loaded file.
/// Each implementation of a <see cref="IItem"/> is supposed to wrap a LibHac model (class or structure)
/// </summary>
public interface IItem : INotifyPropertyChanged, IDisposable
{
    /// <summary>
    /// Get the display name of this item in the tree
    /// </summary>
    string DisplayName { get; }

    /// <summary>
    /// Get the name of the element
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Get the direct parent item
    /// </summary>
    IItem? ParentItem { get; }

    /// <summary>
    /// Get the direct child items
    /// </summary>
    IReadOnlyList<IItem> ChildItems { get; }
        
    /// <summary>
    /// Get the name of the wrapped LibHac object
    /// </summary>
    string LibHacTypeName { get; }

    /// <summary>
    /// Get the name of the item structure format
    /// </summary>
    string? Format { get; }

    /// <summary>
    /// Occurred errors related to this item
    /// </summary>
    IItemErrors Errors { get; }

    /// <summary>
    /// Get the errors of all descendants excluding errors of this item
    /// </summary>
    IReadOnlyList<ItemError> DescendantErrors { get; }

}
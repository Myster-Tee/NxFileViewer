using System;
using System.Collections.Generic;

namespace Emignatik.NxFileViewer.Model.TreeItems
{
    /// <summary>
    /// Base interface exposing the hierarchical tree structure of a loaded file.
    /// Each implementation of a <see cref="IItem"/> is supposed to wrap a LibHac model (class or structure)
    /// </summary>
    public interface IItem : IDisposable
    {
        /// <summary>
        /// Get the display name of this item in the tree
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// Get the end user displayed type of this item 
        /// </summary>
        string ObjectType { get; }

        /// <summary>
        /// Get the direct parent item
        /// </summary>
        IItem? ParentItem { get; }

        /// <summary>
        /// Get the direct child items
        /// </summary>
        IReadOnlyList<IItem> ChildItems { get; }

        /// <summary>
        /// Load the child items.
        /// When force is false, returns the last loaded cached items.
        /// When force is true, discard cached items and forces child items loading.
        /// </summary>
        /// <param name="force"></param>
        /// <returns></returns>
        IReadOnlyList<IItem> LoadChildItems(bool force);
    }
}
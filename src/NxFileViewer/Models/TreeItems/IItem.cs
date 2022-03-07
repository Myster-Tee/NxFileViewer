using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Emignatik.NxFileViewer.Models.TreeItems
{
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
        IEnumerable<IItem> ChildItems { get; }
        
        /// <summary>
        /// Returns true if an error exists in descendants
        /// </summary>
        bool HasErrorInDescendants { get; }

        /// <summary>
        /// Get the name of the wrapped LibHac object
        /// </summary>
        string LibHacTypeName { get; }

        /// <summary>
        /// Get the name of the underlying wrapped LibHac object
        /// </summary>
        string? LibHacUnderlyingTypeName { get; }


        IItemErrors Errors { get; }

        void ReportNbChildErrors(int moreOrLessErrors);

    }
}
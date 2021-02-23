using System.Collections.Generic;
using Emignatik.NxFileViewer.Commands;
using Emignatik.NxFileViewer.Model.TreeItems;
using Emignatik.NxFileViewer.Utils.MVVM;

namespace Emignatik.NxFileViewer.Views.TreeItems
{
    public interface IItemViewModel: IViewModelBase
    {
        public string DisplayName { get; }

        /// <summary>
        /// Get the end user displayed type of this item 
        /// </summary>
        string ObjectType { get; }

        IItem AttachedItem { get; }

        IReadOnlyCollection<IItemViewModel> Children { get; }

        public bool HasErrorInDescendants { get; }

        ISaveItemToFileCommand SaveItemToFileCommand { get; }

        IShowItemErrorsWindowCommand ShowItemErrorsWindowCommand { get; }

        /// <summary>
        /// Returns true if this item has errors
        /// </summary>
        public bool HasErrors { get; }

        /// <summary>
        /// Returns errors formatted for the tooltip
        /// </summary>
        public string ErrorsTooltip { get; }
    }
}
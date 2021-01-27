using System.Collections.Generic;
using System.Linq;
using Emignatik.NxFileViewer.Views.ObjectPropertyViewer;

namespace Emignatik.NxFileViewer.Model.TreeItems
{
    public abstract class ItemBase : IItem
    {
        private IReadOnlyList<IItem>? _childItems;

        [PropertiesView("Type")]
        public abstract string ObjectType { get; }

        public abstract string DisplayName { get; }

        public IReadOnlyList<IItem> ChildItems
        {
            get { return _childItems ??= LoadChildItems().ToArray(); }
        }

        public abstract IItem? ParentItem { get; }

        protected abstract IEnumerable<IItem> LoadChildItems();

        public override string ToString()
        {
            return DisplayName;
        }

        public virtual void Dispose()
        {
        }
    }
}
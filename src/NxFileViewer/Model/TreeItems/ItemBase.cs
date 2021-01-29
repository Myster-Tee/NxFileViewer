using System.Collections.Generic;
using System.Linq;
using Emignatik.NxFileViewer.Views.ObjectPropertyViewer;

namespace Emignatik.NxFileViewer.Model.TreeItems
{
    public abstract class ItemBase : IItem
    {
        [PropertiesView("Type")]
        public abstract string ObjectType { get; }

        public abstract string DisplayName { get; }

        public IReadOnlyList<IItem> ChildItems => LoadChildItems(force: false).ToArray();

        public abstract IItem? ParentItem { get; }


        public abstract IReadOnlyList<IItem> LoadChildItems(bool force);

        public override string ToString()
        {
            return DisplayName;
        }

        public virtual void Dispose()
        {
        }
    }
}
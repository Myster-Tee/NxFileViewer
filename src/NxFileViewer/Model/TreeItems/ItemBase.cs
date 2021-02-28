using System.Collections.Generic;
using Emignatik.NxFileViewer.Model.TreeItems.Impl;
using Emignatik.NxFileViewer.Utils.MVVM;

namespace Emignatik.NxFileViewer.Model.TreeItems
{
    public abstract class ItemBase : NotifyPropertyChangedBase, IItem
    {
        private int _nbErrorsBeforeNextChange = 0;
        private int _nbChildErrors = 0;

        protected ItemBase()
        {
            Errors.ErrorsChanged += OnErrorsChanged;
        }

        public abstract string Name { get; }

        public abstract string DisplayName { get; }

        public abstract IEnumerable<IItem> ChildItems { get; }

        public abstract IItem? ParentItem { get; }


        public abstract string LibHacTypeName { get; }

        public abstract string? LibHacUnderlyingTypeName { get; }

        public bool HasErrorInDescendants => _nbChildErrors > 0;

        public IItemErrors Errors { get; } = new ItemErrors();

        public void ReportNbChildErrors(int moreOrLessErrors)
        {
            _nbChildErrors += moreOrLessErrors;
            NotifyPropertyChanged(nameof(HasErrorInDescendants));
        }

        public override string ToString()
        {
            return DisplayName;
        }

        public virtual void Dispose()
        {
        }

        private void OnErrorsChanged(object sender, ErrorsChangedHandlerArgs args)
        {
            var delta = Errors.Count - _nbErrorsBeforeNextChange;
            _nbErrorsBeforeNextChange = Errors.Count;

            var parentTemp = ParentItem;

            while (parentTemp != null)
            {
                parentTemp.ReportNbChildErrors(delta);
                parentTemp = parentTemp.ParentItem;
            }
        }

    }
}
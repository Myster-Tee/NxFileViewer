using System.Collections.Generic;
using Emignatik.NxFileViewer.Models.TreeItems.Impl;
using Emignatik.NxFileViewer.Utils.MVVM;

namespace Emignatik.NxFileViewer.Models.TreeItems;

public abstract class ItemBase : NotifyPropertyChangedBase, IItem
{
    private int _nbErrorsPrev = 0;
    private int _nbChildErrors = 0;

    protected ItemBase(ItemBase? parent)
    {
        ParentItem = parent;
        Errors.ErrorsChanged += OnErrorsChanged;
    }

    public abstract string Name { get; }

    public abstract string DisplayName { get; }

    public abstract IEnumerable<IItem> ChildItems { get; }

    IItem? IItem.ParentItem => ParentItem;

    public ItemBase? ParentItem { get; }

    public abstract string LibHacTypeName { get; }

    public abstract string? Format { get; }

    public bool HasErrorInDescendants => _nbChildErrors > 0;

    public IItemErrors Errors { get; } = new ItemErrors();

    private void BubbleNbErrors(int moreOrLessErrors)
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
        var delta = Errors.Count - _nbErrorsPrev;
        _nbErrorsPrev = Errors.Count;

        var parentTemp = ParentItem;

        while (parentTemp != null)
        {
            parentTemp.BubbleNbErrors(delta);
            parentTemp = parentTemp.ParentItem;
        }
    }

}
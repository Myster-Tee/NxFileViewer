using System.Collections.Generic;
using Emignatik.NxFileViewer.Models.TreeItems.Impl;
using Emignatik.NxFileViewer.Utils.MVVM;

namespace Emignatik.NxFileViewer.Models.TreeItems;

public abstract class ItemBase : NotifyPropertyChangedBase, IItem
{
    private readonly List<IItem> _childItems = [];
    private readonly List<ItemError> _descendantErrors = new();

    protected ItemBase(ItemBase? parent)
    {
        ParentItem = parent;
        parent?._childItems.Add(this);
        Errors.ErrorsChanged += OnErrorsChanged;
    }

    public abstract string Name { get; }

    public abstract string DisplayName { get; }

    public IReadOnlyList<IItem> ChildItems => _childItems;

    IItem? IItem.ParentItem => ParentItem;

    public ItemBase? ParentItem { get; }

    public abstract string LibHacTypeName { get; }

    public abstract string? Format { get; }

    public IItemErrors Errors { get; } = new ItemErrors();

    public IReadOnlyList<ItemError> DescendantErrors => _descendantErrors;

    public override string ToString()
    {
        return DisplayName;
    }

    public virtual void Dispose()
    {
    }

    private void OnErrorsChanged(object sender, ErrorsChangedHandlerArgs args)
    {
        var parentTemp = ParentItem;

        while (parentTemp != null)
        {
            foreach (var addedError in args.AddedErrors)
            {
                parentTemp._descendantErrors.Add(addedError);
            }

            foreach (var removedError in args.RemovedErrors)
            {
                parentTemp._descendantErrors.Remove(removedError);
            }

            parentTemp.NotifyPropertyChanged(nameof(DescendantErrors));

            parentTemp = parentTemp.ParentItem;
        }
    }

}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Emignatik.NxFileViewer.Models.TreeItems.Impl;

public class ItemErrors : IItemErrors
{

    private readonly HashSet<ItemError> _itemErrors = new();

    public event ErrorsChangedHandler? ErrorsChanged;

    public int Count
    {
        get
        {
            lock (_itemErrors)
            {
                return _itemErrors.Count;
            }
        }
    }

    public IEnumerator<ItemError> GetEnumerator()
    {
        lock (_itemErrors)
        {
            return _itemErrors.GetEnumerator();
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public bool Add(ItemError error)
    {
        if (error == null)
            throw new ArgumentNullException(nameof(error));

        lock (_itemErrors)
        {
            var added = _itemErrors.Add(error);
            if (added)
                NotifyErrorsChanged([], [error]);
            return added;
        }
    }

    public int RemoveAllOfCategory(Category category)
    {
        lock (_itemErrors)
        {
            var errorsToRemove = _itemErrors.Where(e => e.Category == category).ToArray();
            if (errorsToRemove.Length <= 0)
                return 0;
            foreach (var errorToRemove in errorsToRemove)
            {
                _itemErrors.Remove(errorToRemove);
            }

            NotifyErrorsChanged(errorsToRemove, []);
            return errorsToRemove.Length;
        }
    }

    protected virtual void NotifyErrorsChanged(ItemError[] removedErrors, ItemError[] addedErrors)
    {
        ErrorsChanged?.Invoke(this, new ErrorsChangedHandlerArgs(removedErrors, addedErrors));
    }
}
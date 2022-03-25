using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Emignatik.NxFileViewer.Models.TreeItems.Impl;

public class ItemErrors : IItemErrors
{

    private readonly Dictionary<string, List<string>> _errorsByCategory = new();

    private readonly object _lock = new();

    public event ErrorsChangedHandler? ErrorsChanged;

    public int Count
    {
        get
        {
            lock (_lock)
            {
                return _errorsByCategory.Values.Sum(value => value.Count);
            }
        }
    }

    public IEnumerator<string> GetEnumerator()
    {
        lock (_lock)
        {
            var allErrors = new List<string>();
            foreach (var errorsOfCategory in _errorsByCategory.Values)
            {
                allErrors.AddRange(errorsOfCategory);
            }
            return allErrors.GetEnumerator();
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Add(string message)
    {
        AddInternal("", message);
    }

    public void Add(string category, string message)
    {
        if (string.IsNullOrEmpty(category))
            throw new ArgumentNullException(nameof(category));

        AddInternal(category, message);
    }

    private void AddInternal(string category, string message)
    {
        lock (_lock)
        {
            if (!_errorsByCategory.TryGetValue(category, out var errors))
            {
                errors = new List<string>();
                _errorsByCategory.Add(category, errors);
            }

            errors.Add(message);
            NotifyErrorsChanged();
        }
    }

    public int RemoveAll(string category)
    {
        lock (_lock)
        {
            if (!_errorsByCategory.TryGetValue(category, out var errors))
                return 0;

            var nbErrors = errors.Count;
            errors.Clear();
            NotifyErrorsChanged();
            return nbErrors;
        }
    }

    protected virtual void NotifyErrorsChanged()
    {
        ErrorsChanged?.Invoke(this, new ErrorsChangedHandlerArgs());
    }
}
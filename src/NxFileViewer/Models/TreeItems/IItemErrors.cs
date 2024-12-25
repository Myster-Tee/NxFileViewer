using System.Collections.Generic;

namespace Emignatik.NxFileViewer.Models.TreeItems;

public interface IItemErrors : IEnumerable<ItemError>
{
    /// <summary>
    /// Triggered when this collection of errors changes.
    /// </summary>
    event ErrorsChangedHandler? ErrorsChanged;

    /// <summary>
    /// Get the number of errors in the collection.
    /// </summary>
    public int Count { get; }

    /// <summary>
    /// Add an error to the collection.
    /// </summary>
    /// <param name="error"></param>
    /// <returns>True is error added, false if already present</returns>
    bool Add(ItemError error);

    /// <summary>
    /// Remove all errors of a specific category.
    /// </summary>
    /// <param name="category"></param>
    /// <returns>The number of removed errors</returns>
    int RemoveAllOfCategory(Category category);
}


public delegate void ErrorsChangedHandler(object sender, ErrorsChangedHandlerArgs args);

public class ErrorsChangedHandlerArgs
{
    public ItemError[] RemovedErrors { get; }
    public ItemError[] AddedErrors { get; }

    public ErrorsChangedHandlerArgs(ItemError[] removedErrors, ItemError[] addedErrors)
    {
        RemovedErrors = removedErrors;
        AddedErrors = addedErrors;
    }
}


public class ItemError
{
    public string Message { get; init; } = string.Empty;

    public Category Category { get; init; }

    public override int GetHashCode()
    {
        return Message.GetHashCode() + Category.GetHashCode();
    }
}

public enum Category
{
    Loading,
    IntegrityCheck
}



public static class ItemErrorsExtension
{
    public static void Add(this IItemErrors itemErrors, Category category, string message)
    {
        itemErrors.Add(new ItemError { Category = category, Message = message });
    }
}
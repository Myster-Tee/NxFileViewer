using Emignatik.NxFileViewer.Model.TreeItems;

namespace Emignatik.NxFileViewer.Services;

public class SelectedItemService : ISelectedItemService
{
    private IItem? _selectedItem;

    public IItem? SelectedItem
    {
        get => _selectedItem;
        set
        {
            var oldItem = _selectedItem;
            _selectedItem = value;
            NotifySelectedItemChanged(oldItem, value);
        }
    }

    public event SelectedItemChangedHandler? SelectedItemChanged;

    protected virtual void NotifySelectedItemChanged(IItem? oldItem, IItem? newItem)
    {
        SelectedItemChanged?.Invoke(this, new SelectedItemChangedHandlerArgs(oldItem, newItem));
    }
}


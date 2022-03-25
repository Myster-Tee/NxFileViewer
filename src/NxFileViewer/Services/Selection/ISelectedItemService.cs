using Emignatik.NxFileViewer.Models.TreeItems;

namespace Emignatik.NxFileViewer.Services.Selection;

public interface ISelectedItemService
{
    IItem? SelectedItem { get; set; }

    event SelectedItemChangedHandler SelectedItemChanged;
}

public delegate void SelectedItemChangedHandler(object sender, SelectedItemChangedHandlerArgs args);

public class SelectedItemChangedHandlerArgs 
{

    public SelectedItemChangedHandlerArgs(IItem? oldItem, IItem? newItem)
    {
        OldItem = oldItem;
        NewItem = newItem;
    }

    public IItem? OldItem { get; }
    public IItem? NewItem { get; }

}
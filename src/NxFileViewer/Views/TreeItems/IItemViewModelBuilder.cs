using Emignatik.NxFileViewer.Model.TreeItems;

namespace Emignatik.NxFileViewer.Views.TreeItems
{
    public interface IItemViewModelBuilder
    {
        IItemViewModel Build(IItem item);
    }
}
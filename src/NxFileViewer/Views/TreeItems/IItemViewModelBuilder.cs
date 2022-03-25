using Emignatik.NxFileViewer.Models.TreeItems;

namespace Emignatik.NxFileViewer.Views.TreeItems;

public interface IItemViewModelBuilder
{
    IItemViewModel Build(IItem item);
}
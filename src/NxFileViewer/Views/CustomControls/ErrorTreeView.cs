using System.Windows;
using Emignatik.NxFileViewer.Utils.MVVM.CustomControls;

namespace Emignatik.NxFileViewer.Views.CustomControls;

public class ErrorTreeView : TreeViewEx
{
    protected override DependencyObject GetContainerForItemOverride()
    {
        return new ErrorTreeViewItem();
    }

    protected override bool IsItemItsOwnContainerOverride(object item)
    {
        return item is ErrorTreeViewItem;
    }
}
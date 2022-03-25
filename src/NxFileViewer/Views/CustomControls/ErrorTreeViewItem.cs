using System.Windows;
using System.Windows.Controls;

namespace Emignatik.NxFileViewer.Views.CustomControls;

public class ErrorTreeViewItem : TreeViewItem
{

    public static readonly DependencyProperty HasErrorInDescendantsProperty = DependencyProperty.Register(
        "HasErrorInDescendants", typeof(bool), typeof(ErrorTreeViewItem), new PropertyMetadata(default(bool)));

    public bool HasErrorInDescendants
    {
        get => (bool)GetValue(HasErrorInDescendantsProperty);
        set => SetValue(HasErrorInDescendantsProperty, value);
    }

    protected override DependencyObject GetContainerForItemOverride()
    {
        return new ErrorTreeViewItem();
    }

    protected override bool IsItemItsOwnContainerOverride(object item)
    {
        return item is ErrorTreeViewItem;
    }

}
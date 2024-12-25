using System.Windows;
using System.Windows.Controls;

namespace Emignatik.NxFileViewer.Views.CustomControls;

/// <summary>
/// Custom tree view item which adds a property to indicate if there are errors in the descendants
/// </summary>
public class CustomTreeViewItem : TreeViewItem
{

    public static readonly DependencyProperty HasErrorInDescendantsProperty =
        DependencyProperty.Register(nameof(HasErrorInDescendants), typeof(bool), typeof(CustomTreeViewItem), new PropertyMetadata(default(bool)));

    public bool HasErrorInDescendants
    {
        get => (bool)GetValue(HasErrorInDescendantsProperty);
        set => SetValue(HasErrorInDescendantsProperty, value);
    }

    protected override DependencyObject GetContainerForItemOverride()
    {
        return new CustomTreeViewItem();
    }

    protected override bool IsItemItsOwnContainerOverride(object item)
    {
        return item is CustomTreeViewItem;
    }

}
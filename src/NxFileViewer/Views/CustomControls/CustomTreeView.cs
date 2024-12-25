using System.Windows;
using System.Windows.Controls;

namespace Emignatik.NxFileViewer.Views.CustomControls;


/// <summary>
/// Custom tree view with custom items of type <see cref="CustomTreeViewItem"/>
/// </summary>
public class CustomTreeView : TreeView
{
    protected override DependencyObject GetContainerForItemOverride()
    {
        return new CustomTreeViewItem();
    }

    protected override bool IsItemItsOwnContainerOverride(object item)
    {
        return item is CustomTreeViewItem;
    }


    #region Allow the selected item to be bound

    public static readonly DependencyProperty BindableSelectedItemProperty = 
        DependencyProperty.Register(nameof(BindableSelectedItem), typeof(object), typeof(CustomTreeView), new UIPropertyMetadata(default(object)));

    public object BindableSelectedItem
    {
        get => GetValue(BindableSelectedItemProperty);
        set => SetValue(BindableSelectedItemProperty, value);
    }

    protected override void OnSelectedItemChanged(RoutedPropertyChangedEventArgs<object> e)
    {
        base.OnSelectedItemChanged(e);
        BindableSelectedItem = SelectedItem;
    }

    #endregion
}
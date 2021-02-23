using System.Windows;
using System.Windows.Controls;

namespace Emignatik.NxFileViewer.Utils.MVVM.CustomControls
{
    public class TreeViewEx : TreeView
    {

        #region Allow the selected item to be bound

        public static readonly DependencyProperty BindableSelectedItemProperty = DependencyProperty.Register(
            "BindableSelectedItem", typeof(object), typeof(TreeViewEx), new UIPropertyMetadata(default(object)));

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
}

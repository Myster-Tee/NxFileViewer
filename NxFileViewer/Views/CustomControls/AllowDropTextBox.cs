using System.Windows;
using System.Windows.Controls;

namespace Emignatik.NxFileViewer.Views.CustomControls
{

    /// <summary>
    /// A TextBox which doesn't block the drop of parent components
    /// </summary>
    public class AllowDropTextBox : TextBox
    {
        public AllowDropTextBox()
        {
            this.AllowDrop = true;
        }

        protected override void OnDragEnter(DragEventArgs e)
        {
            e.Effects = DragDropEffects.Move;
            e.Handled = true;
        }

        protected override void OnPreviewDragEnter(DragEventArgs e)
        {
            e.Effects = DragDropEffects.Move;
            e.Handled = true;
        }

        protected override void OnDragOver(DragEventArgs e)
        {
            e.Effects = DragDropEffects.Move;
            e.Handled = true;
        }
    }
}

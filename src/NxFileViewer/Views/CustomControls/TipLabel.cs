using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Utils.MVVM.Commands;

namespace Emignatik.NxFileViewer.Views.CustomControls;

public class TipLabel : Label
{

    public TipLabel()
    {

        var contextMenu = new ContextMenu();
        var copyTextMenuItem = new MenuItem
        {
            Command = new RelayCommand(CopyToolTipText)
        };
        contextMenu.Items.Add(copyTextMenuItem);

        const string PROPERTY_PATH = $"{nameof(LocalizationManager.Instance.Current)}.{nameof(LocalizationManager.Instance.Current.Keys)}.{nameof(LocalizationManager.Instance.Current.Keys.MenuItem_CopyTextToClipboard)}";

        var binding = new Binding
        {
            Source = LocalizationManager.Instance,
            Path = new PropertyPath(PROPERTY_PATH)
        };


        BindingOperations.SetBinding(copyTextMenuItem, HeaderedItemsControl.HeaderProperty, binding);

        this.ContextMenu = contextMenu;
    }

    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
        base.OnMouseDown(e);
        e.Handled = true;
    }

    private void CopyToolTipText()
    {
        if (this.ToolTip is not string toolTipText) 
            return;
        try
        {
            Clipboard.SetText(toolTipText);
        }
        catch
        {
            // ignored
        }
    }
}
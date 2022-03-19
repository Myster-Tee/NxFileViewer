using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
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

        var binding = new Binding
        {
            Source = LocalizationManager.Instance,
            Path = new PropertyPath("Current.Keys.MenuItem_CopyTextToClipboard")
        };

        BindingOperations.SetBinding(copyTextMenuItem, HeaderedItemsControl.HeaderProperty, binding);

        this.ContextMenu = contextMenu;
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
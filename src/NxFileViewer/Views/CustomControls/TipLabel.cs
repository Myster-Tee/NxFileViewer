using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Utils.MVVM;
using Emignatik.NxFileViewer.Utils.MVVM.Commands;
using Emignatik.NxFileViewer.Views.Windows;

namespace Emignatik.NxFileViewer.Views.CustomControls;

public class TipLabel : Label
{
    private TipWindow? _tipWindow;
    private Window? _attachedWindow;

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

        var tipText = TipText;
        if (tipText == null)
            return;

        if (_tipWindow == null)
        {
            _tipWindow = new TipWindow
            {
                Owner = GetAttachedWindow(),
            };
            _tipWindow.Closed += OnTipWindowClosed;
        }

        var pointToScreen = this.PointToScreen(new Point());

        _tipWindow.Top = pointToScreen.Y;
        _tipWindow.Left = pointToScreen.X;

        _tipWindow.TextBox.Text = tipText;

        _tipWindow.Show();
        _tipWindow.Activate();
    }

    private Window? GetAttachedWindow()
    {
        return _attachedWindow ??= this.FindAttachedWindow();
    }


    private void OnTipWindowClosed(object? sender, EventArgs e)
    {
        if (_tipWindow != null)
            _tipWindow.Closed -= OnTipWindowClosed;
        _tipWindow = null;
    }

    private void CopyToolTipText()
    {
        var tipText = TipText;
        if (tipText == null)
            return;
        try
        {
            Clipboard.SetText(tipText);
        }
        catch
        {
            // ignored
        }
    }

    public string? TipText => this.ToolTip as string;
}
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Emignatik.NxFileViewer.Utils.MVVM;
using Emignatik.NxFileViewer.Views.Windows;

namespace Emignatik.NxFileViewer.Views.CustomControls;

public class TipLabel : Label
{
    private TipWindow? _tipWindow;
    private Window? _attachedWindow;

    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
        base.OnMouseDown(e);
        if(e.LeftButton != MouseButtonState.Pressed)
            return;

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

    public string? TipText => this.ToolTip as string;
}
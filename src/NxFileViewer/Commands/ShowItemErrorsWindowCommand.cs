using System;
using System.Windows;
using System.Windows.Input;
using Emignatik.NxFileViewer.Models.TreeItems;
using Emignatik.NxFileViewer.Services;
using Emignatik.NxFileViewer.Utils.MVVM.Commands;
using Emignatik.NxFileViewer.Views.Windows;

namespace Emignatik.NxFileViewer.Commands;

public class ShowItemErrorsWindowCommand : CommandBase, IShowItemErrorsWindowCommand
{
    private IItem? _item;
    private static ItemErrorsWindow? _itemErrorsWindow;

    public ShowItemErrorsWindowCommand(ISelectedItemService selectedItemService)
    {
        selectedItemService = selectedItemService ?? throw new ArgumentNullException(nameof(selectedItemService));
        selectedItemService.SelectedItemChanged += OnSelectedItemChanged;
        Update(selectedItemService.SelectedItem);
    }

    private void OnSelectedItemChanged(object sender, SelectedItemChangedHandlerArgs args)
    {
        Update(args.NewItem);
    }

    private void Update(IItem? item)
    {
        if (_item != null)
            _item.Errors.ErrorsChanged -= OnErrorsChanged;
        _item = item;
        if (_item != null)
            _item.Errors.ErrorsChanged += OnErrorsChanged;
        TriggerCanExecuteChanged();
    }

    public override void Execute(object? parameter)
    {
        if(_item == null)
            return;

        var itemErrorsWindow = GetWindow();
        var itemErrorsWindowViewModel = new ItemErrorsWindowViewModel(_item)
        {
            Window = itemErrorsWindow
        };

        itemErrorsWindow.DataContext = itemErrorsWindowViewModel;
        itemErrorsWindowViewModel.Window = itemErrorsWindow;

        itemErrorsWindow.Show();
    }

    public override bool CanExecute(object? parameter)
    {
        return _item != null && _item.Errors.Count > 0;
    }

    private static ItemErrorsWindow GetWindow()
    {
        if (_itemErrorsWindow != null) 
            return _itemErrorsWindow;

        _itemErrorsWindow = new ItemErrorsWindow
        {
            ShowActivated = true,
            Owner = Application.Current.MainWindow,
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        };

        _itemErrorsWindow.Closed += OnWindowClosed;

        return _itemErrorsWindow;
    }

    private static void OnWindowClosed(object? sender, EventArgs e)
    {
        if (_itemErrorsWindow != null)
            _itemErrorsWindow.Closed -= OnWindowClosed;
        _itemErrorsWindow = null;
    }

    private void OnErrorsChanged(object sender, ErrorsChangedHandlerArgs args)
    {
        TriggerCanExecuteChanged();
    }

}

public interface IShowItemErrorsWindowCommand : ICommand
{
}
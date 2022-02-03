using System;
using System.Windows;
using System.Windows.Input;
using Emignatik.NxFileViewer.Utils.MVVM.Commands;
using Emignatik.NxFileViewer.Views.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace Emignatik.NxFileViewer.Commands;

public class ShowBulkRenameWindowCommand : CommandBase, IShowBulkRenameWindowCommand
{
    private readonly IServiceProvider _serviceProvider;
    private BulkRenameWindow? _bulkRenameWindow;

    public ShowBulkRenameWindowCommand(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    public override void Execute(object? parameter)
    {
        if (_bulkRenameWindow != null)
        {
            _bulkRenameWindow.Activate();
            return;
        }

        var bulkRenameViewModel = _serviceProvider.GetRequiredService<BulkRenameWindowViewModel>();

        _bulkRenameWindow = new BulkRenameWindow
        {
            Owner = Application.Current.MainWindow,
        };
        _bulkRenameWindow.Closed += OnBulkRenameWindowClosed;
        _bulkRenameWindow.DataContext = bulkRenameViewModel;
        bulkRenameViewModel.Window = _bulkRenameWindow;

        _bulkRenameWindow.Show();
    }

    private void OnBulkRenameWindowClosed(object? sender, EventArgs e)
    {
        _bulkRenameWindow = null;
        TriggerCanExecuteChanged();
    }
}

public interface IShowBulkRenameWindowCommand : ICommand
{
}
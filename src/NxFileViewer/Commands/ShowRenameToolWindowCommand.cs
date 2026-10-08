using System.Windows;
using System.Windows.Input;
using Emignatik.NxFileViewer.Utils.MVVM.Commands;
using Emignatik.NxFileViewer.Views.Windows;

namespace Emignatik.NxFileViewer.Commands;

public sealed class ShowRenameToolWindowCommand : CommandBase, IShowRenameToolWindowCommand
{
    public override void Execute(object? parameter) =>
        (Application.Current.MainWindow as MainWindow)?.Navigate(WorkspaceSection.Rename);
}

public interface IShowRenameToolWindowCommand : ICommand { }

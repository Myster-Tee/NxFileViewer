using System.Windows;
using System.Windows.Input;
using Emignatik.NxFileViewer.Utils.MVVM.Commands;

namespace Emignatik.NxFileViewer.Commands
{
    public class ExitAppCommand : CommandBase, IExitAppCommand
    {
        public override void Execute(object? parameter)
        {
            Application.Current.Shutdown();
        }
    }

    public interface IExitAppCommand : ICommand
    {
    }
}
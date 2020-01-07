using System.Windows;

namespace Emignatik.NxFileViewer.Commands
{
    public class ExitAppCommand : CommandBase
    {
        public override void Execute(object parameter)
        {
            Application.Current.Shutdown();
        }
    }
}
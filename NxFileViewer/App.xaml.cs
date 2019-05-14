using System.Windows;
using Emignatik.NxFileViewer.Views;

namespace Emignatik.NxFileViewer
{

    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var mainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel()
            };
            Application.Current.MainWindow = mainWindow;
            mainWindow.Show();

            var args = e.Args;
            if (args.Length == 1)
            {
                mainWindow.SafeLoadFile(args[0]);
            }


        }
    }
}

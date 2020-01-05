using System.Windows;
using Emignatik.NxFileViewer.Services;
using Emignatik.NxFileViewer.Views;

namespace Emignatik.NxFileViewer
{

    public partial class App : Application
    {
        private OpenedFileService _openedFileService;
        private SupportedFilesOpenerService _supportedFilesOpenerService;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _openedFileService = new OpenedFileService();
            _supportedFilesOpenerService = new SupportedFilesOpenerService(_openedFileService);
            var mainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(_openedFileService, _supportedFilesOpenerService)
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

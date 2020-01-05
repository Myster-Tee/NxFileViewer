using System.Windows;
using Emignatik.NxFileViewer.Services;
using Emignatik.NxFileViewer.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer
{

    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var serviceProvider = new ServiceCollection()
                .AddSingleton<ISupportedFilesOpenerService, SupportedFilesOpenerService>()
                .AddSingleton<IOpenedFileService, OpenedFileService>()
                .AddSingleton<ILoggerFactory, LoggerFactory>()
                .AddSingleton<MainWindowViewModel>()
                .BuildServiceProvider();


            var mainWindowViewModel = serviceProvider.GetService<MainWindowViewModel>();

            var mainWindow = new MainWindow
            {
                DataContext = mainWindowViewModel
            };

            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            loggerFactory.AddProvider(mainWindow);


            Application.Current.MainWindow = mainWindow;
            mainWindow.Show();

            var args = e.Args;
            if (args.Length == 1)
            {
                serviceProvider.GetService<ISupportedFilesOpenerService>().OpenFile(args[0]);
            }
        }
    }
}

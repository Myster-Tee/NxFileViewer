using System;
using System.Windows;
using Emignatik.NxFileViewer.Commands;
using Emignatik.NxFileViewer.Services;
using Emignatik.NxFileViewer.Settings;
using Emignatik.NxFileViewer.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer
{

    public partial class App : Application
    {
        private ServiceProvider _serviceProvider;
        private ILogger _logger;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _serviceProvider = new ServiceCollection()
                .AddSingleton<ISupportedFilesOpenerService, SupportedFilesOpenerService>()
                .AddSingleton<IOpenedFileService, OpenedFileService>()
                .AddSingleton<IKeySetProviderService, KeySetProviderService>()
                .AddSingleton<ILoggerFactory, LoggerFactory>()
                .AddSingleton<IAppSettings, AppSettings>()
                .AddSingleton<OpenFileCommand>()
                .AddSingleton<OpenLastFileCommand>()
                .AddSingleton<CloseFileCommand>()
                .AddSingleton<MainWindowViewModel>()
                .BuildServiceProvider();

            _logger = _serviceProvider.GetService<ILoggerFactory>().CreateLogger(this.GetType());

            var mainWindow = new MainWindow
            {
                DataContext = _serviceProvider.GetService<MainWindowViewModel>()
            };
            mainWindow.Loaded += OnMainWindowLoaded;

            var loggerFactory = _serviceProvider.GetService<ILoggerFactory>();
            loggerFactory.AddProvider(mainWindow);

            Application.Current.MainWindow = mainWindow;
            mainWindow.Show();

            var args = e.Args;
            if (args.Length == 1)
            {
                _serviceProvider.GetService<ISupportedFilesOpenerService>().OpenFile(args[0]);
            }
        }

        private void OnMainWindowLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _serviceProvider.GetService<IAppSettings>().Load();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to load settings: {ex.Message}");
            }

            //TODO: charger les clés
            try
            {
                //Tries to load keys at startup (not to wait for a game to be loaded)
                _serviceProvider.GetService<IKeySetProviderService>().GetKeySet();
                _logger.LogInformation(NxFileViewer.Properties.Resources.InfoKeysSuccessfullyLoaded);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
    }
}

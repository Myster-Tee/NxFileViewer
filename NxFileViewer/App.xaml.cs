using System;
using System.Windows;
using Emignatik.NxFileViewer.Commands;
using Emignatik.NxFileViewer.FileLoading;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Logging;
using Emignatik.NxFileViewer.Services;
using Emignatik.NxFileViewer.Settings;
using Emignatik.NxFileViewer.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IServiceProvider _serviceProvider;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _serviceProvider = new ServiceCollection()
                .AddSingleton<IKeySetProviderService, KeySetProviderService>()
                .AddSingleton<ILoggerFactory, LoggerFactory>()
                .AddSingleton<IAppSettings, AppSettings>()
                .AddSingleton<IOpenFileCommand, OpenFileCommand>()
                .AddSingleton<IExitAppCommand, ExitAppCommand>()
                .AddSingleton<IOpenLastFileCommand, OpenLastFileCommand>()
                .AddSingleton<ICloseFileCommand, CloseFileCommand>()
                .AddSingleton<IShowSettingsViewCommand, ShowSettingsViewCommand>()
                .AddSingleton<IFileOpenerService, FileOpenerService>()
                .AddSingleton<IOpenedFileService, OpenedFileService>()
                .AddSingleton<IFileTypeAnalyzer, FileTypeAnalyzer>()
                .AddSingleton<SettingsWindowViewModel>()
                .AddSingleton<MainWindowViewModel>()
                .AddSingleton<IAppSettingsManager, AppSettingsManager>()
                .AddSingleton<IFileItemLoader, FileItemLoader>()
                .AddSingleton<IFileOverviewLoader, FileOverviewLoader>()
                .AddSingleton<IChildItemsBuilder, ChildItemsBuilder>()

                .AddTransient<ISaveTitleImageCommand, SaveTitleImageCommand>()
                .AddTransient<ICopyTitleImageCommand, CopyTitleImageCommand>()
                .AddTransient<ISaveItemToFileCommand, SaveItemToFileCommand>()
                .AddTransient<IFileDownloaderService, FileDownloaderService>()

                .AddLogging(builder => builder.AddAppLoggerProvider())

                .BuildServiceProvider();

            _serviceProvider.GetRequiredService<IAppSettingsManager>().Load();

            var mainWindow = new MainWindow
            {
                DataContext = _serviceProvider.GetRequiredService<MainWindowViewModel>()
            };
            mainWindow.Loaded += OnMainWindowLoaded;
            Application.Current.MainWindow = mainWindow;
            mainWindow.Show();

        }

        private async void OnMainWindowLoaded(object sender, RoutedEventArgs e)
        {
            var logger = _serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger(GetType());
            var keySetProviderService = _serviceProvider.GetRequiredService<IKeySetProviderService>();
            var appSettings = _serviceProvider.GetRequiredService<IAppSettings>();
            var fileDownloaderService = _serviceProvider.GetRequiredService<IFileDownloaderService>();
            var prodKeysDownloadUrl = appSettings.ProdKeysDownloadUrl;
            try
            {
                if (!keySetProviderService.ProdKeysFileFound && !string.IsNullOrWhiteSpace(prodKeysDownloadUrl))
                {
                    logger.LogInformation(string.Format(LocalizationManager.Instance.Current.Keys.DownloadingProdKeysFromUrl, prodKeysDownloadUrl));
                    await fileDownloaderService.DownloadFileAsync(prodKeysDownloadUrl, keySetProviderService.AppDirProdKeysFilePath);
                    logger.LogInformation(LocalizationManager.Instance.Current.Keys.ProdKeysSuccessfullyDownloaded);
                    keySetProviderService.UnloadCurrentKeySet(); // To force reloading with the downloaded keys file
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, string.Format(LocalizationManager.Instance.Current.Keys.FailedToDownloadProdKeysFromUrl, prodKeysDownloadUrl, ex.Message));
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            _serviceProvider.GetRequiredService<IAppSettingsManager>().Save();
        }
    }

}

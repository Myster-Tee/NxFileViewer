using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows;
using Emignatik.NxFileViewer.Commands;
using Emignatik.NxFileViewer.FileLoading;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Logging;
using Emignatik.NxFileViewer.Services;
using Emignatik.NxFileViewer.Services.BackgroundTask;
using Emignatik.NxFileViewer.Services.BackgroundTask.RunnableImpl;
using Emignatik.NxFileViewer.Settings;
using Emignatik.NxFileViewer.Settings.Model;
using Emignatik.NxFileViewer.Styling.Theme;
using Emignatik.NxFileViewer.Tools;
using Emignatik.NxFileViewer.Views;
using Emignatik.NxFileViewer.Views.TreeItems;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly ILogger _logger;

        static App()
        {
            ServiceProvider = new ServiceCollection()
              .AddSingleton<ILoggerFactory, LoggerFactory>()

              .AddSingleton<IKeySetProviderService, KeySetProviderService>()
              .AddSingleton<IFileOpenerService, FileOpenerService>()
              .AddSingleton<IOpenedFileService, OpenedFileService>()
              .AddSingleton<ISelectedItemService, SelectedItemService>()
              .AddSingleton<BackgroundTaskService>()
              .AddSingleton<IBackgroundTaskService>(provider => provider.GetRequiredService<BackgroundTaskService>())
              .AddSingleton<IProgressReporter>(provider => provider.GetRequiredService<BackgroundTaskService>())
              .AddSingleton<IFileDownloaderService, FileDownloaderService>()

              .AddSingleton<IOpenFileCommand, OpenFileCommand>()
              .AddSingleton<IExitAppCommand, ExitAppCommand>()
              .AddSingleton<IOpenLastFileCommand, OpenLastFileCommand>()
              .AddSingleton<ICloseFileCommand, CloseFileCommand>()
              .AddSingleton<IShowSettingsViewCommand, ShowSettingsViewCommand>()
              .AddSingleton<ISaveItemToFileCommand, SaveItemToFileCommand>()
              .AddSingleton<IVerifyNcasHashCommand, VerifyNcasHashCommand>()
              .AddSingleton<IVerifyNcasHeaderSignatureCommand, VerifyNcasHeaderSignatureCommand>()
              .AddSingleton<IShowItemErrorsWindowCommand, ShowItemErrorsWindowCommand>()
              .AddSingleton<ISaveTitleImageCommand, SaveTitleImageCommand>()
              .AddSingleton<ICopyImageCommand, CopyImageCommand>()

              .AddSingleton<IFileTypeAnalyzer, FileTypeAnalyzer>()
              .AddSingleton<MainWindowViewModel>()
              .AddSingleton<IAppSettingsWrapper<AppSettingsModel>, AppSettingsWrapper>()
              .AddSingleton<IAppSettings>(provider => provider.GetRequiredService<IAppSettingsWrapper<AppSettingsModel>>())
              .AddSingleton<IAppSettingsManager, AppSettingsManager>()
              .AddSingleton<IFileItemLoader, FileItemLoader>()
              .AddSingleton<IFileOverviewLoader, FileOverviewLoader>()
              .AddSingleton<IChildItemsBuilder, ChildItemsBuilder>()
              .AddSingleton<IItemViewModelBuilder, ItemViewModelBuilder>()
              .AddSingleton<IFileLoader, FileLoader>()
              .AddSingleton<IBrushesProvider, BrushesProvider>()

              .AddTransient<SettingsWindowViewModel>() // Important to let transient so that real actual settings are displayed when settings view is shown
              .AddTransient<ILibHacFileSaver, LibHacFileSaver>()
              .AddTransient<ISaveFileRunnable, SaveFileRunnable>()
              .AddTransient<ISaveDirectoryRunnable, SaveDirectoryRunnable>()
              .AddTransient<IVerifyNcasHashRunnable, VerifyNcasHashRunnable>()
              .AddTransient<IVerifyNcasHeaderSignatureRunnable, VerifyNcasHeaderSignatureRunnable>()
              .AddTransient<IFsSanitizer, FsSanitizer>()

              .AddLogging(builder => builder.AddAppLoggerProvider())

              .BuildServiceProvider();

            _logger = ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger(MethodBase.GetCurrentMethod()!.DeclaringType);
        }

        public static IServiceProvider ServiceProvider { get; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Loads the application settings
            ServiceProvider.GetRequiredService<IAppSettingsManager>().Load();

            LocalizationStringExtension.FormatException += OnLocalizationStringFormatException;

            var mainWindow = new MainWindow
            {
                DataContext = ServiceProvider.GetRequiredService<MainWindowViewModel>()
            };
            mainWindow.Loaded += OnMainWindowLoaded;

            Application.Current.MainWindow = mainWindow;
            mainWindow.Show();

            var fileOpenerService = ServiceProvider.GetRequiredService<IFileOpenerService>();
            if (e.Args.Length > 0)
                fileOpenerService.SafeOpenFile(e.Args[0]);
        }

        private void OnLocalizationStringFormatException(object? sender, FormatExceptionHandlerArgs args)
        {
            var argsFormatted = string.Join(", ", args.FormatArgs.Select(o => $"«{o ?? "NULL"}»"));
            var message = $"Localization key value «{args.KeyValue}» appears to be invalid, at least one of the following values {argsFormatted} couldn't be replaced.";
            Debug.Fail(message);
            _logger.LogError(message);
        }

        private async void OnMainWindowLoaded(object sender, RoutedEventArgs e)
        {
            var keySetProviderService = ServiceProvider.GetRequiredService<IKeySetProviderService>();
            var appSettings = ServiceProvider.GetRequiredService<IAppSettings>();
            var fileDownloaderService = ServiceProvider.GetRequiredService<IFileDownloaderService>();

            var prodKeysDownloadUrl = appSettings.ProdKeysDownloadUrl;
            try
            {
                if (keySetProviderService.ActualProdKeysFilePath == null && !string.IsNullOrWhiteSpace(prodKeysDownloadUrl))
                {
                    _logger.LogInformation(LocalizationManager.Instance.Current.Keys.Log_DownloadingKeysFromUrl.SafeFormat(prodKeysDownloadUrl));
                    await fileDownloaderService.DownloadFileAsync(prodKeysDownloadUrl, keySetProviderService.AppDirProdKeysFilePath);
                    _logger.LogInformation(LocalizationManager.Instance.Current.Keys.Log_KeysSuccessfullyDownloaded);
                    keySetProviderService.UnloadCurrentKeySet(); // To force reloading with the downloaded keys file
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, LocalizationManager.Instance.Current.Keys.Log_FailedToDownloadKeysFromUrl.SafeFormat(IKeySetProviderService.DefaultProdKeysFileName, prodKeysDownloadUrl, ex.Message));
            }

            var titleKeysDownloadUrl = appSettings.TitleKeysDownloadUrl;
            try
            {
                if (keySetProviderService.ActualTitleKeysFilePath == null && !string.IsNullOrWhiteSpace(titleKeysDownloadUrl))
                {
                    _logger.LogInformation(LocalizationManager.Instance.Current.Keys.Log_DownloadingKeysFromUrl.SafeFormat(titleKeysDownloadUrl));
                    await fileDownloaderService.DownloadFileAsync(titleKeysDownloadUrl, keySetProviderService.AppDirTitleKeysFilePath);
                    _logger.LogInformation(LocalizationManager.Instance.Current.Keys.Log_KeysSuccessfullyDownloaded);
                    keySetProviderService.UnloadCurrentKeySet(); // To force reloading with the downloaded keys file
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, LocalizationManager.Instance.Current.Keys.Log_FailedToDownloadKeysFromUrl.SafeFormat(IKeySetProviderService.DefaultTitleKeysFileName, titleKeysDownloadUrl, ex.Message));
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            ServiceProvider.GetRequiredService<IAppSettingsManager>().Save();
        }
    }

}

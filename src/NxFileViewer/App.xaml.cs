using System;
using System.Collections.Generic;
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
              .AddSingleton<IPromptService, PromptService>()
              .AddSingleton<BackgroundTaskService>()
              .AddSingleton<IBackgroundTaskService>(provider => provider.GetRequiredService<BackgroundTaskService>())
              .AddSingleton<IProgressReporter>(provider => provider.GetRequiredService<BackgroundTaskService>())

              .AddSingleton<IOpenFileCommand, OpenFileCommand>()
              .AddSingleton<IExitAppCommand, ExitAppCommand>()
              .AddSingleton<IOpenLastFileCommand, OpenLastFileCommand>()
              .AddSingleton<ICloseFileCommand, CloseFileCommand>()
              .AddSingleton<IShowSettingsViewCommand, ShowSettingsViewCommand>()
              .AddSingleton<IVerifyNcasHashCommand, VerifyNcasHashCommand>()
              .AddSingleton<IVerifyNcasHeaderSignatureCommand, VerifyNcasHeaderSignatureCommand>()
              .AddSingleton<IShowItemErrorsWindowCommand, ShowItemErrorsWindowCommand>()
              .AddSingleton<ISaveTitleImageCommand, SaveTitleImageCommand>()
              .AddSingleton<ICopyImageCommand, CopyImageCommand>()
              .AddSingleton<ILoadKeysCommand, LoadKeysCommand>()
              .AddSingleton<IOpenTitleWebPageCommand, OpenTitleWebPageCommand>()

              .AddSingleton<IFileTypeAnalyzer, FileTypeAnalyzer>()
              .AddSingleton<MainWindowViewModel>()
              .AddSingleton<IAppSettingsWrapper<AppSettingsModel>, AppSettingsWrapper>()
              .AddSingleton<IAppSettings>(provider => provider.GetRequiredService<IAppSettingsWrapper<AppSettingsModel>>())
              .AddSingleton<IAppSettingsManager, AppSettingsManager>()
              .AddSingleton<IFileItemLoader, FileItemLoader>()
              .AddSingleton<IFileOverviewLoader, FileOverviewLoader>()
              .AddSingleton<IItemViewModelBuilder, ItemViewModelBuilder>()
              .AddSingleton<IFileLoader, FileLoader>()
              .AddSingleton<IBrushesProvider, BrushesProvider>()

              .AddTransient<SettingsWindowViewModel>() // Important to let transient so that real actual settings are displayed when settings view is shown
              .AddTransient<IStreamToFileHelper, StreamToFileHelper>()
              .AddTransient<ISaveFileRunnable, SaveFileRunnable>()
              .AddTransient<ISaveDirectoryRunnable, SaveDirectoryRunnable>()
              .AddTransient<IVerifyNcasHashRunnable, VerifyNcasHashRunnable>()
              .AddTransient<IVerifyNcasHeaderSignatureRunnable, VerifyNcasHeaderSignatureRunnable>()
              .AddTransient<IOpenFileLocationCommand, OpenFileLocationCommand>()
              .AddTransient<ISaveDirectoryEntryCommand, SaveDirectoryEntryCommand>()
              .AddTransient<ISavePartitionFileCommand, SavePartitionFileCommand>()
              .AddTransient<ISaveSectionContentCommand, SaveSectionContentCommand>()
              .AddTransient<ISavePlaintextNcaFileCommand, SavePlaintextNcaFileCommand>()
              .AddTransient<IDownloadFileRunnable, DownloadFileRunnable>()
              .AddTransient<ISaveStorageRunnable, SaveStorageRunnable>()
              .AddTransient<IFsSanitizer, FsSanitizer>()
              .AddTransient<IHttpDownloader, HttpDownloader>()

              .AddLogging(builder => builder.AddAppLoggerProvider())

              .BuildServiceProvider();

            _logger = ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger(MethodBase.GetCurrentMethod()!.DeclaringType!);
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

            void MainWindowLoaded(object sender, RoutedEventArgs args)
            {
                mainWindow.Loaded -= MainWindowLoaded;
                Initialize(e.Args);
            }
            mainWindow.Loaded += MainWindowLoaded;

            Application.Current.MainWindow = mainWindow;
            mainWindow.Show();
        }

        private static async void Initialize(IReadOnlyList<string> cmdLineArgs)
        {
            var keySetProviderService = ServiceProvider.GetRequiredService<IKeySetProviderService>();
            var appSettings = ServiceProvider.GetRequiredService<IAppSettings>();
            var backgroundTaskService = ServiceProvider.GetRequiredService<IBackgroundTaskService>();

            var prodKeysDownloadUrl = appSettings.ProdKeysDownloadUrl;

            if (keySetProviderService.ActualProdKeysFilePath == null && !string.IsNullOrWhiteSpace(prodKeysDownloadUrl))
            {
                var downloadFileRunnable = ServiceProvider.GetRequiredService<IDownloadFileRunnable>();
                downloadFileRunnable.Setup(prodKeysDownloadUrl, keySetProviderService.AppDirProdKeysFilePath);
                await backgroundTaskService.RunAsync(downloadFileRunnable);
                keySetProviderService.Reset(); // To force reloading with the downloaded keys file
            }

            var titleKeysDownloadUrl = appSettings.TitleKeysDownloadUrl;
            if (keySetProviderService.ActualTitleKeysFilePath == null && !string.IsNullOrWhiteSpace(titleKeysDownloadUrl))
            {
                var downloadFileRunnable = ServiceProvider.GetRequiredService<IDownloadFileRunnable>();
                downloadFileRunnable.Setup(titleKeysDownloadUrl, keySetProviderService.AppDirTitleKeysFilePath);
                await backgroundTaskService.RunAsync(downloadFileRunnable);
                keySetProviderService.Reset(); // To force reloading with the downloaded keys file
            }

            var fileOpenerService = ServiceProvider.GetRequiredService<IFileOpenerService>();
            if (cmdLineArgs.Count > 0)
                await fileOpenerService.SafeOpenFile(cmdLineArgs[0]);
        }

        private void OnLocalizationStringFormatException(object? sender, FormatExceptionHandlerArgs args)
        {
            var argsFormatted = string.Join(", ", args.FormatArgs.Select(o => $"«{o ?? "NULL"}»"));
            var message = $"Localization key value «{args.KeyValue}» appears to be invalid, at least one of the following values {argsFormatted} couldn't be replaced.";
            Debug.Fail(message);
            _logger.LogError(message);
        }


        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            ServiceProvider.GetRequiredService<IAppSettingsManager>().Save();
        }
    }

}

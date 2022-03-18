using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows;
using Emignatik.NxFileViewer.Commands;
using Emignatik.NxFileViewer.FileLoading;
using Emignatik.NxFileViewer.FileLoading.QuickFileInfoLoading;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Logging;
using Emignatik.NxFileViewer.Services.BackgroundTask;
using Emignatik.NxFileViewer.Services.BackgroundTask.RunnableImpl;
using Emignatik.NxFileViewer.Services.FileOpening;
using Emignatik.NxFileViewer.Services.FileRenaming;
using Emignatik.NxFileViewer.Services.GlobalEvents;
using Emignatik.NxFileViewer.Services.KeysManagement;
using Emignatik.NxFileViewer.Services.OnlineServices;
using Emignatik.NxFileViewer.Services.Prompting;
using Emignatik.NxFileViewer.Services.Selection;
using Emignatik.NxFileViewer.Settings;
using Emignatik.NxFileViewer.Styling.Theme;
using Emignatik.NxFileViewer.Tools;
using Emignatik.NxFileViewer.Views.TreeItems;
using Emignatik.NxFileViewer.Views.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application, IAppEvents
{
    private static readonly ILogger _logger;

    public event Action? AppShuttingDown;

    static App()
    {
        ServiceProvider = new ServiceCollection()
            .AddSingleton<IAppEvents>(_ => (App)Current)
            .AddSingleton<ILoggerFactory, LoggerFactory>()

            .AddSingleton<IKeySetProviderService, KeySetProviderService>()
            .AddSingleton<IFileOpenerService, FileOpenerService>()
            .AddSingleton<IOpenedFileService, OpenedFileService>()
            .AddSingleton<ISelectedItemService, SelectedItemService>()
            .AddSingleton<IPromptService, PromptService>()
            .AddSingleton<IPackageInfoLoader, PackageInfoLoader>()
            .AddSingleton<IFileRenamerService, FileRenamerService>()
            .AddSingleton<IOnlineTitleInfoService, OnlineTitleInfoService>()
            .AddSingleton<ICachedOnlineTitleInfoService, CachedOnlineTitleInfoService>()
            .AddSingleton<IOnlineTitlePageOpenerService, OnlineTitlePageOpenerService>()
            .AddSingleton<MainBackgroundTaskRunnerService>()
            .AddSingleton<IMainBackgroundTaskRunnerService>(provider => provider.GetRequiredService<MainBackgroundTaskRunnerService>())

            .AddSingleton<IOpenFileCommand, OpenFileCommand>()
            .AddSingleton<IExitAppCommand, ExitAppCommand>()
            .AddSingleton<IOpenLastFileCommand, OpenLastFileCommand>()
            .AddSingleton<ICloseFileCommand, CloseFileCommand>()
            .AddSingleton<IShowSettingsWindowCommand, ShowSettingsWindowCommand>()
            .AddSingleton<IVerifyNcasHashCommand, VerifyNcasHashCommand>()
            .AddSingleton<IVerifyNcasHeaderSignatureCommand, VerifyNcasHeaderSignatureCommand>()
            .AddSingleton<IShowItemErrorsWindowCommand, ShowItemErrorsWindowCommand>()
            .AddSingleton<ISaveTitleImageCommand, SaveTitleImageCommand>()
            .AddSingleton<ICopyImageCommand, CopyImageCommand>()
            .AddSingleton<ILoadKeysCommand, LoadKeysCommand>()
            .AddSingleton<IOpenTitleWebPageCommand, OpenTitleWebPageCommand>()
            .AddSingleton<IShowRenameToolWindowCommand, ShowRenameToolWindowCommand>()
            .AddSingleton<IRenameFilesCommand, RenameFilesCommand>()

            .AddSingleton<INamingPatternsParser, NamingPatternsParser>()
            .AddSingleton<IPackageTypeAnalyzer, PackageTypeAnalyzer>()
            .AddSingleton<MainWindowViewModel>()
            .AddSingleton<RenameToolWindowViewModel>()
            .AddSingleton<AppSettings>()
            .AddSingleton<IAppSettings>(provider => provider.GetRequiredService<AppSettings>())
            .AddSingleton<IAppSettingsManager, AppSettingsManager>()
            .AddSingleton<IFileItemLoader, FileItemLoader>()
            .AddSingleton<IFileOverviewLoader, FileOverviewLoader>()
            .AddSingleton<IItemViewModelBuilder, ItemViewModelBuilder>()
            .AddSingleton<IFileLoader, FileLoader>()
            .AddSingleton<IBrushesProvider, BrushesProvider>()
            .AddSingleton<ILocalizationFromSettingsSynchronizerService, LocalizationFromSettingsSynchronizerService>()
            .AddSingleton<IShallowCopier, ShallowCopier>()

            .AddTransient<SettingsWindowViewModel>() // Important to let transient so that real actual settings are displayed when settings view is shown
            .AddTransient<ISaveFileRunnable, SaveFileRunnable>()
            .AddTransient<ISaveDirectoryRunnable, SaveDirectoryRunnable>()
            .AddTransient<IVerifyNcasHashRunnable, VerifyNcasHashRunnable>()
            .AddTransient<IVerifyNcasHeaderSignatureRunnable, VerifyNcasHeaderSignatureRunnable>()
            .AddTransient<IDownloadFileRunnable, DownloadFileRunnable>()
            .AddTransient<ISaveStorageRunnable, SaveStorageRunnable>()
            .AddTransient<IFilesRenamerRunnable, FilesRenamerRunnable>()
            .AddTransient<IFileRenamerRunnable, FileRenamerRunnable>()

            .AddTransient<IStreamToFileHelper, StreamToFileHelper>()
            .AddTransient<IOpenFileLocationCommand, OpenFileLocationCommand>()
            .AddTransient<ISaveDirectoryEntryCommand, SaveDirectoryEntryCommand>()
            .AddTransient<ISavePartitionFileCommand, SavePartitionFileCommand>()
            .AddTransient<ISaveSectionContentCommand, SaveSectionContentCommand>()
            .AddTransient<ISavePlaintextNcaFileCommand, SavePlaintextNcaFileCommand>()
            .AddTransient<IFsSanitizer, FsSanitizer>()
            .AddTransient<IHttpDownloader, HttpDownloader>()
            .AddTransient<IBackgroundTaskRunner, BackgroundTaskRunner>()

            .AddLogging(builder => builder.AddAppLoggerProvider())

            .BuildServiceProvider();

        _logger = ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger(MethodBase.GetCurrentMethod()!.DeclaringType!);
    }

    public static IServiceProvider ServiceProvider { get; }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Loads the application settings
        ServiceProvider.GetRequiredService<IAppSettingsManager>().LoadSafe();

        // Initialize localization
        ServiceProvider.GetRequiredService<ILocalizationFromSettingsSynchronizerService>().Initialize();

        LocalizationStringExtension.FormatException += OnLocalizationStringFormatException;

        var mainWindowViewModel = ServiceProvider.GetRequiredService<MainWindowViewModel>();
        var mainWindow = new MainWindow
        {
            DataContext = mainWindowViewModel
        };
        mainWindowViewModel.Window = mainWindow;

        void MainWindowLoaded(object sender, RoutedEventArgs args)
        {
            mainWindow.Loaded -= MainWindowLoaded;
            Initialize(e.Args);
        }
        mainWindow.Loaded += MainWindowLoaded;

        Application.Current.MainWindow = mainWindow;
        mainWindow.Show();
    }

    private async void Initialize(IReadOnlyList<string> cmdLineArgs)
    {
        var keySetProviderService = ServiceProvider.GetRequiredService<IKeySetProviderService>();
        var appSettings = ServiceProvider.GetRequiredService<IAppSettings>();
        var backgroundTaskService = ServiceProvider.GetRequiredService<IMainBackgroundTaskRunnerService>();

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
        NotifyAppShuttingDown();
    }

    protected virtual void NotifyAppShuttingDown()
    {
        AppShuttingDown?.Invoke();
    }

}
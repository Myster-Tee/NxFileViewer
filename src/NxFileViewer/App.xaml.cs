using Emignatik.NxFileViewer.Services.Updates;
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
using Emignatik.NxFileViewer.Services.FileLocationOpening;
using Emignatik.NxFileViewer.Services.FileOpening;
using Emignatik.NxFileViewer.Services.FileRenaming;
using Emignatik.NxFileViewer.Services.GlobalEvents;
using Emignatik.NxFileViewer.Services.Integrity;
using Emignatik.NxFileViewer.Services.Nsz;
using Emignatik.NxFileViewer.Services.KeysManagement;
using Emignatik.NxFileViewer.Services.OnlineServices;
using Emignatik.NxFileViewer.Services.Prompting;
using Emignatik.NxFileViewer.Services.Selection;
using Emignatik.NxFileViewer.Services.WindowPlacement;
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
            .AddSingleton<IFileOpeningService, FileOpeningService>()
            .AddSingleton<NszPluginManager>()
            .AddSingleton<INszPlugin, NszCliPlugin>()
            .AddSingleton<IConversionVerifier, ConversionVerifier>()
            .AddSingleton<PackageConversionService>()
            .AddSingleton<UpdateCenterViewModel>()
            .AddSingleton<ComponentUpdateChecker>()
            .AddSingleton<ViewerUpdateService>()
            .AddSingleton<ViewerUpdateActions>()
            .AddSingleton<NszActions>()
            .AddSingleton<ISelectedItemService, SelectedItemService>()
            .AddSingleton<IPromptService, PromptService>()
            .AddSingleton<IPackageInfoLoader, PackageInfoLoader>()
            .AddSingleton<IFileRenamerService, FileRenamerService>()
            .AddSingleton<IFileLocationOpenerService, FileLocationOpenerService>()
            .AddSingleton<OnlineTitleInfoService>()
            .AddSingleton<IOnlineTitleInfoService>(sp => sp.GetRequiredService<OnlineTitleInfoService>())
            .AddSingleton<ITitleDbUpdater>(sp => sp.GetRequiredService<OnlineTitleInfoService>())
            .AddSingleton<ICachedOnlineTitleInfoService, CachedOnlineTitleInfoService>()
            .AddSingleton<IOnlineTitlePageOpenerService, OnlineTitlePageOpenerService>()
            .AddSingleton<MainBackgroundTaskRunnerService>()
            .AddSingleton<IMainBackgroundTaskRunnerService>(provider => provider.GetRequiredService<MainBackgroundTaskRunnerService>())

            .AddSingleton<IOpenFileCommand, OpenFileCommand>()
            .AddSingleton<IExitAppCommand, ExitAppCommand>()
            .AddSingleton<IOpenLastFileCommand, OpenLastFileCommand>()
            .AddSingleton<ICloseFileCommand, CloseFileCommand>()
            .AddSingleton<IShowSettingsWindowCommand, ShowSettingsWindowCommand>()
            .AddSingleton<IVerifyNcasIntegrityCommand, VerifyNcasIntegrityCommand>()
            .AddSingleton<IShowBatchIntegrityWindowCommand, ShowBatchIntegrityWindowCommand>()
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
            .AddSingleton<Emignatik.NxFileViewer.Services.Nand.NandCliPlugin>()
            .AddSingleton<Emignatik.NxFileViewer.Services.Nand.NandPluginManager>()
            .AddSingleton<Emignatik.NxFileViewer.Services.Nand.NandPluginActions>()
            .AddSingleton<NandViewModel>()
            .AddSingleton<RenameToolWindowViewModel>()
            .AddTransient<BatchIntegrityWindowViewModel>()
            .AddSingleton<AppSettings>()
            .AddSingleton<IAppSettings>(provider => provider.GetRequiredService<AppSettings>())
            .AddSingleton<IAppSettingsManager, AppSettingsManager>()
            .AddSingleton<IFileItemLoader, FileItemLoader>()
            .AddSingleton<IFileOverviewLoader, FileOverviewLoader>()
            .AddSingleton<IItemViewModelBuilder, ItemViewModelBuilder>()
            .AddSingleton<IFileLoader, FileLoader>()
            .AddSingleton<IBrushesProvider, BrushesProvider>()
            .AddSingleton<IThemeService, ThemeService>()
            .AddSingleton<IWindowPlacementService, WindowPlacementService>()
            .AddSingleton<ILocalizationFromSettingsSynchronizerService, LocalizationFromSettingsSynchronizerService>()
            .AddSingleton<IShallowCopier, ShallowCopier>()
            .AddSingleton<INcaHashService, NcaHashService>()
            .AddSingleton<INcaItemIntegrityService, NcaItemIntegrityService>()

            .AddTransient<SettingsWindowViewModel>() // Important to let transient so that real actual settings are displayed when settings view is shown
            .AddTransient<ISaveFileRunnable, SaveFileRunnable>()
            .AddTransient<ISaveDirectoryRunnable, SaveDirectoryRunnable>()
            .AddTransient<IVerifyNcasIntegrityRunnable, VerifyNcasIntegrityRunnable>()
            .AddTransient<IVerifyDirectoryIntegrityRunnable, VerifyDirectoryIntegrityRunnable>()
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
        KeyDownloads.MigrateLegacyHost(ServiceProvider.GetRequiredService<IAppSettings>());
        var titleSettings = ServiceProvider.GetRequiredService<IAppSettings>();
        if (titleSettings.TitlePageUrl == "https://tinfoil.io/Title/{TitleId}")
            titleSettings.TitlePageUrl = "https://tinfoil.media/Title/{TitleId}";
        if (titleSettings.TitleInfoProvider == TitleInfoProvider.Tinfoil && titleSettings.TitleInfoApiUrl == "https://tinfoil.io/api/title/{TitleId}")
            titleSettings.TitleInfoApiUrl = "https://tinfoil.media/api/title/{TitleId}";
        ServiceProvider.GetRequiredService<AppLoggerProvider>().ConfigureRetention();

        // Initialize localization
        ServiceProvider.GetRequiredService<ILocalizationFromSettingsSynchronizerService>().Initialize();
        ServiceProvider.GetRequiredService<IThemeService>().Initialize();

        LocalizationStringExtension.FormatException += OnLocalizationStringFormatException;

        var mainWindowViewModel = ServiceProvider.GetRequiredService<MainWindowViewModel>();
        var mainWindow = new MainWindow
        {
            DataContext = mainWindowViewModel
        };
        mainWindowViewModel.Window = mainWindow;
        ServiceProvider.GetRequiredService<IThemeService>().RegisterWindow(mainWindow);
        ServiceProvider.GetRequiredService<IWindowPlacementService>().Track(mainWindow);

        void MainWindowLoaded(object sender, RoutedEventArgs args)
        {
            mainWindow.Loaded -= MainWindowLoaded;
            _ = ServiceProvider.GetRequiredService<ViewerUpdateActions>().CheckAsync(true);
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
            await DownloadMissingKeys(prodKeysDownloadUrl, appSettings.ProdKeysFilePath, keySetProviderService.AppDirProdKeysFilePath);
        }

        var titleKeysDownloadUrl = appSettings.TitleKeysDownloadUrl;
        if (keySetProviderService.ActualTitleKeysFilePath == null && !string.IsNullOrWhiteSpace(titleKeysDownloadUrl))
        {
            await DownloadMissingKeys(titleKeysDownloadUrl, appSettings.TitleKeysFilePath, keySetProviderService.AppDirTitleKeysFilePath);
        }

        var fileOpeningService = ServiceProvider.GetRequiredService<IFileOpeningService>();
        if (cmdLineArgs.Count > 0)
            await fileOpeningService.SafeOpenFile(cmdLineArgs[0]);

        async System.Threading.Tasks.Task DownloadMissingKeys(string template, string customPath, string programPath)
        {
            try
            {
                var url = KeyDownloads.ResolveUrl(template, appSettings.KeysDownloadHost);
                var destination = KeyDownloads.Destination(customPath, programPath);
                await backgroundTaskService.RunAsync(new RunnableRelay((progress, token) =>
                {
                    progress.SetText(LocalizationManager.Instance.Current.Keys.Status_DownloadingFile.SafeFormat(System.IO.Path.GetFileName(destination)));
                    _logger.LogInformation(LocalizationManager.Instance.Current.Keys.Log_DownloadingFileFromUrl.SafeFormat(destination, url));
                    KeyDownloads.DownloadAsync(ServiceProvider.GetRequiredService<IHttpDownloader>(), url, destination, token).GetAwaiter().GetResult();
                    _logger.LogInformation(LocalizationManager.Instance.Current.Keys.Log_FileSuccessfullyDownloaded.SafeFormat(destination));
                }) { SupportsCancellation = true });
            }
            catch (OperationCanceledException) { }
            catch (Exception ex) { _logger.LogWarning(ex, "Unable to download missing key file."); }
            finally { keySetProviderService.Reset(); }
        }
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
        var removed = Emignatik.NxFileViewer.Services.EmptyDirectoryCleanup.Clean(AppContext.BaseDirectory);
        if (removed > 0) _logger.LogInformation("Removed {Count} empty program subdirectories on exit.", removed);
        ServiceProvider.GetRequiredService<IAppLoggerProvider>().Dispose();
    }

    protected virtual void NotifyAppShuttingDown()
    {
        AppShuttingDown?.Invoke();
    }

}

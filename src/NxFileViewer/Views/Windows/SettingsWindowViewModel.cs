using System.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Localization.Keys;
using Emignatik.NxFileViewer.Services.BackgroundTask;
using Emignatik.NxFileViewer.Services.BackgroundTask.RunnableImpl;
using Emignatik.NxFileViewer.Services.FileLocationOpening;
using Emignatik.NxFileViewer.Services.KeysManagement;
using Emignatik.NxFileViewer.Styling.Theme;
using Emignatik.NxFileViewer.Settings;
using Emignatik.NxFileViewer.Utils.MVVM;
using Emignatik.NxFileViewer.Utils.MVVM.Commands;
using Emignatik.NxFileViewer.Utils.MVVM.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using Emignatik.NxFileViewer.Services.OnlineServices;

namespace Emignatik.NxFileViewer.Views.Windows;

public class SettingsWindowViewModel : WindowViewModelBase
{
    private readonly IAppSettingsManager _appSettingsManager;
    private readonly IMainBackgroundTaskRunnerService _backgroundTaskRunnerService;
    private readonly IServiceProvider _serviceProvider;
    private readonly IKeySetProviderService _keySetProviderService;
    private readonly IFileLocationOpenerService _fileLocationOpenerService;

    private IAppSettings _editedSettings;
    private ILocalization<ILocalizationKeys>? _selectedLanguage;


    public SettingsWindowViewModel(IAppSettingsManager appSettingsManager, IMainBackgroundTaskRunnerService backgroundTaskRunnerService, IServiceProvider serviceProvider,
        IKeySetProviderService keySetProviderService, IFileLocationOpenerService fileLocationOpenerService)
    {
        _appSettingsManager = appSettingsManager ?? throw new ArgumentNullException(nameof(appSettingsManager));
        _backgroundTaskRunnerService = backgroundTaskRunnerService ?? throw new ArgumentNullException(nameof(backgroundTaskRunnerService));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _keySetProviderService = keySetProviderService ?? throw new ArgumentNullException(nameof(keySetProviderService));
        _fileLocationOpenerService = fileLocationOpenerService ?? throw new ArgumentNullException(nameof(fileLocationOpenerService));

        CopyKeysToSwitchCommand = new RelayCommand(CopyKeysToSwitch, () => !_backgroundTaskRunnerService.IsRunning &&
            (File.Exists(ActualProdKeysFilePath) || File.Exists(ActualTitleKeysFilePath)));
        BrowseProdKeysCommand = new RelayCommand(BrowseProdKeys);
        BrowseTitleKeysCommand = new RelayCommand(BrowseTitleKeys);
        BrowseNandCommand = new RelayCommand(() =>
        {
            var dialog = new OpenFileDialog { Filter = "NxNandManager (*.exe)|*.exe" };
            if (dialog.ShowDialog() == true) EditedSettings.NandExecutablePath = dialog.FileName;
        });
        BrowseBisKeysCommand = new RelayCommand(() =>
        {
            var dialog = new OpenFileDialog { Filter = "BIS keys (*.*)|*.*" };
            if (dialog.ShowDialog() == true) EditedSettings.NandBisKeysPath = dialog.FileName;
        });
        BrowseNszCommand = new RelayCommand(() =>
        {
            var dialog = new OpenFileDialog { Filter = "NSZ CLI (*.exe)|*.exe" };
            if (dialog.ShowDialog() == true) EditedSettings.NszExecutablePath = dialog.FileName;
        });
        ApplySettingsCommand = new RelayCommand(ApplySettings);
        CancelSettingsCommand = new RelayCommand(CancelSettings);
        ResetSettingsCommand = new RelayCommand(ResetSettings);
        DownloadProdKeysCommand = new RelayCommand(DownloadProdKeys, CanDownloadProdKeys);
        DownloadTitleKeysCommand = new RelayCommand(DownloadTitleKeys, CanDownloadTitleKeys);
        EditProdKeysCommand = new RelayCommand(OpenProdKeysLocation, CanOpenProdKeysLocation);
        EditTitleKeysCommand = new RelayCommand(OpenTitleKeysLocation, CanOpenTitleKeysLocation);
        OpenKeyLocationCommand = new RelayCommand(path => _fileLocationOpenerService.OpenFileLocationSafe((string)path!),
            path => path is string file && SafeCheckFileExists(file));
        DownloadKeysCommand = new RelayCommand(() => DownloadKeys(true, true), () => !_backgroundTaskRunnerService.IsRunning);

        InitializeFromSettings(appSettingsManager.Clone());

        _backgroundTaskRunnerService.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName == nameof(IMainBackgroundTaskRunnerService.IsRunning))
            {
                CopyKeysToSwitchCommand.TriggerCanExecuteChanged();
                DownloadProdKeysCommand.TriggerCanExecuteChanged();
                DownloadTitleKeysCommand.TriggerCanExecuteChanged();
                DownloadKeysCommand.TriggerCanExecuteChanged();
            }
        };

        _keySetProviderService.PropertyChanged += (_, args) =>
        {
            CopyKeysToSwitchCommand.TriggerCanExecuteChanged();
            if (args.PropertyName == nameof(IKeySetProviderService.ActualProdKeysFilePath))
                NotifyPropertyChanged(nameof(ActualProdKeysFilePath));
            else if (args.PropertyName == nameof(IKeySetProviderService.ActualTitleKeysFilePath))
                NotifyPropertyChanged(nameof(ActualTitleKeysFilePath));
            else if (args.PropertyName == nameof(IKeySetProviderService.ProdKeysValidation))
            {
                NotifyPropertyChanged(nameof(ProdKeysValidationSummary));
                NotifyPropertyChanged(nameof(AreProdKeysValid));
                NotifyPropertyChanged(nameof(HasProdKeysWarnings));
            }
            else if (args.PropertyName == nameof(IKeySetProviderService.TitleKeysValidation))
            {
                NotifyPropertyChanged(nameof(TitleKeysValidationSummary));
                NotifyPropertyChanged(nameof(AreTitleKeysValid));
                RefreshKeyLocations();
            }
        };
    }

    private const string TinfoilPage = "https://tinfoil.media/Title/{TitleId}";
    private const string NxContentPage = "https://nx-content.ghostland.at/?game={TitleId}";
    private int _titlePageSource;
    private string _customTitlePageUrl = TinfoilPage;
    public IReadOnlyList<TitlePageOption> TitlePageOptions => new[]
    {
        new TitlePageOption(0, "Tinfoil"), new TitlePageOption(1, "NX Content"),
        new TitlePageOption(2, LocalizationManager.Instance.Current.Keys.TitlePage_Custom)
    };
    public int SelectedTitlePageSource
    {
        get => _titlePageSource;
        set
        {
            if (_titlePageSource == value) return;
            if (_titlePageSource == 2) _customTitlePageUrl = EditedSettings.TitlePageUrl;
            _titlePageSource = value;
            EditedSettings.TitlePageUrl = value == 0 ? TinfoilPage : value == 1 ? NxContentPage : _customTitlePageUrl;
            NotifyPropertyChanged();
            NotifyPropertyChanged(nameof(IsCustomTitlePage));
        }
    }
    public bool IsCustomTitlePage => _titlePageSource == 2;
    public RelayCommand CopyKeysToSwitchCommand { get; }
    private void CopyKeysToSwitch()
    {
        var keys = LocalizationManager.Instance.Current.Keys;
        var files = new[] { (Source: ActualProdKeysFilePath, Name: "prod.keys"), (Source: ActualTitleKeysFilePath, Name: "title.keys") };
        try
        {
            var existing = new List<string>();
            foreach (var file in files)
            {
                var target = Path.Combine(SharedKeyFiles.DirectoryPath, file.Name);
                if (File.Exists(file.Source) && File.Exists(target) && !Path.GetFullPath(file.Source!).Equals(target, StringComparison.OrdinalIgnoreCase)) existing.Add(target);
            }
            var comparisons = files.Where(file => existing.Contains(Path.Combine(SharedKeyFiles.DirectoryPath, file.Name)))
                .Select(file => KeyReplacementComparison(file.Source!, Path.Combine(SharedKeyFiles.DirectoryPath, file.Name), file.Name == "title.keys"));
            if (existing.Count > 0 && ThemedDialog.Confirm("Keys", keys.Keys_ReplaceShared + Environment.NewLine + string.Join(Environment.NewLine + Environment.NewLine, comparisons), false) != MessageBoxResult.Yes) return;
            foreach (var file in files)
                if (File.Exists(file.Source)) SharedKeyFiles.Copy(file.Source!, Path.Combine(SharedKeyFiles.DirectoryPath, file.Name), overwrite: existing.Contains(Path.Combine(SharedKeyFiles.DirectoryPath, file.Name)));
            RefreshKeyLocations();
            ThemedDialog.Notice(keys.Keys_SharedCopied + Environment.NewLine + SharedKeyFiles.DirectoryPath, "Keys", MessageBoxImage.Information);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException)
        { ThemedDialog.Notice(ex.Message, "Keys", MessageBoxImage.Error); }
    }
    public IAppSettings EditedSettings
    {
        get => _editedSettings;
        [MemberNotNull(nameof(_editedSettings))]
        private set
        {
            if (_editedSettings != null) _editedSettings.PropertyChanged -= OnEditedKeyPathChanged;
            _editedSettings = value;
            _editedSettings.PropertyChanged += OnEditedKeyPathChanged;
            RefreshKeyLocations();
            _titlePageSource = value.TitlePageUrl == TinfoilPage ? 0 : value.TitlePageUrl == NxContentPage ? 1 : 2;
            _customTitlePageUrl = _titlePageSource == 2 ? value.TitlePageUrl : TinfoilPage;
            NotifyPropertyChanged(nameof(SelectedTitlePageSource));
            NotifyPropertyChanged(nameof(IsCustomTitlePage));
            NotifyPropertyChanged(nameof(SelectedTitleInfoSource));
            NotifyPropertyChanged();
        }
    }

    public ICommand BrowseProdKeysCommand { get; }

    public ICommand BrowseTitleKeysCommand { get; }
    public IReadOnlyList<NszModeOption> NszModeOptions => new[]
    {
        new NszModeOption(NszCompressionMode.Auto, LocalizationManager.Instance.Current.Keys.Nsz_ModeAuto),
        new NszModeOption(NszCompressionMode.Solid, LocalizationManager.Instance.Current.Keys.Nsz_ModeSolid),
        new NszModeOption(NszCompressionMode.Block, LocalizationManager.Instance.Current.Keys.Nsz_ModeBlock)
    };
    public IReadOnlyList<NszBlockSizeOption> NszBlockSizeOptions { get; } = BuildBlockSizeOptions();
    private static IReadOnlyList<NszBlockSizeOption> BuildBlockSizeOptions()
    {
        var options = new List<NszBlockSizeOption>();
        for (var exponent = 14; exponent <= 32; exponent++)
        {
            var bytes = 1L << exponent;
            var label = exponent < 20 ? $"{bytes / 1024} KiB" : exponent < 30 ? $"{bytes / (1024 * 1024)} MiB" : $"{bytes / (1024L * 1024 * 1024)} GiB";
            options.Add(new(exponent, label));
        }
        return options;
    }

    public Emignatik.NxFileViewer.Services.Updates.ViewerUpdateActions ViewerUpdates => _serviceProvider.GetRequiredService<Emignatik.NxFileViewer.Services.Updates.ViewerUpdateActions>();

    public ICommand BrowseNszCommand { get; }
    public ICommand BrowseNandCommand { get; }
    public ICommand BrowseBisKeysCommand { get; }

    public ICommand ApplySettingsCommand { get; }

    public ICommand CancelSettingsCommand { get; }

    public ICommand ResetSettingsCommand { get; }

    public RelayCommand DownloadProdKeysCommand { get; }

    public RelayCommand DownloadTitleKeysCommand { get; }
    public RelayCommand DownloadKeysCommand { get; }
    private static string KeyReplacementComparison(string source, string destination, bool titleKeys)
    {
        var keys = LocalizationManager.Instance.Current.Keys;
        string Summary(string path) => BuildValidationSummary(titleKeys ? KeyFileValidator.ValidateTitleKeys(path) : KeyFileValidator.ValidateProdKeys(path));
        return destination + Environment.NewLine + keys.Keys_ExistingValidation + Environment.NewLine + Summary(destination)
            + Environment.NewLine + keys.Keys_IncomingValidation + Environment.NewLine + Summary(source);
    }
    public RelayCommand OpenKeyLocationCommand { get; }

    public RelayCommand EditProdKeysCommand { get; }

    public RelayCommand EditTitleKeysCommand { get; }


    public IEnumerable<LogLevel> LogLevels => Enum.GetValues<LogLevel>();
    public IEnumerable<TitleProviderOption> TitleProviderOptions { get; } = new[]
    {
        new TitleProviderOption(TitleInfoProvider.Tinfoil, "Tinfoil"),
        new TitleProviderOption(TitleInfoProvider.GitHubTitleDb, "TitleDB (GitHub)"),
        new TitleProviderOption(TitleInfoProvider.NLib, "NLib API"),
        new TitleProviderOption(TitleInfoProvider.Custom, LocalizationManager.Instance.Current.Keys.TitlePage_Custom),
    };
    public TitleInfoProvider SelectedTitleInfoSource
    {
        get => EditedSettings.TitleInfoProvider;
        set {
            EditedSettings.TitleInfoProvider = value;
            if (value == TitleInfoProvider.Tinfoil) EditedSettings.TitleInfoApiUrl = "https://tinfoil.media/api/title/{TitleId}";
            if (value == TitleInfoProvider.NLib) EditedSettings.NLibApiUrl = "https://api.nlib.cc/nx/{TitleId}?lang={Language}";
            NotifyPropertyChanged();
        }
    }
    public IEnumerable<string> TitleDbRegions { get; } = new[] { "DE.de", "US.en", "GB.en", "FR.fr", "ES.es", "IT.it", "JP.ja" };

    public IEnumerable<ThemeOption> ThemeOptions { get; } = new[]
    {
        new ThemeOption(AppTheme.System, "Auto"),
        new ThemeOption(AppTheme.Light, "Light"),
        new ThemeOption(AppTheme.Dark, "Dark"),
    };

    public string ActualProdKeysFilePath => _keySetProviderService.ActualProdKeysFilePath ?? LocalizationManager.Instance.Current.Keys.NoneKeysFile;

    public string ActualTitleKeysFilePath => _keySetProviderService.ActualTitleKeysFilePath ?? LocalizationManager.Instance.Current.Keys.NoneKeysFile;

    public bool AreProdKeysValid => _keySetProviderService.ProdKeysValidation.IsValid;

    public bool HasProdKeysWarnings => _keySetProviderService.ProdKeysValidation.HasWarnings;

    public bool AreTitleKeysValid => _keySetProviderService.TitleKeysValidation.IsValid;

    public string ProdKeysValidationSummary => BuildValidationSummary(_keySetProviderService.ProdKeysValidation);

    public string TitleKeysValidationSummary => BuildValidationSummary(_keySetProviderService.TitleKeysValidation);

    public IReadOnlyList<KeyLocationStatus> ProdKeyLocations { get; private set; } = Array.Empty<KeyLocationStatus>();
    public IReadOnlyList<KeyLocationStatus> TitleKeyLocations { get; private set; } = Array.Empty<KeyLocationStatus>();

    private void OnEditedKeyPathChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(IAppSettings.ProdKeysFilePath) or nameof(IAppSettings.TitleKeysFilePath)) RefreshKeyLocations();
    }

    public void RefreshKeyLocations()
    {
        ProdKeyLocations = KeyLocationStatus.Inspect(_keySetProviderService.AppDirProdKeysFilePath,
            Path.Combine(SharedKeyFiles.DirectoryPath, "prod.keys"), EditedSettings.ProdKeysFilePath,
            _keySetProviderService.ActualProdKeysFilePath, false);
        TitleKeyLocations = KeyLocationStatus.Inspect(_keySetProviderService.AppDirTitleKeysFilePath,
            Path.Combine(SharedKeyFiles.DirectoryPath, "title.keys"), EditedSettings.TitleKeysFilePath,
            _keySetProviderService.ActualTitleKeysFilePath, true);
        NotifyPropertyChanged(nameof(ProdKeyLocations));
        NotifyPropertyChanged(nameof(TitleKeyLocations));
    }

    public IEnumerable<ILocalization<ILocalizationKeys>> AvailableLanguages => LocalizationManager.Instance.AvailableLocalizations;

    public ILocalization<ILocalizationKeys>? SelectedLanguage
    {
        get => _selectedLanguage;
        set
        {
            _selectedLanguage = value;
            if (value != null)
                EditedSettings.AppLanguage = value.CultureName;
            NotifyPropertyChanged();
        }
    }

    [MemberNotNull(nameof(_editedSettings))]
    private void InitializeFromSettings(IAppSettings appSettings)
    {
        KeyDownloads.MigrateLegacyHost(appSettings);
        EditedSettings = appSettings;
        this.SelectedLanguage = LocalizationManager.Instance.AvailableLocalizations.FindByCultureName(appSettings.AppLanguage);
    }

    private void BrowseProdKeys()
    {
        var clonedSettings = EditedSettings;
        if (BrowseKeysFilePath(clonedSettings.ProdKeysFilePath, LocalizationManager.Instance.Current.Keys.BrowseKeysFile_ProdTitle, out var selectedFilePath))
        {
            clonedSettings.ProdKeysFilePath = selectedFilePath;
        }
    }

    private void BrowseTitleKeys()
    {
        var clonedSettings = EditedSettings;
        if (BrowseKeysFilePath(clonedSettings.TitleKeysFilePath, LocalizationManager.Instance.Current.Keys.BrowseKeysFile_TitleTitle, out var selectedFilePath))
        {
            clonedSettings.TitleKeysFilePath = selectedFilePath;
        }
    }

    private void DownloadProdKeys() => DownloadKeys(true, false);

    private void DownloadTitleKeys() => DownloadKeys(false, true);

    private async void DownloadKeys(bool prod, bool title)
    {
        if (_backgroundTaskRunnerService.IsRunning) return;
        var logger = _serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger<SettingsWindowViewModel>();
        try
        {
            var settings = EditedSettings;
            var requests = new List<(string Url, string Destination)>();
            if (prod) requests.Add((KeyDownloads.ResolveUrl(settings.ProdKeysDownloadUrl, settings.KeysDownloadHost),
                KeyDownloads.Destination(settings.ProdKeysFilePath, _keySetProviderService.AppDirProdKeysFilePath)));
            if (title) requests.Add((KeyDownloads.ResolveUrl(settings.TitleKeysDownloadUrl, settings.KeysDownloadHost),
                KeyDownloads.Destination(settings.TitleKeysFilePath, _keySetProviderService.AppDirTitleKeysFilePath)));
            if (requests.Count == 2 && string.Equals(requests[0].Destination, requests[1].Destination, StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("prod.keys and title.keys require different destination paths.");
            var downloader = _serviceProvider.GetRequiredService<IHttpDownloader>();
            await _backgroundTaskRunnerService.RunAsync(new RunnableRelay((progress, token) =>
            {
                for (var i = 0; i < requests.Count; i++)
                {
                    token.ThrowIfCancellationRequested();
                    var request = requests[i];
                    progress.SetText(LocalizationManager.Instance.Current.Keys.Status_DownloadingFile.SafeFormat(Path.GetFileName(request.Destination)));
                    logger.LogInformation(LocalizationManager.Instance.Current.Keys.Log_DownloadingFileFromUrl.SafeFormat(request.Destination, request.Url));
                    var titleKeys = title && (!prod || i == 1);
                    KeyDownloads.DownloadAsync(downloader, request.Url, request.Destination, token, (source, destination) =>
                    {
                        var comparison = KeyReplacementComparison(source, destination, titleKeys);
                        return Application.Current.Dispatcher.Invoke(() => token.IsCancellationRequested ? false :
                            ThemedDialog.Confirm("Keys", LocalizationManager.Instance.Current.Keys.Keys_ReplaceDownloaded + Environment.NewLine + comparison, false) == MessageBoxResult.Yes);
                    }).GetAwaiter().GetResult();
                    logger.LogInformation(LocalizationManager.Instance.Current.Keys.Log_FileSuccessfullyDownloaded.SafeFormat(request.Destination));
                    progress.SetPercentage((i + 1d) / requests.Count);
                }
            }) { SupportsCancellation = true, SupportProgress = true });
        }
        catch (OperationCanceledException) { logger.LogInformation(LocalizationManager.Instance.Current.Keys.Log_DownloadFileCanceled); }
        catch (Exception ex)
        {
            logger.LogError(ex, "Key download failed.");
            ThemedDialog.Notice(ex.Message, "Keys", MessageBoxImage.Error);
        }
        finally { _keySetProviderService.Reset(); RefreshKeyLocations(); }
    }

    private bool CanDownloadTitleKeys()
    {
        return !_backgroundTaskRunnerService.IsRunning;
    }

    private bool CanDownloadProdKeys()
    {
        return !_backgroundTaskRunnerService.IsRunning;
    }


    private bool CanOpenProdKeysLocation()
    {
        return SafeCheckFileExists(ActualProdKeysFilePath);
    }

    private void OpenProdKeysLocation()
    {
        _fileLocationOpenerService.OpenFileLocationSafe(ActualProdKeysFilePath);
    }

    private bool CanOpenTitleKeysLocation()
    {
        return SafeCheckFileExists(ActualTitleKeysFilePath);
    }

    private void OpenTitleKeysLocation()
    {
        _fileLocationOpenerService.OpenFileLocationSafe(ActualTitleKeysFilePath);
    }


    private static bool SafeCheckFileExists(string? filePath)
    {
        try
        {
            if (string.IsNullOrEmpty(filePath))
                return false;
            return File.Exists(filePath);
        }
        catch
        {
            return false;
        }
    }

    internal static string BuildValidationSummary(KeyFileValidationResult result)
    {
        var keys = LocalizationManager.Instance.Current.Keys;
        if (!result.FileExists)
            return keys.KeysValidation_MissingFile;

        var messages = new List<string>();
        if (result.UnsupportedMasterKeys is { Count: > 0 })
            messages.Add(keys.KeysValidation_UnsupportedMasterKeys.SafeFormat(string.Join(", ", result.UnsupportedMasterKeys)));
        if (result.HighestValidMasterKeyRevision is { } revision &&
            MasterKeyFirmwareMap.GetSupportedFirmware(revision) is { } firmware)
        {
            messages.Add(keys.KeysValidation_FirmwareEstimate.SafeFormat(
                $"master_key_{revision:x2}", firmware));
        }
        if (result.MissingKeys.Count > 0)
            messages.Add(keys.KeysValidation_MissingMasterKeys.SafeFormat(string.Join(", ", result.MissingKeys)));
        if (result.InvalidKeys.Count > 0)
            messages.Add(keys.KeysValidation_InvalidMasterKeys.SafeFormat(string.Join(", ", result.InvalidKeys)));
        if (result.InvalidLineNumbers.Count > 0)
            messages.Add(keys.KeysValidation_InvalidLines.SafeFormat(string.Join(", ", result.InvalidLineNumbers)));
        if (result.ValidEntryCount == 0 && messages.Count == 0)
            messages.Add(keys.KeysValidation_EmptyFile);
        if (messages.Count == 0)
            messages.Add(keys.KeysValidation_ValidEntries.SafeFormat(result.ValidEntryCount));

        return string.Join(Environment.NewLine, messages);
    }

    private static bool BrowseKeysFilePath(string initialFilePath, string title, [NotNullWhen(true)] out string? selectedFilePath)
    {
        selectedFilePath = null;

        var openFileDialog = new OpenFileDialog
        {
            Title = title,
            FileName = initialFilePath,
            Filter = LocalizationManager.Instance.Current.Keys.BrowseKeysFile_Filter,
        };

        var result = openFileDialog.ShowDialog();
        if (result != null && result.Value)
        {
            selectedFilePath = openFileDialog.FileName;
            return true;
        }
        else
        {
            return false;
        }
    }

    public Action? EditingCompleted { get; set; }
    public bool PluginSettingsOnly { get; set; }
    public Emignatik.NxFileViewer.Services.Nsz.NszActions Nsz => _serviceProvider.GetRequiredService<Emignatik.NxFileViewer.Services.Nsz.NszActions>();

    private static void CopyPluginSettings(IAppSettings source, IAppSettings destination)
    {
        destination.NandExecutablePath = source.NandExecutablePath;
        destination.NandBisKeysPath = source.NandBisKeysPath;
        destination.NszExecutablePath = source.NszExecutablePath;
        destination.NszCheckUpdates = source.NszCheckUpdates;
        destination.NszCompressionLevel = source.NszCompressionLevel;
        destination.NszCompressionMode = source.NszCompressionMode;
        destination.NszBlockSizeExponent = source.NszBlockSizeExponent;
    }


    private void ApplySettings()
    {
        if (PluginSettingsOnly)
            CopyPluginSettings(EditedSettings, _serviceProvider.GetRequiredService<IAppSettings>());
        else
        {
            // A general-settings draft must not overwrite separately saved plugin settings.
            CopyPluginSettings(_appSettingsManager.Clone(), EditedSettings);
            _appSettingsManager.Load(EditedSettings);
        }
        InitializeFromSettings(_appSettingsManager.Clone());
        EditingCompleted?.Invoke();
    }

    private void CancelSettings()
    {
        InitializeFromSettings(_appSettingsManager.Clone());
        EditingCompleted?.Invoke();
    }

    private void ResetSettings()
    {
        InitializeFromSettings(_appSettingsManager.GetDefault());
    }
}

public sealed record ThemeOption(AppTheme Value, string DisplayName);
public sealed record TitleProviderOption(TitleInfoProvider Value, string DisplayName);

public sealed record NszModeOption(NszCompressionMode Value, string DisplayName);
public sealed record NszBlockSizeOption(int Value, string DisplayName);

public sealed record TitlePageOption(int Value, string DisplayName);

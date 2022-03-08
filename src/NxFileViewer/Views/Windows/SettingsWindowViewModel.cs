using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Windows.Input;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Localization.Keys;
using Emignatik.NxFileViewer.Services;
using Emignatik.NxFileViewer.Services.BackgroundTask;
using Emignatik.NxFileViewer.Services.BackgroundTask.RunnableImpl;
using Emignatik.NxFileViewer.Settings;
using Emignatik.NxFileViewer.Utils.MVVM;
using Emignatik.NxFileViewer.Utils.MVVM.Commands;
using Emignatik.NxFileViewer.Utils.MVVM.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;

namespace Emignatik.NxFileViewer.Views.Windows;

public class SettingsWindowViewModel : WindowViewModelBase
{
    private readonly IAppSettingsManager _appSettingsManager;
    private readonly IBackgroundTaskService _backgroundTaskService;
    private readonly IServiceProvider _serviceProvider;
    private readonly IKeySetProviderService _keySetProviderService;
    private readonly IAppSettings _appSettings;
    private readonly ILogger _logger;

    private string _prodKeysFilePath = "";
    private string _consoleKeysFilePath = "";
    private string _titleKeysFilePath = "";
    private string _prodKeysDownloadUrl = "";
    private string _titleKeysDownloadUrl = "";
    private string _titlePageUrl = "";

    private LogLevel _selectedLogLevel;
    private bool _alwaysReloadKeysBeforeOpen;


    public SettingsWindowViewModel(
        IAppSettingsManager appSettingsManager,
        IBackgroundTaskService backgroundTaskService,
        IServiceProvider serviceProvider,
        IKeySetProviderService keySetProviderService,
        ILoggerFactory loggerFactory)
    {
        _appSettingsManager = appSettingsManager ?? throw new ArgumentNullException(nameof(appSettingsManager));
        _backgroundTaskService = backgroundTaskService ?? throw new ArgumentNullException(nameof(backgroundTaskService));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _keySetProviderService = keySetProviderService ?? throw new ArgumentNullException(nameof(keySetProviderService));
        _logger = (loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory))).CreateLogger(this.GetType());

        _appSettings = appSettingsManager.Settings;

        BrowseProdKeysCommand = new RelayCommand(BrowseProdKeys);
        BrowseConsoleKeysCommand = new RelayCommand(BrowseConsoleKeys);
        BrowseTitleKeysCommand = new RelayCommand(BrowseTitleKeys);
        SaveSettingsCommand = new RelayCommand(SaveSettings);
        CancelSettingsCommand = new RelayCommand(CancelSettings);
        DownloadProdKeysCommand = new RelayCommand(DownloadProdKeys, CanDownloadProdKeys);
        DownloadTitleKeysCommand = new RelayCommand(DownloadTitleKeys, CanDownloadTitleKeys);
        EditProdKeysCommand = new RelayCommand(EditProdKeys, CanEditProdKeys);
        EditTitleKeysCommand = new RelayCommand(EditTitleKeys, CanEditTitleKeys);
        EditConsoleKeysCommand = new RelayCommand(EditConsoleKeys, CanEditConsoleKeys);

        ProdKeysFilePath = _appSettings.ProdKeysFilePath;
        ConsoleKeysFilePath = _appSettings.ConsoleKeysFilePath;
        TitleKeysFilePath = _appSettings.TitleKeysFilePath;
        SelectedLogLevel = _appSettings.LogLevel;
        ProdKeysDownloadUrl = _appSettings.ProdKeysDownloadUrl;
        TitleKeysDownloadUrl = _appSettings.TitleKeysDownloadUrl;
        AlwaysReloadKeysBeforeOpen = _appSettings.AlwaysReloadKeysBeforeOpen;
        TitlePageUrl = _appSettings.TitlePageUrl;
        this.SelectedLanguage = LocalizationManager.Instance.Current;

        _backgroundTaskService.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName == nameof(IBackgroundTaskService.IsRunning))
            {
                DownloadProdKeysCommand.TriggerCanExecuteChanged();
                DownloadTitleKeysCommand.TriggerCanExecuteChanged();
            }
        };

        _keySetProviderService.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName == nameof(IKeySetProviderService.ActualProdKeysFilePath))
                NotifyPropertyChanged(nameof(ActualProdKeysFilePath));
            else if (args.PropertyName == nameof(IKeySetProviderService.ActualTitleKeysFilePath))
                NotifyPropertyChanged(nameof(ActualTitleKeysFilePath));
            else if (args.PropertyName == nameof(IKeySetProviderService.ActualConsoleKeysFilePath))
                NotifyPropertyChanged(nameof(ActualConsoleKeysFilePath));
        };

    }

    public ICommand BrowseProdKeysCommand { get; }

    public ICommand BrowseConsoleKeysCommand { get; }

    public ICommand BrowseTitleKeysCommand { get; }

    public ICommand SaveSettingsCommand { get; }

    public ICommand CancelSettingsCommand { get; }

    public RelayCommand DownloadProdKeysCommand { get; }

    public RelayCommand DownloadTitleKeysCommand { get; }

    public RelayCommand EditProdKeysCommand { get; }

    public RelayCommand EditTitleKeysCommand { get; }

    public RelayCommand EditConsoleKeysCommand { get; }

    public bool AlwaysReloadKeysBeforeOpen
    {
        get => _alwaysReloadKeysBeforeOpen;
        set
        {
            _alwaysReloadKeysBeforeOpen = value;
            NotifyPropertyChanged();
        }
    }

    public string ProdKeysFilePath
    {
        get => _prodKeysFilePath;
        set
        {
            _prodKeysFilePath = value;
            NotifyPropertyChanged(nameof(ProdKeysFilePath));
        }
    }

    public string ConsoleKeysFilePath
    {
        get => _consoleKeysFilePath;
        set
        {
            _consoleKeysFilePath = value;
            NotifyPropertyChanged(nameof(ConsoleKeysFilePath));
        }
    }

    public string TitleKeysFilePath
    {
        get => _titleKeysFilePath;
        set
        {
            _titleKeysFilePath = value;
            NotifyPropertyChanged(nameof(TitleKeysFilePath));
        }
    }

    public IEnumerable<LogLevel> LogLevels => Enum.GetValues<LogLevel>();

    public LogLevel SelectedLogLevel
    {
        get => _selectedLogLevel;
        set
        {
            _selectedLogLevel = value;
            NotifyPropertyChanged();
        }
    }

    public string ProdKeysDownloadUrl
    {
        get => _prodKeysDownloadUrl;
        set
        {
            _prodKeysDownloadUrl = value;
            NotifyPropertyChanged();
        }
    }

    public string TitleKeysDownloadUrl
    {
        get => _titleKeysDownloadUrl;
        set
        {
            _titleKeysDownloadUrl = value;
            NotifyPropertyChanged();
        }
    }

    public string ActualProdKeysFilePath => _keySetProviderService.ActualProdKeysFilePath ?? LocalizationManager.Instance.Current.Keys.NoneKeysFile;

    public string ActualTitleKeysFilePath => _keySetProviderService.ActualTitleKeysFilePath ?? LocalizationManager.Instance.Current.Keys.NoneKeysFile;

    public string ActualConsoleKeysFilePath => _keySetProviderService.ActualConsoleKeysFilePath ?? LocalizationManager.Instance.Current.Keys.NoneKeysFile;

    public string TitlePageUrl
    {
        get => _titlePageUrl;
        set
        {
            _titlePageUrl = value;
            NotifyPropertyChanged();
        }
    }

    public IEnumerable<ILocalization<ILocalizationKeys>> AvailableLanguages => LocalizationManager.Instance.AvailableLocalizations;

    public ILocalization<ILocalizationKeys> SelectedLanguage { get; set; }

    private void BrowseProdKeys()
    {
        if (BrowseKeysFilePath(ProdKeysFilePath, LocalizationManager.Instance.Current.Keys.BrowseKeysFile_ProdTitle, out var selectedFilePath))
        {
            ProdKeysFilePath = selectedFilePath;
        }
    }

    private void BrowseConsoleKeys()
    {
        if (BrowseKeysFilePath(ConsoleKeysFilePath, LocalizationManager.Instance.Current.Keys.BrowseKeysFile_ConsoleTitle, out var selectedFilePath))
        {
            ConsoleKeysFilePath = selectedFilePath;
        }
    }

    private void BrowseTitleKeys()
    {
        if (BrowseKeysFilePath(TitleKeysFilePath, LocalizationManager.Instance.Current.Keys.BrowseKeysFile_TitleTitle, out var selectedFilePath))
        {
            TitleKeysFilePath = selectedFilePath;
        }
    }


    private async void DownloadProdKeys()
    {
        var downloadFileRunnable = _serviceProvider.GetRequiredService<IDownloadFileRunnable>();
        downloadFileRunnable.Setup(ProdKeysDownloadUrl, _keySetProviderService.AppDirProdKeysFilePath);
        await _backgroundTaskService.RunAsync(downloadFileRunnable);
        _keySetProviderService.Reset();
    }

    private async void DownloadTitleKeys()
    {
        var downloadFileRunnable = _serviceProvider.GetRequiredService<IDownloadFileRunnable>();
        downloadFileRunnable.Setup(TitleKeysDownloadUrl, _keySetProviderService.AppDirTitleKeysFilePath);
        await _backgroundTaskService.RunAsync(downloadFileRunnable);
        _keySetProviderService.Reset();
    }

    private bool CanDownloadTitleKeys()
    {
        return !_backgroundTaskService.IsRunning;
    }

    private bool CanDownloadProdKeys()
    {
        return !_backgroundTaskService.IsRunning;
    }


    private bool CanEditProdKeys()
    {
        return SafeCheckFileExists(_keySetProviderService.ActualProdKeysFilePath!);
    }

    private void EditProdKeys()
    {
        SafeOpenFile(ActualProdKeysFilePath);
    }

    private bool CanEditTitleKeys()
    {
        return SafeCheckFileExists(_keySetProviderService.ActualTitleKeysFilePath!);
    }

    private void EditTitleKeys()
    {
        SafeOpenFile(_keySetProviderService.ActualTitleKeysFilePath!);
    }

    private bool CanEditConsoleKeys()
    {
        return SafeCheckFileExists(_keySetProviderService.ActualConsoleKeysFilePath!);
    }

    private void EditConsoleKeys()
    {
        SafeOpenFile(_keySetProviderService.ActualConsoleKeysFilePath!);
    }

    private bool SafeCheckFileExists(string filePath)
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

    private void SafeOpenFile(string filePath)
    {
        try
        {
            Process.Start("explorer", filePath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, LocalizationManager.Instance.Current.Keys.EditFile_Failed_Log.SafeFormat(filePath, ex.Message));
        }
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

    private void SaveSettings()
    {
        _appSettings.ProdKeysFilePath = ProdKeysFilePath;
        _appSettings.ConsoleKeysFilePath = ConsoleKeysFilePath;
        _appSettings.TitleKeysFilePath = TitleKeysFilePath;
        _appSettings.LogLevel = SelectedLogLevel;
        _appSettings.ProdKeysDownloadUrl = ProdKeysDownloadUrl;
        _appSettings.TitleKeysDownloadUrl = TitleKeysDownloadUrl;
        _appSettings.AlwaysReloadKeysBeforeOpen = AlwaysReloadKeysBeforeOpen;
        _appSettings.TitlePageUrl = TitlePageUrl;
        _appSettings.AppLanguage = SelectedLanguage.CultureName;

        _appSettingsManager.SaveSafe();
    }

    private void CancelSettings()
    {
        this.Window?.Close();
    }

}
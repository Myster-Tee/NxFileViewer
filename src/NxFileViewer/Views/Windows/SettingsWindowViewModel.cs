using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Windows.Input;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Localization.Keys;
using Emignatik.NxFileViewer.Services.BackgroundTask;
using Emignatik.NxFileViewer.Services.BackgroundTask.RunnableImpl;
using Emignatik.NxFileViewer.Services.FileLocationOpening;
using Emignatik.NxFileViewer.Services.KeysManagement;
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
    private readonly IMainBackgroundTaskRunnerService _backgroundTaskRunnerService;
    private readonly IServiceProvider _serviceProvider;
    private readonly IKeySetProviderService _keySetProviderService;
    private readonly IFileLocationOpenerService _fileLocationOpenerService;

    private IAppSettings _editedSettings = null!;
    private ILocalization<ILocalizationKeys>? _selectedLanguage;


    public SettingsWindowViewModel(IAppSettingsManager appSettingsManager, IMainBackgroundTaskRunnerService backgroundTaskRunnerService, IServiceProvider serviceProvider,
        IKeySetProviderService keySetProviderService, IFileLocationOpenerService fileLocationOpenerService)
    {
        _appSettingsManager = appSettingsManager ?? throw new ArgumentNullException(nameof(appSettingsManager));
        _backgroundTaskRunnerService = backgroundTaskRunnerService ?? throw new ArgumentNullException(nameof(backgroundTaskRunnerService));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _keySetProviderService = keySetProviderService ?? throw new ArgumentNullException(nameof(keySetProviderService));
        _fileLocationOpenerService = fileLocationOpenerService ?? throw new ArgumentNullException(nameof(fileLocationOpenerService));

        BrowseProdKeysCommand = new RelayCommand(BrowseProdKeys);
        BrowseConsoleKeysCommand = new RelayCommand(BrowseConsoleKeys);
        BrowseTitleKeysCommand = new RelayCommand(BrowseTitleKeys);
        ApplySettingsCommand = new RelayCommand(ApplySettings);
        CancelSettingsCommand = new RelayCommand(CancelSettings);
        ResetSettingsCommand = new RelayCommand(ResetSettings);
        DownloadProdKeysCommand = new RelayCommand(DownloadProdKeys, CanDownloadProdKeys);
        DownloadTitleKeysCommand = new RelayCommand(DownloadTitleKeys, CanDownloadTitleKeys);
        EditProdKeysCommand = new RelayCommand(OpenProdKeysLocation, CanOpenProdKeysLocation);
        EditTitleKeysCommand = new RelayCommand(OpenTitleKeysLocation, CanOpenTitleKeysLocation);
        EditConsoleKeysCommand = new RelayCommand(OpenConsoleKeysLocation, CanOpenConsoleKeysLocation);

        InitializeFromSettings(appSettingsManager.Clone());

        _backgroundTaskRunnerService.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName == nameof(IMainBackgroundTaskRunnerService.IsRunning))
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

    public IAppSettings EditedSettings
    {
        get => _editedSettings;
        set
        {
            _editedSettings = value;
            NotifyPropertyChanged();
        }
    }

    public ICommand BrowseProdKeysCommand { get; }

    public ICommand BrowseConsoleKeysCommand { get; }

    public ICommand BrowseTitleKeysCommand { get; }

    public ICommand ApplySettingsCommand { get; }

    public ICommand CancelSettingsCommand { get; }

    public ICommand ResetSettingsCommand { get; }

    public RelayCommand DownloadProdKeysCommand { get; }

    public RelayCommand DownloadTitleKeysCommand { get; }

    public RelayCommand EditProdKeysCommand { get; }

    public RelayCommand EditTitleKeysCommand { get; }

    public RelayCommand EditConsoleKeysCommand { get; }


    public IEnumerable<LogLevel> LogLevels => Enum.GetValues<LogLevel>();

    public string ActualProdKeysFilePath => _keySetProviderService.ActualProdKeysFilePath ?? LocalizationManager.Instance.Current.Keys.NoneKeysFile;

    public string ActualTitleKeysFilePath => _keySetProviderService.ActualTitleKeysFilePath ?? LocalizationManager.Instance.Current.Keys.NoneKeysFile;

    public string ActualConsoleKeysFilePath => _keySetProviderService.ActualConsoleKeysFilePath ?? LocalizationManager.Instance.Current.Keys.NoneKeysFile;

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

    private void InitializeFromSettings(IAppSettings appSettings)
    {
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

    private void BrowseConsoleKeys()
    {
        var clonedSettings = EditedSettings;
        if (BrowseKeysFilePath(clonedSettings.ConsoleKeysFilePath, LocalizationManager.Instance.Current.Keys.BrowseKeysFile_ConsoleTitle, out var selectedFilePath))
        {
            clonedSettings.ConsoleKeysFilePath = selectedFilePath;
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


    private async void DownloadProdKeys()
    {
        var clonedSettings = EditedSettings;
        var downloadFileRunnable = _serviceProvider.GetRequiredService<IDownloadFileRunnable>();
        downloadFileRunnable.Setup(clonedSettings.ProdKeysDownloadUrl, _keySetProviderService.AppDirProdKeysFilePath);
        await _backgroundTaskRunnerService.RunAsync(downloadFileRunnable);
        _keySetProviderService.Reset();
    }

    private async void DownloadTitleKeys()
    {
        var clonedSettings = EditedSettings;
        var downloadFileRunnable = _serviceProvider.GetRequiredService<IDownloadFileRunnable>();
        downloadFileRunnable.Setup(clonedSettings.TitleKeysDownloadUrl, _keySetProviderService.AppDirTitleKeysFilePath);
        await _backgroundTaskRunnerService.RunAsync(downloadFileRunnable);
        _keySetProviderService.Reset();
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

    private bool CanOpenConsoleKeysLocation()
    {
        return SafeCheckFileExists(ActualConsoleKeysFilePath);
    }

    private void OpenConsoleKeysLocation()
    {
        _fileLocationOpenerService.OpenFileLocationSafe(ActualConsoleKeysFilePath);
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

    private void ApplySettings()
    {
        _appSettingsManager.Load(EditedSettings);
        this.Window?.Close();
    }

    private void CancelSettings()
    {
        this.Window?.Close();
    }

    private void ResetSettings()
    {
        InitializeFromSettings(_appSettingsManager.GetDefault());
    }
}
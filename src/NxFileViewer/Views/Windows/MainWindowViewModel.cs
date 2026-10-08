using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Emignatik.NxFileViewer.Models;
using Emignatik.NxFileViewer.Commands;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Logging;
using Emignatik.NxFileViewer.Services.BackgroundTask;
using Emignatik.NxFileViewer.Services.FileOpening;
using Emignatik.NxFileViewer.Services.KeysManagement;
using Emignatik.NxFileViewer.Settings;
using Emignatik.NxFileViewer.Utils.MVVM;
using Emignatik.NxFileViewer.Utils.MVVM.BindingExtensions.DragAndDrop;
using Emignatik.NxFileViewer.Views.UserControls;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Emignatik.NxFileViewer.Services.Nsz;

namespace Emignatik.NxFileViewer.Views.Windows;

public class MainWindowViewModel : WindowViewModelBase, IFilesDropped
{
    public bool HasBundledRuntime => Emignatik.NxFileViewer.Services.Updates.ViewerDistribution.IsSelfContained;
    public string ProgramArchitecture => System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture.ToString().ToLowerInvariant();
    private readonly ILogger _logger;
    private readonly IKeySetProviderService _keySetProviderService;
    private readonly IAppSettings _appSettings;

    private OpenedFileViewModel? _openedFile;
    private readonly ConditionalWeakTable<NxFile, OpenedFileViewModel> _openedViews = new();
    private readonly string _appNameAndVersion;
    private string _title = "";
    private readonly IFileOpeningService _fileOpeningService;
    private bool _errorAnimationEnabled;
    private string? _selectedArchiveEntry;
    public System.Collections.Generic.IReadOnlyList<string> ArchiveEntries => _fileOpeningService.OpenedFile?.ArchiveEntries ?? Array.Empty<string>();
    public bool HasArchiveEntries => ArchiveEntries.Count > 0;
    public string? SelectedArchiveEntry
    {
        get => _selectedArchiveEntry;
        set
        {
            if (_selectedArchiveEntry == value || value == null || BackgroundTaskRunner.IsRunning) return;
            var archive = _fileOpeningService.OpenedFile?.ArchivePath;
            if (archive != null) _ = _fileOpeningService.SafeOpenFile(Emignatik.NxFileViewer.FileLoading.PackageZip.MemberPath(archive, value));
        }
    }

    public MainWindowViewModel(
        ILoggerFactory loggerFactory,
        IFileOpeningService fileOpeningService,
        IOpenFileCommand openFileCommand,
        IOpenLastFileCommand openLastFileCommand,
        ICloseFileCommand closeFileCommand,
        IExitAppCommand exitAppCommand,
        IShowSettingsWindowCommand showSettingsWindowCommand,
        IVerifyNcasIntegrityCommand verifyNcasIntegrityCommand,
        IShowBatchIntegrityWindowCommand showBatchIntegrityWindowCommand,
        ILoadKeysCommand loadKeysCommand,
        IOpenTitleWebPageCommand openTitleWebPageCommand,
        IServiceProvider serviceProvider,
        ILogSource logSource,
        IMainBackgroundTaskRunnerService backgroundTaskRunnerService,
        IShowRenameToolWindowCommand showRenameToolWindowCommand,
        IKeySetProviderService keySetProviderService,
        IAppSettings appSettings)
    {
        _logger = (loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory))).CreateLogger(this.GetType());
        _keySetProviderService = keySetProviderService ?? throw new ArgumentNullException(nameof(keySetProviderService));
        _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
        _fileOpeningService = fileOpeningService ?? throw new ArgumentNullException(nameof(fileOpeningService));
        OpenFileCommand = openFileCommand ?? throw new ArgumentNullException(nameof(openFileCommand));
        ExitAppCommand = exitAppCommand ?? throw new ArgumentNullException(nameof(exitAppCommand));
        ShowSettingsWindowCommand = showSettingsWindowCommand ?? throw new ArgumentNullException(nameof(showSettingsWindowCommand));
        VerifyNcasIntegrityCommand = verifyNcasIntegrityCommand ?? throw new ArgumentNullException(nameof(verifyNcasIntegrityCommand));
        ShowBatchIntegrityWindowCommand = showBatchIntegrityWindowCommand ?? throw new ArgumentNullException(nameof(showBatchIntegrityWindowCommand));
        LoadKeysCommand = loadKeysCommand ?? throw new ArgumentNullException(nameof(loadKeysCommand));
        ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        LogSource = logSource ?? throw new ArgumentNullException(nameof(logSource));
        BackgroundTaskRunner = backgroundTaskRunnerService ?? throw new ArgumentNullException(nameof(backgroundTaskRunnerService));
        OpenLastFileCommand = openLastFileCommand ?? throw new ArgumentNullException(nameof(openLastFileCommand));
        CloseFileCommand = closeFileCommand ?? throw new ArgumentNullException(nameof(closeFileCommand));
        OpenTitleWebPageCommand = openTitleWebPageCommand ?? throw new ArgumentNullException(nameof(openTitleWebPageCommand));
        ShowRenameToolWindowCommand = showRenameToolWindowCommand ?? throw new ArgumentNullException(nameof(showRenameToolWindowCommand));

        var assemblyName = Assembly.GetExecutingAssembly().GetName();
        var assemblyVersion = (assemblyName.Version ?? new Version());
        var displayVersion = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion.Split('+')[0] ?? $"{assemblyVersion.Major}.{assemblyVersion.Minor}.{assemblyVersion.Build}";
        _appNameAndVersion = $"{assemblyName.Name} v{displayVersion}";

        UpdateTitle();
        _fileOpeningService.OpenedFileChanged += OnFileOpeningChanged;
        _keySetProviderService.PropertyChanged += OnKeySetProviderPropertyChanged;
        LogSource.Log += OnLog;
    }

    private IServiceProvider ServiceProvider { get; }
    public Emignatik.NxFileViewer.Services.Updates.ViewerUpdateActions ViewerUpdates => ServiceProvider.GetRequiredService<Emignatik.NxFileViewer.Services.Updates.ViewerUpdateActions>();

    public Emignatik.NxFileViewer.Services.Nand.NandPluginActions NandPlugin => ServiceProvider.GetRequiredService<Emignatik.NxFileViewer.Services.Nand.NandPluginActions>();
    public NszActions Nsz => ServiceProvider.GetRequiredService<NszActions>();

    public IOpenFileCommand OpenFileCommand { get; }

    public IExitAppCommand ExitAppCommand { get; }

    public IShowSettingsWindowCommand ShowSettingsWindowCommand { get; }

    public IVerifyNcasIntegrityCommand VerifyNcasIntegrityCommand { get; }

    public IShowBatchIntegrityWindowCommand ShowBatchIntegrityWindowCommand { get; }

    public ILoadKeysCommand LoadKeysCommand { get; }

    public IOpenLastFileCommand OpenLastFileCommand { get; }

    public ICloseFileCommand CloseFileCommand { get; }

    public IOpenTitleWebPageCommand OpenTitleWebPageCommand { get; }

    public IShowRenameToolWindowCommand ShowRenameToolWindowCommand { get; }

    public bool NoProdKeysLoaded => _keySetProviderService.ActualProdKeysFilePath == null;

    public bool HasProdKeysProblems => NoProdKeysLoaded || !_keySetProviderService.ProdKeysValidation.IsValid || _keySetProviderService.ProdKeysValidation.HasWarnings;

    public string ProdKeysValidationSummary => SettingsWindowViewModel.BuildValidationSummary(_keySetProviderService.ProdKeysValidation);

    public string ProgramVersion => _appNameAndVersion;

    public string Title
    {
        get => _title;
        set
        {
            _title = value;
            NotifyPropertyChanged();
        }
    }

    public OpenedFileViewModel? OpenedFile
    {
        get => _openedFile;
        set
        {
            _openedFile = value;
            NotifyPropertyChanged();
        }
    }

    public ILogSource LogSource { get; }

    public IMainBackgroundTaskRunnerService BackgroundTaskRunner { get; }

    public bool ErrorAnimationEnabled
    {
        get => _errorAnimationEnabled;
        set
        {
            _errorAnimationEnabled = value;
            NotifyPropertyChanged();
        }
    }

    private void OnLog(LogLevel logLevel, string message)
    {
        if (logLevel == LogLevel.Error)
        {
            this.ErrorAnimationEnabled = true;
        }
    }

    private void OnFileOpeningChanged(object sender, OpenedFileChangedHandlerArgs args)
    {
        var newFile = args.NewFile;
        OpenedFile = newFile != null ? _openedViews.GetValue(newFile, file => new OpenedFileViewModel(file, ServiceProvider)) : null;
        _selectedArchiveEntry = newFile?.ArchiveEntry;
        NotifyPropertyChanged(nameof(ArchiveEntries));
        NotifyPropertyChanged(nameof(HasArchiveEntries));
        NotifyPropertyChanged(nameof(SelectedArchiveEntry));
        UpdateTitle();
        if (newFile != null) (System.Windows.Application.Current.MainWindow as MainWindow)?.Navigate(WorkspaceSection.File);
    }

    private void OnKeySetProviderPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(IKeySetProviderService.ActualProdKeysFilePath))
        {
            NotifyPropertyChanged(nameof(NoProdKeysLoaded));
        }
        if (string.IsNullOrEmpty(e.PropertyName) || e.PropertyName == nameof(IKeySetProviderService.ActualProdKeysFilePath) ||
            e.PropertyName == nameof(IKeySetProviderService.ProdKeysValidation))
        {
            NotifyPropertyChanged(nameof(HasProdKeysProblems));
            NotifyPropertyChanged(nameof(ProdKeysValidationSummary));
        }
    }

    public void OnFilesDropped(string[] files)
    {
        if (files.Length < 1)
            return;

        if (files.Length > 1)
            _logger.LogWarning(LocalizationManager.Instance.Current.Keys.MultipleFilesDragAndDropNotSupported);

        var filePath = files.First();
        var fileName = Path.GetFileName(filePath);
        if (string.Equals(fileName, IKeySetProviderService.DEFAULT_PROD_KEYS_FILE_NAME, StringComparison.OrdinalIgnoreCase))
            _appSettings.ProdKeysFilePath = filePath;
        else if (string.Equals(fileName, IKeySetProviderService.DEFAULT_TITLE_KEYS_FILE_NAME, StringComparison.OrdinalIgnoreCase))
            _appSettings.TitleKeysFilePath = filePath;
        else
            _fileOpeningService.SafeOpenFile(filePath);

    }

    private void UpdateTitle()
    {
        var openedFile = _fileOpeningService.OpenedFile;
        if (openedFile == null)
            Title = _appNameAndVersion;
        else
            Title = $"{_appNameAndVersion} - {openedFile.FileName}";
    }

}

using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Data;
using Emignatik.NxFileViewer.Services.BackgroundTask;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Services.BackgroundTask.RunnableImpl;
using Emignatik.NxFileViewer.Services.Integrity;
using Emignatik.NxFileViewer.Models.Overview;
using Emignatik.NxFileViewer.Services.Prompting;
using Emignatik.NxFileViewer.Utils.MVVM;
using Emignatik.NxFileViewer.Utils.MVVM.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Emignatik.NxFileViewer.Settings;
using Emignatik.NxFileViewer.Models;
using Emignatik.NxFileViewer.Views.UserControls;
using System.Windows;
using System.Threading;
using System.Threading.Tasks;
using Emignatik.NxFileViewer.FileLoading;
using Emignatik.NxFileViewer.Services.Nsz;
using Emignatik.NxFileViewer.Services.FileRenaming;
using Emignatik.NxFileViewer.Services.FileRenaming.Models;

namespace Emignatik.NxFileViewer.Views.Windows;

public sealed class BatchIntegrityWindowViewModel : WindowViewModelBase
{
    private readonly IPromptService _promptService;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<BatchIntegrityWindowViewModel> _logger;
    private string _inputDirectory = "";
    private readonly BatchHistoryStore _historyStore = new(BatchHistoryStore.DefaultPath);
    private readonly Dictionary<string, string> _fingerprints = new(StringComparer.OrdinalIgnoreCase);
    private BatchHistoryEntry? _session;
    private DateTime _lastHistorySave;
    private readonly IAppSettings _appSettings;
    public bool IsHistoryEnabled => _appSettings.EnableBatchHistory;
    public ObservableCollection<BatchHistoryEntry> History { get; } = new();
    private BatchHistoryEntry? _selectedHistory;
    public BatchHistoryEntry? SelectedHistory
    {
        get => _selectedHistory;
        set { _selectedHistory = value; NotifyPropertyChanged(); ResumeHistoryCommand?.TriggerCanExecuteChanged(); LoadHistoryCommand?.TriggerCanExecuteChanged(); }
    }
    public RelayCommand ResumeHistoryCommand { get; }
    public RelayCommand LoadHistoryCommand { get; }
    private bool _showOnlyErrors;
    private NxFile? _previewFile;
    private bool _closed;
    // Overview models contain metadata already read by the checker; retaining them
    // does not require retaining the open NxFile or its decoded NCZ cache.
    private readonly Dictionary<string, FileOverviewViewModel> _completedPreviews = new(StringComparer.OrdinalIgnoreCase);
    private FileOverviewViewModel? _previewOverview;

    public BatchIntegrityWindowViewModel(IPromptService promptService, IServiceProvider serviceProvider,
        IMainBackgroundTaskRunnerService backgroundTaskRunner, ILogger<BatchIntegrityWindowViewModel> logger,
        IAppSettings appSettings)
    {
        _appSettings = appSettings;
        _promptService = promptService;
        _serviceProvider = serviceProvider;
        _logger = logger;
        BackgroundTask = backgroundTaskRunner;
        ResumeHistoryCommand = new RelayCommand(ResumeHistory, () => IsHistoryEnabled && !BackgroundTask.IsRunning && SelectedHistory != null);
        LoadHistoryCommand = new RelayCommand(() => RestoreHistory(), () => IsHistoryEnabled && !BackgroundTask.IsRunning && SelectedHistory != null);
        ReloadHistory();
        ResultsView = CollectionViewSource.GetDefaultView(Results);
        ResultsView.Filter = ShouldDisplayResult;
        BrowseCommand = new RelayCommand(Browse);
        BrowseFirmwareZipCommand = new RelayCommand(() =>
        {
            var dialog = new Microsoft.Win32.OpenFileDialog { Filter = "ZIP / 7z (*.zip;*.7z)|*.zip;*.7z", Title = LocalizationManager.Instance.Current.Keys.BatchIntegrity_Title };
            if (dialog.ShowDialog() == true) InputDirectory = dialog.FileName;
        });
        StartCommand = new RelayCommand(() => Start(false), CanStart);
        ScanAndVerifyCommand = new RelayCommand(() => Start(true, rescan: true), CanStart);
        VerifyAllCommand = new RelayCommand(() => Start(true), () => Results.Count > 0 && !BackgroundTask.IsRunning);
        CheckNamingCommand = new RelayCommand(() => ProcessNaming(false), CanProcessNaming);
        RenameAllCommand = new RelayCommand(() => ProcessNaming(true), CanProcessNaming);
        PropertyChangedEventManager.AddHandler(appSettings.RenamingOptions, OnNamingSettingsChanged, string.Empty);
        ExportCommand = new RelayCommand(Export, () => Results.Count > 0 && !BackgroundTask.IsRunning);
        MoveValidCommand = new RelayCommand(MoveValid, CanMoveValid);
        CompressValidCommand = new RelayCommand(() => ConvertValid(NszOperation.Compress), () => CanConvertValid(NszOperation.Compress));
        DecompressValidCommand = new RelayCommand(() => ConvertValid(NszOperation.Decompress), () => CanConvertValid(NszOperation.Decompress));
        _inputDirectory = Directory.Exists(appSettings.LastUsedDir) ? appSettings.LastUsedDir : "";
        BackgroundTask.PropertyChanged += BackgroundTaskOnPropertyChanged;
        PropertyChangedEventManager.AddHandler(appSettings, OnHistorySettingChanged, nameof(IAppSettings.EnableBatchHistory));
    }

    public IMainBackgroundTaskRunnerService BackgroundTask { get; }
    public ObservableCollection<BatchIntegrityResult> Results { get; } = new();
    public ICollectionView ResultsView { get; }
    public RelayCommand BrowseFirmwareZipCommand { get; }
    private BatchIntegrityResult? _selectedResult;
    public BatchIntegrityResult? SelectedResult
    {
        get => _selectedResult;
        set
        {
            if (ReferenceEquals(_selectedResult, value)) return;
            _selectedResult = value;
            SelectedDetailsTabIndex = value?.IsNand == true ? 2 : value?.IsFirmware == true ? 1 : 0;
            NotifyPropertyChanged();
            UpdateSelectedPreview();
        }
    }
    private int _selectedDetailsTabIndex;
    public int SelectedDetailsTabIndex
    {
        get => _selectedDetailsTabIndex;
        set
        {
            if (_selectedDetailsTabIndex == value) return;
            _selectedDetailsTabIndex = value;
            NotifyPropertyChanged();
        }
    }
    public RelayCommand BrowseCommand { get; }
    public RelayCommand StartCommand { get; }
    public RelayCommand ScanAndVerifyCommand { get; }
    public RelayCommand VerifyAllCommand { get; }
    public bool IncludeArchives { get; set; } = true;
    public RelayCommand CheckNamingCommand { get; }
    public RelayCommand RenameAllCommand { get; }
    public RelayCommand ExportCommand { get; }
    public RelayCommand MoveValidCommand { get; }
    public RelayCommand CompressValidCommand { get; }
    public RelayCommand DecompressValidCommand { get; }
    public bool IncludeSubdirectories { get; set; } = true;
    public bool ShowStoredOverview => !_closed && PreviewOverview == null && SelectedResult is { IsFirmware: false, IsNand: false };
    public IReadOnlyList<KeyValuePair<string, string>> StoredOverviewFields => ShowStoredOverview
        ? BatchStoredOverview.Build(SelectedResult!, LocalizationManager.Instance.Current.Keys) : Array.Empty<KeyValuePair<string, string>>();
    public FileOverviewViewModel? PreviewOverview
    {
        get => _previewOverview;
        private set { _previewOverview = value; NotifyPropertyChanged(); NotifyPropertyChanged(nameof(ShowStoredOverview)); NotifyPropertyChanged(nameof(StoredOverviewFields)); }
    }

    private string _searchText = "";
    private string _fileTypeFilter = "";
    private string _integrityFilter = "";
    private string _namingFilter = "";
    public string NamingFilter { get => _namingFilter; set { _namingFilter = value ?? ""; NotifyPropertyChanged(); ResultsView.Refresh(); } }
    public IReadOnlyList<BatchNamingFilterOption> NamingFilters { get; } = new[] { "", "Matches", "Differs", "Unchecked", "Error" }.Select(value => new BatchNamingFilterOption(value)).ToArray();
    public string SearchText { get => _searchText; set { _searchText = value ?? ""; NotifyPropertyChanged(); ResultsView.Refresh(); } }
    public string FileTypeFilter { get => _fileTypeFilter; set { _fileTypeFilter = value ?? ""; NotifyPropertyChanged(); ResultsView.Refresh(); } }
    public string IntegrityFilter { get => _integrityFilter; set { _integrityFilter = value ?? ""; NotifyPropertyChanged(); ResultsView.Refresh(); } }
    public IReadOnlyList<BatchFilterOption> FileTypeFilters { get; } = new[] { "", "NSP", "NSZ", "XCI", "XCZ", "NAND", "ZIP", "7Z", "Folder" }.Select(value => new BatchFilterOption(value)).ToArray();
    public IReadOnlyList<BatchFilterOption> IntegrityFilters { get; } = new[] { new BatchFilterOption("") }.Concat(Enum.GetValues<NcasIntegrity>().Select(value => new BatchFilterOption(value.ToString()))).ToArray();
    public RelayCommand ResetFiltersCommand => new(() => { SearchText = ""; FileTypeFilter = ""; IntegrityFilter = ""; NamingFilter = ""; ShowOnlyErrors = false; });
    public bool ShowOnlyErrors
    {
        get => _showOnlyErrors;
        set
        {
            _showOnlyErrors = value;
            NotifyPropertyChanged();
            ResultsView.Refresh();
        }
    }

    public string InputDirectory
    {
        get => _inputDirectory;
        set { _inputDirectory = value; NotifyPropertyChanged(); StartCommand.TriggerCanExecuteChanged(); ScanAndVerifyCommand.TriggerCanExecuteChanged(); }
    }

    private void Browse()
    {
        var directory = _promptService.PromptSelectDir(LocalizationManager.Instance.Current.Keys.BatchIntegrity_SelectDirectory);
        if (directory != null) InputDirectory = directory;
    }

    private bool CanStart() => (Directory.Exists(InputDirectory) || (File.Exists(InputDirectory) && PackageZip.IsArchive(InputDirectory))) && !BackgroundTask.IsRunning;

    private async void Start(bool verifyIntegrity, bool rescan = false, bool resume = false)
    {
        var paths = verifyIntegrity && !rescan && !resume ? Results.Select(r => r.FilePath).ToArray() : null;
        if (!resume)
        {
            _fingerprints.Clear();
            _session = new BatchHistoryEntry(Guid.NewGuid(), DateTime.UtcNow, InputDirectory, IncludeSubdirectories, IncludeArchives,
                verifyIntegrity, _appSettings.IgnoreMissingDeltaFragments, "Running", Array.Empty<BatchIntegrityResult>(), new());
        }
        if (!IsHistoryEnabled) _session = null;
        var checkpoint = _session;
        var state = "Completed";
        if (!resume && (!verifyIntegrity || rescan))
        {
            Results.Clear();
            _completedPreviews.Clear();
        }
        SelectedResult = null;
        ClearPreview();
        ExportCommand.TriggerCanExecuteChanged();
        try
        {
            var runnable = _serviceProvider.GetRequiredService<IVerifyDirectoryIntegrityRunnable>()
                .Setup(InputDirectory, IncludeSubdirectories, ShowPreview, ShowCompletedResult, IncludeArchives, verifyIntegrity, paths,
                    resume ? path => checkpoint?.CanSkip(path, _appSettings.IgnoreMissingDeltaFragments) == true : null);
            await BackgroundTask.RunAsync(runnable);
        }
        catch (OperationCanceledException) { state = "Interrupted"; }
        catch (Exception ex) { state = "Interrupted"; _logger.LogError(ex, "Batch integrity check failed."); }
        finally
        {
            SaveHistory(state, true);
            if (_closed) ClearPreview();
            VerifyAllCommand.TriggerCanExecuteChanged();
            ExportCommand.TriggerCanExecuteChanged();
            MoveValidCommand.TriggerCanExecuteChanged();
            CheckNamingCommand.TriggerCanExecuteChanged();
            RenameAllCommand.TriggerCanExecuteChanged();
        }
    }

    private void OnHistorySettingChanged(object? sender, PropertyChangedEventArgs args)
    {
        NotifyPropertyChanged(nameof(IsHistoryEnabled));
        if (!IsHistoryEnabled) { History.Clear(); SelectedHistory = null; _session = null; _fingerprints.Clear(); }
        else ReloadHistory();
    }
    private void ReloadHistory()
    {
        if (!IsHistoryEnabled) return;
        try
        {
            var selectedId = SelectedHistory?.Id;
            History.Clear();
            foreach (var entry in _historyStore.Load()) History.Add(entry);
            SelectedHistory = History.FirstOrDefault(e => e.Id == selectedId) ?? History.FirstOrDefault();
            NotifyPropertyChanged(nameof(SelectedHistory));
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or System.Text.Json.JsonException)
        { _logger.LogWarning(ex, "Could not load batch history."); }
    }
    private void SaveHistory(string state, bool force = false)
    {
        if (!IsHistoryEnabled || _session == null || (!force && DateTime.UtcNow - _lastHistorySave < TimeSpan.FromSeconds(1))) return;
        try
        {
            _session = _session with { UpdatedUtc = DateTime.UtcNow, State = state, IgnoreMissingDeltaFragments = _appSettings.IgnoreMissingDeltaFragments, Results = Results.ToArray(),
                Fingerprints = new Dictionary<string, string>(_fingerprints, StringComparer.OrdinalIgnoreCase) };
            _historyStore.Save(_session);
            _lastHistorySave = DateTime.UtcNow;
            ReloadHistory();
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or System.Text.Json.JsonException)
        { _logger.LogWarning(ex, "Could not save batch history."); }
    }
    private void ResumeHistory()
    {
        if (SelectedHistory == null || (!Directory.Exists(SelectedHistory.Source) && !File.Exists(SelectedHistory.Source))) return;
        if (RestoreHistory(true)) Start(_session!.VerifyIntegrity, resume: true);
    }
    private bool RestoreHistory(bool prepareResume = false)
    {
        if (!IsHistoryEnabled || BackgroundTask.IsRunning || SelectedHistory == null) return false;
        var saved = SelectedHistory;

        InputDirectory = saved.Source;
        IncludeArchives = saved.IncludeArchives; IncludeSubdirectories = saved.IncludeSubdirectories;
        NotifyPropertyChanged(nameof(IncludeArchives)); NotifyPropertyChanged(nameof(IncludeSubdirectories));
        SelectedResult = null;
        ClearPreview(); _completedPreviews.Clear(); Results.Clear(); _fingerprints.Clear();
        foreach (var result in saved.Results)
            Results.Add(!prepareResume || saved.CanSkip(result.FilePath, _appSettings.IgnoreMissingDeltaFragments) ? result :
                result with { Integrity = NcasIntegrity.Unchecked });
        foreach (var pair in saved.Fingerprints) _fingerprints[pair.Key] = pair.Value;
        _session = saved;
        SelectedResult = ResultsView.Cast<BatchIntegrityResult>().FirstOrDefault();
        ExportCommand.TriggerCanExecuteChanged(); VerifyAllCommand.TriggerCanExecuteChanged();
        MoveValidCommand.TriggerCanExecuteChanged();
        return true;
    }
    private void ShowPreview(NxFile file)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            _previewFile?.Dispose();
            _previewFile = file;
            if (_closed) return;
            // Follow the package being read, while retaining completed snapshots
            // for manual selection after the scan.
            _completedPreviews[file.FilePath] = new FileOverviewViewModel(file.Overview, _serviceProvider);
            SelectedResult = Results.FirstOrDefault(result => result.FilePath.Equals(file.FilePath, StringComparison.OrdinalIgnoreCase));
            SelectedDetailsTabIndex = 0;
            PreviewOverview = _completedPreviews[file.FilePath];
        });
    }

    private void ShowCompletedResult(BatchIntegrityResult result)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            // The cached model observes integrity changes from the verifier.
            // Publish it before adding a row, which can trigger selection bindings.
            if (!result.IsFirmware && !result.IsNand && _completedPreviews.TryGetValue(result.FilePath, out var completed) &&
                SelectedResult?.FilePath.Equals(result.FilePath, StringComparison.OrdinalIgnoreCase) == true)
                PreviewOverview = completed;
            if (!result.IsFirmware && !result.IsNand && _completedPreviews.TryGetValue(result.FilePath, out var overview))
            {
                var packages = overview.CnmtContainers;
                string Join(Func<CnmtContainerViewModel, string?> selector) => string.Join(" / ",
                    packages.Select(selector).Where(value => !string.IsNullOrWhiteSpace(value)).Distinct());
                result = result with
                {
                    Title = Join(package => package.Titles.FirstOrDefault()?.AppName),
                    TitleId = Join(package => package.TitleId),
                    Publisher = Join(package => package.Titles.FirstOrDefault()?.Publisher),
                    Version = Join(package => package.TitleVersion),
                    DisplayVersion = Join(package => package.DisplayVersion),
                    SystemVersion = Join(package => package.MinimumSystemVersion),
                    MasterKey = Join(package => package.MasterKey),
                    BuildId = Join(package => package.BuildID),
                    Distribution = Join(package => package.Distribution),
                    Languages = string.Join(", ", packages.SelectMany(package => package.Titles).Select(title => title.Language.ToString()).Distinct()),
                    FileSize = _previewFile?.FilePath == result.FilePath ? _previewFile.Overview.FileSize : null,
                    CompressionRatio = _previewFile?.FilePath == result.FilePath ? _previewFile.Overview.CompressionRatio : null
                };
            }
            var existing = Results.FirstOrDefault(r => r.FilePath.Equals(result.FilePath, StringComparison.OrdinalIgnoreCase));
            if (existing != null) Results[Results.IndexOf(existing)] = result;
            else Results.Add(result);
            var fingerprint = result.SourceFingerprint;
            if (fingerprint != null) _fingerprints[result.FilePath] = fingerprint;
            else _fingerprints.Remove(result.FilePath);
            SaveHistory("Running");
            SelectedResult = result;
            ExportCommand.TriggerCanExecuteChanged();
            MoveValidCommand.TriggerCanExecuteChanged();
            CheckNamingCommand.TriggerCanExecuteChanged();
            RenameAllCommand.TriggerCanExecuteChanged();
        });
    }

    private void ClearPreview()
    {
        _previewFile?.Dispose();
        _previewFile = null;
        PreviewOverview = null;
    }

    public void ClosePreview()
    {
        _closed = true;
        _completedPreviews.Clear();
        BackgroundTask.PropertyChanged -= BackgroundTaskOnPropertyChanged;
        // The checking file remains in use until the verifier task ends.
        if (!BackgroundTask.IsRunning) ClearPreview();
    }

    private void UpdateSelectedPreview()
    {
        PreviewOverview = null;
        var selected = SelectedResult;
        if (_closed || selected == null || selected.IsFirmware || selected.IsNand) return;
        // Completed results are snapshots. Selection must never reopen a package,
        // including when its source was moved/deleted after the check.
        if (_completedPreviews.TryGetValue(selected.FilePath, out var completed))
            PreviewOverview = completed;
    }
    public bool CanActOnFile(BatchIntegrityResult file, NszOperation? operation = null) => !BackgroundTask.IsRunning &&
        !file.IsFirmware && !file.IsNand && !file.HasMissingKeys && File.Exists(file.FilePath) &&
        (operation == null || PackageConversionService.Supports(file.FilePath, operation.Value));
    public bool CanCheckNaming(BatchIntegrityResult file) => !BackgroundTask.IsRunning &&
        !file.IsFirmware && !file.IsNand && !file.HasMissingKeys &&
        File.Exists(file.FilePath) && Path.GetExtension(file.FilePath).ToLowerInvariant() is ".nsp" or ".nsz" or ".xci" or ".xcz";
    private bool CanProcessNaming() => Results.Any(CanCheckNaming);
    public void CheckSelectedNaming(BatchIntegrityResult file) => ProcessNaming(false, file);
    public void RenameSelected(BatchIntegrityResult file) => ProcessNaming(true, file);

    private void OnNamingSettingsChanged(object? sender, PropertyChangedEventArgs e)
    {
        foreach (var result in Results.ToArray())
            if (result.NamingMatches != null || result.NamingError != null)
                Results[Results.IndexOf(result)] = result with { NamingMatches = null, ProposedPath = null, NamingError = null };
    }

    private async void ProcessNaming(bool rename, BatchIntegrityResult? selected = null)
    {
        if (BackgroundTask.IsRunning) return;
        var files = (selected == null ? Results.AsEnumerable() : new[] { selected }).Where(CanCheckNaming).ToArray();
        if (files.Length == 0) return;
        RenamingResult? selectedOutcome = null;
        try
        {
            var options = _appSettings.RenamingOptions;
            var parser = _serviceProvider.GetRequiredService<INamingPatternsParser>();
            var settings = new NamingSettings
            {
                ApplicationPattern = parser.ParseApplicationPattern(options.ApplicationPattern),
                PatchPattern = parser.ParsePatchPattern(options.PatchPattern),
                AddonPattern = parser.ParseAddonPattern(options.AddonPattern),
                TargetDirectory = options.TargetDirectory,
                InvalidFileNameCharsReplacement = options.InvalidFileNameCharsReplacement,
                ReplaceWhiteSpaceChars = options.ReplaceWhiteSpaceChars,
                WhiteSpaceCharsReplacement = options.WhiteSpaceCharsReplacement
            };
            var autoClose = options.AutoCloseOpenedFile;
            var service = _serviceProvider.GetRequiredService<IFileRenamerService>();
            var runnable = new RunnableRelay((progress, token) =>
            {
                for (var i = 0; i < files.Length; i++)
                {
                    token.ThrowIfCancellationRequested();
                    var original = files[i];
                    progress.SetText(original.FileName);
                    var outcome = service.RenameFileAsync(original.FilePath, autoClose, settings, true, _logger, token).GetAwaiter().GetResult();
                    token.ThrowIfCancellationRequested();
                    if (rename && outcome.Exception == null && outcome.IsRenamed)
                    {
                        if (selected != null)
                        {
                            var confirmed = Application.Current.Dispatcher.Invoke(() =>
                                ThemedDialog.Confirm(LocalizationManager.Instance.Current.Keys.RenamingTool_Button_Rename,
                                    NamingOutcomeMessage(outcome), allowNo: false) == MessageBoxResult.Yes);
                            if (!confirmed) throw new OperationCanceledException();
                            token.ThrowIfCancellationRequested();
                        }
                        outcome = service.RenameFileAsync(original.FilePath, autoClose, settings, false, _logger, token).GetAwaiter().GetResult();
                    }
                    if (selected != null) selectedOutcome = outcome;
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        var index = Results.IndexOf(original);
                        if (index < 0) return;
                        var moved = rename && outcome.Exception == null && outcome.IsRenamed;
                        var path = moved ? outcome.NewFilePath! : original.FilePath;
                        var updated = original with
                        {
                            FilePath = path,
                            NamingMatches = outcome.Exception != null ? null : rename || !outcome.IsRenamed,
                            ProposedPath = outcome.NewFilePath,
                            NamingError = outcome.Exception?.Message,
                            SourceFingerprint = moved ? BatchHistoryStore.Fingerprint(path) : original.SourceFingerprint
                        };
                        if (moved)
                        {
                            if (_completedPreviews.Remove(original.FilePath, out var preview)) _completedPreviews[path] = preview;
                            _fingerprints.Remove(original.FilePath);
                            if (updated.SourceFingerprint != null) _fingerprints[path] = updated.SourceFingerprint;
                        }
                        var wasSelected = ReferenceEquals(SelectedResult, original);
                        Results[index] = updated;
                        if (wasSelected) SelectedResult = updated;
                    });
                    progress.SetPercentage((i + 1d) / files.Length);
                }
            }) { SupportsCancellation = true, SupportProgress = true };
            await BackgroundTask.RunAsync(runnable);
            if (selectedOutcome != null)
                ThemedDialog.Notice(NamingOutcomeMessage(selectedOutcome),
                    LocalizationManager.Instance.Current.Keys.BatchNaming_Check,
                    selectedOutcome.Exception == null ? MessageBoxImage.Information : MessageBoxImage.Error);
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Batch naming operation failed.");
            if (selected != null)
                ThemedDialog.Notice(ex.Message, LocalizationManager.Instance.Current.Keys.BatchNaming_Error, MessageBoxImage.Error);
            foreach (var file in files)
            {
                var index = Results.IndexOf(file);
                if (index >= 0) Results[index] = file with { NamingMatches = null, NamingError = ex.Message };
            }
        }
        finally { SaveHistory(_session?.State ?? "Completed", force: true); }
    }

    private static string NamingOutcomeMessage(RenamingResult outcome)
    {
        var keys = LocalizationManager.Instance.Current.Keys;
        var status = outcome.Exception != null ? keys.BatchNaming_Error
            : outcome.IsSimulation ? (outcome.IsRenamed ? keys.BatchNaming_Differs : keys.BatchNaming_Matches)
            : outcome.Status;
        return $"{status}\n\n{keys.RenamingTool_OldName}:\n{outcome.OldFilePath}\n\n{keys.RenamingTool_NewName}:\n{outcome.NewFilePath ?? outcome.OldFilePath}"
            + (outcome.Exception == null ? "" : $"\n\n{outcome.Exception.Message}");
    }

    public bool CanOpenInSingle(BatchIntegrityResult file) => !BackgroundTask.IsRunning && File.Exists(PackageZip.ArchivePath(file.FilePath));
    public async void OpenInSingle(BatchIntegrityResult file)
    {
        if (!CanOpenInSingle(file)) return;
        ClearPreview();
        await _serviceProvider.GetRequiredService<Emignatik.NxFileViewer.Services.FileOpening.IFileOpeningService>().SafeOpenFile(file.FilePath);
    }
    public void ConvertSelected(BatchIntegrityResult file, NszOperation operation) => ConvertValid(operation, file);
    public void MoveSelected(BatchIntegrityResult file) => MoveFiles(file);
    public bool CanOpenTitle(BatchIntegrityResult file) => !BackgroundTask.IsRunning &&
        ulong.TryParse(file.TitleId.Split('/')[0].Trim(), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out _);
    public void OpenSelectedTitle(BatchIntegrityResult file)
    {
        if (!CanOpenTitle(file)) return;
        try { _serviceProvider.GetRequiredService<Emignatik.NxFileViewer.Services.OnlineServices.IOnlineTitlePageOpenerService>()
            .OpenTitlePage(file.TitleId.Split('/')[0].Trim()); }
        catch (Exception ex) { _logger.LogError(ex, "Failed to open title website."); }
    }
    public async void VerifySelected(BatchIntegrityResult file)
    {
        if (file.IsNand || BackgroundTask.IsRunning || (!Directory.Exists(file.FilePath) && !File.Exists(PackageZip.ArchivePath(file.FilePath)))) return;
        try
        {
            var runnable = _serviceProvider.GetRequiredService<IVerifyDirectoryIntegrityRunnable>()
                .Setup(InputDirectory, IncludeSubdirectories, ShowPreview, ShowCompletedResult, true, true, new[] { file.FilePath });
            await BackgroundTask.RunAsync(runnable);
        }
        catch (OperationCanceledException) { }
        catch (Exception ex) { _logger.LogError(ex, "Failed to verify selected file."); }
    }
    private bool CanMoveValid() => !BackgroundTask.IsRunning &&
        Results.Any(result => !result.IsFirmware && !result.IsNand && result.Integrity == NcasIntegrity.Original && File.Exists(result.FilePath));

    private bool CanConvertValid(NszOperation operation) => !BackgroundTask.IsRunning && Results.Any(result =>
        !result.IsFirmware && !result.IsNand && result.Integrity == NcasIntegrity.Original && File.Exists(result.FilePath) &&
        PackageConversionService.Supports(result.FilePath, operation));

    private async void ConvertValid(NszOperation operation, BatchIntegrityResult? selected = null)
    {
        if (selected == null ? !CanConvertValid(operation) : !CanActOnFile(selected, operation)) return;
        var paths = (selected == null ? Results.AsEnumerable() : new[] { selected }).Where(result => !result.IsFirmware && !result.IsNand && (selected != null || result.Integrity == NcasIntegrity.Original) &&
            File.Exists(result.FilePath) && PackageConversionService.Supports(result.FilePath, operation))
            .Select(result => result.FilePath).Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
        ClearPreview();
        var attempts = await _serviceProvider.GetRequiredService<NszActions>().ConvertFilesAsync(paths, operation, selected == null ? InputDirectory : null);
        foreach (var attempt in attempts)
        {
            var existing = Results.FirstOrDefault(result => result.FilePath == attempt.SourcePath);
            if (existing == null) continue;
            var result = attempt.Result;
            Results[Results.IndexOf(existing)] = existing with
            {
                ConversionStatus = result != null ? LocalizationManager.Instance.Current.Keys.Nsz_Verified +
                    (result.SourceDeleted ? "; " + LocalizationManager.Instance.Current.Keys.Nsz_SourceDeleted :
                    result.SourceDeletionError != null ? "; " + LocalizationManager.Instance.Current.Keys.Nsz_SourceDeleteFailed + ": " + result.SourceDeletionError : "") : attempt.Error,
                ConversionFailed = result == null || result.SourceDeletionError != null,
                ConvertedPath = result?.OutputPath, SourceSize = result?.SourceSize, OutputSize = result?.OutputSize
            };
            if (result != null && !Results.Any(row => row.FilePath.Equals(result.OutputPath, StringComparison.OrdinalIgnoreCase)))
                Results.Add(result.Verification with { ConversionStatus = LocalizationManager.Instance.Current.Keys.Nsz_Verified,
                    SourceSize = result.SourceSize, OutputSize = result.OutputSize });
        }
        ResultsView.Refresh();
    }

    private void MoveValid() => MoveFiles(null);
    private async void MoveFiles(BatchIntegrityResult? selected)
    {
        if (BackgroundTask.IsRunning || (selected != null && !CanActOnFile(selected))) return;
        var destinationRoot = _promptService.PromptSelectDir(
            LocalizationManager.Instance.Current.Keys.BatchIntegrity_SelectMoveDestination);
        if (destinationRoot == null) return;

        var sourceRoot = selected == null ? Path.GetFullPath(InputDirectory) : Path.GetDirectoryName(Path.GetFullPath(selected.FilePath))!;
        var destinationRootFull = Path.GetFullPath(destinationRoot);
        var opening = _serviceProvider.GetRequiredService<Emignatik.NxFileViewer.Services.FileOpening.IFileOpeningService>();
        if (opening.OpenedFile != null && (selected == null
            ? Results.Any(file => file.FilePath.Equals(opening.OpenedFile.FilePath, StringComparison.OrdinalIgnoreCase))
            : selected.FilePath.Equals(opening.OpenedFile.FilePath, StringComparison.OrdinalIgnoreCase))) opening.SafeClose();
        ClearPreview();
        var validFiles = (selected == null ? Results.AsEnumerable() : new[] { selected }).Where(result =>
            !result.IsFirmware && !result.IsNand && (selected != null || result.Integrity == NcasIntegrity.Original) && File.Exists(result.FilePath)).ToArray();
        var runnable = new RunnableRelay<int>((reporter, cancellationToken) =>
        {
            var moved = 0;
            for (var index = 0; index < validFiles.Length; index++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var source = validFiles[index].FilePath;
                reporter.SetText($"{LocalizationManager.Instance.Current.Keys.BatchIntegrity_Moving} {index + 1}/{validFiles.Length}: {Path.GetFileName(source)}");
                var relativePath = Path.GetRelativePath(sourceRoot, source);
                var destination = Path.GetFullPath(Path.Combine(destinationRootFull, relativePath));
                if (!destination.StartsWith(destinationRootFull + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
                    continue;
                Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
                if (!File.Exists(destination))
                {
                    File.Move(source, destination);
                    moved++;
                }
                reporter.SetPercentage((double)(index + 1) / validFiles.Length);
            }
            return moved;
        }) { SupportProgress = true, SupportsCancellation = true };

        try
        {
            await BackgroundTask.RunAsync(runnable);
            foreach (var movedResult in Results.Where(result => !result.IsFirmware && !result.IsNand && !File.Exists(result.FilePath)).ToArray())
                Results.Remove(movedResult);
        }
        catch (OperationCanceledException) { }
        catch (Exception ex) { _logger.LogError(ex, "Failed to move verified files."); }
        finally
        {
            ExportCommand.TriggerCanExecuteChanged();
            MoveValidCommand.TriggerCanExecuteChanged();
            CheckNamingCommand.TriggerCanExecuteChanged();
            RenameAllCommand.TriggerCanExecuteChanged();
        }
    }

    private void Export()
    {
        var path = _promptService.PromptSaveFile("integrity-results.csv", "Export integrity results", "CSV files (*.csv)|*.csv");
        if (path == null) return;
        var csv = new StringBuilder("File;FileType;PackageType;Structure;Compression;Integrity;Error;FirmwareDetails;Conversion;ConvertedPath;SourceBytes;OutputBytes;Title;TitleId;Publisher;Version;DisplayVersion;Firmware;MasterKey;BuildId;Distribution;Languages;FileBytes;CompressionRatio;Naming;ProposedPath;NamingError;IsNand;NandDetails\r\n");
        foreach (var result in ResultsView.Cast<BatchIntegrityResult>())
            csv.Append(Escape(result.FilePath)).Append(';')
                .Append(Escape(result.FileType)).Append(';')
                .Append(Escape(result.PackageType)).Append(';')
                .Append(Escape(result.Structure)).Append(';')
                .Append(Escape(result.Compression)).Append(';')
                .Append(Escape(result.Integrity.ToString())).Append(';')
                .Append(Escape(result.Error ?? "")).Append(';')
                .Append(Escape(result.FirmwareDetails ?? "")).Append(';')
                .Append(Escape(result.ConversionStatus ?? "")).Append(';')
                .Append(Escape(result.ConvertedPath ?? "")).Append(';')
                .Append(result.SourceSize).Append(';').Append(result.OutputSize).Append(';')
                .Append(string.Join(";", new[] { result.Title, result.TitleId, result.Publisher, result.Version, result.DisplayVersion,
                    result.SystemVersion, result.MasterKey, result.BuildId, result.Distribution, result.Languages }.Select(Escape)))
                .Append(';').Append(result.FileSize).Append(';')
                .Append(result.CompressionRatio?.ToString(System.Globalization.CultureInfo.InvariantCulture))
                .Append(';').Append(Escape(result.NamingStatus)).Append(';').Append(Escape(result.ProposedPath ?? ""))
                .Append(';').Append(Escape(result.NamingError ?? ""))
                .Append(';').Append(result.IsNand).Append(';').Append(Escape(result.NandDetails ?? "")).Append("\r\n");
        File.WriteAllText(path, csv.ToString(), new UTF8Encoding(true));
    }

    private static string Escape(string value) => $"\"{value.Replace("\"", "\"\"")}\"";

    private bool ShouldDisplayResult(object item) =>
        item is BatchIntegrityResult result && BatchResultFilter.Matches(result, SearchText, FileTypeFilter, IntegrityFilter, ShowOnlyErrors, NamingFilter);

    private void BackgroundTaskOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(IBackgroundTaskRunner.IsRunning)) return;
        ResumeHistoryCommand.TriggerCanExecuteChanged(true);
        LoadHistoryCommand.TriggerCanExecuteChanged(true);
        StartCommand.TriggerCanExecuteChanged(true);
        ScanAndVerifyCommand.TriggerCanExecuteChanged(true);
        VerifyAllCommand.TriggerCanExecuteChanged(true);
        CheckNamingCommand.TriggerCanExecuteChanged(true);
        RenameAllCommand.TriggerCanExecuteChanged(true);
        ExportCommand.TriggerCanExecuteChanged(true);
        MoveValidCommand.TriggerCanExecuteChanged(true);
        CompressValidCommand.TriggerCanExecuteChanged(true);
        DecompressValidCommand.TriggerCanExecuteChanged(true);
    }
}

public sealed record BatchFilterOption(string Value)
{
    public string Label => Value.Length == 0 ? LocalizationManager.Instance.Current.Keys.BatchTable_All : Value;
}

public sealed record BatchNamingFilterOption(string Value)
{
    public string Label => Value switch
    {
        "Matches" => LocalizationManager.Instance.Current.Keys.BatchNaming_Matches,
        "Differs" => LocalizationManager.Instance.Current.Keys.BatchNaming_Differs,
        "Error" => LocalizationManager.Instance.Current.Keys.BatchNaming_Error,
        "Unchecked" => LocalizationManager.Instance.Current.Keys.BatchNaming_Unchecked,
        _ => LocalizationManager.Instance.Current.Keys.BatchTable_All
    };
}

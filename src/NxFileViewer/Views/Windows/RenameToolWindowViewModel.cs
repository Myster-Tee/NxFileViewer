using System;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows;
using Emignatik.NxFileViewer.Services.FileRenaming.Models;
using Emignatik.NxFileViewer.Commands;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Logging;
using Emignatik.NxFileViewer.Services.BackgroundTask;
using Emignatik.NxFileViewer.Services.FileRenaming;
using Emignatik.NxFileViewer.Services.Prompting;
using Emignatik.NxFileViewer.Settings;
using Emignatik.NxFileViewer.Utils.MVVM;
using Emignatik.NxFileViewer.Utils.MVVM.BindingExtensions.DragAndDrop;
using Emignatik.NxFileViewer.Utils.MVVM.Commands;

namespace Emignatik.NxFileViewer.Views.Windows;

public class RenameToolWindowViewModel : WindowViewModelBase, IFilesDropped
{
    private readonly INamingPatternsParser _namingPatternsParser;
    private readonly IPromptService _promptService;

    private string? _applicationPatternError;
    private string? _patchPatternError;
    private string? _addonPatternError;
    private readonly LoggerSource _loggerSource = new();
    private readonly IAppSettings _appSettings;

    public RenameToolWindowViewModel(INamingPatternsParser namingPatternsParser, IAppSettings appSettings, IPromptService promptService, IRenameFilesCommand renameFilesCommand, IBackgroundTaskRunner backgroundTaskRunner)
    {
        _namingPatternsParser = namingPatternsParser ?? throw new ArgumentNullException(nameof(namingPatternsParser));
        _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
        _promptService = promptService ?? throw new ArgumentNullException(nameof(promptService));
        BackgroundTask = backgroundTaskRunner ?? throw new ArgumentNullException(nameof(backgroundTaskRunner));
        RenameCommand = renameFilesCommand ?? throw new ArgumentNullException(nameof(renameFilesCommand));
        RenameCommand.BackgroundTaskRunner = BackgroundTask;
        RenameCommand.Started += () => Results.Clear();
        RenameCommand.ResultReported += result => Application.Current.Dispatcher.Invoke(() => Results.Add(result));

        CancelCommand = new RelayCommand(Cancel);
        BrowseInputDirectoryCommand = new RelayCommand(BrowseInputDirectory);
        BrowseTargetDirectoryCommand = new RelayCommand(() =>
        {
            var selected = _promptService.PromptSelectDir(LocalizationManager.Instance.Current.Keys.RenamingTool_TargetDirectory);
            if (selected != null) TargetDirectory = selected;
        });

        _appSettings.RenamingOptions.PropertyChanged += OnRenamingOptionsPropertyChanged;

        UpdateApplicationPatternParts();
        UpdatePatchPatternParts();
        UpdateAddonPatternParts();
    }

    public ObservableCollection<RenamingResult> Results { get; } = new();

    public IBackgroundTaskRunner BackgroundTask { get; }

    public ILogSource LogSource => _loggerSource;

    public IRenameFilesCommand RenameCommand { get; }

    public RelayCommand CancelCommand { get; }

    public RelayCommand BrowseInputDirectoryCommand { get; }
    public RelayCommand BrowseTargetDirectoryCommand { get; }
    public string TargetDirectory
    {
        get => _appSettings.RenamingOptions.TargetDirectory;
        set { _appSettings.RenamingOptions.TargetDirectory = value; NotifyPropertyChanged(); }
    }

    public string ApplicationPattern
    {
        get => _appSettings.RenamingOptions.ApplicationPattern;
        set
        {
            _appSettings.RenamingOptions.ApplicationPattern = value;
            NotifyPropertyChanged();
        }
    }

    public string? ApplicationPatternError
    {
        get => _applicationPatternError;
        set
        {
            _applicationPatternError = value;
            NotifyPropertyChanged();
        }
    }

    public string PatchPattern
    {
        get => _appSettings.RenamingOptions.PatchPattern;
        set
        {
            _appSettings.RenamingOptions.PatchPattern = value;
            NotifyPropertyChanged();
        }
    }

    public string? PatchPatternError
    {
        get => _patchPatternError;
        set
        {
            _patchPatternError = value;
            NotifyPropertyChanged();
        }
    }

    public string AddonPattern
    {
        get => _appSettings.RenamingOptions.AddonPattern;
        set
        {
            _appSettings.RenamingOptions.AddonPattern = value;
            NotifyPropertyChanged();
        }
    }

    public string? AddonPatternError
    {
        get => _addonPatternError;
        set
        {
            _addonPatternError = value;
            NotifyPropertyChanged();
        }
    }

    private void OnRenamingOptionsPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(IRenamingOptions.TargetDirectory))
            NotifyPropertyChanged(nameof(TargetDirectory));
        if (e.PropertyName == nameof(IRenamingOptions.ApplicationPattern))
        {
            UpdateApplicationPatternParts();
            NotifyPropertyChanged(nameof(ApplicationPattern));
        }
        else if (e.PropertyName == nameof(IRenamingOptions.PatchPattern))
        {
            UpdatePatchPatternParts();
            NotifyPropertyChanged(nameof(PatchPattern));
        }
        else if (e.PropertyName == nameof(IRenamingOptions.AddonPattern))
        {
            UpdateAddonPatternParts();
            NotifyPropertyChanged(nameof(AddonPattern));
        }
    }

    private void UpdateApplicationPatternParts()
    {
        try
        {
            RenameCommand.ApplicationPatternParts = _namingPatternsParser.ParseApplicationPattern(this.ApplicationPattern);
            ApplicationPatternError = null;
        }
        catch (Exception ex)
        {
            RenameCommand.ApplicationPatternParts = null;
            ApplicationPatternError = ex.Message;
        }
    }

    private void UpdatePatchPatternParts()
    {
        try
        {
            RenameCommand.PatchPatternParts = _namingPatternsParser.ParsePatchPattern(this.PatchPattern);
            PatchPatternError = null;
        }
        catch (Exception ex)
        {
            RenameCommand.PatchPatternParts = null;
            PatchPatternError = ex.Message;
        }
    }

    private void UpdateAddonPatternParts()
    {
        try
        {
            RenameCommand.AddonPatternParts = _namingPatternsParser.ParseAddonPattern(this.AddonPattern);
            AddonPatternError = null;
        }
        catch (Exception ex)
        {
            RenameCommand.AddonPatternParts = null;
            AddonPatternError = ex.Message;
        }
    }

    private void BrowseInputDirectory()
    {
        var selectedDir = _promptService.PromptSelectDir(LocalizationManager.Instance.Current.Keys.RenamingTool_BrowseDirTitle);

        if (selectedDir != null)
            RenameCommand.InputPath = selectedDir;
    }

    public Action? EditingCompleted { get; set; }

    private void Cancel()
    {
        EditingCompleted?.Invoke();
    }

    public void OnFilesDropped(string[] files)
    {
        if (files.Length > 0)
            this.RenameCommand.InputPath = files[0];
    }
}

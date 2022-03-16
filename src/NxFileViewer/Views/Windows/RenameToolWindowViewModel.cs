using System;
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
        RenameCommand.Logger = _loggerSource;

        CancelCommand = new RelayCommand(Cancel);
        BrowseInputDirectoryCommand = new RelayCommand(BrowseInputDirectory);

        UpdateApplicationPatternParts();
        UpdatePatchPatternParts();
        UpdateAddonPatternParts();
    }

    public IBackgroundTaskRunner BackgroundTask { get; }

    public ILogSource LogSource => _loggerSource;

    public IRenameFilesCommand RenameCommand { get; }

    public RelayCommand CancelCommand { get; }

    public RelayCommand BrowseInputDirectoryCommand { get; }

    public string ApplicationPattern
    {
        get => _appSettings.ApplicationPattern;
        set
        {
            _appSettings.ApplicationPattern = value;

            UpdateApplicationPatternParts();
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

    public string InvalidWindowsCharsReplacement
    {
        get => RenameCommand.InvalidWindowsCharsReplacement;
        set
        {
            RenameCommand.InvalidWindowsCharsReplacement = value;
            NotifyPropertyChanged();
        }

    }

    public string PatchPattern
    {
        get => _appSettings.PatchPattern;
        set
        {
            _appSettings.PatchPattern = value;
            UpdatePatchPatternParts();
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
        get => _appSettings.AddonPattern;
        set
        {
            _appSettings.AddonPattern = value;
            UpdateAddonPatternParts();
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

    public bool IsSimulation
    {
        get => RenameCommand.IsSimulation;
        set
        {
            RenameCommand.IsSimulation = value;
            NotifyPropertyChanged();
        }
    }

    public bool ReplaceWhiteSpaceChars
    {
        get => RenameCommand.ReplaceWhiteSpaceChars;
        set
        {
            RenameCommand.ReplaceWhiteSpaceChars = value;
            NotifyPropertyChanged();
        }
    }

    public string WhiteSpaceCharsReplacement
    {
        get => RenameCommand.WhiteSpaceCharsReplacement;
        set
        {
            RenameCommand.WhiteSpaceCharsReplacement = value;
            NotifyPropertyChanged();
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

    private void Cancel()
    {
        this.Window?.Close();
    }

    public void OnFilesDropped(string[] files)
    {
        
    }
}
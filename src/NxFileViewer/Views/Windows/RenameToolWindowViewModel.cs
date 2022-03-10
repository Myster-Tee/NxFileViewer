using System;
using Emignatik.NxFileViewer.Commands;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Services.FileRenaming;
using Emignatik.NxFileViewer.Services.Prompting;
using Emignatik.NxFileViewer.Settings;
using Emignatik.NxFileViewer.Utils.MVVM;
using Emignatik.NxFileViewer.Utils.MVVM.Commands;

namespace Emignatik.NxFileViewer.Views.Windows;

public class RenameToolWindowViewModel : WindowViewModelBase
{
    private readonly INamingPatternsParser _namingPatternsParser;
    private readonly IAppSettingsManager _appSettingsManager;
    private readonly IPromptService _promptService;

    private string? _applicationPatternError;
    private string? _patchPatternError;
    private string? _addonPatternError;

    public RenameToolWindowViewModel(INamingPatternsParser namingPatternsParser, IAppSettingsManager appSettingsManager, IPromptService promptService, IRenameFilesCommand renameFilesCommand)
    {
        _namingPatternsParser = namingPatternsParser ?? throw new ArgumentNullException(nameof(namingPatternsParser));
        _appSettingsManager = appSettingsManager ?? throw new ArgumentNullException(nameof(appSettingsManager));
        _promptService = promptService ?? throw new ArgumentNullException(nameof(promptService));
        RenameCommand = renameFilesCommand ?? throw new ArgumentNullException(nameof(renameFilesCommand));
        CancelCommand = new RelayCommand(Cancel);
        BrowseInputDirectoryCommand = new RelayCommand(BrowseInputDirectory);

        UpdateApplicationPatternParts();
        UpdatePatchPatternParts();
        UpdateAddonPatternParts();
    }

    public IRenameFilesCommand RenameCommand { get; }

    public RelayCommand CancelCommand { get; }

    public RelayCommand BrowseInputDirectoryCommand { get; }

    public string ApplicationPattern
    {
        get => _appSettingsManager.Settings.ApplicationPattern;
        set
        {
            _appSettingsManager.Settings.ApplicationPattern = value;

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

    public string PatchPattern
    {
        get => _appSettingsManager.Settings.PatchPattern;
        set
        {
            _appSettingsManager.Settings.PatchPattern = value;
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
        get => _appSettingsManager.Settings.AddonPattern;
        set
        {
            _appSettingsManager.Settings.AddonPattern = value;
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

    private void UpdateApplicationPatternParts()
    {
        try
        {
            RenameCommand.ApplicationPatternParts = _namingPatternsParser.ParseApplicationPattern(this.ApplicationPattern);
            ApplicationPatternError = null;
            _appSettingsManager.SaveSafe();
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
            _appSettingsManager.SaveSafe();
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
            _appSettingsManager.SaveSafe();
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
            RenameCommand.InputDirectory = selectedDir;
    }

    private void Cancel()
    {
        this.Window?.Close();
    }

}
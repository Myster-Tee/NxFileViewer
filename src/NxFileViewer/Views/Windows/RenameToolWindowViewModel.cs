using System;
using Emignatik.NxFileViewer.Commands;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Services;
using Emignatik.NxFileViewer.Services.FileRenaming;
using Emignatik.NxFileViewer.Settings;
using Emignatik.NxFileViewer.Utils.MVVM;
using Emignatik.NxFileViewer.Utils.MVVM.Commands;

namespace Emignatik.NxFileViewer.Views.Windows;

public class RenameToolWindowViewModel : WindowViewModelBase
{
    private readonly INamingPatternsParser _namingPatternsParser;
    private readonly IAppSettingsManager _appSettingsManager;
    private readonly IPromptService _promptService;

    private string _patchPattern;
    private string _addonPattern;
    private string? _applicationPatternError;

    public RenameToolWindowViewModel(INamingPatternsParser namingPatternsParser, IAppSettingsManager appSettingsManager, IPromptService promptService, IRenameFilesCommand renameFilesCommand)
    {
        _namingPatternsParser = namingPatternsParser ?? throw new ArgumentNullException(nameof(namingPatternsParser));
        _appSettingsManager = appSettingsManager ?? throw new ArgumentNullException(nameof(appSettingsManager));
        _promptService = promptService ?? throw new ArgumentNullException(nameof(promptService));
        RenameCommand = renameFilesCommand ?? throw new ArgumentNullException(nameof(renameFilesCommand));
        CancelCommand = new RelayCommand(Cancel);
        BrowseInputDirectoryCommand = new RelayCommand(BrowseInputDirectory);

        UpdateApplicationPatternParts();
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

    private void UpdateApplicationPatternParts()
    {
        try
        {
            RenameCommand.ApplicationPatternParts = _namingPatternsParser.ParseApplicationPatterns(this.ApplicationPattern);
            ApplicationPatternError = null;
            _appSettingsManager.SaveSafe();
        }
        catch (Exception ex)
        {
            RenameCommand.ApplicationPatternParts = null;
            ApplicationPatternError = ex.Message;
        }
    }

    public string PatchPattern
    {
        get => _patchPattern;
        set
        {
            _patchPattern = value;
            NotifyPropertyChanged();
        }
    }

    public string AddonPattern
    {
        get => _addonPattern;
        set
        {
            _addonPattern = value;
            NotifyPropertyChanged();
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
using System;
using System.Windows.Input;
using Emignatik.NxFileViewer.Services.FileRenaming;
using Emignatik.NxFileViewer.Settings;
using Emignatik.NxFileViewer.Utils.MVVM;
using Emignatik.NxFileViewer.Utils.MVVM.Commands;

namespace Emignatik.NxFileViewer.Views.Windows;

public class BulkRenameWindowViewModel : WindowViewModelBase
{
    private readonly IFileRenamerService _fileRenamerService;
    private readonly INamingPatternsParser _namingPatternsParser;
    private readonly IAppSettingsManager _appSettingsManager;

    public BulkRenameWindowViewModel(IFileRenamerService fileRenamerService, INamingPatternsParser namingPatternsParser, IAppSettingsManager appSettingsManager)
    {
        _fileRenamerService = fileRenamerService ?? throw new ArgumentNullException(nameof(fileRenamerService));
        _namingPatternsParser = namingPatternsParser ?? throw new ArgumentNullException(nameof(namingPatternsParser));
        _appSettingsManager = appSettingsManager ?? throw new ArgumentNullException(nameof(appSettingsManager));
        RenameCommand = new RelayCommand(OnRename);
        BrowseInputDirectoryCommand = new RelayCommand(OnBrowseInputDirectory);
    }

    public ICommand RenameCommand { get; }
    public ICommand BrowseInputDirectoryCommand { get; }

    private string _inputDirectory;
    private string _patchPattern;
    private string _addonPattern;

    public string InputDirectory
    {
        get => _inputDirectory;
        set
        {
            _inputDirectory = value;
            NotifyPropertyChanged();
        }
    }

    public string ApplicationPattern
    {
        get => _appSettingsManager.Settings.ApplicationPattern;
        set
        {
            _appSettingsManager.Settings.ApplicationPattern = value;
            NotifyPropertyChanged();
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

    private void OnBrowseInputDirectory()
    {
        //TODO: à implémenter
    }


    private void OnRename()
    {
        //TODO: exposer tous les paramètres
        try
        {
            var namingPatterns = _namingPatternsParser.Parse(ApplicationPattern);
            _fileRenamerService.RenameFromDirectory(InputDirectory, namingPatterns, new[] { ".nsp", ".nsz", ".xci", ".xcz" }, true);
        }
        catch (Exception ex)
        {
            //TODO: gérer l'exception
        }
    }


}
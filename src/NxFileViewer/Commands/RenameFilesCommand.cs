using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Input;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Services.BackgroundTask;
using Emignatik.NxFileViewer.Services.BackgroundTask.RunnableImpl;
using Emignatik.NxFileViewer.Services.FileRenaming.Models;
using Emignatik.NxFileViewer.Settings;
using Emignatik.NxFileViewer.Utils.MVVM.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer.Commands;

public class RenameFilesCommand : CommandBase, IRenameFilesCommand
{
    private readonly IAppSettings _appSettings;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger _logger;

    private Pattern? _applicationPatternParts;
    private Pattern? _patchPatternParts;
    private Pattern? _addonPatternParts;
    private IBackgroundTaskRunner? _backgroundTaskRunner;

    public RenameFilesCommand(IAppSettings appSettings, IServiceProvider serviceProvider, ILoggerFactory loggerFactory)
    {
        _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _logger = (loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory))).CreateLogger(this.GetType());

        _appSettings.RenamingOptions.PropertyChanged += OnRenamingOptionsPropertyChanged;
    }

    public Pattern? ApplicationPatternParts
    {
        get => _applicationPatternParts;
        set
        {
            _applicationPatternParts = value;
            NotifyPropertyChanged();
            TriggerCanExecuteChanged();
        }
    }

    public Pattern? PatchPatternParts
    {
        get => _patchPatternParts;
        set
        {
            _patchPatternParts = value;
            NotifyPropertyChanged();
            TriggerCanExecuteChanged();
        }
    }

    public Pattern? AddonPatternParts
    {
        get => _addonPatternParts;
        set
        {
            _addonPatternParts = value;
            NotifyPropertyChanged();
            TriggerCanExecuteChanged();
        }
    }

    public bool AutoCloseOpenedFile
    {
        get => _appSettings.RenamingOptions.AutoCloseOpenedFile;
        set => _appSettings.RenamingOptions.AutoCloseOpenedFile = value;
    }

    public string InputPath
    {
        get => _appSettings.RenamingOptions.LastRenamePath;
        set => _appSettings.RenamingOptions.LastRenamePath = value;
    }

    public string? FileFilters
    {
        get => _appSettings.RenamingOptions.FileFilters;
        set => _appSettings.RenamingOptions.FileFilters = value;
    }

    public bool IncludeSubdirectories
    {
        get => _appSettings.RenamingOptions.IncludeSubdirectories;
        set => _appSettings.RenamingOptions.IncludeSubdirectories = value;
    }

    public bool IsSimulation
    {
        get => _appSettings.RenamingOptions.IsSimulation;
        set => _appSettings.RenamingOptions.IsSimulation = value;
    }

    public ILogger? Logger { get; set; }

    public string InvalidWindowsCharsReplacement
    {
        get => _appSettings.RenamingOptions.InvalidFileNameCharsReplacement;
        set => _appSettings.RenamingOptions.InvalidFileNameCharsReplacement = value;
    }

    public bool ReplaceWhiteSpaceChars
    {
        get => _appSettings.RenamingOptions.ReplaceWhiteSpaceChars;
        set => _appSettings.RenamingOptions.ReplaceWhiteSpaceChars = value;
    }

    public string WhiteSpaceCharsReplacement
    {
        get => _appSettings.RenamingOptions.WhiteSpaceCharsReplacement;
        set => _appSettings.RenamingOptions.WhiteSpaceCharsReplacement = value;
    }

    public IBackgroundTaskRunner? BackgroundTaskRunner
    {
        get => _backgroundTaskRunner;
        set
        {
            if (_backgroundTaskRunner != null) _backgroundTaskRunner.PropertyChanged -= OnBackgroundTaskRunnerPropertyChanged;
            if (value != null) value.PropertyChanged += OnBackgroundTaskRunnerPropertyChanged;

            _backgroundTaskRunner = value;

            NotifyPropertyChanged();
            TriggerCanExecuteChanged();
        }
    }
    public override async void Execute(object? parameter)
    {
        try
        {
            var inputPath = InputPath;

            var namingPatterns = new NamingSettings
            {
                ApplicationPattern = ApplicationPatternParts!,
                PatchPattern = PatchPatternParts!,
                AddonPattern = AddonPatternParts!,
                InvalidFileNameCharsReplacement = InvalidWindowsCharsReplacement,
                ReplaceWhiteSpaceChars = ReplaceWhiteSpaceChars,
                WhiteSpaceCharsReplacement = WhiteSpaceCharsReplacement,
            };

            IRunnable runnable;
            if (File.Exists(inputPath))
            {
                runnable = _serviceProvider.GetRequiredService<IFileRenamerRunnable>()
                    .Setup(namingPatterns, AutoCloseOpenedFile, inputPath, IsSimulation, Logger);
            }
            else
            {
                runnable = _serviceProvider.GetRequiredService<IFilesRenamerRunnable>()
                    .Setup(namingPatterns, AutoCloseOpenedFile, inputPath, FileFilters, IncludeSubdirectories, IsSimulation, Logger);
            }

            await _backgroundTaskRunner!.RunAsync(runnable);
        }
        catch (Exception ex)
        {
            _logger.LogError(LocalizationManager.Instance.Current.Keys.RenamingTool_LogRenamingFailed.SafeFormat(ex.Message));
        }
    }

    private void OnRenamingOptionsPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(IRenamingOptions.FileFilters):
                NotifyPropertyChanged(nameof(FileFilters));
                break;
            case nameof(IRenamingOptions.IncludeSubdirectories):
                NotifyPropertyChanged(nameof(IncludeSubdirectories));
                break;
            case nameof(IRenamingOptions.IsSimulation):
                NotifyPropertyChanged(nameof(IsSimulation));
                break;
            case nameof(IRenamingOptions.InvalidFileNameCharsReplacement):
                NotifyPropertyChanged(nameof(InvalidWindowsCharsReplacement));
                break;
            case nameof(IRenamingOptions.WhiteSpaceCharsReplacement):
                NotifyPropertyChanged(nameof(WhiteSpaceCharsReplacement));
                break;
            case nameof(IRenamingOptions.ReplaceWhiteSpaceChars):
                NotifyPropertyChanged(nameof(ReplaceWhiteSpaceChars));
                break;
            case nameof(IRenamingOptions.LastRenamePath):
                NotifyPropertyChanged(nameof(InputPath));
                break;

        }
    }

    private void OnBackgroundTaskRunnerPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(IBackgroundTaskRunner.IsRunning))
            TriggerCanExecuteChanged();
    }

    public override bool CanExecute(object? parameter)
    {
        return _applicationPatternParts != null && _patchPatternParts != null && _addonPatternParts != null && _backgroundTaskRunner is { IsRunning: false };
    }
}

public interface IRenameFilesCommand : ICommand, INotifyPropertyChanged
{
    Pattern? ApplicationPatternParts { get; set; }

    Pattern? PatchPatternParts { get; set; }

    Pattern? AddonPatternParts { get; set; }

    string InputPath { get; set; }

    string? FileFilters { get; set; }

    IBackgroundTaskRunner? BackgroundTaskRunner { get; set; }

    public bool IncludeSubdirectories { get; set; }

    public bool AutoCloseOpenedFile { get; set; }

    bool IsSimulation { get; set; }

    ILogger? Logger { get; set; }

    string InvalidWindowsCharsReplacement { get; set; }

    bool ReplaceWhiteSpaceChars { get; set; }

    string WhiteSpaceCharsReplacement { get; set; }
}
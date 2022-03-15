using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using Emignatik.NxFileViewer.Services.BackgroundTask;
using Emignatik.NxFileViewer.Services.BackgroundTask.RunnableImpl;
using Emignatik.NxFileViewer.Services.FileRenaming.Models;
using Emignatik.NxFileViewer.Services.FileRenaming.Models.PatternParts.Addon;
using Emignatik.NxFileViewer.Services.FileRenaming.Models.PatternParts.Application;
using Emignatik.NxFileViewer.Services.FileRenaming.Models.PatternParts.Patch;
using Emignatik.NxFileViewer.Settings;
using Emignatik.NxFileViewer.Utils.MVVM.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer.Commands
{
    public class RenameFilesCommand : CommandBase, IRenameFilesCommand
    {
        private readonly IAppSettingsManager _appSettingsManager;
        private readonly IServiceProvider _serviceProvider;
        private List<ApplicationPatternPart>? _applicationPatternParts;
        private List<PatchPatternPart>? _patchPatternParts;
        private List<AddonPatternPart>? _addonPatternParts;
        private IBackgroundTaskRunner? _backgroundTaskRunner;

        public RenameFilesCommand(IAppSettingsManager appSettingsManager, IServiceProvider serviceProvider)
        {
            _appSettingsManager = appSettingsManager ?? throw new ArgumentNullException(nameof(appSettingsManager));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));


            _appSettingsManager.Settings.PropertyChanged += OnAppSettingsPropertyChanged;
            _appSettingsManager.Settings.RenamingOptions.PropertyChanged += OnRenamingOptionsPropertyChanged;
        }



        public List<ApplicationPatternPart>? ApplicationPatternParts
        {
            get => _applicationPatternParts;
            set
            {
                _applicationPatternParts = value;
                NotifyPropertyChanged();
                TriggerCanExecuteChanged();
            }
        }

        public List<PatchPatternPart>? PatchPatternParts
        {
            get => _patchPatternParts;
            set
            {
                _patchPatternParts = value;
                NotifyPropertyChanged();
                TriggerCanExecuteChanged();
            }
        }

        public List<AddonPatternPart>? AddonPatternParts
        {
            get => _addonPatternParts;
            set
            {
                _addonPatternParts = value;
                NotifyPropertyChanged();
                TriggerCanExecuteChanged();
            }
        }

        public string InputDirectory
        {
            get => _appSettingsManager.Settings.LastUsedDir;
            set => _appSettingsManager.Settings.LastUsedDir = value;
        }

        public string? FileFilters
        {
            get => _appSettingsManager.Settings.RenamingOptions.FileFilters;
            set => _appSettingsManager.Settings.RenamingOptions.FileFilters = value;
        }

        public bool IncludeSubdirectories
        {
            get => _appSettingsManager.Settings.RenamingOptions.IncludeSubdirectories;
            set => _appSettingsManager.Settings.RenamingOptions.IncludeSubdirectories = value;
        }

        public bool IsSimulation
        {
            get => _appSettingsManager.Settings.RenamingOptions.IsSimulation;
            set => _appSettingsManager.Settings.RenamingOptions.IsSimulation = value;
        }

        public ILogger? Logger { get; set; }

        public string InvalidWindowsCharsReplacement
        {
            get => _appSettingsManager.Settings.RenamingOptions.InvalidFileNameCharsReplacement;
            set => _appSettingsManager.Settings.RenamingOptions.InvalidFileNameCharsReplacement = value;
        }

        public bool ReplaceWhiteSpaceChars
        {
            get => _appSettingsManager.Settings.RenamingOptions.ReplaceWhiteSpaceChars;
            set => _appSettingsManager.Settings.RenamingOptions.ReplaceWhiteSpaceChars = value;
        }

        public string WhiteSpaceCharsReplacement
        {
            get => _appSettingsManager.Settings.RenamingOptions.WhiteSpaceCharsReplacement;
            set => _appSettingsManager.Settings.RenamingOptions.WhiteSpaceCharsReplacement = value;
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
                var namingPatterns = new NamingSettings
                {
                    ApplicationPattern = ApplicationPatternParts!,
                    PatchPattern = PatchPatternParts!,
                    AddonPattern = AddonPatternParts!,
                    InvalidFileNameCharsReplacement = InvalidWindowsCharsReplacement,
                    ReplaceWhiteSpaceChars = ReplaceWhiteSpaceChars,
                    WhiteSpaceCharsReplacement = WhiteSpaceCharsReplacement,
                };

                var filesRenamerRunnable = _serviceProvider.GetRequiredService<IFilesRenamerRunnable>();

                filesRenamerRunnable.Setup(InputDirectory, namingPatterns, FileFilters, IncludeSubdirectories, IsSimulation, Logger);

                await _backgroundTaskRunner!.RunAsync(filesRenamerRunnable);
            }
            catch (Exception ex)
            {
                //TODO: gérer l'exception
            }
        }

        private void OnAppSettingsPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(IAppSettings.LastUsedDir):
                    NotifyPropertyChanged(nameof(InputDirectory));
                    TriggerCanExecuteChanged();
                    break;
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
        List<ApplicationPatternPart>? ApplicationPatternParts { get; set; }

        List<PatchPatternPart>? PatchPatternParts { get; set; }

        List<AddonPatternPart>? AddonPatternParts { get; set; }

        string InputDirectory { get; set; }

        string? FileFilters { get; set; }

        IBackgroundTaskRunner? BackgroundTaskRunner { get; set; }

        public bool IncludeSubdirectories { get; set; }

        bool IsSimulation { get; set; }

        ILogger? Logger { get; set; }

        string InvalidWindowsCharsReplacement { get; set; }

        bool ReplaceWhiteSpaceChars { get; set; }

        string WhiteSpaceCharsReplacement { get; set; }
    }
}

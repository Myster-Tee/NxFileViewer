using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Input;
using Emignatik.NxFileViewer.Services.BackgroundTask;
using Emignatik.NxFileViewer.Services.BackgroundTask.RunnableImpl;
using Emignatik.NxFileViewer.Services.FileRenaming.Models;
using Emignatik.NxFileViewer.Services.FileRenaming.Models.PatternParts;
using Emignatik.NxFileViewer.Settings;
using Emignatik.NxFileViewer.Utils.MVVM.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer.Commands
{
    public class RenameFilesCommand : CommandBase, IRenameFilesCommand
    {
        private readonly IAppSettings _appSettings;
        private readonly IServiceProvider _serviceProvider;
        private List<PatternPart>? _applicationPatternParts;
        private List<PatternPart>? _patchPatternParts;
        private List<PatternPart>? _addonPatternParts;
        private IBackgroundTaskRunner? _backgroundTaskRunner;

        public RenameFilesCommand(IAppSettings appSettings, IServiceProvider serviceProvider)
        {
            _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));


            _appSettings.PropertyChanged += OnAppSettingsPropertyChanged;
            _appSettings.RenamingOptions.PropertyChanged += OnRenamingOptionsPropertyChanged;
        }

        public List<PatternPart>? ApplicationPatternParts
        {
            get => _applicationPatternParts;
            set
            {
                _applicationPatternParts = value;
                NotifyPropertyChanged();
                TriggerCanExecuteChanged();
            }
        }

        public List<PatternPart>? PatchPatternParts
        {
            get => _patchPatternParts;
            set
            {
                _patchPatternParts = value;
                NotifyPropertyChanged();
                TriggerCanExecuteChanged();
            }
        }

        public List<PatternPart>? AddonPatternParts
        {
            get => _addonPatternParts;
            set
            {
                _addonPatternParts = value;
                NotifyPropertyChanged();
                TriggerCanExecuteChanged();
            }
        }

        public string InputPath
        {
            get => _appSettings.LastRenamePath;
            set => _appSettings.LastRenamePath = value;
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

                //TODO: gérer si l'input path est un dossier ou un fichier (créer le Runnable pour un renommage de fichier?)
                IRunnable runnable;
                if (File.Exists(inputPath))
                {
                    runnable = _serviceProvider.GetRequiredService<IFileRenamerRunnable>()
                        .Setup(inputPath, namingPatterns, IsSimulation, Logger);
                }
                else
                {
                    runnable = _serviceProvider.GetRequiredService<IFilesRenamerRunnable>()
                        .Setup(inputPath, namingPatterns, FileFilters, IncludeSubdirectories, IsSimulation, Logger);
                }

                await _backgroundTaskRunner!.RunAsync(runnable);
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
                case nameof(IAppSettings.LastRenamePath):
                    NotifyPropertyChanged(nameof(InputPath));
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
        List<PatternPart>? ApplicationPatternParts { get; set; }

        List<PatternPart>? PatchPatternParts { get; set; }

        List<PatternPart>? AddonPatternParts { get; set; }

        string InputPath { get; set; }

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

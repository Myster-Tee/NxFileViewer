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
        }

        public override void Execute(object? parameter)
        {
            //TODO: exposer tous les paramètres
            try
            {
                var namingPatterns = new NamingPatterns
                {
                    ApplicationPattern = ApplicationPatternParts!,
                    PatchPattern = PatchPatternParts!,
                    AddonPattern = AddonPatternParts!,
                };

                var filesRenamerRunnable = _serviceProvider.GetRequiredService<IFilesRenamerRunnable>();

                filesRenamerRunnable.Setup(InputDirectory, namingPatterns, FileFilters, true);

                _backgroundTaskRunner?.RunAsync(filesRenamerRunnable);
            }
            catch (Exception ex)
            {
                //TODO: gérer l'exception
            }
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
            set
            {
                _appSettingsManager.Settings.LastUsedDir = value;
                _appSettingsManager.SaveSafe();
                NotifyPropertyChanged();
                TriggerCanExecuteChanged();
            }
        }

        public string? FileFilters
        {
            get => _appSettingsManager.Settings.RenamingFileFilters;
            set
            {
                _appSettingsManager.Settings.RenamingFileFilters = value;
                NotifyPropertyChanged();
            }
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

    public interface IRenameFilesCommand : ICommand
    {
        List<ApplicationPatternPart>? ApplicationPatternParts { get; set; }

        List<PatchPatternPart>? PatchPatternParts { get; set; }

        List<AddonPatternPart>? AddonPatternParts { get; set; }

        string InputDirectory { get; set; }

        string? FileFilters { get; set; }

        IBackgroundTaskRunner? BackgroundTaskRunner { get; set; }
    }
}

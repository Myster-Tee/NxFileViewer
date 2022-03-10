using System;
using System.Collections.Generic;
using System.Windows.Input;
using Emignatik.NxFileViewer.Services.FileRenaming;
using Emignatik.NxFileViewer.Services.FileRenaming.Models;
using Emignatik.NxFileViewer.Services.FileRenaming.Models.PatternParts.Addon;
using Emignatik.NxFileViewer.Services.FileRenaming.Models.PatternParts.Application;
using Emignatik.NxFileViewer.Services.FileRenaming.Models.PatternParts.Patch;
using Emignatik.NxFileViewer.Settings;
using Emignatik.NxFileViewer.Utils.MVVM.Commands;

namespace Emignatik.NxFileViewer.Commands
{
    public class RenameFilesCommand : CommandBase, IRenameFilesCommand
    {
        private readonly IFileRenamerService _fileRenamerService;
        private readonly IAppSettingsManager _appSettingsManager;
        private List<ApplicationPatternPart>? _applicationPatternParts;
        private List<PatchPatternPart>? _patchPatternParts;
        private List<AddonPatternPart>? _addonPatternParts;

        public RenameFilesCommand(IFileRenamerService fileRenamerService, IAppSettingsManager appSettingsManager)
        {
            _fileRenamerService = fileRenamerService ?? throw new ArgumentNullException(nameof(fileRenamerService));
            _appSettingsManager = appSettingsManager ?? throw new ArgumentNullException(nameof(appSettingsManager));
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

                _fileRenamerService.RenameFromDirectory(InputDirectory, namingPatterns, new[] { ".nsp", ".nsz", ".xci", ".xcz" }, true);
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


        public override bool CanExecute(object? parameter)
        {
            return _applicationPatternParts != null && _patchPatternParts != null && _addonPatternParts != null;
        }
    }

    public interface IRenameFilesCommand : ICommand
    {
        List<ApplicationPatternPart>? ApplicationPatternParts { get; set; }

        List<PatchPatternPart>? PatchPatternParts { get; set; }

        List<AddonPatternPart>? AddonPatternParts { get; set; }

        string InputDirectory { get; set; }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Windows.Input;
using Emignatik.NxFileViewer.Services.FileRenaming;
using Emignatik.NxFileViewer.Services.FileRenaming.Models;
using Emignatik.NxFileViewer.Services.FileRenaming.Models.PatternParts.Application;
using Emignatik.NxFileViewer.Settings;
using Emignatik.NxFileViewer.Utils.MVVM.Commands;

namespace Emignatik.NxFileViewer.Commands
{
    public class RenameFilesCommand : CommandBase, IRenameFilesCommand
    {
        private readonly IFileRenamerService _fileRenamerService;
        private readonly IAppSettingsManager _appSettingsManager;
        private List<ApplicationPatternPart>? _applicationPatternParts;

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
            return _applicationPatternParts != null;
        }
    }

    public interface IRenameFilesCommand : ICommand
    {
        List<ApplicationPatternPart>? ApplicationPatternParts { get; set; }

        string InputDirectory { get; set; }
    }
}

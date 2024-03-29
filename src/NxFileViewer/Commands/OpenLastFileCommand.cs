﻿using System;
using System.IO;
using System.Windows.Input;
using Emignatik.NxFileViewer.Services.FileOpening;
using Emignatik.NxFileViewer.Settings;
using Emignatik.NxFileViewer.Utils.MVVM.Commands;

namespace Emignatik.NxFileViewer.Commands;

public class OpenLastFileCommand : CommandBase, IOpenLastFileCommand
{
    private readonly IFileOpenerService _fileOpenerService;
    private readonly IAppSettings _appSettings;

    public OpenLastFileCommand(IFileOpenerService fileOpenerService, IAppSettings appSettings)
    {
        _fileOpenerService = fileOpenerService ?? throw new ArgumentNullException(nameof(fileOpenerService));
        _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));

        _appSettings.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == nameof(IAppSettings.LastOpenedFile))
                TriggerCanExecuteChanged();
        };
    }

    public override bool CanExecute(object? parameter)
    {
        var lastOpenedFile = _appSettings.LastOpenedFile;
        return !string.IsNullOrEmpty(lastOpenedFile) && File.Exists(lastOpenedFile);
    }

    public override void Execute(object? parameter)
    {
        var lastOpenedFile = _appSettings.LastOpenedFile;
        _fileOpenerService.SafeOpenFile(lastOpenedFile);
    }
}

public interface IOpenLastFileCommand : ICommand
{
}
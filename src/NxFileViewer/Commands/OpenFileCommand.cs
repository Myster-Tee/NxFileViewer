using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Services.FileOpening;
using Emignatik.NxFileViewer.Settings;
using Emignatik.NxFileViewer.Utils.MVVM.Commands;
using Microsoft.Win32;

namespace Emignatik.NxFileViewer.Commands;

public class OpenFileCommand : CommandBase, IOpenFileCommand
{
    private readonly IFileOpeningService _fileOpeningService;
    private readonly IAppSettings _appSettings;

    public OpenFileCommand(IFileOpeningService fileOpeningService, IAppSettings appSettings)
    {
        _fileOpeningService = fileOpeningService ?? throw new ArgumentNullException(nameof(fileOpeningService));
        _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
    }

    public override void Execute(object? parameter)
    {
        var lastOpenedFile = _appSettings.LastOpenedFile;

        var initialDirectory = string.Empty;

        if (!string.IsNullOrEmpty(lastOpenedFile))
        {
            try
            {
                initialDirectory = Path.GetDirectoryName(lastOpenedFile);
            }
            catch
            {
                // ignored
            }

            if (!string.IsNullOrEmpty(initialDirectory) )
            {

                try
                {
                    if (!Directory.Exists(initialDirectory))
                        initialDirectory = string.Empty;
                }
                catch
                {
                    initialDirectory = string.Empty;
                }
            }

        }

        var openFileDialog = new OpenFileDialog
        {
            InitialDirectory = initialDirectory,
            FileName = lastOpenedFile,
            Filter = LocalizationManager.Instance.Current.Keys.OpenFile_Filter
        };

        var currentMainWindow = Application.Current.MainWindow;
        if (openFileDialog.ShowDialog(currentMainWindow) != true) return;

        var filePath = openFileDialog.FileName;

        _fileOpeningService.SafeOpenFile(filePath);
    }
}

public interface IOpenFileCommand : ICommand
{
}
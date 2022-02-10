using System;
using System.IO;
using System.Windows;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Settings;
using Emignatik.NxFileViewer.Tools;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Emignatik.NxFileViewer.Services;

public class PromptService : IPromptService
{
    private readonly IAppSettings _appSettings;
    private readonly IFsSanitizer _fsSanitizer;

    public PromptService(IAppSettings appSettings, IFsSanitizer fsSanitizer)
    {
        _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
        _fsSanitizer = fsSanitizer ?? throw new ArgumentNullException(nameof(fsSanitizer));
    }

    public string? PromptSaveDir()
    {
        var fileDialog = new CommonOpenFileDialog
        {
            InitialDirectory = _appSettings.LastSaveDir,
            Multiselect = false,
            IsFolderPicker = true,
            Title = LocalizationManager.Instance.Current.Keys.SaveDialog_Title
        };

        if (fileDialog.ShowDialog(Application.Current.MainWindow) != CommonFileDialogResult.Ok)
            return null;

        var filePath = fileDialog.FileName;

        _appSettings.LastSaveDir = filePath;

        return filePath;
    }

    public string? PromptSaveFile(string proposedFileName)
    {
        var sanitizedFileName = _fsSanitizer.SanitizeFileName(proposedFileName);

        var fileDialog = new CommonSaveFileDialog
        {
            Title = LocalizationManager.Instance.Current.Keys.SaveDialog_Title,
            InitialDirectory = _appSettings.LastSaveDir,

            Filters = { new CommonFileDialogFilter
            {
                DisplayName = LocalizationManager.Instance.Current.Keys.SaveDialog_AnyFileFilter,
                ShowExtensions = false
            } },
            DefaultFileName = sanitizedFileName,
        };

        if (fileDialog.ShowDialog(Application.Current.MainWindow) != CommonFileDialogResult.Ok)
            return null;

        var filePath = fileDialog.FileName;

        _appSettings.LastSaveDir = Path.GetDirectoryName(filePath)!;

        return filePath;
    }
}
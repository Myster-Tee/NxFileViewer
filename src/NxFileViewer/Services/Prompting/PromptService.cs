using System;
using System.IO;
using System.Windows;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Settings;
using Emignatik.NxFileViewer.Tools;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Emignatik.NxFileViewer.Services.Prompting;

public class PromptService : IPromptService
{
    private readonly IAppSettings _appSettings;
    private readonly IFsSanitizer _fsSanitizer;

    public PromptService(IAppSettings appSettings, IFsSanitizer fsSanitizer)
    {
        _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
        _fsSanitizer = fsSanitizer ?? throw new ArgumentNullException(nameof(fsSanitizer));
    }

    public string? PromptSelectDir(string title)
    {
        var fileDialog = new CommonOpenFileDialog
        {
            InitialDirectory = _appSettings.LastUsedDir,
            Multiselect = false,
            IsFolderPicker = true,
            Title = title
        };

        if (fileDialog.ShowDialog(Application.Current.MainWindow) != CommonFileDialogResult.Ok)
            return null;

        var dirPath = fileDialog.FileName;

        _appSettings.LastUsedDir = dirPath;

        return dirPath;
    }

    public string? PromptSaveFile(string defaultFileName, string? title = null, string? filter = null)
    {
        title ??= LocalizationManager.Instance.Current.Keys.SaveDialog_Title;
        var sanitizedFileName = _fsSanitizer.SanitizeFileName(defaultFileName);

        var saveFileDialog = new SaveFileDialog
        {
            InitialDirectory = _appSettings.LastUsedDir,
            Title = title,
            FileName = sanitizedFileName,
            Filter = filter ?? $"{LocalizationManager.Instance.Current.Keys.SaveDialog_AnyFileFilter} (*.*)|*.*"
        };

        if (saveFileDialog.ShowDialog() == false) 
            return null;

        var filePath = saveFileDialog.FileName;

        _appSettings.LastUsedDir = Path.GetDirectoryName(filePath)!;

        return filePath;

    }
}
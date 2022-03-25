using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Settings;
using Emignatik.NxFileViewer.Tools;
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

    public string? PromptSaveFile(string defaultFileName, string? title = null, IEnumerable<CommonFileDialogFilter>? filters = null)
    {
        title ??= LocalizationManager.Instance.Current.Keys.SaveDialog_Title;
        var sanitizedFileName = _fsSanitizer.SanitizeFileName(defaultFileName);

        var fileDialog = new CommonSaveFileDialog
        {
            Title = title,
            InitialDirectory = _appSettings.LastUsedDir,
            DefaultFileName = sanitizedFileName,
        };

        if (filters == null)
        {

            fileDialog.Filters.Add(new CommonFileDialogFilter
            {
                DisplayName = LocalizationManager.Instance.Current.Keys.SaveDialog_AnyFileFilter,
                ShowExtensions = false
            });
        }
        else
        {
            foreach (var filter in filters)
            {
                fileDialog.Filters.Add(filter);
            }
        }


        if (fileDialog.ShowDialog(Application.Current.MainWindow) != CommonFileDialogResult.Ok)
            return null;

        var filePath = fileDialog.FileName;

        _appSettings.LastUsedDir = Path.GetDirectoryName(filePath)!;

        return filePath;
    }
}
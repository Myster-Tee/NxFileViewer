using System;
using System.IO;
using System.Windows.Forms;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Settings;
using Emignatik.NxFileViewer.Tools;

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
        var folderDialog = new FolderBrowserDialog
        {
            InitialDirectory = _appSettings.LastUsedDir,
            Description = title
        };

        if (folderDialog.ShowDialog() != DialogResult.OK)
            return null;

        var dirPath = folderDialog.SelectedPath;

        _appSettings.LastUsedDir = dirPath;

        return dirPath;
    }

    public string? PromptSaveFile(string defaultFileName, string? title = null, string? filters = null)
    {
        title ??= filters != null ? LocalizationManager.Instance.Current.Keys.SaveDialog_Title : LocalizationManager.Instance.Current.Keys.SaveDialog_AnyFileFilter;
        var sanitizedFileName = _fsSanitizer.SanitizeFileName(defaultFileName);

        var fileDialog = new SaveFileDialog
        {
            Title = title,
            InitialDirectory = _appSettings.LastUsedDir,
            FileName = sanitizedFileName,
            Filter = filters
        };


        if (fileDialog.ShowDialog() != DialogResult.OK)
            return null;

        var filePath = fileDialog.FileName;

        _appSettings.LastUsedDir = Path.GetDirectoryName(filePath)!;

        return filePath;
    }
}
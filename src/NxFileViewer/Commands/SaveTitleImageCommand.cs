using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Model.Overview;
using Emignatik.NxFileViewer.Settings;
using Emignatik.NxFileViewer.Tools;
using Emignatik.NxFileViewer.Utils.MVVM.Commands;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Emignatik.NxFileViewer.Commands;

public class SaveTitleImageCommand : CommandBase, ISaveTitleImageCommand
{
    private readonly IAppSettings _appSettings;
    private readonly IFsSanitizer _fsSanitizer;
    private readonly ILogger _logger;
    private TitleInfo? _title;

    public SaveTitleImageCommand(IAppSettings appSettings, ILoggerFactory loggerFactory, IFsSanitizer fsSanitizer)
    {
        _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
        _fsSanitizer = fsSanitizer ?? throw new ArgumentNullException(nameof(fsSanitizer));
        _logger = (loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory))).CreateLogger(this.GetType());
    }

    public TitleInfo? Title
    {
        get => _title;
        set
        {
            _title = value;
            TriggerCanExecuteChanged();
        }
    }

    public override void Execute(object? parameter)
    {
        try
        {
            var title = Title;

            var icon = title?.Icon;
            if (title == null || icon == null)
                return;

            var fileDialog = new CommonSaveFileDialog
            {
                Title = LocalizationManager.Instance.Current.Keys.SaveDialog_Title,
                InitialDirectory = _appSettings.LastSaveDir,

                Filters = { new CommonFileDialogFilter
                {
                    DisplayName = LocalizationManager.Instance.Current.Keys.SaveDialog_ImageFilter,
                    Extensions =
                    {
                        "jpg"
                    },
                    ShowExtensions = true
                } },
                DefaultFileName = _fsSanitizer.SanitizeFileName($"{title.AppName}_({title.Language}).jpg"),
                DefaultExtension = "jpg",
            };

            if (fileDialog.ShowDialog(Application.Current.MainWindow) != CommonFileDialogResult.Ok)
                return;

            var filePath = fileDialog.FileName;
            _appSettings.LastSaveDir = Path.GetDirectoryName(filePath)!;

            File.WriteAllBytes(filePath, icon);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, LocalizationManager.Instance.Current.Keys.SaveTitleImageError.SafeFormat(ex.Message));
        }
    }

    public override bool CanExecute(object? parameter)
    {
        var selectedTitle = Title;
        return selectedTitle?.Icon != null;
    }

}

public interface ISaveTitleImageCommand : ICommand
{
    TitleInfo? Title { set; }
}
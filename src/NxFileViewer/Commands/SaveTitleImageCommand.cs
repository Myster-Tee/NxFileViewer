using System;
using System.IO;
using System.Windows.Input;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Models.Overview;
using Emignatik.NxFileViewer.Services.Prompting;
using Emignatik.NxFileViewer.Utils.MVVM.Commands;
using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer.Commands;

public class SaveTitleImageCommand : CommandBase, ISaveTitleImageCommand
{
    private readonly IPromptService _promptService;
    private readonly ILogger _logger;
    private TitleInfo? _title;

    public SaveTitleImageCommand(ILoggerFactory loggerFactory, IPromptService promptService)
    {
        _promptService = promptService ?? throw new ArgumentNullException(nameof(promptService));
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

            var defaultFileName = $"{title.AppName}_({title.Language}).jpg";
            var filter = $"{LocalizationManager.Instance.Current.Keys.SaveDialog_ImageFilter} (*.jpg)|*.jpg";

            var filePath = _promptService.PromptSaveFile(defaultFileName, LocalizationManager.Instance.Current.Keys.SaveDialog_Title, filter);

            if (filePath == null)
                return;

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
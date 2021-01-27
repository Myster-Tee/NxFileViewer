using System;
using System.IO;
using System.Windows;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Model.Overview;
using Emignatik.NxFileViewer.Settings;
using Emignatik.NxFileViewer.Utils.MVVM.Commands;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;

namespace Emignatik.NxFileViewer.Commands
{
    public class SaveTitleImageCommand : CommandBase, ISaveTitleImageCommand
    {
        private readonly IAppSettings _appSettings;
        private readonly ILogger _logger;

        public SaveTitleImageCommand(IAppSettings appSettings, ILoggerFactory loggerFactory)
        {
            _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
            _logger = (loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory))).CreateLogger(this.GetType());
        }

        public TitleInfo? Title { get; set; }

        public override void Execute(object? parameter)
        {
            try
            {
                var title = Title;

                var icon = title?.Icon;
                if (title == null || icon == null)
                    return;

                var openFileDialog = new SaveFileDialog
                {
                    Title = LocalizationManager.Instance.Current.Keys.SaveFileDialog_SaveImageTitle,
                    InitialDirectory = _appSettings.LastSaveDir,
                    Filter = $"{LocalizationManager.Instance.Current.Keys.SaveFileDialog_JpegImageFilter}|*.jpg",
                    DefaultExt = "jpg",
                    FileName = $"{title.AppName}_({title.Language}).jpg",
                };

                var result = openFileDialog.ShowDialog(Application.Current.MainWindow);
                if (result == null || !result.Value)
                    return;

                var filePath = openFileDialog.FileName;
                _appSettings.LastSaveDir = Path.GetDirectoryName(filePath);

                using var fileStream = File.Create(filePath);
                icon.StreamSource.Position = 0;
                icon.StreamSource.CopyTo(fileStream);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Format(LocalizationManager.Instance.Current.Keys.SaveTitleImageError, ex.Message));
            }
        }

        public override bool CanExecute(object? parameter)
        {
            var selectedTitle = Title;
            return selectedTitle?.Icon != null;
        }

    }
}

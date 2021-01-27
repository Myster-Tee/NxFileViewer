using System;
using System.Windows;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Model.Overview;
using Emignatik.NxFileViewer.Utils.MVVM.Commands;
using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer.Commands
{
    public class CopyTitleImageCommand : CommandBase, ICopyTitleImageCommand
    {
        private readonly ILogger _logger;

        public CopyTitleImageCommand(ILoggerFactory loggerFactory)
        {
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

                Clipboard.SetImage(icon);
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(LocalizationManager.Instance.Current.Keys.CopyTitleImageError, ex.Message));
            }
        }

        public override bool CanExecute(object? parameter)
        {
            var selectedTitle = Title;
            return selectedTitle?.Icon != null;
        }
    }
}

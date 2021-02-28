using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Utils.MVVM.Commands;
using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer.Commands
{
    public class CopyImageCommand : CommandBase, ICopyImageCommand
    {
        private readonly ILogger _logger;
        private BitmapSource? _titleImage;

        public CopyImageCommand(ILoggerFactory loggerFactory)
        {
            _logger = (loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory))).CreateLogger(this.GetType());
        }

        public BitmapSource? Image
        {
            private get => _titleImage;
            set
            {
                _titleImage = value;
                TriggerCanExecuteChanged();
            }
        }

        public override void Execute(object? parameter)
        {
            try
            {
                var titleImage = Image;

                if (titleImage == null)
                    return;
                Clipboard.SetImage(titleImage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, LocalizationManager.Instance.Current.Keys.CopyTitleImageError.SafeFormat(ex.Message));
            }
        }

        public override bool CanExecute(object? parameter)
        {
            var titleImage = Image;
            return titleImage != null;
        }
    }

    public interface ICopyImageCommand : ICommand
    {
        BitmapSource? Image { set; }
    }
}

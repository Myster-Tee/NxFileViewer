using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Utils.MVVM.Commands;
using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer.Commands
{
    public class OpenFileLocationCommand : CommandBase, IOpenFileLocationCommand
    {
        private readonly ILogger _logger;
        private string? _filePath;

        public OpenFileLocationCommand(ILoggerFactory loggerFactory)
        {
            _logger = (loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory))).CreateLogger(this.GetType());
        }

        public string? FilePath
        {
            get => _filePath;
            set
            {
                _filePath = value;
                TriggerCanExecuteChanged();
            }
        }

        public override void Execute(object? parameter)
        {
            var filePath = FilePath;
            try
            {

                var argument = $"/select, \"{filePath}\"";
                Process.Start("explorer.exe", argument);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, LocalizationManager.Instance.Current.Keys.OpenFileLocation_Failed_Log.SafeFormat(filePath, ex.Message));
            }
        }

        public override bool CanExecute(object? parameter)
        {
            try
            {
                var filePath = FilePath;
                return filePath != null && File.Exists(filePath);
            }
            catch
            {
                return false;
            }
        }
    }

    public interface IOpenFileLocationCommand : ICommand
    {
        string? FilePath { get; set; }
    }
}

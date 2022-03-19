using System;
using System.IO;
using System.Windows.Input;
using Emignatik.NxFileViewer.Services.FileLocationOpening;
using Emignatik.NxFileViewer.Utils.MVVM.Commands;
using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer.Commands;

public class OpenFileLocationCommand : CommandBase, IOpenFileLocationCommand
{
    private readonly IFileLocationOpenerService _fileLocationOpenerService;
    private readonly ILogger _logger;
    private string? _filePath;

    public OpenFileLocationCommand(ILoggerFactory loggerFactory, IFileLocationOpenerService fileLocationOpenerService)
    {
        _fileLocationOpenerService = fileLocationOpenerService ?? throw new ArgumentNullException(nameof(fileLocationOpenerService));
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
        _fileLocationOpenerService.OpenFileLocationSafe(FilePath);
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
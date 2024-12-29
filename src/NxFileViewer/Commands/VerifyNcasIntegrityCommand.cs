using System;
using System.Windows.Input;
using Emignatik.NxFileViewer.Services.BackgroundTask;
using Emignatik.NxFileViewer.Services.BackgroundTask.RunnableImpl;
using Emignatik.NxFileViewer.Services.FileOpening;
using Emignatik.NxFileViewer.Settings;
using Emignatik.NxFileViewer.Utils.MVVM.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace Emignatik.NxFileViewer.Commands;

public class VerifyNcasIntegrityCommand : CommandBase, IVerifyNcasIntegrityCommand
{
    private readonly IFileOpeningService _fileOpeningService;
    private readonly IMainBackgroundTaskRunnerService _backgroundTaskRunnerService;
    private readonly IServiceProvider _serviceProvider;
    private readonly IAppSettings _appSettings;


    public VerifyNcasIntegrityCommand(IFileOpeningService fileOpeningService, IMainBackgroundTaskRunnerService backgroundTaskRunnerService, IServiceProvider serviceProvider, IAppSettings appSettings)
    {
        _fileOpeningService = fileOpeningService ?? throw new ArgumentNullException(nameof(fileOpeningService));
        _backgroundTaskRunnerService = backgroundTaskRunnerService ?? throw new ArgumentNullException(nameof(backgroundTaskRunnerService));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));

        _fileOpeningService.OpenedFileChanged += (_, _) =>
        {
            TriggerCanExecuteChanged();
        };

        _backgroundTaskRunnerService.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName == nameof(IMainBackgroundTaskRunnerService.IsRunning))
                TriggerCanExecuteChanged();
        };
    }

    public override bool CanExecute(object? parameter)
    {
        var openedFile = _fileOpeningService.OpenedFile;
        return openedFile != null && !_backgroundTaskRunnerService.IsRunning;
    }

    public override void Execute(object? parameter)
    {
        var openedFile = _fileOpeningService.OpenedFile;
        if (openedFile == null)
            return;

        var fileOverview = openedFile.Overview;
        var verifyNcasHashRunnable = _serviceProvider.GetRequiredService<IVerifyNcasIntegrityRunnable>();
        verifyNcasHashRunnable.Setup(fileOverview, _appSettings.IgnoreMissingDeltaFragments);
        _backgroundTaskRunnerService.RunAsync(verifyNcasHashRunnable);
    }
}

public interface IVerifyNcasIntegrityCommand : ICommand
{

}
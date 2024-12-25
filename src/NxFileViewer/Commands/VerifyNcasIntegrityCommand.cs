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
    private readonly IOpenedFileService _openedFileService;
    private readonly IMainBackgroundTaskRunnerService _backgroundTaskRunnerService;
    private readonly IServiceProvider _serviceProvider;
    private readonly IAppSettings _appSettings;


    public VerifyNcasIntegrityCommand(IOpenedFileService openedFileService, IMainBackgroundTaskRunnerService backgroundTaskRunnerService, IServiceProvider serviceProvider, IAppSettings appSettings)
    {
        _openedFileService = openedFileService ?? throw new ArgumentNullException(nameof(openedFileService));
        _backgroundTaskRunnerService = backgroundTaskRunnerService ?? throw new ArgumentNullException(nameof(backgroundTaskRunnerService));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));

        _openedFileService.OpenedFileChanged += (_, _) =>
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
        var openedFile = _openedFileService.OpenedFile;
        return openedFile != null && !_backgroundTaskRunnerService.IsRunning;
    }

    public override void Execute(object? parameter)
    {
        var openedFile = _openedFileService.OpenedFile;
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
using System;
using System.Windows.Input;
using Emignatik.NxFileViewer.Services.BackgroundTask;
using Emignatik.NxFileViewer.Services.BackgroundTask.RunnableImpl;
using Emignatik.NxFileViewer.Services.FileOpening;
using Emignatik.NxFileViewer.Utils.MVVM.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace Emignatik.NxFileViewer.Commands;

public class VerifyNcasHeaderSignatureCommand : CommandBase, IVerifyNcasHeaderSignatureCommand
{
    private readonly IOpenedFileService _openedFileService;
    private readonly IBackgroundTaskService _backgroundTaskService;
    private readonly IServiceProvider _serviceProvider;

    public VerifyNcasHeaderSignatureCommand(IOpenedFileService openedFileService, IBackgroundTaskService backgroundTaskService, IServiceProvider serviceProvider)
    {
        _openedFileService = openedFileService ?? throw new ArgumentNullException(nameof(openedFileService));
        _backgroundTaskService = backgroundTaskService ?? throw new ArgumentNullException(nameof(backgroundTaskService));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _openedFileService.OpenedFileChanged += (sender, args) =>
        {
            TriggerCanExecuteChanged();
        };

        _backgroundTaskService.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == nameof(IBackgroundTaskService.IsRunning))
                TriggerCanExecuteChanged();
        };
    }

    public override bool CanExecute(object? parameter)
    {
        var openedFile = _openedFileService.OpenedFile;
        return openedFile != null && openedFile.Overview.NcaItems.Count > 0 && !_backgroundTaskService.IsRunning;
    }

    public override void Execute(object? parameter)
    {
        var openedFile = _openedFileService.OpenedFile;
        if (openedFile == null)
            return;

        var fileOverview = openedFile.Overview;
        if (fileOverview.NcaItems.Count <= 0)
            return;

        var verifyNcasHeaderSignatureRunnable = _serviceProvider.GetRequiredService<IVerifyNcasHeaderSignatureRunnable>();
        verifyNcasHeaderSignatureRunnable.Setup(fileOverview);
        _backgroundTaskService.RunAsync(verifyNcasHeaderSignatureRunnable);
    }
}

public interface IVerifyNcasHeaderSignatureCommand : ICommand
{
}
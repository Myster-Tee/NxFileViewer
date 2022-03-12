namespace Emignatik.NxFileViewer.Services.BackgroundTask;

/// <summary>
/// The service in charge of launching tasks in background for the main Window
/// </summary>
public interface IMainBackgroundTaskRunnerService : IBackgroundTaskRunner
{
}

public class MainBackgroundTaskRunnerService : BackgroundTaskRunner, IMainBackgroundTaskRunnerService
{
}
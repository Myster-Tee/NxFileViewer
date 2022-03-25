using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Emignatik.NxFileViewer.Services.BackgroundTask;

/// <summary>
/// Exposes a logic for launching a task in background and reporting progress
/// </summary>
public interface IBackgroundTaskRunner : INotifyPropertyChanged
{
    /// <summary>
    /// The progress text
    /// </summary>
    public string? ProgressText { get; }

    /// <summary>
    /// The progress value in range of [0-100]
    /// </summary>
    public double ProgressValue { get; }

    /// <summary>
    /// The progress value as string
    /// </summary>
    public string ProgressValueText { get; }

    /// <summary>
    /// Get a boolean indicating if a task is currently running
    /// </summary>
    public bool IsRunning { get; }

    /// <summary>
    /// Get a boolean indicating that progress value is not available
    /// </summary>
    public bool IsIndeterminate { get; }

    /// <summary>
    /// Launches the specified <see cref="IRunnable"/>
    /// Throws if <see cref="IsRunning"/> is true
    /// </summary>
    /// <param name="runnable"></param>
    /// <returns></returns>
    Task RunAsync(IRunnable runnable);

    /// <summary>
    /// Launches the specified <see cref="IRunnable"/>
    /// Throws if <see cref="IsRunning"/> is true
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="runnable"></param>
    /// <returns></returns>
    Task<T> RunAsync<T>(IRunnable<T> runnable);

    /// <summary>
    /// Expose the command which can be used to cancel the currently running task
    /// </summary>
    public ICommand CancelCommand { get; }

}
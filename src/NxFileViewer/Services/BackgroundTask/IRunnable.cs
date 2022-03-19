using System.Threading;

namespace Emignatik.NxFileViewer.Services.BackgroundTask;

public interface IRunnableBase
{
    bool SupportsCancellation { get; }

    bool SupportProgress { get; }
}


/// <summary>
/// Exposes the logic of a runnable action
/// </summary>
public interface IRunnable: IRunnableBase
{

    void Run(IProgressReporter progressReporter, CancellationToken cancellationToken);
}


/// <summary>
/// </summary>
/// <typeparam name="T">Type of object returned by the method <see cref="Run"/></typeparam>
public interface IRunnable<out T> : IRunnableBase
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="progressReporter"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    T Run(IProgressReporter progressReporter, CancellationToken cancellationToken);
}
using System.Threading;

namespace Emignatik.NxFileViewer.Services.BackgroundTask
{
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

    public interface IRunnable<out T> : IRunnableBase
    {
        T Run(IProgressReporter progressReporter, CancellationToken cancellationToken);
    }
}
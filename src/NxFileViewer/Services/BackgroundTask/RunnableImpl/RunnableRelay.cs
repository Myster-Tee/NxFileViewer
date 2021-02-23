using System;
using System.Threading;

namespace Emignatik.NxFileViewer.Services.BackgroundTask.RunnableImpl
{
    public class RunnableRelay : IRunnable
    {
        private readonly RunHandler _runHandler;

        public RunnableRelay(RunHandler runHandler)
        {
            _runHandler = runHandler ?? throw new ArgumentNullException(nameof(runHandler));
        }

        public bool SupportsCancellation { get; set; }

        public bool SupportProgress { get; set; }

        public void Run(IProgressReporter progressReporter, CancellationToken cancellationToken)
        {
            _runHandler(progressReporter, cancellationToken);
        }
    }

    public class RunnableRelay<T> : IRunnable<T>
    {
        private readonly RunHandler<T> _runHandler;

        public RunnableRelay(RunHandler<T> runHandler)
        {
            _runHandler = runHandler ?? throw new ArgumentNullException(nameof(runHandler));
        }

        public bool SupportsCancellation { get; set; }

        public bool SupportProgress { get; set; }

        public T Run(IProgressReporter progressReporter, CancellationToken cancellationToken)
        {
            return _runHandler(progressReporter, cancellationToken);
        }
    }

    public delegate void RunHandler(IProgressReporter progressReporter, CancellationToken cancellationToken);
    public delegate T RunHandler<out T>(IProgressReporter progressReporter, CancellationToken cancellationToken);
}
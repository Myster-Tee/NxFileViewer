using log4net.Appender;
using log4net.Core;

namespace Emignatik.NxFileViewer.Logging
{
    public class LogNotifier : AppenderSkeleton
    {
        public event LogEventHandler Log;

        protected override void Append(LoggingEvent loggingEvent)
        {
            NotifyLog(loggingEvent);
        }

        protected virtual void NotifyLog(LoggingEvent loggingEvent)
        {
            Log?.Invoke(this, new LogEventHandlerArgs(loggingEvent));
        }
    }

    public delegate void LogEventHandler(object sender, LogEventHandlerArgs args);

    public class LogEventHandlerArgs
    {
        public LogEventHandlerArgs(LoggingEvent loggingEvent)
        {
            LogEvent = loggingEvent;
        }

        public LoggingEvent LogEvent { get; }

    }
}

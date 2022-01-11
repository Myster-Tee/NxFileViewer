using System;
using LibHac.Common;

namespace Emignatik.NxFileViewer.Services.BackgroundTask
{
    /// <summary>
    /// </summary>
    /// <param name="value">A value in range of [0-1]</param>
    public delegate void ProgressValueHandler(double value);

    public delegate void ProgressMessageHandler(string message);

    /// <summary>
    ///  Redirects the LibHac <see cref="IProgressReport"/> to the <see cref="IProgressReporter"/>
    /// </summary>
    public class LibHacProgressReportRelay : IProgressReport
    {
        private readonly ProgressValueHandler? _progressValueHandler;
        private readonly ProgressMessageHandler? _progressMessageHandler;

        private long _currentValue = 0;
        private long _total = 0;

        public LibHacProgressReportRelay(ProgressValueHandler? progressValueHandler, ProgressMessageHandler? progressMessageHandler)
        {
            _progressValueHandler = progressValueHandler ?? throw new ArgumentNullException(nameof(progressValueHandler));
            _progressMessageHandler = progressMessageHandler ?? throw new ArgumentNullException(nameof(progressMessageHandler));
            UpdateProgress();
        }

        void IProgressReport.Report(long value)
        {
            _currentValue = value;
            UpdateProgress();
        }

        void IProgressReport.ReportAdd(long value)
        {
            _currentValue += value;
            UpdateProgress();
        }

        void IProgressReport.SetTotal(long value)
        {
            _total = value;
            UpdateProgress();
        }

        void IProgressReport.LogMessage(string message)
        {
            _progressMessageHandler?.Invoke(message);
        }

        private void UpdateProgress()
        {
            if (_progressValueHandler == null)
                return;
            double value;
            if (_total <= 0)
                value = 0.0;
            else
                value = _currentValue / (double)_total;
            _progressValueHandler(value);
        }

    }
}
namespace Emignatik.NxFileViewer.Services.BackgroundTask
{
    public interface IProgressReporter
    {
        /// <summary>
        /// Set the progress mode
        /// </summary>
        /// <param name="isIndeterminate"></param>
        void SetMode(bool isIndeterminate);

        /// <summary>
        /// Sets the progress text
        /// </summary>
        /// <param name="text"></param>

        void SetText(string text);

        /// <summary>
        /// Sets the progress percentage value
        /// </summary>
        /// <param name="value">A value in range of [0-1]</param>
        void SetPercentage(double value);
    }
}
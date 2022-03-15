using System;

namespace Emignatik.NxFileViewer.Services.GlobalEvents
{
    public interface IAppEvents
    {
        /// <summary>
        /// Fired when application is shutting down
        /// </summary>
        public event Action AppShuttingDown;
    }
}

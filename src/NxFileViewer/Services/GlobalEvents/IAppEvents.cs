using System;

namespace Emignatik.NxFileViewer.Services.GlobalEvents
{
    public interface IAppEvents
    {
        public event Action AppShuttingDown;

    }
}

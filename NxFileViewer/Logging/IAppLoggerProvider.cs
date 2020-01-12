using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer.Logging
{
    public interface IAppLoggerProvider : ILoggerProvider, ILogSource
    {
    }
}
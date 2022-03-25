using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer.Logging;

public delegate void LogHandler(LogLevel logLevel, string message);

/// <summary>
/// Provides log events 
/// </summary>
public interface ILogSource
{
    event LogHandler? Log;
}
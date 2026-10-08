using System.Threading;
using Emignatik.NxFileViewer.Services.BackgroundTask;

namespace Emignatik.NxFileViewer.Services.Nsz;

public enum NszOperation { Compress, Decompress }

/// <summary>Out-of-process plugin boundary; the viewer owns verification and file publication.</summary>
public interface INszPlugin
{
    void Convert(string source, string outputDirectory, NszOperation operation,
        IProgressReporter progress, CancellationToken cancellationToken);
}

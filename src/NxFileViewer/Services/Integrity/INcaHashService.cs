using System;
using System.Threading;
using System.Threading.Tasks;
using Emignatik.NxFileViewer.Services.BackgroundTask;
using LibHac.Tools.FsSystem.NcaUtils;

namespace Emignatik.NxFileViewer.Services.Integrity;

public interface INcaHashService
{

    /// <summary>
    /// Computes the SHA256 hash of the given NCA file.
    /// </summary>
    /// <param name="nca"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="progressReporter"></param>
    /// <param name="bufferSize"></param>
    /// <returns></returns>
    /// <exception cref="OperationCanceledException"></exception>
    Task<byte[]> ComputeSha256Async(Nca nca, CancellationToken? cancellationToken = null, IProgressReporter? progressReporter = null, int? bufferSize = null);

}
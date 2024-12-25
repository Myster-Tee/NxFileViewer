using System.Threading;
using System.Threading.Tasks;
using Emignatik.NxFileViewer.Models.TreeItems.Impl;
using Emignatik.NxFileViewer.Services.BackgroundTask;

namespace Emignatik.NxFileViewer.Services.Integrity;

public interface INcaItemIntegrityService
{
    /// <summary>
    /// Checks the integrity of the given NCA item.
    /// </summary>
    /// <param name="ncaItem"></param>
    /// <param name="expectedHash">When null, hash will be extracted from <see cref="NcaItem.Id"/></param>
    /// <param name="cancellationToken"></param>
    /// <param name="progressReporter"></param>
    /// <param name="bufferSize"></param>
    /// <returns></returns>
    public Task<NcaIntegrity> SafeCheckAsync(NcaItem ncaItem, byte[]? expectedHash, CancellationToken? cancellationToken = null, IProgressReporter? progressReporter = default, int? bufferSize = null);
}


public enum NcaIntegrity
{
    /// <summary>
    /// Original Nintendo Content Archive (NCA) file.
    /// </summary>
    Original,
    /// <summary>
    /// File has been modified and is not original.
    /// </summary>
    Modified,
    /// <summary>
    /// File is corrupted.
    /// </summary>
    Corrupted,
    /// <summary>
    /// An error occurred while checking the file.
    /// </summary>
    Error,
}
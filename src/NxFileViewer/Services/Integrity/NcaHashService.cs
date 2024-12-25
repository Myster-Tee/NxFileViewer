using System;
using System.Security.Cryptography;
using Emignatik.NxFileViewer.Services.BackgroundTask;
using LibHac;
using LibHac.Tools.FsSystem.NcaUtils;
using System.Threading;
using System.Threading.Tasks;
using LibHac.Fs;
using LibHac.Tools.FsSystem;

namespace Emignatik.NxFileViewer.Services.Integrity;

public class NcaHashService : INcaHashService
{
    public async Task<byte[]> ComputeSha256Async(Nca nca, CancellationToken? cancellationToken = null, IProgressReporter? progressReporter = null, int? bufferSize = null)
    {
        if (nca == null)
            throw new ArgumentNullException(nameof(nca));

        if (bufferSize is <= 0)
            throw new ArgumentOutOfRangeException(nameof(bufferSize), "Buffer size can't be less or equal to zero.");

        using var file = nca.BaseStorage.AsFile(OpenMode.Read);

        if (file.GetSize(out var fileSize) != Result.Success)
            fileSize = 0;

        var sha256 = SHA256.Create();
        await using var ncaStream = file.AsStream();
        var buffer = new byte[bufferSize ?? 4096];

        decimal totalRead = 0;
        int read;
        while ((read = await ncaStream.ReadAsync(buffer)) > 0)
        {
            cancellationToken?.ThrowIfCancellationRequested();

            sha256.TransformBlock(buffer, 0, read, null, 0);
            totalRead += read;
            progressReporter?.SetPercentage(fileSize == 0 ? 0.0 : (double)(totalRead / fileSize));
        }

        sha256.TransformFinalBlock([], 0, 0);
        var currentNcaHash = sha256.Hash;

        if (currentNcaHash == null)
            throw new InvalidOperationException("Hash is null.");

        progressReporter?.SetPercentage(1);

        return currentNcaHash;
    }

}
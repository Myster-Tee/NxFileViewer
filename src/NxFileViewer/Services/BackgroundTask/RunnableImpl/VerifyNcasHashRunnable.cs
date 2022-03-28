using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Models.Overview;
using Emignatik.NxFileViewer.Models.TreeItems.Impl;
using Emignatik.NxFileViewer.Settings;
using Emignatik.NxFileViewer.Utils;
using LibHac;
using LibHac.Fs.Fsa;
using LibHac.Tools.FsSystem;
using LibHac.Tools.Ncm;
using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer.Services.BackgroundTask.RunnableImpl;

public class VerifyNcasHashRunnable : IVerifyNcasHashRunnable
{
    private readonly IAppSettings _appSettings;
    private const string NCA_HASH_CATEGORY = "NcaHash";

    private FileOverview? _fileOverview;
    private readonly ILogger _logger;

    public VerifyNcasHashRunnable(ILoggerFactory loggerFactory, IAppSettings appSettings)
    {
        _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
        _logger = (loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory))).CreateLogger(this.GetType());
    }

    public bool SupportsCancellation => true;

    public bool SupportProgress => true;

    public void Setup(FileOverview fileOverview)
    {
        _fileOverview = fileOverview ?? throw new ArgumentNullException(nameof(fileOverview));
    }

    public void Run(IProgressReporter progressReporter, CancellationToken cancellationToken)
    {
        if (_fileOverview == null)
            throw new InvalidOperationException($"{nameof(Setup)} should be called first.");

        _logger.LogInformation(LocalizationManager.Instance.Current.Keys.NcaHash_VerificationStart_Log);
        try
        {
            VerifyHashes(progressReporter, _fileOverview, cancellationToken);
        }
        finally
        {
            _logger.LogInformation(LocalizationManager.Instance.Current.Keys.NcaHash_VerificationEnd_Log);
        }
    }


    private void VerifyHashes(IProgressReporter progressReporter, FileOverview fileOverview, CancellationToken cancellationToken)
    {
        fileOverview.NcasHashExceptions = null;

        // Build the list of all CnmtContentEntry with their corresponding CnmtItem
        // The CnmtContentEntry contains a reference to an NCA with the expected Sha256 hash
        var itemsToProcess = new List<Tuple<CnmtContentEntry, CnmtItem>>();
        foreach (var cnmtItem in fileOverview.CnmtContainers.Select(container => container.CnmtItem))
        {
            itemsToProcess.AddRange(cnmtItem.Cnmt.ContentEntries.Select(cnmtContentEntry =>
            {
                cnmtItem.Errors.RemoveAll(NCA_HASH_CATEGORY);
                return new Tuple<CnmtContentEntry, CnmtItem>(cnmtContentEntry, cnmtItem);
            }));
        }

        if (itemsToProcess.Count <= 0)
        {
            fileOverview.NcasHashValidity = NcasValidity.NoNca;
            return;
        }

        var occurredExceptions = new List<Exception>();
        var allValid = true;
        var operationCanceled = false;
        try
        {

            fileOverview.NcasHashValidity = NcasValidity.InProgress;

            var processedItem = 0;
            foreach (var (cnmtContentEntry, cnmtItem) in itemsToProcess)
            {
                cancellationToken.ThrowIfCancellationRequested();

                progressReporter.SetPercentage(0.0);

                var progressText = LocalizationManager.Instance.Current.Keys.NcaHash_ProgressText.SafeFormat(++processedItem, itemsToProcess.Count);
                progressReporter.SetText(progressText);

                var expectedNcaHash = cnmtContentEntry.Hash;
                var expectedNcaId = cnmtContentEntry.NcaId.ToStrId();

                // Search for the referenced NCA
                var ncaItem = fileOverview.NcaItems.FirstOrDefault(item => item.FileName.StartsWith(expectedNcaId + ".", StringComparison.OrdinalIgnoreCase));

                if (ncaItem == null)
                {
                    cnmtItem.Errors.Add(NCA_HASH_CATEGORY, LocalizationManager.Instance.Current.Keys.NcaHash_CnmtItem_Error_NcaMissing.SafeFormat(expectedNcaId));
                    continue;
                }

                try
                {
                    ncaItem.Errors.RemoveAll(NCA_HASH_CATEGORY);

                    //=============================================//
                    //===============> Verify Hash <===============//
                    VerifyFileHash(progressReporter, ncaItem.File, _appSettings.ProgressBufferSize, expectedNcaHash, cancellationToken, out var hashValid);
                    //===============> Verify Hash <===============//
                    //=============================================//

                    if (!hashValid)
                    {
                        allValid = false;
                        ncaItem.Errors.Add(NCA_HASH_CATEGORY, LocalizationManager.Instance.Current.Keys.NcaHash_NcaItem_Invalid);
                        _logger.LogError(LocalizationManager.Instance.Current.Keys.NcaHash_Invalid_Log.SafeFormat(ncaItem.DisplayName));
                    }
                    else
                        _logger.LogInformation(LocalizationManager.Instance.Current.Keys.NcaHash_Valid_Log.SafeFormat(ncaItem.DisplayName));

                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    allValid = false;
                    occurredExceptions.Add(ex);
                    ncaItem.Errors.Add(NCA_HASH_CATEGORY, LocalizationManager.Instance.Current.Keys.NcaHash_NcaItem_Exception.SafeFormat(ex.Message));
                    _logger.LogError(ex, LocalizationManager.Instance.Current.Keys.NcaHash_Exception_Log.SafeFormat(ncaItem.DisplayName, ex.Message));
                }
            }

        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning(LocalizationManager.Instance.Current.Keys.Log_NcaHashCanceled);
            operationCanceled = true;
        }
        catch (Exception ex)
        {
            allValid = false;
            occurredExceptions.Add(ex);
            _logger.LogError(ex, LocalizationManager.Instance.Current.Keys.NcasHash_Error_Log.SafeFormat(ex.Message));
        }

        if (operationCanceled)
        {
            fileOverview.NcasHashValidity = NcasValidity.Unchecked;
        }
        else if (occurredExceptions.Count > 0)
        {
            fileOverview.NcasHashExceptions = occurredExceptions;
            fileOverview.NcasHashValidity = NcasValidity.Error;
        }
        else
        {
            fileOverview.NcasHashValidity = allValid ? NcasValidity.Valid : NcasValidity.Invalid;
        }

    }

    private static void VerifyFileHash(IProgressReporter progressReporter, IFile file, int bufferSize, IReadOnlyCollection<byte> expectedNcaHash, CancellationToken cancellationToken, out bool hashValid)
    {
        if (file.GetSize(out var fileSize) != Result.Success)
            fileSize = 0;

        var sha256 = SHA256.Create();

        var ncaStream = file.AsStream();
        var buffer = new byte[bufferSize];

        decimal totalRead = 0;
        int read;
        while ((read = ncaStream.Read(buffer, 0, buffer.Length)) > 0)
        {
            cancellationToken.ThrowIfCancellationRequested();

            sha256.TransformBlock(buffer, 0, read, null, 0);
            totalRead += read;
            progressReporter.SetPercentage(fileSize == 0 ? 0.0 : (double)(totalRead / fileSize));
        }

        sha256.TransformFinalBlock(Array.Empty<byte>(), 0, 0);
        var currentNcaHash = sha256.Hash!;

        hashValid = IsHashEqual(currentNcaHash, expectedNcaHash);
    }

    private static bool IsHashEqual(IReadOnlyList<byte> currentNcaHash, IReadOnlyCollection<byte> expectedNcaHash)
    {
        if (currentNcaHash.Count != expectedNcaHash.Count)
            return false;

        return !expectedNcaHash.Where((expectedByte, byteIndex) => currentNcaHash[byteIndex] != expectedByte).Any();
    }

}

public interface IVerifyNcasHashRunnable : IRunnable
{
    void Setup(FileOverview fileOverview);
}
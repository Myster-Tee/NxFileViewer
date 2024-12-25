using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Models.TreeItems;
using Emignatik.NxFileViewer.Models.TreeItems.Impl;
using Emignatik.NxFileViewer.Services.BackgroundTask;
using LibHac.Common;
using LibHac.Tools.FsSystem.NcaUtils;
using Microsoft.Extensions.Logging;
using Category = Emignatik.NxFileViewer.Models.TreeItems.Category;

namespace Emignatik.NxFileViewer.Services.Integrity;

public class NcaItemIntegrityService : INcaItemIntegrityService
{
    private readonly INcaHashService _ncaHashService;
    private readonly ILogger<NcaItemIntegrityService> _logger;

    public NcaItemIntegrityService(INcaHashService ncaHashService, ILogger<NcaItemIntegrityService> logger)
    {
        _ncaHashService = ncaHashService ?? throw new ArgumentNullException(nameof(ncaHashService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<NcaIntegrity> SafeCheckAsync(NcaItem ncaItem, byte[]? expectedHash, CancellationToken? cancellationToken = null, IProgressReporter? progressReporter = default, int? bufferSize = null)
    {
        if (ncaItem == null)
            throw new ArgumentNullException(nameof(ncaItem));

        // Clear previous errors
        ncaItem.Errors.RemoveAllOfCategory(Category.IntegrityCheck);

        Nca nca;
        try
        {
            nca = ncaItem.GetOriginalNca();
        }
        catch (Exception ex)
        {
            ncaItem.Errors.Add(Category.IntegrityCheck, LocalizationManager.Instance.Current.Keys.NcaIntegrity_GetOriginalNcaError.SafeFormat(ex.Message));
            _logger.LogError(LocalizationManager.Instance.Current.Keys.NcaIntegrity_GetOriginalNcaError_Log.SafeFormat(ncaItem.DisplayName, ex.Message));
            return NcaIntegrity.Error;
        }

        cancellationToken?.ThrowIfCancellationRequested();

        // Check NCA signature
        bool signatureValid;
        Exception? signatureEx = null;
        try
        {
            var signatureValidity = nca.VerifyHeaderSignature();
            ncaItem.HeaderSignatureValidity = signatureValidity;

            switch (signatureValidity)
            {
                case Validity.Valid:
                    signatureValid = true;
                    _logger.LogInformation(LocalizationManager.Instance.Current.Keys.NcaHeaderSignature_Valid_Log.SafeFormat(ncaItem.DisplayName, signatureValidity.ToString()));
                    break;
                default:
                    signatureValid = false;
                    ncaItem.Errors.Add(Category.IntegrityCheck, LocalizationManager.Instance.Current.Keys.NcaHeaderSignature_Invalid.SafeFormat(signatureValidity.ToString()));
                    _logger.LogError(LocalizationManager.Instance.Current.Keys.NcaHeaderSignature_Invalid_Log.SafeFormat(ncaItem.DisplayName, signatureValidity.ToString()));
                    break;
            }
        }
        catch (Exception ex)
        {
            signatureValid = false;
            signatureEx = ex;
            ncaItem.Errors.Add(Category.IntegrityCheck, LocalizationManager.Instance.Current.Keys.NcaHeaderSignature_Error.SafeFormat(ex.Message));
            _logger.LogError(ex, LocalizationManager.Instance.Current.Keys.NcaHeaderSignature_Error_log.SafeFormat(ncaItem.DisplayName, ex.Message));
        }

        cancellationToken?.ThrowIfCancellationRequested();

        // Check NCA hash
        bool hashValid;
        Exception? hashEx = null;
        try
        {
            var actualHash = await _ncaHashService.ComputeSha256Async(nca, cancellationToken, progressReporter, bufferSize);

            if (expectedHash == null)
            {
                if (ncaItem.TryGetExpectedHashFromId(out var expectedHashFromId))
                {
                    // Hash from name is shorter than the real hash
                    var actualHashShort = actualHash[new Range(0, expectedHashFromId.Length)];
                    hashValid = actualHashShort.SequenceEqual(expectedHashFromId);
                }
                else
                {
                    ncaItem.Errors.Add(Category.IntegrityCheck, LocalizationManager.Instance.Current.Keys.NcaHash_NcaItem_CantExtractHashFromName);
                    _logger.LogError(LocalizationManager.Instance.Current.Keys.NcaHash_CantExtractHashFromName_Log.SafeFormat(ncaItem.DisplayName));
                    hashValid = false;
                }
            }
            else
            {
                hashValid = expectedHash.SequenceEqual(actualHash);
            }

            ncaItem.HashValid = hashValid;
            if (!hashValid)
            {
                ncaItem.Errors.Add(Category.IntegrityCheck, LocalizationManager.Instance.Current.Keys.NcaHash_NcaItem_Invalid);
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
            hashValid = false;
            hashEx = ex;
            ncaItem.Errors.Add(Category.IntegrityCheck, LocalizationManager.Instance.Current.Keys.NcaHash_NcaItem_Exception.SafeFormat(ex.Message));
            _logger.LogError(ex, LocalizationManager.Instance.Current.Keys.NcaHash_Exception_Log.SafeFormat(ncaItem.DisplayName, ex.Message));
        }

        if (signatureEx != null || hashEx != null)
            return NcaIntegrity.Error;

        var ncaIntegrity = (signatureValid, hashValid) switch
        {
            (false, false) => NcaIntegrity.Corrupted,
            (true, false) => NcaIntegrity.Corrupted,
            (false, true) => NcaIntegrity.Modified,
            (true, true) => NcaIntegrity.Original
        };
        return ncaIntegrity;
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Models.Overview;
using Emignatik.NxFileViewer.Models.TreeItems;
using Emignatik.NxFileViewer.Models.TreeItems.Impl;
using Emignatik.NxFileViewer.Services.Integrity;
using Emignatik.NxFileViewer.Settings;
using LibHac.Ncm;
using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer.Services.BackgroundTask.RunnableImpl;

public class VerifyNcasIntegrityRunnable : IVerifyNcasIntegrityRunnable
{
    private readonly IAppSettings _appSettings;
    private readonly INcaItemIntegrityService _ncaItemIntegrityService;

    private FileOverview? _fileOverview;
    private readonly ILogger _logger;
    private bool _ignoreMissingDeltaFragments;

    public VerifyNcasIntegrityRunnable(ILogger<VerifyNcasIntegrityRunnable> logger, IAppSettings appSettings, INcaItemIntegrityService ncaItemIntegrityService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
        _ncaItemIntegrityService = ncaItemIntegrityService ?? throw new ArgumentNullException(nameof(ncaItemIntegrityService));
    }

    public bool SupportsCancellation => true;

    public bool SupportProgress => true;

    public void Setup(FileOverview fileOverview, bool ignoreMissingDeltaFragments)
    {
        _fileOverview = fileOverview ?? throw new ArgumentNullException(nameof(fileOverview));
        _ignoreMissingDeltaFragments = ignoreMissingDeltaFragments;
    }

    public void Run(IProgressReporter progressReporter, CancellationToken cancellationToken)
    {
        if (_fileOverview == null)
            throw new InvalidOperationException($"{nameof(Setup)} should be called first.");

        _logger.LogInformation(LocalizationManager.Instance.Current.Keys.NcaHash_VerificationStart_Log);
        try
        {
            Verify(progressReporter, _fileOverview, cancellationToken).Wait(cancellationToken);
        }
        finally
        {
            _logger.LogInformation(LocalizationManager.Instance.Current.Keys.NcaHash_VerificationEnd_Log);
        }
    }


    private async Task Verify(IProgressReporter progressReporter, FileOverview fileOverview, CancellationToken cancellationToken)
    {
        // Build the list NCA items to process
        var atLeastOneNcaMissing = false;
        var ncaItemsToProcess = new List<(NcaItem, byte[]?)>();
        foreach (var cnmtItem in fileOverview.CnmtContainers.Select(container => container.CnmtItem))
        {
            // Add the NCA of the CNMT (which is not referenced by the CNMT content entries)
            ncaItemsToProcess.Add((cnmtItem.ParentItem.ParentItem, null));

            foreach (var cnmtContentEntryItem in cnmtItem.ChildItems)
            {
                cnmtContentEntryItem.Errors.RemoveAllOfCategory(Category.IntegrityCheck);

                var ncaItem = cnmtContentEntryItem.FindReferencedNcaItem();
                if (ncaItem == null)
                {
                    if (cnmtContentEntryItem.NcaContentType == ContentType.DeltaFragment && _ignoreMissingDeltaFragments)
                        // Delta fragment is missing, but it's ok
                        continue;
                    atLeastOneNcaMissing = true;
                    cnmtContentEntryItem.Errors.Add(Category.IntegrityCheck, LocalizationManager.Instance.Current.Keys.NcasIntegrity_Error_NcaMissing.SafeFormat(cnmtContentEntryItem.NcaId));
                    continue;
                }
                ncaItemsToProcess.Add((ncaItem, cnmtContentEntryItem.NcaHash));
            }
        }

        var atLeastOneModified = false; var atLeastOneCorrupted = false; var atLeastOneError = false;

        try
        {

            fileOverview.NcasIntegrity = NcasIntegrity.InProgress;

            var processedItem = 0;
            foreach (var (ncaItem, expectedHash) in ncaItemsToProcess)
            {
                cancellationToken.ThrowIfCancellationRequested();

                progressReporter.SetPercentage(0.0);

                var progressText = LocalizationManager.Instance.Current.Keys.NcaHash_ProgressText.SafeFormat(++processedItem, ncaItemsToProcess.Count);
                progressReporter.SetText(progressText);

                //=============================================//
                //===============> Verify Hash <===============//
                var ncaIntegrity = await _ncaItemIntegrityService.SafeCheckAsync(ncaItem, expectedHash, cancellationToken, progressReporter, _appSettings.ProgressBufferSize);
                //===============> Verify Hash <===============//
                //=============================================//

                switch (ncaIntegrity)
                {
                    case NcaIntegrity.Original:
                        break;
                    case NcaIntegrity.Modified:
                        atLeastOneModified = true;
                        break;
                    case NcaIntegrity.Corrupted:
                        atLeastOneCorrupted = true;
                        break;
                    case NcaIntegrity.Error:
                        atLeastOneError = true;
                        break;
                }

            }

            if (ncaItemsToProcess.Count <= 0)
                fileOverview.NcasIntegrity = NcasIntegrity.NoNca;
            else if (atLeastOneError)
                fileOverview.NcasIntegrity = NcasIntegrity.Error;
            else if (atLeastOneCorrupted)
                fileOverview.NcasIntegrity = NcasIntegrity.Corrupted;
            else if (atLeastOneModified)
                fileOverview.NcasIntegrity = NcasIntegrity.Modified;
            else if (atLeastOneNcaMissing)
                fileOverview.NcasIntegrity = NcasIntegrity.Incomplete;
            else
                fileOverview.NcasIntegrity = NcasIntegrity.Original;
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning(LocalizationManager.Instance.Current.Keys.Log_NcasIntegrityCanceled);
            fileOverview.NcasIntegrity = NcasIntegrity.Unchecked;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, LocalizationManager.Instance.Current.Keys.NcasIntegrity_Error_Log.SafeFormat(ex.Message));
            fileOverview.NcasIntegrity = NcasIntegrity.Error;
        }

    }

}

public interface IVerifyNcasIntegrityRunnable : IRunnable
{
    /// <summary>
    /// Setup
    /// </summary>
    /// <param name="fileOverview"></param>
    /// <param name="ignoreMissingDeltaFragments"></param>
    void Setup(FileOverview fileOverview, bool ignoreMissingDeltaFragments);
}
using System;
using System.Collections.Generic;
using System.Threading;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Model.Overview;
using LibHac;
using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer.Services.BackgroundTask.RunnableImpl
{
    public class VerifyNcasHeaderSignatureRunnable : IVerifyNcasHeaderSignatureRunnable
    {
        private const string NCA_HEADER_SIGNATURE_CATEGORY = "NcaHeaderSignature";

        private FileOverview? _fileOverview;
        private readonly ILogger _logger;

        public VerifyNcasHeaderSignatureRunnable(ILoggerFactory loggerFactory)
        {
            _logger = (loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory))).CreateLogger(this.GetType());
        }

        public bool SupportsCancellation => false;

        public bool SupportProgress => true;

        public void Setup(FileOverview fileOverview)
        {
            _fileOverview = fileOverview ?? throw new ArgumentNullException(nameof(fileOverview));
        }

        public void Run(IProgressReporter progressReporter, CancellationToken cancellationToken)
        {
            if (_fileOverview == null)
                throw new InvalidOperationException($"{nameof(Setup)} should be called first.");

            _logger.LogInformation(LocalizationManager.Instance.Current.Keys.NcaHeaderSignature_VerificationStart_Log);
            try
            {
                VerifySignatures(progressReporter, _fileOverview);
            }
            finally
            {
                _logger.LogInformation(LocalizationManager.Instance.Current.Keys.NcaHeaderSignature_VerificationEnd_Log);
            }
        }

        private void VerifySignatures(IProgressReporter progressReporter, FileOverview fileOverview)
        {
            fileOverview.NcasHeadersSignatureExceptions = null;
            var ncaItems = fileOverview.NcaItems;
            if (ncaItems.Length <= 0)
            {
                fileOverview.NcasHeadersSignatureValidity = NcasValidity.NoNca;
                return;
            }
            fileOverview.NcasHeadersSignatureValidity = NcasValidity.InProgress;

            var allValid = true;
            var occurredExceptions = new List<Exception>();
            try
            {
                progressReporter.SetPercentage(0);

                var i = 0;
                foreach (var ncaItem in ncaItems)
                {
                    ncaItem.Errors.RemoveAll(NCA_HEADER_SIGNATURE_CATEGORY);

                    try
                    {
                        //=============================================//
                        //===============> Verify Hash <===============//
                        var validity = ncaItem.Nca.VerifyHeaderSignature();
                        //===============> Verify Hash <===============//
                        //=============================================//
                        ncaItem.HeaderSignatureValidity = validity;

                        if (validity != Validity.Valid)
                        {
                            allValid = false;
                            ncaItem.Errors.Add(NCA_HEADER_SIGNATURE_CATEGORY, LocalizationManager.Instance.Current.Keys.NcaHeaderSignature_Invalid.SafeFormat(validity.ToString()));
                            _logger.LogError(LocalizationManager.Instance.Current.Keys.NcaHeaderSignature_Invalid_Log.SafeFormat(ncaItem.DisplayName, validity.ToString()));
                        }
                        else
                            _logger.LogInformation(LocalizationManager.Instance.Current.Keys.NcaHeaderSignature_Valid_Log.SafeFormat(ncaItem.DisplayName, validity.ToString()));
                    }
                    catch (Exception ex)
                    {
                        allValid = false;
                        occurredExceptions.Add(ex);
                        ncaItem.Errors.Add(NCA_HEADER_SIGNATURE_CATEGORY, LocalizationManager.Instance.Current.Keys.NcaHeaderSignature_Error.SafeFormat(ex.Message));
                        _logger.LogError(ex, LocalizationManager.Instance.Current.Keys.NcaHeaderSignature_Error_log.SafeFormat(ncaItem.DisplayName, ex.Message));
                    }

                    progressReporter.SetPercentage(++i / (double)ncaItems.Length);
                }

                progressReporter.SetPercentage(1);
            }
            catch (Exception ex)
            {
                allValid = false;
                occurredExceptions.Add(ex);
                _logger.LogError(ex, LocalizationManager.Instance.Current.Keys.NcasHeaderSignature_Error_Log.SafeFormat(ex.Message));
            }

            if (occurredExceptions.Count > 0)
            {
                fileOverview.NcasHeadersSignatureExceptions = occurredExceptions;
                fileOverview.NcasHeadersSignatureValidity = NcasValidity.Error;
            }
            else
            {
                fileOverview.NcasHeadersSignatureValidity = allValid ? NcasValidity.Valid : NcasValidity.Invalid;
            }
        }
    }

    public interface IVerifyNcasHeaderSignatureRunnable : IRunnable
    {
        void Setup(FileOverview fileOverview);
    }
}

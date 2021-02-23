using System;
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

            var ncaItems = _fileOverview.NcaItems;
            if (ncaItems.Length <= 0)
            {
                _fileOverview.NcasHeadersSignatureValidity = NcasValidity.NoNca;
                return;
            }
            _fileOverview.NcasHeadersSignatureValidity = NcasValidity.InProgress;

            try
            {
                progressReporter.SetPercentage(0);

                var i = 0;
                var allValid = true;
                foreach (var ncaItem in ncaItems)
                {
                    ncaItem.Errors.RemoveAll(NCA_HEADER_SIGNATURE_CATEGORY);
                    var validity = ncaItem.Nca.VerifyHeaderSignature();
                    ncaItem.HeaderSignatureValidity = validity;

                    if (validity != Validity.Valid)
                    {
                        ncaItem.Errors.Add(NCA_HEADER_SIGNATURE_CATEGORY, LocalizationManager.Instance.Current.Keys.NcaHeaderSignatureInvalid.SafeFormat(validity.ToString()));
                        _logger.LogError(LocalizationManager.Instance.Current.Keys.NcaHeaderSignatureInvalid_Log.SafeFormat(ncaItem.DisplayName, validity.ToString()));

                        allValid = false;
                    }
                    else
                    {
                        _logger.LogInformation(LocalizationManager.Instance.Current.Keys.NcaHeaderSignatureValid_Log.SafeFormat(ncaItem.DisplayName, validity.ToString()));
                    }
                    progressReporter.SetPercentage(++i / (double)ncaItems.Length);
                }

                progressReporter.SetPercentage(1);

                _fileOverview.NcasHeadersSignatureValidity = allValid ? NcasValidity.Valid : NcasValidity.Invalid;
            }
            catch (Exception ex)
            {
                _fileOverview.NcasHeadersSignatureValidity = NcasValidity.Error;
                _logger.LogError(LocalizationManager.Instance.Current.Keys.NcasHeaderSignatureError_Log.SafeFormat(ex.Message));
            }

        }


    }

    public interface IVerifyNcasHeaderSignatureRunnable : IRunnable
    {
        void Setup(FileOverview fileOverview);
    }
}

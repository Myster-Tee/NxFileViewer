using System;
using System.Threading;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Model.Overview;
using LibHac;
using LibHac.FsSystem.NcaUtils;
using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer.Services.BackgroundTask.RunnableImpl
{
    public class VerifyNcasHashRunnable : IVerifyNcasHashRunnable
    {
        private const string NCA_HASH_CATEGORY = "NcaHash";

        private FileOverview? _fileOverview;
        private readonly ILogger _logger;

        public VerifyNcasHashRunnable(ILoggerFactory loggerFactory)
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
                _fileOverview.NcasHashValidity = NcasValidity.NoNca;
                return;
            }
            _fileOverview.NcasHashValidity = NcasValidity.InProgress;

            try
            {
                var allValid = true;
                var ncaNum = 0;
                foreach (var ncaItem in ncaItems)
                {
                    ncaNum++;
                    ncaItem.Errors.RemoveAll(NCA_HASH_CATEGORY);

                    //==============================================//
                    //===============> Compute Hash <===============//
                    var numCopy = ncaNum;
                    var validity = ncaItem.Nca.VerifyNca(new LibHacProgressReportRelay(value =>
                    {
                        progressReporter.SetPercentage(value);
                    }, message =>
                    {
                        progressReporter.SetText($"{numCopy}/{ncaItems.Length}: {message}");
                    }));
                    //===============> Compute Hash <===============//
                    //==============================================//

                    ncaItem.HashValidity = validity;
                    if (validity != Validity.Valid)
                    {
                        ncaItem.Errors.Add(NCA_HASH_CATEGORY, LocalizationManager.Instance.Current.Keys.NcaHashInvalid.SafeFormat(validity.ToString()));
                        _logger.LogError(LocalizationManager.Instance.Current.Keys.NcaHashInvalid_Log.SafeFormat(ncaItem.DisplayName, validity.ToString()));

                        allValid = false;
                    }
                    else
                    {
                        _logger.LogInformation(LocalizationManager.Instance.Current.Keys.NcaHashValid_Log.SafeFormat(ncaItem.DisplayName, validity.ToString()));
                    }
                }

                _fileOverview.NcasHashValidity = allValid ? NcasValidity.Valid : NcasValidity.Invalid;
            }
            catch (Exception ex)
            {
                _fileOverview.NcasHashValidity = NcasValidity.Error;
                _logger.LogError(LocalizationManager.Instance.Current.Keys.NcasHashError_Log.SafeFormat(ex.Message));
            }
        }


    }

    public interface IVerifyNcasHashRunnable : IRunnable
    {
        void Setup(FileOverview fileOverview);
    }
}
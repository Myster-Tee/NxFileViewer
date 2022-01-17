using System;
using System.Collections.Generic;
using System.Threading;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Model.Overview;
using Emignatik.NxFileViewer.Model.TreeItems.Impl;
using LibHac.Common;
using LibHac.Tools.FsSystem.NcaUtils;
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

            _logger.LogInformation(LocalizationManager.Instance.Current.Keys.NcaSectionHash_VerificationStart_Log);
            try
            {
                VerifyHashes(progressReporter, _fileOverview);
            }
            finally
            {
                _logger.LogInformation(LocalizationManager.Instance.Current.Keys.NcaSectionHash_VerificationEnd_Log);
            }
        }

        private void VerifyHashes(IProgressReporter progressReporter, FileOverview fileOverview)
        {

            fileOverview.NcasHashExceptions = null;
            var ncaItems = fileOverview.NcaItems;
            if (ncaItems.Count <= 0)
            {
                fileOverview.NcasHashValidity = NcasValidity.NoNca;
                return;
            }
            fileOverview.NcasHashValidity = NcasValidity.InProgress;

            var occurredExceptions = new List<Exception>();
            var allValid = true;
            try
            {
                var sectionNum = 0;
                var allSectionItems = ListAllNcaSections(ncaItems);

                foreach (var sectionItem in allSectionItems)
                {
                    sectionNum++;
                    sectionItem.Errors.RemoveAll(NCA_HASH_CATEGORY);

                    progressReporter.SetText($"Section {sectionNum}/{allSectionItems.Count}");
                    try
                    {
                        //=============================================//
                        //===============> Verify Hash <===============//
                        var validity = sectionItem.ParentItem.Nca.VerifySection(sectionItem.SectionIndex, new LibHacProgressReportRelay(
                            value =>
                            {
                                progressReporter.SetPercentage(value);
                            },
                            message =>
                            {
                                _logger.LogInformation(message);
                            })
                        );
                        //===============> Verify Hash <===============//
                        //=============================================//
                        sectionItem.HashValidity = validity;

                        if (validity != Validity.Valid)
                        {
                            allValid = false;
                            sectionItem.Errors.Add(NCA_HASH_CATEGORY, LocalizationManager.Instance.Current.Keys.NcaSectionHash_Invalid.SafeFormat(validity));
                            _logger.LogError(LocalizationManager.Instance.Current.Keys.NcaSectionHash_Invalid_Log.SafeFormat(sectionItem.ParentItem.DisplayName, sectionItem.SectionIndex, validity));
                        }
                        else
                            _logger.LogInformation(LocalizationManager.Instance.Current.Keys.NcaSectionHash_Valid_Log.SafeFormat(sectionItem.ParentItem.DisplayName, sectionItem.SectionIndex, validity));
                    }
                    catch (Exception ex)
                    {
                        allValid = false;
                        occurredExceptions.Add(ex);
                        sectionItem.Errors.Add(NCA_HASH_CATEGORY, LocalizationManager.Instance.Current.Keys.NcaSectionHash_Error.SafeFormat(ex.Message));
                        _logger.LogError(ex, LocalizationManager.Instance.Current.Keys.NcaSectionHash_Error_Log.SafeFormat(sectionItem.ParentItem.DisplayName, sectionItem.SectionIndex, ex.Message));
                    }
                }
            }
            catch (Exception ex)
            {
                allValid = false;
                occurredExceptions.Add(ex);
                _logger.LogError(ex, LocalizationManager.Instance.Current.Keys.NcasSectionHash_Error_Log.SafeFormat(ex.Message));
            }

            if (occurredExceptions.Count > 0)
            {
                fileOverview.NcasHashExceptions = occurredExceptions;
                fileOverview.NcasHashValidity = NcasValidity.Error;
            }
            else
            {
                fileOverview.NcasHashValidity = allValid ? NcasValidity.Valid : NcasValidity.Invalid;
            }
        }

        private List<SectionItem> ListAllNcaSections(IEnumerable<NcaItem> ncaItems)
        {
            var sectionItems = new List<SectionItem>();
            foreach (var ncaItem in ncaItems)
            {
                sectionItems.AddRange(ncaItem.ChildItems);
            }

            return sectionItems;
        }
    }

    public interface IVerifyNcasHashRunnable : IRunnable
    {
        void Setup(FileOverview fileOverview);
    }
}
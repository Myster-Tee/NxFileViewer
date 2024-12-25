using System;
using System.Collections.Generic;
using System.Linq;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Models.Overview;
using Emignatik.NxFileViewer.Models.TreeItems;
using Emignatik.NxFileViewer.Models.TreeItems.Impl;
using LibHac.Common;
using LibHac.Fs;
using LibHac.Fs.Fsa;
using LibHac.Tools.Fs;
using LibHac.Tools.FsSystem;
using LibHac.Tools.FsSystem.NcaUtils;
using Microsoft.Extensions.Logging;
using ContentType = LibHac.Ncm.ContentType;

namespace Emignatik.NxFileViewer.FileLoading;

public class FileOverviewLoader : IFileOverviewLoader
{
    private readonly ILogger _logger;

    public FileOverviewLoader(ILoggerFactory loggerFactory)
    {
        _logger = (loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory))).CreateLogger(this.GetType());
    }

    public FileOverview Load(XciItem xciItem)
    {
        return new FileOverviewLoaderInternal(_logger).CreateXciOverview(xciItem);
    }

    public FileOverview Load(NspItem nspItem)
    {
        return new FileOverviewLoaderInternal(_logger).CreateNspOverview(nspItem);
    }

    private class FileOverviewLoaderInternal
    {
        private readonly ILogger _logger;

        public FileOverviewLoaderInternal(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public FileOverview CreateXciOverview(XciItem xciItem)
        {
            // NOTE: the secure partition of an XCI is equivalent to an NSP
            var securePartitionItem = xciItem.ChildItems.FirstOrDefault(partition => partition.XciPartitionType == XciPartitionType.Secure);

            var fileOverview = new FileOverview(xciItem);

            if (securePartitionItem == null)
            {
                _logger.LogError(LocalizationManager.Instance.Current.Keys.LoadingError_XciSecurePartitionNotFound_Log);
                return fileOverview;
            }

            return FillOverview(fileOverview, securePartitionItem);
        }

        public FileOverview CreateNspOverview(NspItem nspItem)
        {
            var fileOverview = new FileOverview(nspItem);
            return FillOverview(fileOverview, nspItem);
        }

        private FileOverview FillOverview(FileOverview fileOverview, PartitionFileSystemItemBase partitionItem)
        {
            var cnmtContainers = BuildCnmtContainers(partitionItem).ToArray();
            fileOverview.CnmtContainers.AddRange(cnmtContainers);
            return fileOverview;
        }


        private IEnumerable<CnmtContainer> BuildCnmtContainers(PartitionFileSystemItemBase partitionItem)
        {

            // Find all Cnmt (kind of manifest containing contents information such a base title, a patch, etc.)
            var cnmtItems = partitionItem.FindAllCnmtItems().ToArray();

            if (cnmtItems.Length <= 0) 
                _logger.LogError(LocalizationManager.Instance.Current.Keys.LoadingError_NoCnmtFound_Log);

            foreach (var cnmtItem in cnmtItems)
            {
                var cnmtContainer = new CnmtContainer(cnmtItem);

                foreach (var cnmtEntryItem in cnmtItem.ChildItems)
                {
                    var referencedNcaItem = cnmtEntryItem.FindReferencedNcaItem();

                    // Search for the NCA referenced by CNMT
                    if (referencedNcaItem == null)
                    {
                        _logger.LogError(LocalizationManager.Instance.Current.Keys.LoadingError_NcaFileMissing_Log.SafeFormat(cnmtEntryItem.NcaId, cnmtEntryItem.NcaContentType));
                        continue;
                    }

                    if (cnmtEntryItem.NcaContentType == ContentType.Control)
                    {
                        var nacpItem = referencedNcaItem.FindNacpItem();
                        if (nacpItem == null)
                            _logger.LogError(LocalizationManager.Instance.Current.Keys.LoadingError_NacpFileMissing_Log.SafeFormat(NacpItem.NACP_FILE_NAME));
                        else
                            cnmtContainer.NacpContainer = LoadContentDetails(nacpItem);
                    }

                    if (cnmtEntryItem.NcaContentType == ContentType.Program)
                    {
                        // Search for corresponding Program (Code) section
                        var programSection = referencedNcaItem.ChildItems.FirstOrDefault(section => section.NcaSectionType == NcaSectionType.Code);

                        if (programSection == null)
                        {
                            _logger.LogError(LocalizationManager.Instance.Current.Keys.LoadingError_NcaMissingSection_Log.SafeFormat(cnmtEntryItem.NcaContentType, NcaSectionType.Code));
                        }
                        else
                        {
                            cnmtContainer.MainItemSectionIsSparse = programSection.IsSparse;

                            if (!programSection.IsSparse)
                            {
                                var mainItem = programSection.FindChildrenOfType<MainItem>(includeItem: false).FirstOrDefault();
                                cnmtContainer.MainItem = mainItem;

                                if (mainItem == null)
                                    _logger.LogError(LocalizationManager.Instance.Current.Keys.LoadingError_MainFileMissing_Log.SafeFormat(MainItem.MAIN_FILE_NAME));
                            }
                            else
                            {
                                cnmtContainer.MainItem = null;
                            }
                        }

                    }
                }

                yield return cnmtContainer;
            }

        }

        private NacpContainer LoadContentDetails(NacpItem nacpItem)
        {
            var contentDetails = new NacpContainer(nacpItem);

            var nacp = nacpItem.Nacp;

            var language = -1;
            foreach (ref var applicationTitle in nacp.Title.Items)
            {
                language++;

                if (applicationTitle.NameString.IsEmpty())
                    continue;

                var titleInfo = new TitleInfo(ref applicationTitle, (NacpLanguage)language);

                titleInfo.Icon = LoadExpectedIcon(nacpItem.ContainerSectionItem, titleInfo.Language);
                contentDetails.Titles.Add(titleInfo);
            }

            return contentDetails;
        }

        private byte[]? LoadExpectedIcon(SectionItem sectionItem, NacpLanguage nacpLanguage)
        {
            var languageName = nacpLanguage.ToString();

            var expectedFileName = $"icon_{languageName}.dat";

            var iconItem = sectionItem.ChildItems.FirstOrDefault(item => string.Equals(item.Name, expectedFileName, StringComparison.OrdinalIgnoreCase));
            if (iconItem == null)
            {
                _logger.LogError(LocalizationManager.Instance.Current.Keys.LoadingError_IconMissing_Log.SafeFormat(expectedFileName));
                return null;
            }

            var fileSystem = sectionItem.FileSystem;
            if (fileSystem == null)
                return null;

            try
            {
                using var uniqueRefFile = new UniqueRef<IFile>();

                fileSystem.OpenFile(ref uniqueRefFile.Ref, iconItem.Path.ToU8Span(), OpenMode.Read).ThrowIfFailure();
                var file = uniqueRefFile.Release();

                file.GetSize(out var fileSize).ThrowIfFailure();
                var bytes = new byte[fileSize];
                _ = file.AsStream().Read(bytes);
                return bytes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, LocalizationManager.Instance.Current.Keys.LoadingError_FailedToLoadIcon_Log.SafeFormat(ex.Message));
                return null;
            }
        }

    }

}
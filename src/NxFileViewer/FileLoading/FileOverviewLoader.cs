using System;
using System.Collections.Generic;
using System.Linq;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Models.Overview;
using Emignatik.NxFileViewer.Models.TreeItems;
using Emignatik.NxFileViewer.Models.TreeItems.Impl;
using Emignatik.NxFileViewer.Utils;
using LibHac.Common;
using LibHac.Fs;
using LibHac.Fs.Fsa;
using LibHac.Tools.Fs;
using LibHac.Tools.FsSystem;
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
                var message = LocalizationManager.Instance.Current.Keys.LoadingError_XciSecurePartitionNotFound;
                xciItem.Errors.Add(message);
                _logger.LogError(message);
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

            var packageType = DeterminePackageType(fileOverview, cnmtContainers);

            fileOverview.PackageType = packageType;
            fileOverview.CnmtContainers.AddRange(cnmtContainers);

            return fileOverview;
        }

        private static Models.Overview.PackageType DeterminePackageType(FileOverview fileOverview, CnmtContainer[] cnmtContainers)
        {
            Models.Overview.PackageType packageType;
            if (cnmtContainers.Length > 1)
            {
                var rootItemType = fileOverview.RootItem.GetType();
                if (rootItemType == typeof(XciItem))
                    packageType = Models.Overview.PackageType.SuperXCI;
                else if (rootItemType == typeof(NspItem))
                    packageType = Models.Overview.PackageType.SuperNSP;
                else
                    packageType = Models.Overview.PackageType.Unknown;
            }
            else
                packageType = Models.Overview.PackageType.Normal;

            return packageType;
        }

        private IEnumerable<CnmtContainer> BuildCnmtContainers(PartitionFileSystemItemBase partitionItem)
        {

            // Find all Cnmt (kind of manifest containing contents information such a base title, a patch, etc.)
            var cnmtItems = partitionItem.FindAllCnmtItems().ToArray();

            if (cnmtItems.Length <= 0)
            {
                var message = LocalizationManager.Instance.Current.Keys.LoadingError_NoCnmtFound;
                partitionItem.Errors.Add(message);
                _logger.LogError(message);
            }

            foreach (var cnmtItem in cnmtItems)
            {
                var cnmtContainer = new CnmtContainer(cnmtItem);

                foreach (var cnmtContentEntry in cnmtItem.Cnmt.ContentEntries)
                {
                    var ncaId = cnmtContentEntry.NcaId.ToStrId();

                    var parentPartitionFileSystemItem = cnmtItem.ContainerFsSectionItem.ParentItem.ParentItem;
                    var ncaItem = parentPartitionFileSystemItem.FindNcaItem(ncaId);
                    if (ncaItem == null)
                    {
                        if (cnmtContentEntry.Type == ContentType.DeltaFragment)
                            _logger.LogWarning(LocalizationManager.Instance.Current.Keys.LoadingError_NcaFileMissing.SafeFormat(ncaId, cnmtContentEntry.Type));
                        else
                        {
                            var message = LocalizationManager.Instance.Current.Keys.LoadingError_NcaFileMissing.SafeFormat(ncaId, cnmtContentEntry.Type);
                            parentPartitionFileSystemItem.Errors.Add(message);
                            _logger.LogError(message);
                        }
                        continue;
                    }

                    if (cnmtContentEntry.Type == ContentType.Control)
                    {
                        var nacpItem = ncaItem.FindNacpItem();
                        if (nacpItem == null)
                        {
                            var message = LocalizationManager.Instance.Current.Keys.LoadingError_NacpFileMissing.SafeFormat(NacpItem.NacpFileName);
                            ncaItem.Errors.Add(message);
                            _logger.LogError(message);
                        }
                        else
                        {
                            cnmtContainer.NacpContainer = LoadContentDetails(nacpItem);
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

                titleInfo.Icon = LoadExpectedIcon(nacpItem.ContainerFsSectionItem, titleInfo.Language);
                contentDetails.Titles.Add(titleInfo);
            }

            return contentDetails;
        }

        private byte[]? LoadExpectedIcon(FsSectionItem fsSectionItem, NacpLanguage nacpLanguage)
        {
            var languageName = nacpLanguage.ToString();

            var expectedFileName = $"icon_{languageName}.dat";

            var iconItem = fsSectionItem.ChildItems.FirstOrDefault(item => string.Equals(item.Name, expectedFileName, StringComparison.OrdinalIgnoreCase));
            if (iconItem == null)
            {
                var message = LocalizationManager.Instance.Current.Keys.LoadingError_IconMissing.SafeFormat(expectedFileName);
                fsSectionItem.Errors.Add(message);
                _logger.LogError(message);
                return null;
            }

            var fileSystem = fsSectionItem.FileSystem;
            if (fileSystem == null)
                return null;

            try
            {
                using var uniqueRefFile = new UniqueRef<IFile>();

                fileSystem.OpenFile(ref uniqueRefFile.Ref(), iconItem.Path.ToU8Span(), OpenMode.Read).ThrowIfFailure();
                var file = uniqueRefFile.Release();

                file.GetSize(out var fileSize).ThrowIfFailure();
                var bytes = new byte[fileSize];
                file.AsStream().Read(bytes);
                return bytes;
            }
            catch (Exception ex)
            {
                var message = LocalizationManager.Instance.Current.Keys.LoadingError_FailedToLoadIcon.SafeFormat(ex.Message);
                iconItem.Errors.Add(message);
                _logger.LogError(ex, message);
                return null;
            }
        }

    }

}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Model.Overview;
using Emignatik.NxFileViewer.Model.TreeItems.Impl;
using Emignatik.NxFileViewer.Utils;
using LibHac;
using LibHac.Common;
using LibHac.Fs;
using LibHac.FsSystem;
using LibHac.FsSystem.NcaUtils;
using Microsoft.Extensions.Logging;
using ContentType = LibHac.Ncm.ContentType;

namespace Emignatik.NxFileViewer.FileLoading
{
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
                var securePartitionItem = xciItem.Partitions.FirstOrDefault(partition => partition.XciPartitionType == XciPartitionType.Secure);

                var fileOverview = new FileOverview(xciItem);

                if (securePartitionItem == null)
                {
                    _logger.LogError(LocalizationManager.Instance.Current.Keys.LoadingError_XciSecurePartitionNotFound);
                    return fileOverview;
                }

                return FillOverview(fileOverview, securePartitionItem);
            }

            public FileOverview CreateNspOverview(NspItem nspItem)
            {
                var fileOverview = new FileOverview(nspItem);
                return FillOverview(fileOverview, nspItem);
            }

            private FileOverview FillOverview(FileOverview fileOverview, PartitionFileSystemItem partitionItem)
            {
                var cnmtContainers = BuildCnmtContainers(partitionItem).ToArray();

                var packageType = DeterminePackageType(fileOverview, cnmtContainers);

                fileOverview.PackageType = packageType;
                fileOverview.CnmtContainers.AddRange(cnmtContainers);

                return fileOverview;
            }

            private static PackageType DeterminePackageType(FileOverview fileOverview, CnmtContainer[] cnmtContainers)
            {
                PackageType packageType;
                if (cnmtContainers.Length > 1)
                {
                    var rootItemType = fileOverview.RootItem.GetType();
                    if (rootItemType == typeof(XciItem))
                        packageType = PackageType.SuperXCI;
                    else if (rootItemType == typeof(NspItem))
                        packageType = PackageType.SuperNSP;
                    else
                        packageType = PackageType.Unknown;
                }
                else
                    packageType = PackageType.Normal;

                return packageType;
            }

            private IEnumerable<CnmtContainer> BuildCnmtContainers(PartitionFileSystemItem partitionItem)
            {

                // Find all Cnmt (kind of manifest containing contents information such a base title, a patch, etc.)
                var cnmtItems = FindAllCnmtItems(partitionItem).ToArray();

                if (cnmtItems.Length <= 0)
                    _logger.LogError(LocalizationManager.Instance.Current.Keys.LoadingError_NoCnmtFound);

                foreach (var cnmtItem in cnmtItems)
                {
                    var cnmtContainer = new CnmtContainer(cnmtItem);

                    foreach (var cnmtContentEntry in cnmtItem.Cnmt.ContentEntries)
                    {
                        var ncaId = cnmtContentEntry.NcaId.ToStrId();

                        var ncaItem = FindNcaItem(cnmtItem.ContainerSectionItem.ParentNcaItem.ParentPartitionFileSystemItem, ncaId);
                        if (ncaItem == null)
                        {
                            if (cnmtContentEntry.Type == ContentType.DeltaFragment)
                                _logger.LogWarning(string.Format(LocalizationManager.Instance.Current.Keys.LoadingError_NcaFileMissing, ncaId, cnmtContentEntry.Type));
                            else
                                _logger.LogError(string.Format(LocalizationManager.Instance.Current.Keys.LoadingError_NcaFileMissing, ncaId, cnmtContentEntry.Type));
                            continue;
                        }

                        if (cnmtContentEntry.Type == ContentType.Control)
                        {
                            var nacpItem = FindNacpItem(ncaItem);
                            if (nacpItem == null)
                            {
                                _logger.LogError(string.Format(LocalizationManager.Instance.Current.Keys.LoadingError_NacpFileMissing, NacpItem.NacpFileName));
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

            private static IEnumerable<CnmtItem> FindAllCnmtItems(PartitionFileSystemItem partitionItem)
            {
                foreach (var ncaItem in partitionItem.NcaItems)
                {
                    if (ncaItem.ContentType != NcaContentType.Meta)
                        continue;
                    foreach (var sectionItem in ncaItem.Sections)
                    {
                        foreach (var child in sectionItem.ChildItems)
                        {
                            if (child is CnmtItem cnmtItem)
                                yield return cnmtItem;
                        }
                    }
                }
            }

            private NacpContainer LoadContentDetails(NacpItem nacpItem)
            {
                var contentDetails = new NacpContainer(nacpItem);

                var nacp = nacpItem.Nacp;

                foreach (var description in nacp.Descriptions)
                {
                    if (string.IsNullOrEmpty(description.Title))
                        continue;

                    var titleInfo = new TitleInfo(description);

                    titleInfo.Icon = LoadExpectedIcon(nacpItem.ContainerSectionItem, titleInfo.Language);
                    contentDetails.Titles.Add(titleInfo);
                }

                return contentDetails;
            }

            private BitmapImage? LoadExpectedIcon(SectionItem sectionItem, NacpLanguage nacpLanguage)
            {
                try
                {
                    var languageName = nacpLanguage.ToString();

                    var expectedFileName = $"icon_{languageName}.dat";

                    var iconItem = sectionItem.ChildDirectoryEntryItems.FirstOrDefault(item => string.Equals(item.Name, expectedFileName, StringComparison.OrdinalIgnoreCase));
                    if (iconItem == null)
                    {
                        _logger.LogError(string.Format(LocalizationManager.Instance.Current.Keys.LoadingError_IconMissing, expectedFileName));
                        return null;
                    }

                    sectionItem.FileSystem.OpenFile(out var file, new U8Span(iconItem.Path), OpenMode.Read).ThrowIfFailure();

                    var memoryStream = new MemoryStream(); // NOTE: Do not dispose this memory stream because WPF default loading is lazy
                    file.AsStream().CopyTo(memoryStream);
                    memoryStream.Position = 0;

                    using (file)
                    {

                        var bitmapImage = new BitmapImage();
                        bitmapImage.BeginInit();
                        bitmapImage.StreamSource = memoryStream;
                        bitmapImage.EndInit();
                        return bitmapImage;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, string.Format(LocalizationManager.Instance.Current.Keys.LoadingError_FailedToLoadIcon, ex.Message));
                    return null;
                }
            }

            private static NcaItem? FindNcaItem(PartitionFileSystemItem partitionItem, string ncaId)
            {
                var expectedFileName = ncaId;
                return partitionItem.NcaItems.FirstOrDefault(ncaItem => string.Equals(ncaItem.Id, expectedFileName, StringComparison.OrdinalIgnoreCase));
            }

            private static NacpItem? FindNacpItem(NcaItem ncaItem)
            {
                foreach (var sectionItem in ncaItem.Sections)
                {
                    foreach (var dirEntry in sectionItem.ChildDirectoryEntryItems)
                    {
                        if (dirEntry is NacpItem nacpItem)
                            return nacpItem;
                    }
                }

                return null;
            }

        }

    }
}

using System;
using System.Collections.Generic;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Model.TreeItems;
using Emignatik.NxFileViewer.Model.TreeItems.Impl;
using LibHac;
using LibHac.Common;
using LibHac.Fs;
using LibHac.Fs.Fsa;
using LibHac.FsSystem;
using LibHac.FsSystem.NcaUtils;
using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer.FileLoading
{
    /// <summary>
    /// Exposes the logic for building child items of a given <see cref="IItem"/>
    /// </summary>
    public class ChildItemsBuilder : IChildItemsBuilder
    {
        private readonly ILogger _logger;

        public ChildItemsBuilder(ILoggerFactory loggerFactory)
        {
            _logger = (loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory))).CreateLogger(this.GetType());
        }

        public IEnumerable<XciPartitionItem> Build(XciItem parentItem)
        {
            var children = new List<XciPartitionItem>();
            var xci = parentItem.Xci;

            try
            {
                foreach (XciPartitionType xciPartitionType in Enum.GetValues(typeof(XciPartitionType)))
                {
                    try
                    {
                        if (!xci.HasPartition(xciPartitionType))
                            continue;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, string.Format(LocalizationManager.Instance.Current.Keys.LoadingError_FailedToCheckIfXciPartitionExists, ex.Message));
                        continue;
                    }

                    XciPartition xciPartition;
                    try
                    {
                        xciPartition = xci.OpenPartition(xciPartitionType);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, string.Format(LocalizationManager.Instance.Current.Keys.LoadingError_FailedToOpenXciPartition, ex.Message));
                        continue;
                    }
                    children.Add(new XciPartitionItem(xciPartition, xciPartitionType, parentItem, this));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Format(LocalizationManager.Instance.Current.Keys.LoadingError_FailedToLoadXciContent, ex.Message));
            }

            return children;
        }

        public PartitionChildByTypes Build(PartitionFileSystemItem parentItem)
        {
            var partitionChildByTypes = new PartitionChildByTypes();

            try
            {
                var partitionFileSystem = parentItem.PartitionFileSystem;
                foreach (var partitionFileEntry in partitionFileSystem.Files)
                {
                    IFile file;
                    try
                    {
                        file = partitionFileSystem.OpenFile(partitionFileEntry, OpenMode.Read);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, string.Format(LocalizationManager.Instance.Current.Keys.LoadingError_FailedToOpenPartitionFile, ex.Message));
                        continue;
                    }

                    var fileName = partitionFileEntry.Name;
                    IItem child;
                    if (fileName.EndsWith(".nca", StringComparison.OrdinalIgnoreCase) || fileName.EndsWith(".ncz", StringComparison.OrdinalIgnoreCase))
                    {
                        Nca nca;
                        try
                        {
                            nca = new Nca(parentItem.KeySet, new FileStorage(file));
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, string.Format(LocalizationManager.Instance.Current.Keys.LoadingError_FailedToLoadNcaFile, ex.Message));
                            continue;
                        }
                        var ncaItem = new NcaItem(nca, partitionFileEntry, file, parentItem, this);
                        partitionChildByTypes.NcaItems.Add(ncaItem);
                        child = ncaItem;
                    }
                    else
                    {
                        var partitionFileEntryItem = new PartitionFileEntryItem(partitionFileEntry, file, parentItem, this);
                        partitionChildByTypes.PartitionFileEntryItems.Add(partitionFileEntryItem);
                        child = partitionFileEntryItem;
                    }
                    partitionChildByTypes.AllChildItems.Add(child);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Format(LocalizationManager.Instance.Current.Keys.LoadingError_FailedToLoadPartitionFileSystemContent, ex.Message));
            }

            return partitionChildByTypes;
        }

        public IEnumerable<SectionItem> Build(NcaItem parentItem)
        {
            var children = new List<SectionItem>();

            try
            {
                var nca = parentItem.Nca;
                for (var sectionIndex = 0; sectionIndex < 4; sectionIndex++) // TODO: demander l'accès à la constante 4?
                {
                    try
                    {
                        if (!nca.Header.IsSectionEnabled(sectionIndex))
                            continue;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, string.Format(LocalizationManager.Instance.Current.Keys.LoadingError_FailedToCheckIfSectionCanBeOpened, ex.Message));
                        continue;
                    }

                    IFileSystem fileSystem;
                    try
                    {
                        fileSystem = nca.OpenFileSystem(sectionIndex, IntegrityCheckLevel.ErrorOnInvalid);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, string.Format(LocalizationManager.Instance.Current.Keys.LoadingError_FailedToOpenFileSystem, ex.Message));
                        continue;
                    }
                    children.Add(new SectionItem(sectionIndex, fileSystem, parentItem, this));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Format(LocalizationManager.Instance.Current.Keys.LoadingError_FailedToLoadNcaContent, ex.Message));
            }

            return children;
        }

        public IEnumerable<DirectoryEntryItem> Build(SectionItem parentItem)
        {
            var children = new List<DirectoryEntryItem>();

            try
            {
                const string? rootPath = "/";

                var directoryEntries = SafeGetDirectoryEntries(parentItem.FileSystem, rootPath);

                foreach (var directoryEntry in directoryEntries)
                {
                    var entryName = StringUtils.Utf8ZToString(directoryEntry.Name);
                    var entryPath = PathTools.Combine(rootPath, entryName);

                    if (parentItem.ParentNcaItem.ContentType == NcaContentType.Control && string.Equals(entryName, NacpItem.NacpFileName, StringComparison.OrdinalIgnoreCase) && directoryEntry.Type == DirectoryEntryType.File)
                    {
                        IFile nacpFile;
                        try
                        {
                            parentItem.FileSystem.OpenFile(out nacpFile, new U8Span(entryPath), OpenMode.Read).ThrowIfFailure();
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, string.Format(LocalizationManager.Instance.Current.Keys.LoadingError_FailedToOpenNacpFile, ex.Message));
                            continue;
                        }

                        Nacp nacp;
                        try
                        {
                            nacp = new Nacp(nacpFile.AsStream());
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, string.Format(LocalizationManager.Instance.Current.Keys.LoadingError_FailedToLoadNacpFile, ex.Message));
                            continue;
                        }

                        children.Add(new NacpItem(nacp, parentItem, directoryEntry, entryName, entryPath, this));
                    }
                    else if (parentItem.ParentNcaItem.ContentType == NcaContentType.Meta && entryName.EndsWith(".cnmt", StringComparison.OrdinalIgnoreCase) && directoryEntry.Type == DirectoryEntryType.File)
                    {
                        IFile cnmtFile;
                        try
                        {
                            parentItem.FileSystem.OpenFile(out cnmtFile, new U8Span(entryPath), OpenMode.Read).ThrowIfFailure();
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, string.Format(LocalizationManager.Instance.Current.Keys.LoadingError_FailedToOpenCnmtFile, ex.Message));
                            continue;
                        }

                        Cnmt cnmt;
                        try
                        {
                            cnmt = new Cnmt(cnmtFile.AsStream());
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, string.Format(LocalizationManager.Instance.Current.Keys.LoadingError_FailedToLoadCnmtFile, ex.Message));
                            continue;
                        }
                        children.Add(new CnmtItem(cnmt, parentItem, directoryEntry, entryName, entryPath, this));
                    }
                    else
                    {
                        children.Add(new DirectoryEntryItem(parentItem, directoryEntry, entryName, entryPath, this));
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Format(LocalizationManager.Instance.Current.Keys.LoadingError_FailedToLoadSectionContent, ex.Message));
            }

            return children;
        }

        public IEnumerable<DirectoryEntryItem> Build(DirectoryEntryItem parentItem)
        {
            var children = new List<DirectoryEntryItem>();

            try
            {
                if (parentItem.DirectoryEntry.Type == DirectoryEntryType.File)
                    return children;

                var currentPath = parentItem.Path;
                var directoryEntries = SafeGetDirectoryEntries(parentItem.ContainerSectionItem.FileSystem, currentPath);

                foreach (var directoryEntry in directoryEntries)
                {
                    var entryName = StringUtils.Utf8ZToString(directoryEntry.Name);
                    var entryPath = PathTools.Combine(currentPath, entryName);

                    children.Add(new DirectoryEntryItem(parentItem.ContainerSectionItem, directoryEntry, entryName, entryPath, parentItem, this));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Format(LocalizationManager.Instance.Current.Keys.LoadingError_FailedToLoadDirectoryContent, ex.Message));
            }

            return children;
        }

        public IEnumerable<IItem> Build(PartitionFileEntryItem partitionFileEntryItem)
        {
            yield break;
        }

        private DirectoryEntry[] SafeGetDirectoryEntries(IFileSystem fileSystem, string currentPath)
        {
            try
            {
                fileSystem.OpenDirectory(out IDirectory directory, currentPath.ToU8Span(), OpenDirectoryMode.All).ThrowIfFailure();

                directory.GetEntryCount(out var nbEntries).ThrowIfFailure();

                var entries = new DirectoryEntry[nbEntries];

                directory.Read(out _, new Span<DirectoryEntry>(entries)).ThrowIfFailure();
                return entries;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Format(LocalizationManager.Instance.Current.Keys.LoadingError_FailedToGetFileSystemDirectoryEntries, ex.Message));
                return new DirectoryEntry[0];
            }
        }
    }


}
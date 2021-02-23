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
using LibHac.Loader;
using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer.FileLoading
{
    /// <summary>
    /// Exposes the logic for building child items of a given <see cref="IItem"/>
    /// </summary>
    public class ChildItemsBuilder : IChildItemsBuilder
    {
        private const string CHILD_LOADING_CATEGORY = "CHILD_LOADING_CATEGORY";

        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;

        public ChildItemsBuilder(ILoggerFactory loggerFactory, IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = (loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory))).CreateLogger(this.GetType());
        }

        public IReadOnlyList<XciPartitionItem> Build(XciItem parentItem)
        {
            if (parentItem == null)
                throw new ArgumentNullException(nameof(parentItem));
            OnBeforeLoadChildren(parentItem);

            var children = new List<XciPartitionItem>();
            var xci = parentItem.Xci;

            try
            {
                foreach (var xciPartitionType in Enum.GetValues<XciPartitionType>())
                {
                    try
                    {
                        if (!xci.HasPartition(xciPartitionType))
                            continue;
                    }
                    catch (Exception ex)
                    {
                        var message = LocalizationManager.Instance.Current.Keys.LoadingError_FailedToCheckIfXciPartitionExists.SafeFormat(ex.Message);
                        parentItem.Errors.Add(CHILD_LOADING_CATEGORY, message);
                        _logger.LogError(ex, message);
                        continue;
                    }

                    XciPartition xciPartition;
                    try
                    {
                        xciPartition = xci.OpenPartition(xciPartitionType);
                    }
                    catch (Exception ex)
                    {
                        var message = LocalizationManager.Instance.Current.Keys.LoadingError_FailedToOpenXciPartition.SafeFormat(ex.Message);
                        parentItem.Errors.Add(CHILD_LOADING_CATEGORY, message);
                        _logger.LogError(ex, message);
                        continue;
                    }
                    children.Add(new XciPartitionItem(xciPartition, xciPartitionType, parentItem, this));
                }
            }
            catch (Exception ex)
            {
                var message = LocalizationManager.Instance.Current.Keys.LoadingError_FailedToLoadXciContent.SafeFormat(ex.Message);
                parentItem.Errors.Add(CHILD_LOADING_CATEGORY, message);
                _logger.LogError(ex, message);
            }

            return children;
        }

        public IPartitionChildByTypes Build(PartitionFileSystemItem parentItem)
        {
            if (parentItem == null)
                throw new ArgumentNullException(nameof(parentItem));
            OnBeforeLoadChildren(parentItem);

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
                        var message = LocalizationManager.Instance.Current.Keys.LoadingError_FailedToOpenPartitionFile.SafeFormat(ex.Message);
                        parentItem.Errors.Add(CHILD_LOADING_CATEGORY, message);
                        _logger.LogError(ex, message);
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
                            var message = LocalizationManager.Instance.Current.Keys.LoadingError_FailedToLoadNcaFile.SafeFormat(ex.Message);
                            parentItem.Errors.Add(CHILD_LOADING_CATEGORY, message);
                            _logger.LogError(ex, message);
                            continue;
                        }
                        var ncaItem = new NcaItem(nca, partitionFileEntry, file, parentItem, this);
                        partitionChildByTypes.NcaItemsInternal.Add(ncaItem);
                        child = ncaItem;
                    }
                    else
                    {
                        var partitionFileEntryItem = new PartitionFileEntryItem(partitionFileEntry, file, parentItem, this);
                        partitionChildByTypes.PartitionFileEntryItemsInternal.Add(partitionFileEntryItem);
                        child = partitionFileEntryItem;
                    }
                    partitionChildByTypes.AllChildItemsInternal.Add(child);
                }
            }
            catch (Exception ex)
            {
                var message = LocalizationManager.Instance.Current.Keys.LoadingError_FailedToLoadPartitionFileSystemContent.SafeFormat(ex.Message);
                parentItem.Errors.Add(CHILD_LOADING_CATEGORY, message);
                _logger.LogError(ex, message);
            }

            return partitionChildByTypes;
        }

        public IReadOnlyList<SectionItem> Build(NcaItem parentItem)
        {
            if (parentItem == null)
                throw new ArgumentNullException(nameof(parentItem));
            OnBeforeLoadChildren(parentItem);

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
                        var message = LocalizationManager.Instance.Current.Keys.LoadingError_FailedToCheckIfSectionCanBeOpened.SafeFormat(ex.Message);
                        parentItem.Errors.Add(CHILD_LOADING_CATEGORY, message);
                        _logger.LogError(ex, message);
                        continue;
                    }

                    NcaFsHeader ncaFsHeader;
                    try
                    {
                        ncaFsHeader = nca.GetFsHeader(sectionIndex);
                    }
                    catch (Exception ex)
                    {
                        var message = LocalizationManager.Instance.Current.Keys.LoadingError_FailedToGetNcaFsHeader.SafeFormat(sectionIndex, ex.Message);
                        parentItem.Errors.Add(CHILD_LOADING_CATEGORY, message);
                        _logger.LogError(ex, message);
                        continue;
                    }

                    IFileSystem fileSystem;
                    try
                    {
                        fileSystem = nca.OpenFileSystem(sectionIndex, IntegrityCheckLevel.ErrorOnInvalid);
                    }
                    catch (Exception ex)
                    {
                        var message = LocalizationManager.Instance.Current.Keys.LoadingError_FailedToOpenNcaSection.SafeFormat(sectionIndex, ex.Message);
                        parentItem.Errors.Add(CHILD_LOADING_CATEGORY, message);
                        _logger.LogError(ex, message);
                        continue;
                    }

                    children.Add(new SectionItem(sectionIndex, ncaFsHeader, fileSystem, parentItem, this));
                }
            }
            catch (Exception ex)
            {
                var message = LocalizationManager.Instance.Current.Keys.LoadingError_FailedToLoadNcaContent.SafeFormat(ex.Message);
                parentItem.Errors.Add(CHILD_LOADING_CATEGORY, message);
                _logger.LogError(ex, message);
            }

            return children;
        }

        public IReadOnlyList<DirectoryEntryItem> Build(SectionItem parentItem)
        {
            if (parentItem == null)
                throw new ArgumentNullException(nameof(parentItem));
            OnBeforeLoadChildren(parentItem);

            var children = new List<DirectoryEntryItem>();

            try
            {
                const string? rootPath = "/";

                var directoryEntries = SafeGetDirectoryEntries(parentItem.FileSystem, rootPath, parentItem);

                foreach (var directoryEntry in directoryEntries)
                {
                    var entryName = StringUtils.Utf8ZToString(directoryEntry.Name);
                    var entryPath = PathTools.Combine(rootPath, entryName);

                    // NACP File
                    if (parentItem.ParentNcaItem.ContentType == NcaContentType.Control && string.Equals(entryName, NacpItem.NacpFileName, StringComparison.OrdinalIgnoreCase) && directoryEntry.Type == DirectoryEntryType.File)
                    {
                        IFile nacpFile;
                        try
                        {
                            parentItem.FileSystem.OpenFile(out nacpFile, new U8Span(entryPath), OpenMode.Read).ThrowIfFailure();
                        }
                        catch (Exception ex)
                        {
                            var message = LocalizationManager.Instance.Current.Keys.LoadingError_FailedToOpenNacpFile.SafeFormat(ex.Message);
                            parentItem.Errors.Add(CHILD_LOADING_CATEGORY, message);
                            _logger.LogError(ex, message);
                            continue;
                        }

                        Nacp nacp;
                        try
                        {
                            nacp = new Nacp(nacpFile.AsStream());
                        }
                        catch (Exception ex)
                        {
                            var message = LocalizationManager.Instance.Current.Keys.LoadingError_FailedToLoadNacpFile.SafeFormat(ex.Message);
                            parentItem.Errors.Add(CHILD_LOADING_CATEGORY, message);
                            _logger.LogError(ex, message);
                            continue;
                        }

                        children.Add(new NacpItem(nacp, parentItem, directoryEntry, entryName, entryPath, this));
                    }
                    // CNMT File
                    else if (parentItem.ParentNcaItem.ContentType == NcaContentType.Meta && entryName.EndsWith(".cnmt", StringComparison.OrdinalIgnoreCase) && directoryEntry.Type == DirectoryEntryType.File)
                    {
                        IFile cnmtFile;
                        try
                        {
                            parentItem.FileSystem.OpenFile(out cnmtFile, new U8Span(entryPath), OpenMode.Read).ThrowIfFailure();
                        }
                        catch (Exception ex)
                        {
                            var message = LocalizationManager.Instance.Current.Keys.LoadingError_FailedToOpenCnmtFile.SafeFormat(ex.Message);
                            parentItem.Errors.Add(CHILD_LOADING_CATEGORY, message);
                            _logger.LogError(ex, message);
                            continue;
                        }

                        Cnmt cnmt;
                        try
                        {
                            cnmt = new Cnmt(cnmtFile.AsStream());
                        }
                        catch (Exception ex)
                        {
                            var message = LocalizationManager.Instance.Current.Keys.LoadingError_FailedToLoadCnmtFile.SafeFormat(ex.Message);
                            parentItem.Errors.Add(CHILD_LOADING_CATEGORY, message);
                            _logger.LogError(ex, message);
                            continue;
                        }
                        children.Add(new CnmtItem(cnmt, parentItem, directoryEntry, entryName, entryPath, this));
                    }
                    // MAIN file
                    else if (parentItem.ParentNcaItem.ContentType == NcaContentType.Program && string.Equals(entryName, "main", StringComparison.OrdinalIgnoreCase) && directoryEntry.Type == DirectoryEntryType.File)
                    {
                        IFile nsoFile;
                        try
                        {
                            parentItem.FileSystem.OpenFile(out nsoFile, new U8Span(entryPath), OpenMode.Read).ThrowIfFailure();
                        }
                        catch (Exception ex)
                        {
                            var message = LocalizationManager.Instance.Current.Keys.LoadingError_FailedToOpenMainFile.SafeFormat(ex.Message);
                            parentItem.Errors.Add(CHILD_LOADING_CATEGORY, message);
                            _logger.LogError(ex, message);
                            continue;
                        }

                        NsoHeader? nsoHeader;
                        try
                        {
                            var nsoReader = new NsoReader();
                            nsoReader.Initialize(nsoFile).ThrowIfFailure();
                            nsoHeader = nsoReader.Header;
                        }
                        catch (Exception ex)
                        {
                            var message = LocalizationManager.Instance.Current.Keys.LoadingError_FailedToLoadMainFile.SafeFormat(ex.Message);
                            parentItem.Errors.Add(CHILD_LOADING_CATEGORY, message);
                            _logger.LogError(ex, message);
                            continue;
                        }

                        children.Add(new MainItem(nsoHeader.Value, parentItem, directoryEntry, entryName, entryPath, this));
                    }
                    else
                    {
                        children.Add(new DirectoryEntryItem(parentItem, directoryEntry, entryName, entryPath, this));
                    }
                }
            }
            catch (Exception ex)
            {
                var message = LocalizationManager.Instance.Current.Keys.LoadingError_FailedToLoadSectionContent.SafeFormat(ex.Message);
                parentItem.Errors.Add(CHILD_LOADING_CATEGORY, message);
                _logger.LogError(ex, message);
            }

            return children;
        }

        public IReadOnlyList<DirectoryEntryItem> Build(DirectoryEntryItem parentItem)
        {
            if (parentItem == null)
                throw new ArgumentNullException(nameof(parentItem));
            OnBeforeLoadChildren(parentItem);

            var children = new List<DirectoryEntryItem>();

            try
            {
                if (parentItem.DirectoryEntryType == DirectoryEntryType.File)
                    return children;

                var currentPath = parentItem.Path;
                var directoryEntries = SafeGetDirectoryEntries(parentItem.ContainerSectionItem.FileSystem, currentPath, parentItem);

                foreach (var directoryEntry in directoryEntries)
                {
                    var entryName = StringUtils.Utf8ZToString(directoryEntry.Name);
                    var entryPath = PathTools.Combine(currentPath, entryName);

                    children.Add(new DirectoryEntryItem(parentItem.ContainerSectionItem, directoryEntry, entryName, entryPath, parentItem, this));
                }
            }
            catch (Exception ex)
            {
                var message = LocalizationManager.Instance.Current.Keys.LoadingError_FailedToLoadDirectoryContent.SafeFormat(ex.Message);
                parentItem.Errors.Add(CHILD_LOADING_CATEGORY, message);
                _logger.LogError(ex, message);
            }

            return children;
        }

        public IReadOnlyList<IItem> Build(PartitionFileEntryItem parentItem)
        {
            if (parentItem == null)
                throw new ArgumentNullException(nameof(parentItem));
            OnBeforeLoadChildren(parentItem);

            return new List<IItem>();
        }

        private void OnBeforeLoadChildren(IItem parentItem)
        {
            parentItem.Errors.RemoveAll(CHILD_LOADING_CATEGORY);
        }

        private DirectoryEntry[] SafeGetDirectoryEntries(IFileSystem fileSystem, string currentPath, IItem parentItem)
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
                var message = LocalizationManager.Instance.Current.Keys.LoadingError_FailedToGetFileSystemDirectoryEntries.SafeFormat(ex.Message);
                parentItem.Errors.Add(CHILD_LOADING_CATEGORY, message);
                _logger.LogError(ex, message);
                return new DirectoryEntry[0];
            }
        }
    }


}
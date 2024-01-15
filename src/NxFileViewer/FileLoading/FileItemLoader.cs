using System;
using System.Collections.Generic;
using System.Linq;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Models.TreeItems;
using Emignatik.NxFileViewer.Models.TreeItems.Impl;
using Emignatik.NxFileViewer.Services.KeysManagement;
using Emignatik.NxFileViewer.Settings;
using Emignatik.NxFileViewer.Utils;
using LibHac;
using LibHac.Common;
using LibHac.Common.Keys;
using LibHac.Fs;
using LibHac.Fs.Fsa;
using LibHac.FsSystem;
using LibHac.Loader;
using LibHac.Ns;
using LibHac.Spl;
using LibHac.Tools.Es;
using LibHac.Tools.Fs;
using LibHac.Tools.FsSystem;
using LibHac.Tools.FsSystem.NcaUtils;
using LibHac.Tools.Ncm;
using Microsoft.Extensions.Logging;
using NcaFsHeader = LibHac.Tools.FsSystem.NcaUtils.NcaFsHeader;


namespace Emignatik.NxFileViewer.FileLoading;

public class FileItemLoader : IFileItemLoader
{
    private const string TREE_LOADING_CATEGORY = "TREE_LOADING_CATEGORY";

    private readonly IKeySetProviderService _keySetProviderService;
    private readonly IAppSettings _appSettings;
    private readonly ILogger _logger;

    public FileItemLoader(IKeySetProviderService keySetProviderService, ILoggerFactory loggerFactory, IAppSettings appSettings)
    {
        _keySetProviderService = keySetProviderService ?? throw new ArgumentNullException(nameof(keySetProviderService));
        _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
        _logger = (loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory))).CreateLogger(this.GetType());
    }

    public NspItem LoadNsp(string nspFilePath)
    {
        var keySet = _keySetProviderService.GetKeySet(_appSettings.AlwaysReloadKeysBeforeOpen);
        var nspItem = NspItem.FromFile(nspFilePath, keySet);
        BuildChildItems(nspItem);
        return nspItem;
    }

    public event MissingKeyExceptionHandler? MissingKey;

    public XciItem LoadXci(string xciFilePath)
    {
        var keySet = _keySetProviderService.GetKeySet(_appSettings.AlwaysReloadKeysBeforeOpen);
        var xciItem = XciItem.FromFile(xciFilePath, keySet);
        BuildChildItems(xciItem);
        return xciItem;
    }


    private void BuildChildItems(XciItem parentItem)
    {
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
                    OnLoadingException(ex, parentItem);

                    var message = LocalizationManager.Instance.Current.Keys.LoadingError_FailedToCheckIfXciPartitionExists.SafeFormat(ex.Message);
                    parentItem.Errors.Add(TREE_LOADING_CATEGORY, message);
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
                    OnLoadingException(ex, parentItem);

                    var message = LocalizationManager.Instance.Current.Keys.LoadingError_FailedToOpenXciPartition.SafeFormat(ex.Message);
                    parentItem.Errors.Add(TREE_LOADING_CATEGORY, message);
                    _logger.LogError(ex, message);
                    continue;
                }

                var xciPartitionItem = new XciPartitionItem(xciPartition, xciPartitionType, parentItem);
                BuildChildItems(xciPartitionItem);

                parentItem.ChildItems.Add(xciPartitionItem);
            }
        }
        catch (Exception ex)
        {
            OnLoadingException(ex, parentItem);

            var message = LocalizationManager.Instance.Current.Keys.LoadingError_FailedToLoadXciContent.SafeFormat(ex.Message);
            parentItem.Errors.Add(TREE_LOADING_CATEGORY, message);
            _logger.LogError(ex, message);
        }

    }

    private void BuildChildItems(PartitionFileSystemItemBase parentItem)
    {
        try
        {
            var partitionFileSystem = parentItem.PartitionFileSystem;

            var remainingEntries = new List<DirectoryEntryEx>();

            // First loop on *.tik files to inject title keys in KeySet
            foreach (var partitionFileEntry in partitionFileSystem.EnumerateEntries().Where(e => e.Type == DirectoryEntryType.File))
            {
                var fileName = partitionFileEntry.Name;
                if (!fileName.EndsWith(".tik", StringComparison.OrdinalIgnoreCase))
                {
                    remainingEntries.Add(partitionFileEntry);
                    continue;
                }

                IFile file;
                try
                {
                    file = partitionFileSystem.LoadFile(partitionFileEntry);
                }
                catch (Exception ex)
                {
                    OnLoadingException(ex, parentItem);

                    var message = LocalizationManager.Instance.Current.Keys.LoadingError_FailedToOpenPartitionFile.SafeFormat(ex.Message);
                    parentItem.Errors.Add(TREE_LOADING_CATEGORY, message);
                    _logger.LogError(ex, message);
                    continue;
                }

                Ticket ticket;
                try
                {
                    using var asStream = file.AsStream();
                    ticket = new Ticket(asStream);
                }
                catch (Exception ex)
                {
                    OnLoadingException(ex, parentItem);

                    var message = LocalizationManager.Instance.Current.Keys.LoadingError_FailedToLoadTicketFile.SafeFormat(ex.Message);
                    parentItem.Errors.Add(TREE_LOADING_CATEGORY, message);
                    _logger.LogError(ex, message);
                    continue;
                }

                var ticketItem = new TicketItem(ticket, partitionFileEntry, file, parentItem);

                try
                {
                    var rightsId = new RightsId(ticket.RightsId);
                    var accessKey = new AccessKey(ticket.TitleKeyBlock);

                    ticketItem.RightsId = rightsId;
                    ticketItem.AccessKey = accessKey;

                    if (parentItem.KeySet.ExternalKeySet.Get(rightsId, out var existingAccessKey) == Result.Success)
                    {
                        // Here RightID key is already defined
                        if (existingAccessKey != accessKey)
                        {
                            // Replaces the RightID key with the one defined in the ticket
                            parentItem.KeySet.ExternalKeySet.Remove(rightsId);
                            parentItem.KeySet.ExternalKeySet.Add(rightsId, accessKey).ThrowIfFailure();
                            _logger.LogWarning(LocalizationManager.Instance.Current.Keys.LoadingWarning_TitleIdKeyReplaced.SafeFormat(rightsId.ToString(), accessKey.ToString(), fileName, existingAccessKey));
                        }
                        else
                        {
                            _logger.LogDebug(LocalizationManager.Instance.Current.Keys.LoadingDebug_TitleIdKeyAlreadyExists.SafeFormat(rightsId.ToString(), accessKey.ToString(), fileName));
                        }
                    }
                    else
                    {
                        parentItem.KeySet.ExternalKeySet.Add(rightsId, accessKey).ThrowIfFailure();
                        _logger.LogInformation(LocalizationManager.Instance.Current.Keys.LoadingInfo_TitleIdKeySuccessfullyInjected.SafeFormat(rightsId.ToString(), accessKey.ToString(), fileName));
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, LocalizationManager.Instance.Current.Keys.LoadingError_FailedToLoadTitleIdKey.SafeFormat(fileName, ex.Message));
                }

                parentItem.TicketChildItems.Add(ticketItem);
            }

            foreach (var partitionFileEntry in remainingEntries)
            {
                IFile file;
                try
                {
                    file = partitionFileSystem.LoadFile(partitionFileEntry);
                }
                catch (Exception ex)
                {
                    OnLoadingException(ex, parentItem);

                    var message = LocalizationManager.Instance.Current.Keys.LoadingError_FailedToOpenPartitionFile.SafeFormat(ex.Message);
                    parentItem.Errors.Add(TREE_LOADING_CATEGORY, message);
                    _logger.LogError(ex, message);
                    continue;
                }

                var fileName = partitionFileEntry.Name;
                if (fileName.EndsWith(".nca", StringComparison.OrdinalIgnoreCase) || fileName.EndsWith(".ncz", StringComparison.OrdinalIgnoreCase))
                {
                    Nca nca;
                    try
                    {
                        nca = new Nca(parentItem.KeySet, new FileStorage(file));
                    }
                    catch (Exception ex)
                    {
                        OnLoadingException(ex, parentItem);

                        var message = LocalizationManager.Instance.Current.Keys.LoadingError_FailedToLoadNcaFile.SafeFormat(ex.Message);
                        parentItem.Errors.Add(TREE_LOADING_CATEGORY, message);
                        _logger.LogError(ex, message);
                        continue;
                    }

                    var ncaItem = new NcaItem(nca, partitionFileEntry, file, parentItem);
                    BuildChildItems(ncaItem);
                    parentItem.NcaChildItems.Add(ncaItem);
                }
                else
                {
                    var partitionFileEntryItem = new PartitionFileEntryItem(partitionFileEntry, file, parentItem);
                    parentItem.PartitionFileEntryChildItems.Add(partitionFileEntryItem);
                }
            }
        }
        catch (Exception ex)
        {
            OnLoadingException(ex, parentItem);

            var message = LocalizationManager.Instance.Current.Keys.LoadingError_FailedToLoadPartitionFileSystemContent.SafeFormat(ex.Message);
            parentItem.Errors.Add(TREE_LOADING_CATEGORY, message);
            _logger.LogError(ex, message);
        }
    }

    private void BuildChildItems(NcaItem parentItem)
    {

        try
        {

            var nca = parentItem.Nca;
            for (var sectionIndex = 0; sectionIndex < NcaItem.MaxSections; sectionIndex++)
            {
                try
                {
                    if (!nca.Header.IsSectionEnabled(sectionIndex))
                        continue;
                }
                catch (Exception ex)
                {
                    OnLoadingException(ex, parentItem);

                    var message = LocalizationManager.Instance.Current.Keys.LoadingError_FailedToCheckIfSectionCanBeOpened.SafeFormat(ex.Message);
                    parentItem.Errors.Add(TREE_LOADING_CATEGORY, message);
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
                    OnLoadingException(ex, parentItem);

                    var message = LocalizationManager.Instance.Current.Keys.LoadingError_FailedToGetNcaSectionFsHeader.SafeFormat(sectionIndex, ex.Message);
                    parentItem.Errors.Add(TREE_LOADING_CATEGORY, message);
                    _logger.LogError(ex, message);
                    continue;
                }


                var existsSparseLayer = ncaFsHeader.ExistsSparseLayer();
                var ncaSparseInfo = existsSparseLayer ? ncaFsHeader.GetSparseInfo() : (NcaSparseInfo?)null;

                var isPatchSection = ncaFsHeader.IsPatchSection();
                var ncaFsPatchInfo = isPatchSection ? ncaFsHeader.GetPatchInfo() : (NcaFsPatchInfo?)null;


                var sectionItem = new SectionItem(sectionIndex, ncaFsHeader, parentItem, ncaFsPatchInfo, ncaSparseInfo);

                parentItem.ChildItems.Add(sectionItem);


                if (!existsSparseLayer && !isPatchSection)
                {
                    IFileSystem? fileSystem = null;
                    try
                    {
                        fileSystem = nca.OpenFileSystem(sectionIndex, IntegrityCheckLevel.ErrorOnInvalid);
                    }
                    catch (Exception ex)
                    {
                        OnLoadingException(ex, sectionItem);

                        var message = LocalizationManager.Instance.Current.Keys.LoadingError_FailedToOpenNcaSectionFileSystem.SafeFormat(sectionIndex, ex.Message);
                        sectionItem.Errors.Add(TREE_LOADING_CATEGORY, message);
                        _logger.LogError(ex, message);
                    }

                    sectionItem.FileSystem = fileSystem;

                    BuildChildItems(sectionItem);
                }
            }
        }
        catch (Exception ex)
        {
            OnLoadingException(ex, parentItem);

            var message = LocalizationManager.Instance.Current.Keys.LoadingError_FailedToLoadNcaContent.SafeFormat(ex.Message);
            parentItem.Errors.Add(TREE_LOADING_CATEGORY, message);
            _logger.LogError(ex, message);
        }
    }

    private void BuildChildItems(SectionItem parentItem)
    {
        try
        {
            const string? ROOT_PATH = "/";

            var fileSystem = parentItem.FileSystem;
            if (fileSystem == null)
                return;

            var directoryEntries = SafeGetChildDirectoryEntries(fileSystem, ROOT_PATH, parentItem).ToArray();

            foreach (var directoryEntry in directoryEntries)
            {
                var entryName = directoryEntry.Name;
                var entryPath = directoryEntry.FullPath;

                // NACP File
                if (parentItem.ParentItem.ContentType == NcaContentType.Control && string.Equals(entryName, NacpItem.NACP_FILE_NAME, StringComparison.OrdinalIgnoreCase) && directoryEntry.Type == DirectoryEntryType.File)
                {
                    IFile nacpFile;
                    try
                    {
                        using var uniqueRefFile = new UniqueRef<IFile>();
                        fileSystem.OpenFile(ref uniqueRefFile.Ref, entryPath.ToU8Span(), OpenMode.Read).ThrowIfFailure();
                        nacpFile = uniqueRefFile.Release();
                    }
                    catch (Exception ex)
                    {
                        OnLoadingException(ex, parentItem);

                        var message = LocalizationManager.Instance.Current.Keys.LoadingError_FailedToOpenNacpFile.SafeFormat(ex.Message);
                        parentItem.Errors.Add(TREE_LOADING_CATEGORY, message);
                        _logger.LogError(ex, message);
                        continue;
                    }

                    ApplicationControlProperty nacp;
                    try
                    {
                        var blitStruct = new BlitStruct<ApplicationControlProperty>(1);
                        nacpFile.Read(out _, 0, blitStruct.ByteSpan).ThrowIfFailure();

                        nacp = blitStruct.Value;
                    }
                    catch (Exception ex)
                    {
                        OnLoadingException(ex, parentItem);

                        var message = LocalizationManager.Instance.Current.Keys.LoadingError_FailedToLoadNacpFile.SafeFormat(ex.Message);
                        parentItem.Errors.Add(TREE_LOADING_CATEGORY, message);
                        _logger.LogError(ex, message);
                        continue;
                    }

                    parentItem.ChildItems.Add(new NacpItem(nacp, parentItem, directoryEntry));
                }
                // CNMT File
                else if (parentItem.ParentItem.ContentType == NcaContentType.Meta && entryName.EndsWith(".cnmt", StringComparison.OrdinalIgnoreCase) && directoryEntry.Type == DirectoryEntryType.File)
                {
                    IFile cnmtFile;
                    try
                    {
                        using var uniqueRefFile = new UniqueRef<IFile>();
                        fileSystem.OpenFile(ref uniqueRefFile.Ref, entryPath.ToU8Span(), OpenMode.Read).ThrowIfFailure();
                        cnmtFile = uniqueRefFile.Release();
                    }
                    catch (Exception ex)
                    {
                        OnLoadingException(ex, parentItem);

                        var message = LocalizationManager.Instance.Current.Keys.LoadingError_FailedToOpenCnmtFile.SafeFormat(ex.Message);
                        parentItem.Errors.Add(TREE_LOADING_CATEGORY, message);
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
                        OnLoadingException(ex, parentItem);

                        var message = LocalizationManager.Instance.Current.Keys.LoadingError_FailedToLoadCnmtFile.SafeFormat(ex.Message);
                        parentItem.Errors.Add(TREE_LOADING_CATEGORY, message);
                        _logger.LogError(ex, message);
                        continue;
                    }
                    parentItem.ChildItems.Add(new CnmtItem(cnmt, parentItem, directoryEntry));
                }
                // MAIN file
                else if (parentItem.ParentItem.ContentType == NcaContentType.Program && string.Equals(entryName, MainItem.MAIN_FILE_NAME, StringComparison.OrdinalIgnoreCase) && directoryEntry.Type == DirectoryEntryType.File)
                {
                    IFile nsoFile;
                    try
                    {
                        using var uniqueRefFile = new UniqueRef<IFile>();
                        fileSystem.OpenFile(ref uniqueRefFile.Ref, entryPath.ToU8Span(), OpenMode.Read).ThrowIfFailure();
                        nsoFile = uniqueRefFile.Release();
                    }
                    catch (Exception ex)
                    {
                        OnLoadingException(ex, parentItem);

                        var message = LocalizationManager.Instance.Current.Keys.LoadingError_FailedToOpenMainFile.SafeFormat(ex.Message);
                        parentItem.Errors.Add(TREE_LOADING_CATEGORY, message);
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
                        OnLoadingException(ex, parentItem);

                        var message = LocalizationManager.Instance.Current.Keys.LoadingError_FailedToLoadMainFile.SafeFormat(ex.Message);
                        parentItem.Errors.Add(TREE_LOADING_CATEGORY, message);
                        _logger.LogError(ex, message);
                        continue;
                    }

                    parentItem.ChildItems.Add(new MainItem(nsoHeader.Value, parentItem, directoryEntry));
                }
                else
                {
                    var directoryEntryItem = new DirectoryEntryItem(parentItem, directoryEntry);
                    BuildChildItems(directoryEntryItem);
                    parentItem.ChildItems.Add(directoryEntryItem);
                }
            }
        }
        catch (Exception ex)
        {
            OnLoadingException(ex, parentItem);

            var message = LocalizationManager.Instance.Current.Keys.LoadingError_FailedToLoadSectionContent.SafeFormat(ex.Message);
            parentItem.Errors.Add(TREE_LOADING_CATEGORY, message);
            _logger.LogError(ex, message);
        }
    }

    private void BuildChildItems(DirectoryEntryItem parentItem)
    {
        try
        {
            if (parentItem.DirectoryEntryType == DirectoryEntryType.File)
                return;

            var currentPath = parentItem.Path;
            var directoryEntries = SafeGetChildDirectoryEntries(parentItem.ContainerSectionItem.FileSystem!, currentPath, parentItem);

            foreach (var directoryEntry in directoryEntries)
            {
                var directoryEntryItem = new DirectoryEntryItem(parentItem.ContainerSectionItem, directoryEntry, parentItem);
                BuildChildItems(directoryEntryItem);
                parentItem.ChildItems.Add(directoryEntryItem);
            }
        }
        catch (Exception ex)
        {
            OnLoadingException(ex, parentItem);

            var message = LocalizationManager.Instance.Current.Keys.LoadingError_FailedToLoadDirectoryContent.SafeFormat(ex.Message);
            parentItem.Errors.Add(TREE_LOADING_CATEGORY, message);
            _logger.LogError(ex, message);
        }
    }

    private IEnumerable<DirectoryEntryEx> SafeGetChildDirectoryEntries(IFileSystem fileSystem, string currentPath, IItem parentItem)
    {
        try
        {
            return fileSystem.EnumerateEntries(currentPath, "*", SearchOptions.Default);
        }
        catch (Exception ex)
        {
            OnLoadingException(ex, parentItem);

            var message = LocalizationManager.Instance.Current.Keys.LoadingError_FailedToGetFileSystemDirectoryEntries.SafeFormat(ex.Message);
            parentItem.Errors.Add(TREE_LOADING_CATEGORY, message);
            _logger.LogError(ex, message);
            return Array.Empty<DirectoryEntryEx>();
        }
    }

    private void OnLoadingException(Exception ex, IItem parentItem)
    {
        if (ex is MissingKeyException missingKeyException)
            NotifyMissingKey(missingKeyException, parentItem);
    }


    private void NotifyMissingKey(MissingKeyException ex, IItem parentItem)
    {
        MissingKey?.Invoke(this, new MissingKeyExceptionHandlerArgs(ex, parentItem));
    }
};
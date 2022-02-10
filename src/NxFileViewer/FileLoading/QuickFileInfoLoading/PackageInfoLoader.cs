using System;
using System.Collections.Generic;
using System.Linq;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Model.TreeItems.Impl;
using Emignatik.NxFileViewer.Services;
using LibHac.Common;
using LibHac.Common.Keys;
using LibHac.Fs;
using LibHac.Fs.Fsa;
using LibHac.FsSystem;
using LibHac.Tools.FsSystem;
using LibHac.Tools.FsSystem.NcaUtils;
using LibHac.Tools.Ncm;

namespace Emignatik.NxFileViewer.FileLoading.QuickFileInfoLoading;

public class PackageInfoLoader : IPackageInfoLoader
{
    private readonly IFileTypeAnalyzer _fileTypeAnalyzer;
    private readonly IKeySetProviderService _keySetProviderService;

    public PackageInfoLoader(IFileTypeAnalyzer fileTypeAnalyzer, IKeySetProviderService keySetProviderService)
    {
        _fileTypeAnalyzer = fileTypeAnalyzer ?? throw new ArgumentNullException(nameof(fileTypeAnalyzer));
        _keySetProviderService = keySetProviderService ?? throw new ArgumentNullException(nameof(keySetProviderService));
    }

    public PackageInfo GetPackageInfo(string filePath)
    {
        _fileTypeAnalyzer.GetFileType(filePath);
        var keySet = _keySetProviderService.GetKeySet();


        switch (_fileTypeAnalyzer.GetFileType(filePath))
        {
            case FileType.UNKNOWN:
                throw new FileNotSupportedException(filePath);

            case FileType.XCI:
                return LoadXciPackageInfo(filePath, keySet);
            case FileType.NSP:
                return LoadNspPackageInfo(filePath, keySet);

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private PackageInfo LoadNspPackageInfo(string nspFilePath, KeySet keySet)
    {
        using var localFile = new LocalFile(nspFilePath, OpenMode.Read);
        var fileStorage = new FileStorage(localFile);
        var nspPartition = new PartitionFileSystem(fileStorage);

        var cnmts = new List<Cnmt>();

        foreach (var partitionFileEntry in nspPartition.Files)
        {
            var fileName = partitionFileEntry.Name;
            if (!fileName.EndsWith("cnmt.nca", StringComparison.OrdinalIgnoreCase))
                continue;

            var file = nspPartition.OpenFile(partitionFileEntry, OpenMode.Read);

            var nca = new Nca(keySet, new FileStorage(file));

            if(nca.Header.ContentType != NcaContentType.Meta)
                continue;

            for (var sectionIndex = 0; sectionIndex < NcaItem.MaxSections; sectionIndex++)
            {
                if (!nca.Header.IsSectionEnabled(sectionIndex))
                    continue;

                var openFileSystem = nca.OpenFileSystem(sectionIndex, IntegrityCheckLevel.ErrorOnInvalid);

                var cnmtFileEntries = openFileSystem.EnumerateEntries("/", "*.cnmt").ToArray();

                var cnmtFileEntry = cnmtFileEntries[0];

                using var uniqueRefFile = new UniqueRef<IFile>();
                openFileSystem.OpenFile(ref uniqueRefFile.Ref(), cnmtFileEntry.FullPath.ToU8Span(), OpenMode.Read).ThrowIfFailure();
                var cnmtFile = uniqueRefFile.Release();

                var cnmt = new Cnmt(cnmtFile.AsStream());
                cnmts.Add(cnmt);

            }

        }

        //TODO: à finir
        return new PackageInfo
        {
            Metadata = cnmts,
        };
    }

    private PackageInfo LoadXciPackageInfo(string filePath, KeySet keySet)
    {
        //TODO: à finir
        return new PackageInfo();
    }



}
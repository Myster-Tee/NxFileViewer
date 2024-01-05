using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Emignatik.NxFileViewer.Services.KeysManagement;
using Emignatik.NxFileViewer.Utils;
using LibHac.Common.Keys;
using LibHac.Fs;
using LibHac.Fs.Fsa;
using LibHac.FsSystem;
using LibHac.Tools.Fs;
using LibHac.Tools.FsSystem;
using ContentType = LibHac.Ncm.ContentType;

namespace Emignatik.NxFileViewer.FileLoading.QuickFileInfoLoading;

public class PackageInfoLoader : IPackageInfoLoader
{
    private readonly IPackageTypeAnalyzer _packageTypeAnalyzer;
    private readonly IKeySetProviderService _keySetProviderService;

    public PackageInfoLoader(IPackageTypeAnalyzer packageTypeAnalyzer, IKeySetProviderService keySetProviderService)
    {
        _packageTypeAnalyzer = packageTypeAnalyzer ?? throw new ArgumentNullException(nameof(packageTypeAnalyzer));
        _keySetProviderService = keySetProviderService ?? throw new ArgumentNullException(nameof(keySetProviderService));
    }

    public PackageInfo GetPackageInfo(string filePath)
    {
        var keySet = _keySetProviderService.GetKeySet();

        AccuratePackageType accuratePackageType;
        List<Content> contents;
        switch (_packageTypeAnalyzer.GetType(filePath))
        {
            case PackageType.UNKNOWN:
                throw new FileNotSupportedException(filePath);

            case PackageType.XCI:
                contents = LoadXciContents(filePath, keySet, out accuratePackageType);
                break;
            case PackageType.NSP:
                contents = LoadNspContents(filePath, keySet, out accuratePackageType);

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        var packageInfo = new PackageInfo
        {
            PackageType = _packageTypeAnalyzer.GetType(filePath),
            AccuratePackageType = accuratePackageType,
            Contents = contents
        };

        return packageInfo;

    }

    private static List<Content> LoadNspContents(string nspFilePath, KeySet keySet, out AccuratePackageType accuratePackageType)
    {
        using var localFile = new LocalStorage(nspFilePath, FileAccess.Read);
        using var nspPartition = localFile.LoadPartition();

        var contents = LoadContentsFromPartition(nspPartition, keySet, out var containsNcz);

        accuratePackageType = containsNcz ? AccuratePackageType.NSZ : AccuratePackageType.NSP;

        return contents;
    }

    private static List<Content> LoadXciContents(string filePath, KeySet keySet, out AccuratePackageType accuratePackageType)
    {
        accuratePackageType = AccuratePackageType.XCI;

        using var localFile = new LocalFile(filePath, OpenMode.Read);

        var fileStorage = new FileStorage(localFile);
        var xci = new Xci(keySet, fileStorage);

        var contents = new List<Content>();

        if (xci.HasPartition(XciPartitionType.Secure))
        {
            var xciPartition = xci.OpenPartition(XciPartitionType.Secure);

            var partitionContents = LoadContentsFromPartition(xciPartition, keySet, out var containsNcz);
            contents.AddRange(partitionContents);

            if (accuratePackageType == AccuratePackageType.XCI && containsNcz)
                accuratePackageType = AccuratePackageType.XCZ;
        }

        return contents;
    }

    private static List<Content> LoadContentsFromPartition(IFileSystem partition, KeySet keySet, out bool containsNcz)
    {
        containsNcz = partition.EnumerateEntries()
            .Where(e => e.Type == DirectoryEntryType.File)
            .Any(entry => entry.Name.EndsWith(".ncz", StringComparison.OrdinalIgnoreCase));

        var contents = new List<Content>();

        foreach (var cnmt in partition.LoadCnmts(keySet))
        {
            var content = new Content(cnmt);
            contents.Add(content);

            var ncaControlEntry = cnmt.ContentEntries.FirstOrDefault(entry => entry.Type == ContentType.Control);

            if (ncaControlEntry != null)
            {
                var ncaId = ncaControlEntry.NcaId.ToStrId();

                var nacp = partition.LoadNacp(ncaId, keySet);

                if (nacp != null)
                {
                    content.NacpData = new NacpData(nacp.Value);
                }
            }
        }

        return contents;
    }

}
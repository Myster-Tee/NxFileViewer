using System;
using System.Collections.Generic;
using System.Linq;
using Emignatik.NxFileViewer.Services.KeysManagement;
using Emignatik.NxFileViewer.Utils;
using LibHac.Common.Keys;
using LibHac.Fs;
using LibHac.FsSystem;
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
        using var localFile = new LocalFile(nspFilePath, OpenMode.Read);
        var fileStorage = new FileStorage(localFile);
        var nspPartition = new PartitionFileSystem(fileStorage);

        accuratePackageType = nspPartition.Files.Any(entry => entry.Name.EndsWith(".ncz", StringComparison.OrdinalIgnoreCase)) ? 
            AccuratePackageType.NSZ :
            AccuratePackageType.NSP;

        var contents = new List<Content>();

        foreach (var cnmt in nspPartition.LoadCnmts(keySet))
        {


            var content = new Content(cnmt);
            contents.Add(content);

            var ncaControlEntry = cnmt.ContentEntries.FirstOrDefault(entry => entry.Type == ContentType.Control);

            if (ncaControlEntry != null)
            {
                var ncaId = ncaControlEntry.NcaId.ToStrId();

                var nacp = nspPartition.LoadNacp(ncaId, keySet);

                if (nacp != null)
                {
                    content.NacpData = new NacpData(nacp.Value);
                }

            }
        }

        return contents;
        //TODO: à finir
    }

    private static List<Content> LoadXciContents(string filePath, KeySet keySet, out AccuratePackageType accuratePackageType)
    {
        accuratePackageType = default;
        return new List<Content>();
        //TODO: à finir
    }



}
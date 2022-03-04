﻿using System;
using System.Collections.Generic;
using System.Linq;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Model.TreeItems.Impl;
using Emignatik.NxFileViewer.Services;
using Emignatik.NxFileViewer.Utils;
using LibHac.Common;
using LibHac.Common.Keys;
using LibHac.Fs;
using LibHac.Fs.Fsa;
using LibHac.FsSystem;
using LibHac.Tools.FsSystem;
using LibHac.Tools.FsSystem.NcaUtils;
using LibHac.Tools.Ncm;
using ContentType = LibHac.Ncm.ContentType;

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

        foreach (var cnmt in nspPartition.LoadCnmts(keySet))
        {
            var ncaControlEntry = cnmt.ContentEntries.FirstOrDefault(entry => entry.Type == ContentType.Control);

            if (ncaControlEntry != null)
            {
                var ncaId = ncaControlEntry.NcaId.ToStrId();

                var nacp = nspPartition.LoadNacp(ncaId, keySet);

                if (nacp != null)
                {
                    var s = nacp.Value.Titles[0].Name.ToString();


                    var valueSupportedLanguages = nacp.Value.SupportedLanguages;
                }

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
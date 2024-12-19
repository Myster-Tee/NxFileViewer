using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Emignatik.NxFileViewer.Models.Overview;
using Emignatik.NxFileViewer.Utils;
using LibHac.Common;
using LibHac.Ncm;
using LibHac.Ns;
using LibHac.Tools.FsSystem.NcaUtils;
using LibHac.Tools.Ncm;

namespace Emignatik.NxFileViewer.FileLoading.QuickFileInfoLoading;

public class PackageInfo
{
    public List<Content> Contents { get; init; } = null!;

    public AccuratePackageType AccuratePackageType { get; init; }

    public PackageType PackageType { get; init; }
}

public enum AccuratePackageType
{
    NSP,
    NSZ,
    XCI,
    XCZ
}

/// <summary>
/// A Nintendo Switch package (XCI, NSP, ...) can contain a single content or multiple contents (known as SuperXCI or SuperNSP).
/// Here a <see cref="Content"/> is a model aggregating various information based on a <see cref="Cnmt"/> metadata entry.
/// A <see cref="Content"/> is created for each <see cref="Cnmt"/> found in a Nintendo Switch package.
/// </summary>
public class Content
{
    private readonly Cnmt _cnmt;

    public Content(Cnmt cnmt)
    {
        _cnmt = cnmt ?? throw new ArgumentNullException(nameof(cnmt));
    }

    public ContentMetaType Type => _cnmt.Type;

    public string TitleId => _cnmt.TitleId.ToStrId();

    public string ApplicationTitleId => _cnmt.ApplicationTitleId.ToStrId();

    public string PatchTitleId => _cnmt.PatchTitleId.ToStrId();

    public TitleVersion MinimumApplicationVersion => _cnmt.MinimumApplicationVersion;

    public NacpData? NacpData { get; set; }

    public TitleVersion Version => _cnmt.TitleVersion;

    public int PatchNumber => _cnmt.TitleVersion.GetPatchNumber();

    public string DisplayVersion
    {
        get
        {
            U8Span _displayVersion = new U8Span("0");
            if (NacpData != null)
            {
                _displayVersion = NacpData.DisplayVersionString;
            }
            return _displayVersion.ToString();
        }
    }

    public string XCIUpdateVersion = "zzz";

}

public class NacpData
{
    private readonly ApplicationControlProperty _applicationControlProperty;
 
    public U8Span DisplayVersionString => new U8Span(_applicationControlProperty.DisplayVersion.ItemsRo);

    public NacpData(ApplicationControlProperty applicationControlProperty)
    {
        _applicationControlProperty = applicationControlProperty;

        var mask = 1;

        var titles = new Title?[16];

        for (var i = 0; i < 16; i++)
        {

            if ((_applicationControlProperty.SupportedLanguageFlag & mask) == mask)
            {
                titles[i] = new Title(_applicationControlProperty.Title[i]);
            }
            else
            {
                titles[i] = null;
            }

            mask <<= 1;
        }

        Titles = titles;
    }

    public IReadOnlyList<Title?> Titles { get; }

}

public class Title
{
    private readonly ApplicationControlProperty.ApplicationTitle _applicationControlTitle;

    public Title(ApplicationControlProperty.ApplicationTitle applicationControlTitle)
    {
        _applicationControlTitle = applicationControlTitle;
    }

    public string Name => _applicationControlTitle.NameString.ToString();

    public string Publisher => _applicationControlTitle.PublisherString.ToString();
}
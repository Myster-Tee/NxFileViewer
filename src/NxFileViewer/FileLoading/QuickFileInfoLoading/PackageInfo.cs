using System;
using System.Collections.Generic;
using Emignatik.NxFileViewer.Utils;
using LibHac.Ncm;
using LibHac.Ns;
using LibHac.Tools.FsSystem.NcaUtils;
using LibHac.Tools.Ncm;

namespace Emignatik.NxFileViewer.FileLoading.QuickFileInfoLoading;

public class PackageInfo
{
    public List<Content> Contents { get; init; }

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

public class Content
{
    private readonly Cnmt _cnmt;

    public Content(Cnmt cnmt)
    {
        _cnmt = cnmt ?? throw new ArgumentNullException(nameof(cnmt));
    }

    public ContentMetaType Type => _cnmt.Type;

    public string TitleId => _cnmt.TitleId.ToStrId();

    public TitleVersion MinimumApplicationVersion => _cnmt.MinimumApplicationVersion;

    public NacpData? NacpData { get; set; }

}

public class NacpData
{
    private readonly ApplicationControlProperty _applicationControlProperty;

    public NacpData(ApplicationControlProperty applicationControlProperty)
    {
        _applicationControlProperty = applicationControlProperty;

        var mask = 1;

        var titles = new Title?[16];

        for (var i = 0; i < 16; i++)
        {

            if ((_applicationControlProperty.SupportedLanguages & mask) == mask)
            {
                titles[i] = new Title(_applicationControlProperty.Titles[i]);
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
    private readonly ApplicationControlTitle _applicationControlTitle;

    public Title(ApplicationControlTitle applicationControlTitle)
    {
        _applicationControlTitle = applicationControlTitle;
    }

    public string Name => _applicationControlTitle.Name.ToString();

    public string Publisher => _applicationControlTitle.Publisher.ToString();
}
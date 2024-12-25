using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Emignatik.NxFileViewer.Models.TreeItems;
using Emignatik.NxFileViewer.Models.TreeItems.Impl;
using Emignatik.NxFileViewer.Utils.MVVM;

namespace Emignatik.NxFileViewer.Models.Overview;

public class FileOverview : NotifyPropertyChangedBase
{
    private NcasIntegrity _ncasIntegrity;
    private NcaCompressionType? _ncaCompressionType;
    private NxBaseFileType? _basePackageType;
    private NxFileType? _packageType;


    public FileOverview(IItem rootItem)
    {
        RootItem = rootItem ?? throw new ArgumentNullException(nameof(rootItem));
        NcasIntegrity = NcasIntegrity.Unchecked;
    }

    public ObservableCollection<MissingKey> MissingKeys { get; } = new();

    public IItem RootItem { get; }

    public bool IsSuperPackage => CnmtContainers.Count > 1;

    public NxBaseFileType BaseFileType => _basePackageType ??= DetermineBasePackageType();

    public NxFileType FileType => _packageType ??= DeterminePackageType();

    public NcaCompressionType NcaCompressionType => _ncaCompressionType ??= DetermineNcaCompressionType();

    private NcaCompressionType DetermineNcaCompressionType()
    {
        var nczItems = RootItem.FindChildrenOfType<NcaItem>(includeItem: true).OfType<NczItem>().ToArray();
        if (nczItems.Length <= 0)
            return NcaCompressionType.None;

        var blocklessCompressionFound = nczItems.Any(nczItem => !nczItem.Ncz.NczHeader.IsUsingBlockCompression);

        return blocklessCompressionFound ? NcaCompressionType.Blockless : NcaCompressionType.Block;
    }

    private NxFileType DeterminePackageType()
    {
        var compressed = NcaCompressionType != NcaCompressionType.None;
        var isSuperPackage = IsSuperPackage;

        return BaseFileType switch
        {
            NxBaseFileType.XCI => (isSuperPackage, compressed) switch
            {
                (false, false) => NxFileType.XCI,
                (false, true) => NxFileType.XCZ,
                (true, false) => NxFileType.SuperXCI,
                (true, true) => NxFileType.SuperXCZ
            },
            NxBaseFileType.NSP => (isSuperPackage, compressed) switch
            {
                (false, false) => NxFileType.NSP,
                (false, true) => NxFileType.NSZ,
                (true, false) => NxFileType.SuperNSP,
                (true, true) => NxFileType.SuperNSZ
            },
            _ => NxFileType.Unknown
        };
    }

    public List<CnmtContainer> CnmtContainers { get; } = new();

    public NcasIntegrity NcasIntegrity
    {
        get => _ncasIntegrity;
        set
        {
            _ncasIntegrity = value;
            NotifyPropertyChanged();
        }
    }

    private NxBaseFileType DetermineBasePackageType()
    {
        NxBaseFileType baseFileType;

        var rootItemType = RootItem.GetType();
        if (rootItemType == typeof(XciItem))
            baseFileType = NxBaseFileType.XCI;
        else if (rootItemType == typeof(NspItem))
            baseFileType = NxBaseFileType.NSP;
        else
            baseFileType = NxBaseFileType.Unknown;

        return baseFileType;
    }


}

public enum NcasIntegrity
{
    /// <summary>
    /// Unchecked integrity
    /// </summary>
    Unchecked,
    /// <summary>
    /// Checking the integrity of the NCAs is in progress
    /// </summary>
    InProgress,
    /// <summary>
    /// All present NCAs are original
    /// </summary>
    Original,
    /// <summary>
    /// All present NCAs are original, but some are missing
    /// </summary>
    Incomplete,
    /// <summary>
    /// At least one NCA is modified (signature failed, but hash is OK)
    /// </summary>
    Modified,
    /// <summary>
    /// At least one NCA is corrupted (hash failed)
    /// </summary>
    Corrupted,
    /// <summary>
    /// An error occurred during the integrity check
    /// </summary>
    Error,
    /// <summary>
    /// No NCA files are present
    /// </summary>
    NoNca
}

public enum NxFileType
{
    SuperXCI,
    SuperXCZ,
    SuperNSP,
    SuperNSZ,
    XCI,
    XCZ,
    NSP,
    NSZ,
    Unknown,
}

public enum NxBaseFileType
{
    XCI,
    NSP,
    Unknown,
}

public enum NcaCompressionType
{
    None,
    Blockless,
    Block,
}


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
    private NcasValidity _ncasHashValidity;
    private NcaItem[]? _ncaItems;
    private NcasValidity _ncasHeadersSignatureValidity;
    private IReadOnlyList<Exception>? _ncasHeadersSignatureExceptions;
    private IReadOnlyList<Exception>? _ncasHashExceptions;

    public FileOverview(IItem rootItem)
    {
        RootItem = rootItem ?? throw new ArgumentNullException(nameof(rootItem));
        NcasHashValidity = NcaItems.Count <= 0 ? NcasValidity.NoNca : NcasValidity.Unchecked;
        NcasHeadersSignatureValidity = NcaItems.Count <= 0 ? NcasValidity.NoNca : NcasValidity.Unchecked;
    }

    public ObservableCollection<MissingKey> MissingKeys { get; } = new();

    public IItem RootItem { get; }

    public List<CnmtContainer> CnmtContainers { get; } = new();

    public PackageType PackageType { get; internal set; } = PackageType.Unknown;

    /// <summary>
    /// Shortcut which returns the list of all <see cref="NcaItem"/> contained in the <see cref="RootItem"/>
    /// (<see cref="ItemExtension.FindAllNcaItems"/>)
    /// </summary>
    public IReadOnlyCollection<NcaItem> NcaItems
    {
        get
        {
            return _ncaItems ??= RootItem.FindAllNcaItems().ToArray();
        }
    }

    public NcasValidity NcasHashValidity
    {
        get => _ncasHashValidity;
        set
        {
            _ncasHashValidity = value;
            NotifyPropertyChanged();
        }
    }

    public IReadOnlyList<Exception>? NcasHashExceptions
    {
        get => _ncasHashExceptions;
        internal set
        {
            _ncasHashExceptions = value;
            NotifyPropertyChanged();
        }
    }

    public NcasValidity NcasHeadersSignatureValidity
    {
        get => _ncasHeadersSignatureValidity;
        set
        {
            _ncasHeadersSignatureValidity = value;
            NotifyPropertyChanged();
        }
    }

    public IReadOnlyList<Exception>? NcasHeadersSignatureExceptions
    {
        get => _ncasHeadersSignatureExceptions;
        internal set
        {
            _ncasHeadersSignatureExceptions = value;
            NotifyPropertyChanged();
        }
    }

}

public enum NcasValidity
{
    NoNca,
    Unchecked,
    InProgress,
    Invalid,
    Valid,
    Error
}

public enum PackageType
{
    SuperXCI,
    SuperNSP,
    Normal,
    Unknown,
}
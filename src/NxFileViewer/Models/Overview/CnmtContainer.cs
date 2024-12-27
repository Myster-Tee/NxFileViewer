using System;
using Emignatik.NxFileViewer.Models.TreeItems.Impl;
using LibHac.Tools.FsSystem.NcaUtils;

namespace Emignatik.NxFileViewer.Models.Overview;

/// <summary>
/// Aggregates a <see cref="TreeItems.Impl.CnmtItem"/> with its corresponding <see cref="Overview.NacpContainer"/>
/// </summary>
public class CnmtContainer 
{
    public CnmtContainer(CnmtItem cnmtItem)
    {
        CnmtItem = cnmtItem ?? throw new ArgumentNullException(nameof(cnmtItem));
    }

    public CnmtItem CnmtItem { get; }

    /// <summary>
    /// Get the NACP container referenced by this CNMT container if found
    /// </summary>
    public NacpContainer? NacpContainer { get; set; }

    /// <summary>
    /// The main item
    /// </summary>
    public MainItem? MainItem { get; set; }

    /// <summary>
    /// True when <see cref="SectionItem"/> of type <see cref="NcaSectionType.Code"/> is sparse, which means <see cref="MainItem"/> is normally null
    /// </summary>
    public bool MainItemSectionIsSparse { get; set; }
}
using System;
using Emignatik.NxFileViewer.Models.TreeItems.Impl;

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

}
using System.Collections.Generic;
using LibHac.Tools.FsSystem.NcaUtils;

namespace Emignatik.NxFileViewer.Models.TreeItems.Impl;

public class PatchSectionItem : SectionItemBase
{
    private readonly NcaFsPatchInfo _ncaFsPatchInfo;

    public PatchSectionItem(int sectionIndex, NcaFsHeader ncaFsHeader, NcaItem parentItem, ref NcaFsPatchInfo ncaFsPatchInfo) : base(sectionIndex, ncaFsHeader, parentItem)
    {
        _ncaFsPatchInfo = ncaFsPatchInfo;
    }

    public long RelocationTreeOffset => _ncaFsPatchInfo.RelocationTreeOffset;
    
    public long RelocationTreeSize => _ncaFsPatchInfo.RelocationTreeSize;
    
    public long EncryptionTreeOffset => _ncaFsPatchInfo.EncryptionTreeOffset;

    public long EncryptionTreeSize => _ncaFsPatchInfo.EncryptionTreeSize;

    public override IEnumerable<IItem> ChildItems
    {
        get
        {
            yield break;
        }
    }
}
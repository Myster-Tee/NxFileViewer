using LibHac.NSZ;
using LibHac.Tools.Fs;
using LibHac.Tools.FsSystem.NcaUtils;

namespace Emignatik.NxFileViewer.Models.TreeItems.Impl;

public class NczItem : NcaItem
{
    public Ncz Ncz { get; }

    public NczItem(Ncz ncz, DirectoryEntryEx partitionFileEntry, PartitionFileSystemItemBase parentItem) : base(ncz, partitionFileEntry, parentItem)
    {
        Ncz = ncz;
    }

    public override Nca GetOriginalNca()
    {
        return Ncz.SwitchReadMode(NczReadMode.Original);
    }
}
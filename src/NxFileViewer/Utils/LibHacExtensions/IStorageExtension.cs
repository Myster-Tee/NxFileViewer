using LibHac.Common;
using LibHac.FsSystem;
using LibHac.Fs;

namespace Emignatik.NxFileViewer.Utils.LibHacExtensions;

public static class IStorageExtension
{
    public static PartitionFileSystem LoadPartition(this IStorage storage)
    {
        var pfs = new UniqueRef<PartitionFileSystem>(new PartitionFileSystem());
        pfs.Get.Initialize(storage).ThrowIfFailure();
        return pfs.Get;
    }
}
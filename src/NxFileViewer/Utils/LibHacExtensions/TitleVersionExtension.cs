using LibHac.Tools.FsSystem.NcaUtils;

namespace Emignatik.NxFileViewer.Utils.LibHacExtensions;

public static class TitleVersionExtension
{

    public static int GetPatchNumber(this TitleVersion titleVersion)
    {
        return titleVersion.Minor;
    }
}
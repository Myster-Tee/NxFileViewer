using LibHac.FsSystem.NcaUtils;

namespace Emignatik.NxFileViewer.NSP.Models
{
    public class NcaSectionInfo
    {
        public NcaFormatType FormatType { get; set; }

        public NcaEncryptionType EncryptionType { get; set; }

        public int Index { get; set; }
    }
}
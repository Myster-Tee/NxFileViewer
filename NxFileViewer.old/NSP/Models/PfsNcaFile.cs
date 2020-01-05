using LibHac.FsSystem.NcaUtils;

namespace Emignatik.NxFileViewer.NSP.Models
{
    public class PfsNcaFile : PfsFile
    {

        public ulong TitleId { get; set; }

        public NcaContentType ContentType { get; set; }

        public string SdkVersion { get; set; }

        public NcaSectionInfo[] DefinedSections { get; set; }

        public DistributionType DistributionType { get; set; }
    }
}
using LibHac.Fs.NcaUtils;

namespace Emignatik.NxFileViewer.NSP.Models
{
    public class PfsNcaFile : PfsFile
    {

        public ulong TitleId { get; set; }

        public ContentType ContentType { get; set; }

        public string SdkVersion { get; set; }
    }


}
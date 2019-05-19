using LibHac.Fs.NcaUtils;

namespace Emignatik.NxFileViewer.NSP.Models
{
    public class PfsNcaFile : PfsFile
    {
        public NcaHeader Header { get; set; }
    }
}
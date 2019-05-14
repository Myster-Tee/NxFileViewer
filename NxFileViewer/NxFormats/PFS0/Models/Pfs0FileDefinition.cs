using Emignatik.NxFileViewer.NxFormats.PFS0.Structs;

namespace Emignatik.NxFileViewer.NxFormats.PFS0.Models
{
    /// <summary>
    /// Represents information of a file contained in a PFS0
    /// </summary>
    public class Pfs0FileDefinition
    {
        public string FileName { get; set; }

        public ulong FileStartPos { get; set; }

        public ulong FileSize { get; set; }

        public Pfs0FileEntryStruct RawStruct { get; set; }

        public override string ToString()
        {
            return FileName;
        }
    }
}
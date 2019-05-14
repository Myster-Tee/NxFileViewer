using Emignatik.NxFileViewer.NxFormats.NCA.Structs;

namespace Emignatik.NxFileViewer.NxFormats.NCA.Models
{
    public class NcaSectionHeader
    {
        public NcaSectionHeaderWithoutSuperBlockStruct SectionHeaderStruct { get; set; }

        public object SuperBlock { get; set; }

        public NcaSectionIndex SectionIndex { get; set; }
    }
}
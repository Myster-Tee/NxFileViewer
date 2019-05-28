using LibHac;

namespace Emignatik.NxFileViewer.NSP.Models
{
    public class CnmtInfo
    {
        public TitleType Type { get; set; }

        public string TitleId { get; set; }

        public uint TitleVersion { get; set; }

        /// <summary>
        /// Gets the id of the NCA of content type "ControlPartition"
        /// Can be null if package contains no such NCA, which is observed for add-ons
        /// </summary>
        public string LinkedNcaControlId { get; set; }

        public string FilePath { get; set; }

        /// <summary>
        /// Gets information contained in control partition (control.nacp file and icons)
        /// Maybe empty for add-ons
        /// </summary>
        public ControlPartitionInfo ControlPartition { get; set; }
    }
}
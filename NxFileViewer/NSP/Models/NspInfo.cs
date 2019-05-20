using System.Collections.Generic;
using Emignatik.NxFileViewer.NxFormats.CNMT.Models;
using Emignatik.NxFileViewer.NxFormats.NCA.Models;
using Emignatik.NxFileViewer.NxFormats.NCA.Structs;

namespace Emignatik.NxFileViewer.NSP.Models
{
    /// <summary>
    /// Model aggregating all useful information contained in a NSP file
    /// </summary>
    public class NspInfo
    {

        /// <summary>
        /// Gets the header of the metadata file (*.cnmt.nca)
        /// </summary>
        public CnmtInfo CnmtInfo { get; set; }

        /// <summary>
        /// Gets the list of files in NSP
        /// </summary>
        public PfsFile[] Files { get; set; }

        public NacpInfo NacpInfo { get; set; }

        public List<IconInfo> Icons { get; set; }

        ///// <summary>
        ///// Header of the NCA file of <see cref="NcaContentType.META"/> type
        ///// </summary>
        //public NcaHeader NcaMetaHeader { get; set; }

        ///// <summary>
        ///// Can be null if NSP contains no NCA of type <see cref="NcaContentType.CONTROL"/>
        ///// (Which is observed to be normal for Add-ons)
        ///// </summary>
        //public NcaControlContent NcaControlContent { get; set; }
    }
}
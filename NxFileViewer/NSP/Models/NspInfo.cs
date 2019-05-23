using System.Collections.Generic;

namespace Emignatik.NxFileViewer.NSP.Models
{
    /// <summary>
    /// Model aggregating all useful information contained in a NSP file
    /// </summary>
    public class NspInfo
    {

        /// <summary>
        /// Gets the list of files in NSP
        /// </summary>
        public PfsFile[] Files { get; set; }

        //TODO: use this property instead of CnmtInfo (to super SuperNSP)
        /// <summary>
        /// Gets the header of the metadata file (*.cnmt.nca)
        /// </summary>
        public CnmtInfo[] Cnmts { get; set; }

        public bool IsSuperNsp
        {
            get
            {
                var cnmts = Cnmts;
                return cnmts != null && cnmts.Length > 1;
            }
        }

    }
}
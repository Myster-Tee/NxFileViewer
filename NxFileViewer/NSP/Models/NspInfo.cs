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

        /// <summary>
        /// Gets or sets the file location
        /// </summary>
        public string Location { get; set; }
    }
}
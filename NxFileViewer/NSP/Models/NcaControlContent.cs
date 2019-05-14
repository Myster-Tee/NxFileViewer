using System.Collections.Generic;
using Emignatik.NxFileViewer.NxFormats.NACP.Models;
using Emignatik.NxFileViewer.NxFormats.NCA.Structs;

namespace Emignatik.NxFileViewer.NSP.Models
{

    /// <summary>
    /// Model aggregating informations contained in an NCA file of type <see cref="NcaContentType.CONTROL"/>
    /// </summary>
    public class NcaControlContent
    {
        public NacpContent NacpContent { get; set; }

        public List<LocalizedIcon> Icons { get; set; }
    }
}
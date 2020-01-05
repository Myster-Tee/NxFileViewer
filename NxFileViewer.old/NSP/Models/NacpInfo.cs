using System.Collections.Generic;

namespace Emignatik.NxFileViewer.NSP.Models
{
    public class NacpInfo
    {
        public string DisplayVersion { get; set; }

        public TitleInfo[] Titles { get; set; }
    }
}
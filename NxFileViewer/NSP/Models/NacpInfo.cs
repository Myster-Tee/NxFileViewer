using System.Collections.Generic;

namespace Emignatik.NxFileViewer.NSP.Models
{
    public class NacpInfo
    {
        public string DisplayVersion { get; set; }

        public List<TitleInfo> Titles { get; set; }
    }
}
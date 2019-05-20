using System.Windows.Media.Imaging;
using Emignatik.NxFileViewer.NxFormats.NACP.Structs;

namespace Emignatik.NxFileViewer.NSP.Models
{
    public class IconInfo
    {
        public BitmapImage Image { get; set; }

        public NacpLanguage Language { get; set; }
    }
}

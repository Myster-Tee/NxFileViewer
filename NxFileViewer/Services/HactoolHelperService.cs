using Emignatik.NxFileViewer.Hactool;
using Emignatik.NxFileViewer.Properties;
using Emignatik.NxFileViewer.Utils;

namespace Emignatik.NxFileViewer.Services
{
    public static class HactoolHelperProviderService
    {
        private static HactoolHelper _hactoolHelper;

        public static HactoolHelper Get()
        {
            if (_hactoolHelper != null)
                return _hactoolHelper;

            _hactoolHelper = new HactoolHelper
            {
                HactoolFilePath = Settings.Default.HactoolFilePath.ToFullPath(),
                KeySetFilePath = Settings.Default.KeysFilePath.ToFullPath(),
            };

            return _hactoolHelper;
        }

    }
}

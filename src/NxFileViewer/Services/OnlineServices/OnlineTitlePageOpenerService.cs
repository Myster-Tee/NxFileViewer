using System;
using System.Diagnostics;
using Emignatik.NxFileViewer.Settings;

namespace Emignatik.NxFileViewer.Services.OnlineServices
{
    public class OnlineTitlePageOpenerService : IOnlineTitlePageOpenerService
    {
        private readonly IAppSettings _appSettings;

        public OnlineTitlePageOpenerService(IAppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        public void OpenTitlePage(string titleId)
        {
            var url = _appSettings.TitlePageUrl.Replace("{TitleId}", Uri.EscapeDataString(titleId), StringComparison.OrdinalIgnoreCase);

            var processStartInfo = new ProcessStartInfo(url)
            {
                CreateNoWindow = true,
                UseShellExecute = true
            };
            Process.Start(processStartInfo);
        }
    }

    public interface IOnlineTitlePageOpenerService
    {
        void OpenTitlePage(string titleId);
    }
}

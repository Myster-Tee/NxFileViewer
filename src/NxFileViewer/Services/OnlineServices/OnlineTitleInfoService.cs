using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Emignatik.NxFileViewer.Settings;

namespace Emignatik.NxFileViewer.Services.OnlineServices;

public class OnlineTitleInfoService : IOnlineTitleInfoService
{
    private readonly IAppSettings _appSettings;

    public OnlineTitleInfoService(IAppSettings appSettings)
    {
        _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
    }


    public async Task<IOnlineTitleInfo?> GetTitleInfoAsync(string titleId)
    {
        var url = _appSettings.TitleInfoApiUrl.Replace("{TitleId}", Uri.EscapeDataString(titleId), StringComparison.OrdinalIgnoreCase);

        var uri = new Uri(url);

        using var httpClient = new HttpClient();

        var onlineTitleInfo = await httpClient.GetFromJsonAsync<OnlineTitleInfo>(uri);
        return onlineTitleInfo;
    }
}
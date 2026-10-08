namespace Emignatik.NxFileViewer.Services.OnlineServices;

public interface ICachedOnlineTitleInfoService : IOnlineTitleInfoService
{
    void ClearCache();
    public bool IsEnabled { get; set; }

}
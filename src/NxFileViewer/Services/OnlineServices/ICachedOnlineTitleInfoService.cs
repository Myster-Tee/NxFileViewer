namespace Emignatik.NxFileViewer.Services.OnlineServices;

public interface ICachedOnlineTitleInfoService : IOnlineTitleInfoService
{
    public bool IsEnabled { get; set; }

}
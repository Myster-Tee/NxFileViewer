using System.Threading.Tasks;

namespace Emignatik.NxFileViewer.Services.OnlineServices;

public interface IOnlineTitleInfoService
{
    Task<IOnlineTitleInfo?> GetTitleInfoAsync(string titleId);
}
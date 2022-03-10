namespace Emignatik.NxFileViewer.Services.OnlineServices;

public interface IOnlineTitleInfo
{
    string Id { get; set; }
    string Name { get; set; }
    string Publisher { get; set; }
    string IconUrl { get; set; }
    string Description { get; set; }
    object Playtime { get; set; }
    double Rating { get; set; }
}
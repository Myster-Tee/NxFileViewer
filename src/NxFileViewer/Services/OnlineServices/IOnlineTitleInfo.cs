namespace Emignatik.NxFileViewer.Services.OnlineServices;

public interface IOnlineTitleInfo
{
    string Id { get; }
    string Name { get; }
    string Publisher { get; }
    string IconUrl { get; }
    string Description { get; }
    object Playtime { get; }
    double Rating { get; }
}
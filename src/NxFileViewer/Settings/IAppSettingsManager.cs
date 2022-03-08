namespace Emignatik.NxFileViewer.Settings;

public interface IAppSettingsManager
{
    IAppSettings Settings { get; }

    public void LoadDefault();

    public bool LoadSafe();

    public void SaveSafe();
}
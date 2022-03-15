namespace Emignatik.NxFileViewer.Settings;

public interface IAppSettingsManager
{
    /// <summary>
    /// Provides application settings.
    /// This instance is never changed even when settings are reloaded.
    /// </summary>
    IAppSettings Settings { get; }

    // TODO: utiliser cette méthode
    public void RestoreDefaultSettings();

    public bool LoadSafe();

    public void SaveSafe();
}
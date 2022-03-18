namespace Emignatik.NxFileViewer.Settings;

public interface IAppSettingsManager
{
    /// <summary>
    /// Provides application settings.
    /// This instance is never changed even when settings are reloaded.
    /// </summary>
    IAppSettings Settings { get; }


    IAppSettings Clone();

    // TODO: utiliser cette méthode
    void RestoreDefaultSettings();

    bool LoadSafe();

    void SaveSafe();
}
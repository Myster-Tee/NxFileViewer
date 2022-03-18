namespace Emignatik.NxFileViewer.Settings;

public interface IAppSettingsManager
{
    /// <summary>
    /// Provides application settings.
    /// This instance is never changed even when settings are reloaded.
    /// </summary>
    IAppSettings Settings { get; }

    /// <summary>
    /// Returns a copy of actual settings
    /// </summary>
    /// <returns></returns>
    IAppSettings Clone();

    /// <summary>
    /// Returns a copy default settings
    /// </summary>
    /// <returns></returns>
    IAppSettings GetDefault();

    /// <summary>
    /// Loads the settings from the specified settings.
    /// (Updates <see cref="Settings"/>( with the geven)
    /// </summary>
    /// <param name="appSettings"></param>
    void Load(IAppSettings appSettings);

    bool LoadSafe();

    void SaveSafe();

}
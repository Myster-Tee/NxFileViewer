using Emignatik.NxFileViewer.Settings;

namespace Emignatik.NxFileViewer.Localization
{

    /// <summary>
    /// Service in charge of updating the current application localization according to the language selected in the settings (<see cref="IAppSettings.AppLanguage"/>)
    /// </summary>
    public interface ILocalizationFromSettingsSynchronizerService
    {
        /// <summary>
        /// Initializes the service
        /// </summary>
        void Initialize();

    }
}

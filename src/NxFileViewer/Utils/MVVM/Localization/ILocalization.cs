namespace Emignatik.NxFileViewer.Utils.MVVM.Localization
{

    public interface ILocalization<out T> where T: ILocalizationKeysBase
    {
        /// <summary>
        /// The localized language name (<see cref="ILocalizationKeysBase.DisplayName"/>)
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// The Culture name from <see cref="ILocalizationKeysBase.CultureName"/> or "Auto" when <see cref="IsAuto"/> is true
        /// </summary>
        string CultureName { get; }

        /// <summary>
        /// The localized keys
        /// </summary>
        T Keys { get; }

        /// <summary>
        /// True when this localization is dynamically corresponding to the system culture or corresponding to the fallback localization
        /// </summary>
        bool IsAuto { get; }
    }
}




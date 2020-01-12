using System.Collections.Generic;
using System.ComponentModel;

namespace Emignatik.NxFileViewer.Utils.MVVM.Localization
{
    public interface ILocalizationManager<TKeys> : INotifyPropertyChanged where TKeys : ILocalizationKeysBase
    {
        /// <summary>
        /// Occurs when <see cref="Current"/> changes
        /// </summary>
        event LocalizationChangedHandler<TKeys>? LocalizationChanged;

        /// <summary>
        /// List of available localizations
        /// </summary>
        IEnumerable<ILocalization<TKeys>> AvailableLocalizations { get; }

        /// <summary>
        /// The fallback localization
        /// </summary>
        ILocalization<TKeys> FallbackLocalization { get; }

        /// <summary>
        /// Get the localization matching the current system language or null if none available.
        /// </summary>
        ILocalization<TKeys>? SystemLocalization { get; }

        /// <summary>
        /// The current localization (can't be null)
        /// </summary>
        ILocalization<TKeys> Current { get; set; }

        /// <summary>
        /// Get or set a boolean indicating if automatic localization should be added to the list of <see cref="AvailableLocalizations"/>
        /// </summary>
        bool UseAutoLocalization { get; set; }

    }

    public delegate void LocalizationChangedHandler<T>(object sender, LocalizationChangedHandlerArgs<T> args) where T : ILocalizationKeysBase;

    public class LocalizationChangedHandlerArgs<T> where T : ILocalizationKeysBase
    {
        public LocalizationChangedHandlerArgs(ILocalization<T> newLocalization)
        {
            NewLocalization = newLocalization;
        }

        public ILocalization<T> NewLocalization { get; }
    }

}
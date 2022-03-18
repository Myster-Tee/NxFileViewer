using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Emignatik.NxFileViewer.Utils.MVVM.Localization;

public interface ILocalizationManager<TKeys> : INotifyPropertyChanged where TKeys : ILocalizationKeysBase
{
    /// <summary>
    /// Occurs when <see cref="Current"/> changes
    /// </summary>
    event EventHandler<LocalizationChangedHandlerArgs<TKeys>>? LocalizationChanged;

    /// <summary>
    /// List of available localizations
    /// </summary>
    ILocalizationCollection<TKeys> AvailableLocalizations { get; }

    /// <summary>
    /// The fallback localization
    /// </summary>
    ILocalization<TKeys> FallbackLocalization { get; }

    /// <summary>
    /// Get the localization matching the current system language or null if none available.
    /// </summary>
    ILocalization<TKeys>? SystemLocalization { get; }

    /// <summary>
    /// Get or set the current localization, can't be null.
    /// </summary>
    ILocalization<TKeys> Current { get; set; }

    /// <summary>
    /// Get the automatic localization <see cref="UseAutoLocalization"/>.
    /// </summary>
    IAutoLocalization<TKeys>? AutoLocalization { get; }

    /// <summary>
    /// Get the real declared available localizations
    /// </summary>
    IEnumerable<ILocalization<TKeys>> RealLocalizations { get; }

    /// <summary>
    /// Get or set a boolean indicating if an automatic localization should be added to the list of <see cref="AvailableLocalizations"/>.
    /// When set to true, a <see cref="IAutoLocalization{T}"/> is added to the list of <see cref="AvailableLocalizations"/>
    /// with the available <see cref="TKeys"/> best matching the current thread culture.
    /// </summary>
    bool UseAutoLocalization { get; set; }

    /// <summary>
    /// Adds a weak reference subscription to the <see cref="LocalizationChanged"/> event
    /// </summary>
    /// <param name="localizationChangedHandler"></param>
    void AddWeakLocalizationChangedHandler(EventHandler<LocalizationChangedHandlerArgs<TKeys>> localizationChangedHandler);

    /// <summary>
    /// Removes a weak reference subscription to the <see cref="LocalizationChanged"/> event
    /// </summary>
    /// <param name="localizationChangedHandler"></param>
    void RemoveWeakLocalizationChangedHandler(EventHandler<LocalizationChangedHandlerArgs<TKeys>> localizationChangedHandler);
}

public class LocalizationChangedHandlerArgs<TKeys> : EventArgs where TKeys : ILocalizationKeysBase
{
    public LocalizationChangedHandlerArgs(ILocalization<TKeys> newLocalization)
    {
        NewLocalization = newLocalization;
    }

    public ILocalization<TKeys> NewLocalization { get; }
}


/// <summary>
/// 
/// </summary>
/// <typeparam name="TKeys"></typeparam>
public interface ILocalizationCollection<out TKeys> : IEnumerable<ILocalization<TKeys>> where TKeys : ILocalizationKeysBase
{
    ILocalization<TKeys>? FindByCultureName(string cultureName);
}

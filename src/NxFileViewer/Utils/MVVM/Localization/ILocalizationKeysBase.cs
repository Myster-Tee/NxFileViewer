using System;

namespace Emignatik.NxFileViewer.Utils.MVVM.Localization;

/// <summary>
/// Represents a set of localized keys
/// </summary>
public interface ILocalizationKeysBase
{
    /// <summary>
    /// True if those keys should be used when no better keys are found
    /// </summary>
    bool IsFallback { get; }

    /// <summary>
    /// The localized language name
    /// </summary>
    string DisplayName { get; }

    /// <summary>
    /// The culture name corresponding to the language of those keys
    /// </summary>
    string CultureName { get; }

    /// <summary>
    /// Localization of «Automatic»
    /// </summary>
    string LanguageAuto { get; }

    /// <summary>
    /// Get the localized value from the given key.
    /// Returns true when a localized value is found, otherwise returns false.
    /// When no localized value is found, the output value is assigned with the key value.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="stringComparison"></param>
    /// <returns></returns>
    bool TryGetValue(string key, out string value, StringComparison stringComparison = StringComparison.Ordinal);

    /// <summary>
    /// Get the localized value from the specified key.
    /// Returns null if key doesn't exist.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    string? this[string key] { get; }

}
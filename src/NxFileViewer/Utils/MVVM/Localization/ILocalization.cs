namespace Emignatik.NxFileViewer.Utils.MVVM.Localization;

/// <summary>
/// Represents a localization associated with a model exposing localization keys (<see cref="TKeys"/>)
/// </summary>
/// <typeparam name="TKeys"></typeparam>
public interface ILocalization<out TKeys> where TKeys: ILocalizationKeysBase
{
    /// <summary>
    /// The localized language name (<see cref="ILocalizationKeysBase.DisplayName"/>)
    /// </summary>
    string DisplayName { get; }

    /// <summary>
    /// The Culture name from <see cref="ILocalizationKeysBase.CultureName"/> or "Auto"
    /// </summary>
    string CultureName { get; }

    /// <summary>
    /// The localized keys
    /// </summary>
    TKeys Keys { get; }

}
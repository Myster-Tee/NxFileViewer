using System;

namespace Emignatik.NxFileViewer.Utils.MVVM.Localization;

public class RealLocalization<T> : ILocalization<T> where T : ILocalizationKeysBase
{
    public RealLocalization(T keys)
    {
        Keys = keys ?? throw new ArgumentNullException(nameof(keys));
    }

    public T Keys { get; }

    public bool IsAuto => false;

    public string DisplayName => Keys.DisplayName;

    public string CultureName => Keys.CultureName;

    public override string ToString()
    {
        return $"{DisplayName} ({CultureName})";
    }
}
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Emignatik.NxFileViewer.Utils.MVVM.Localization;

public class AutoLocalization<TKeys> : IAutoLocalization<TKeys> where TKeys : ILocalizationKeysBase
{
    private string _displayName = string.Empty;

    public AutoLocalization(TKeys keys)
    {
        Keys = keys ?? throw new ArgumentNullException(nameof(keys));
        DisplayName = keys.LanguageAuto;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public string DisplayName
    {
        get => _displayName;
        set
        {
            _displayName = value;
            NotifyPropertyChanged();
        }
    }

    public string CultureName => IAutoLocalization<TKeys>.CULTURE_NAME;

    public TKeys Keys { get; }

    protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null!)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public override string ToString()
    {
        return $"{DisplayName} ({CultureName})";
    }

}
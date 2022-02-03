using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Emignatik.NxFileViewer.Utils.MVVM.Localization;

public class AutoLocalization<T> : ILocalization<T>, INotifyPropertyChanged where T : ILocalizationKeysBase
{
    private string _displayName = string.Empty;

    public AutoLocalization(T keys)
    {
        Keys = keys ?? throw new ArgumentNullException(nameof(keys));
        DisplayName = keys.LanguageAuto;
    }

    public string DisplayName
    {
        get => _displayName;
        set
        {
            _displayName = value;
            NotifyPropertyChanged();
        }
    }

    public string CultureName => "Auto";

    public T Keys { get; }

    public bool IsAuto => true;

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null!)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public override string ToString()
    {
        return $"{DisplayName} ({CultureName})";
    }

}
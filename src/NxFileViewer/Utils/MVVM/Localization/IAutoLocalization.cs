using System.ComponentModel;

namespace Emignatik.NxFileViewer.Utils.MVVM.Localization;

public interface IAutoLocalization<out TKeys> : ILocalization<TKeys>, INotifyPropertyChanged where TKeys : ILocalizationKeysBase
{
    /// <summary>
    /// The value which should be returned by the property <see cref="IAutoLocalization{TKeys}.CultureName"/>
    /// </summary>
    public const string CULTURE_NAME = "Auto";

}
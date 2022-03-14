using System.ComponentModel;

namespace Emignatik.NxFileViewer.Settings;

/// <summary>
/// Exposes the list of application settings
/// </summary>
public interface IAppSettings: ISerializedSettings, INotifyPropertyChanged
{
    int ProgressBufferSize { get; set; }
}
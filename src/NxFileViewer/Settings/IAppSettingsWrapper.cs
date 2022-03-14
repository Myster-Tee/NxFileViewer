namespace Emignatik.NxFileViewer.Settings;

/// <summary>
/// Wraps the serialized settings
/// </summary>
public interface IAppSettingsWrapper : IAppSettings
{
    ISerializedSettings SerializedModel { get; set; }
}
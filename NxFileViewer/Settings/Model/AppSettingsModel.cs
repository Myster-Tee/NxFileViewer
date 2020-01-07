namespace Emignatik.NxFileViewer.Settings.Model
{
    /// <summary>
    /// Model for the serialization
    /// </summary>
    public class AppSettingsModel
    {

        public string LastOpenedFile { get; set; }

        public string KeysFilePath { get; set; } = "Keys.dat";

    }
}

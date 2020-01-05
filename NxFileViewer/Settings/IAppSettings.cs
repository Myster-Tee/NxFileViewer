namespace Emignatik.NxFileViewer.Settings
{
    public interface IAppSettings
    {

        string LastOpenedFile { get; set; }

        string KeysFilePath { get; set; }

        void Save();

        void Load();
    }
}
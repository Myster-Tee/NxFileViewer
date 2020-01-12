namespace Emignatik.NxFileViewer.Settings
{
    public interface IAppSettingsManager
    {
        IAppSettings Settings { get; }

        public void Load();

        public void Save();
    }
}
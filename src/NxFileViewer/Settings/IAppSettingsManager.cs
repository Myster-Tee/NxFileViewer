namespace Emignatik.NxFileViewer.Settings
{
    public interface IAppSettingsManager
    {
        IAppSettings Settings { get; }

        public void LoadDefault();

        public bool SafeLoad();

        public void SafeSave();
    }
}
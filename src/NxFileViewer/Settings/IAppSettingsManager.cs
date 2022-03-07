namespace Emignatik.NxFileViewer.Settings
{
    public interface IAppSettingsManager
    {
        IAppSettings Settings { get; }

        public void SafeLoad();

        public void SafeSave();
    }
}
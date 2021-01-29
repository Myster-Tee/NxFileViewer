
namespace Emignatik.NxFileViewer.Settings
{
    public interface IAppSettingsWrapper<T> : IAppSettings
    {
        /// <summary>
        /// Get the real serialized model wrapping the <see cref="IAppSettings"/>
        /// </summary>
        public T WrappedModel { get; }

        /// <summary>
        /// Updates the settings with the specified wrapped model
        /// </summary>
        /// <param name="newModel"></param>
        public void Update(T newModel);
    }
}
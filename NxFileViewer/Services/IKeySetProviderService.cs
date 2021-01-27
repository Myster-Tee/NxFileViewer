using LibHac;

namespace Emignatik.NxFileViewer.Services
{
    /// <summary>
    /// Service in charge of providing the Key set to use for decrypting files to open
    /// </summary>
    public interface IKeySetProviderService
    {

        /// <summary>
        /// Returns true if a prod.keys file was found
        /// </summary>
        bool ProdKeysFileFound { get; }

        /// <summary>
        /// Get the location of the prod.keys expected in the current application's directory
        /// </summary>
        string AppDirProdKeysFilePath { get; }

        /// <summary>
        /// Returns the KeySet
        /// </summary>
        /// <param name="forceReload"></param>
        /// <returns></returns>
        Keyset GetKeySet(bool forceReload = false);

        /// <summary>
        /// Unloads the current KeySet
        /// </summary>
        void UnloadCurrentKeySet();
    }
}
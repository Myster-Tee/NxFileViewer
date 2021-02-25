using System.ComponentModel;
using LibHac;

namespace Emignatik.NxFileViewer.Services
{
    /// <summary>
    /// Service in charge of providing the Key set to use for decrypting files to open
    /// </summary>
    public interface IKeySetProviderService : INotifyPropertyChanged
    {
        public const string DefaultProdKeysFileName = "prod.keys";
        public const string DefaultConsoleKeysFileName = "console.keys";
        public const string DefaultTitleKeysFileName = "title.keys";

        /// <summary>
        /// Get the location of the <see cref="DefaultProdKeysFileName"/> expected in the current application's directory
        /// </summary>
        string AppDirProdKeysFilePath { get; }

        /// <summary>
        /// Get the location of the <see cref="DefaultTitleKeysFileName"/> expected in the current application's directory
        /// </summary>
        string AppDirTitleKeysFilePath { get; }

        /// <summary>
        /// Get the path of the actual «prod.keys» file or null if none is found
        /// </summary>
        public string? ActualProdKeysFilePath { get; }

        /// <summary>
        /// Get the path of the actual «title.keys» file or null if none is found
        /// </summary>
        public string? ActualTitleKeysFilePath { get; }

        /// <summary>
        /// Get the path of the actual «console.keys» file or null if none is found
        /// </summary>
        public string? ActualConsoleKeysFilePath { get; }

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
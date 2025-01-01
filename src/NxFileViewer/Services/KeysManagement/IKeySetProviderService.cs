using System.ComponentModel;
using LibHac.Common.Keys;

namespace Emignatik.NxFileViewer.Services.KeysManagement;

/// <summary>
/// Service in charge of providing the Key set to use for decrypting files to open
/// </summary>
public interface IKeySetProviderService : INotifyPropertyChanged
{
    public const string DEFAULT_PROD_KEYS_FILE_NAME = "prod.keys";
    public const string DEFAULT_TITLE_KEYS_FILE_NAME = "title.keys";

    /// <summary>
    /// Get the location of the <see cref="DEFAULT_PROD_KEYS_FILE_NAME"/> expected in the current application's directory
    /// </summary>
    string AppDirProdKeysFilePath { get; }

    /// <summary>
    /// Get the location of the <see cref="DEFAULT_TITLE_KEYS_FILE_NAME"/> expected in the current application's directory
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
    /// Returns the KeySet
    /// </summary>
    /// <param name="forceReload"></param>
    /// <returns></returns>
    KeySet GetKeySet(bool forceReload = false);

    /// <summary>
    /// Unloads the current KeySet and re-detects the keys file paths
    /// </summary>
    void Reset();

}
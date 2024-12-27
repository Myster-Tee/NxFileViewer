using System.Text.Json.Serialization;
using Emignatik.NxFileViewer.Utils.MVVM;
using Emignatik.NxFileViewer.Utils.MVVM.Localization;
using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer.Settings;

public class AppSettings : NotifyPropertyChangedBase, IAppSettings
{
    private string _appLanguage = IAutoLocalization<ILocalizationKeysBase>.CULTURE_NAME;
    private string _lastRenamePath = "";
    private string _lastOpenedFile = "";
    private string _prodKeysFilePath = "";
    private string _consoleKeysFilePath = "";
    private string _titleKeysFilePath = "";
    private LogLevel _logLevel = LogLevel.Information;
    private string _prodKeysDownloadUrl = "";
    private string _titleKeysDownloadUrl = "";
    private bool _alwaysReloadKeysBeforeOpen = false;

    private string _titlePageUrl = "https://tinfoil.media/Title/{TitleId}";
    private string _titleInfoApiUrl = "https://tinfoil.media/api/title/{TitleId}";
    private string _lastUsedDir = "";
    private bool _allowNczBlocklessCompressionOpening = true;
    private bool _acceptMissingDeltaFragments = true;

    public string AppLanguage
    {
        get => _appLanguage;
        set
        {
            _appLanguage = value;
            NotifyPropertyChanged();
        }
    }

    public string LastUsedDir
    {
        get => _lastUsedDir;
        set
        {
            _lastUsedDir = value;
            NotifyPropertyChanged();
        }
    }

    public string LastRenamePath
    {
        get => _lastRenamePath;
        set
        {
            _lastRenamePath = value;
            NotifyPropertyChanged();
        }
    }

    public string LastOpenedFile
    {
        get => _lastOpenedFile;
        set
        {
            _lastOpenedFile = value;
            NotifyPropertyChanged();
        }
    }

    public string ProdKeysFilePath
    {
        get => _prodKeysFilePath;
        set
        {
            _prodKeysFilePath = value;
            NotifyPropertyChanged();
        }
    }

    public string ConsoleKeysFilePath
    {
        get => _consoleKeysFilePath;
        set
        {
            _consoleKeysFilePath = value;
            NotifyPropertyChanged();
        }
    }

    public bool OpenBlocklessCompressionNCZ
    {
        get => _allowNczBlocklessCompressionOpening;
        set
        {
            _allowNczBlocklessCompressionOpening = value;
            NotifyPropertyChanged();
        }
    }

    public bool IgnoreMissingDeltaFragments
    {
        get => _acceptMissingDeltaFragments;
        set
        {
            _acceptMissingDeltaFragments = value;
            NotifyPropertyChanged();
        }
    }

    public string TitleKeysFilePath
    {
        get => _titleKeysFilePath;
        set
        {
            _titleKeysFilePath = value;
            NotifyPropertyChanged();
        }
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public LogLevel LogLevel
    {
        get => _logLevel;
        set
        {
            _logLevel = value;
            NotifyPropertyChanged();
        }
    }

    public string ProdKeysDownloadUrl
    {
        get => _prodKeysDownloadUrl;
        set
        {
            _prodKeysDownloadUrl = value;
            NotifyPropertyChanged();
        }
    }

    public string TitleKeysDownloadUrl
    {
        get => _titleKeysDownloadUrl;
        set
        {
            _titleKeysDownloadUrl = value;
            NotifyPropertyChanged();
        }
    }

    public bool AlwaysReloadKeysBeforeOpen
    {
        get => _alwaysReloadKeysBeforeOpen;
        set
        {
            _alwaysReloadKeysBeforeOpen = value;
            NotifyPropertyChanged();
        }
    }

    public string TitlePageUrl
    {
        get => _titlePageUrl;
        set
        {
            _titlePageUrl = value;
            NotifyPropertyChanged();
        }
    }

    public string TitleInfoApiUrl
    {
        get => _titleInfoApiUrl;
        set
        {
            _titleInfoApiUrl = value;
            NotifyPropertyChanged();
        }
    }

    [JsonIgnore]
    IRenamingOptions IAppSettings.RenamingOptions => RenamingOptions;

    public RenamingOptions RenamingOptions { get; set; } = new();

    [JsonIgnore]
    public int ProgressBufferSize { get; } = 4 * 1024 * 1024;

}

public class RenamingOptions : NotifyPropertyChangedBase, IRenamingOptions
{
    private string? _fileFilters = "*.nsp;*.nsz;*.xci;*.xcz";
    private bool _includeSubdirectories = true;
    private string _applicationPattern = "{WAppTitle} [{TitleId:U}][v{VerNum}]+[u{XCIUpdateVer}].{Ext:L}";
    private string _patchPattern = "{WAppTitle} [{TitleId:U}][v{PatchNum}][u{DisplayVer}].{Ext:L}";
    private string _addonPattern = "{WAppTitle} - {WTitle} [{TitleId:U}][v{PatchNum}].{Ext:L}";
    private bool _isSimulation = true;
    private string _invalidFileNameCharsReplacement = "꞉";
    private bool _replaceWhiteSpaceChars = false;
    private string _whiteSpaceCharsReplacement = "_";
    private string _lastRenamePath = "";

    public string LastRenamePath
    {
        get => _lastRenamePath;
        set
        {
            _lastRenamePath = value;
            NotifyPropertyChanged();
        }
    }

    public string? FileFilters
    {
        get => _fileFilters;
        set
        {
            _fileFilters = value;
            NotifyPropertyChanged();
        }
    }

    public bool IncludeSubdirectories
    {
        get => _includeSubdirectories;
        set
        {
            _includeSubdirectories = value;
            NotifyPropertyChanged();
        }
    }

    public string ApplicationPattern
    {
        get => _applicationPattern;
        set
        {
            _applicationPattern = value;
            NotifyPropertyChanged();
        }
    }

    public string PatchPattern
    {
        get => _patchPattern;
        set
        {
            _patchPattern = value;
            NotifyPropertyChanged();
        }
    }

    public string AddonPattern
    {
        get => _addonPattern;
        set
        {
            _addonPattern = value;
            NotifyPropertyChanged();
        }
    }

    public bool IsSimulation
    {
        get => _isSimulation;
        set
        {
            _isSimulation = value;
            NotifyPropertyChanged();
        }
    }

    public string InvalidFileNameCharsReplacement
    {
        get => _invalidFileNameCharsReplacement;
        set
        {
            _invalidFileNameCharsReplacement = value;
            NotifyPropertyChanged();
        }
    }

    public bool ReplaceWhiteSpaceChars
    {
        get => _replaceWhiteSpaceChars;
        set
        {
            _replaceWhiteSpaceChars = value;
            NotifyPropertyChanged();
        }
    }

    public string WhiteSpaceCharsReplacement
    {
        get => _whiteSpaceCharsReplacement;
        set
        {
            _whiteSpaceCharsReplacement = value;
            NotifyPropertyChanged();
        }
    }
}


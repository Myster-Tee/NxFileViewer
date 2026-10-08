using System.Text.Json.Serialization;
using Emignatik.NxFileViewer.Utils.MVVM;
using Emignatik.NxFileViewer.Utils.MVVM.Localization;
using Microsoft.Extensions.Logging;
using Emignatik.NxFileViewer.Styling.Theme;

namespace Emignatik.NxFileViewer.Settings;

public class AppSettings : NotifyPropertyChangedBase, IAppSettings
{
    private string _nandExecutablePath = "";
    private string _nandBisKeysPath = "";
    public string NandExecutablePath
    {
        get => _nandExecutablePath;
        set { _nandExecutablePath = value; NotifyPropertyChanged(); }
    }
    public string NandBisKeysPath
    {
        get => _nandBisKeysPath;
        set { _nandBisKeysPath = value; NotifyPropertyChanged(); }
    }
    private int _logFileRetentionCount = 5;
    public int LogFileRetentionCount
    {
        get => _logFileRetentionCount;
        set { _logFileRetentionCount = System.Math.Clamp(value, 1, 100); NotifyPropertyChanged(); }
    }
    private bool _enableBatchHistory = true;
    public bool EnableBatchHistory
    {
        get => _enableBatchHistory;
        set { _enableBatchHistory = value; NotifyPropertyChanged(); }
    }
    private bool _includeViewerPrereleases;
    public bool IncludeViewerPrereleases
    {
        get => _includeViewerPrereleases;
        set { _includeViewerPrereleases = value; NotifyPropertyChanged(); }
    }
    private bool _checkViewerUpdatesOnStartup = true;
    public bool CheckViewerUpdatesOnStartup
    {
        get => _checkViewerUpdatesOnStartup;
        set { _checkViewerUpdatesOnStartup = value; NotifyPropertyChanged(); }
    }
    private string _nszExecutablePath = "";
    private bool _nszCheckUpdates = true;
    private int _nszCompressionLevel = 18;
    public string NszExecutablePath
    {
        get => _nszExecutablePath;
        set { _nszExecutablePath = value; NotifyPropertyChanged(); }
    }
    public bool NszCheckUpdates
    {
        get => _nszCheckUpdates;
        set { _nszCheckUpdates = value; NotifyPropertyChanged(); }
    }
    public int NszCompressionLevel
    {
        get => _nszCompressionLevel;
        set { _nszCompressionLevel = System.Math.Clamp(value, 1, 22); NotifyPropertyChanged(); }
    }
    private NszCompressionMode _nszCompressionMode = NszCompressionMode.Auto;
    private int _nszBlockSizeExponent = 20;
    public NszCompressionMode NszCompressionMode
    {
        get => _nszCompressionMode;
        set { _nszCompressionMode = System.Enum.IsDefined(value) ? value : NszCompressionMode.Auto; NotifyPropertyChanged(); }
    }
    public int NszBlockSizeExponent
    {
        get => _nszBlockSizeExponent;
        set { _nszBlockSizeExponent = System.Math.Clamp(value, 14, 32); NotifyPropertyChanged(); }
    }
    private string _appLanguage = IAutoLocalization<ILocalizationKeysBase>.CULTURE_NAME;
    private string _lastRenamePath = "";
    private string _targetDirectory = "";
    public string TargetDirectory
    {
        get => _targetDirectory;
        set { _targetDirectory = value; NotifyPropertyChanged(); }
    }
    private string _lastOpenedFile = "";
    private string _prodKeysFilePath = "";
    private string _titleKeysFilePath = "";
    private LogLevel _logLevel = LogLevel.Information;
    private string _keysDownloadHost = "192.168.178.100";
    public string KeysDownloadHost
    {
        get => _keysDownloadHost;
        set { _keysDownloadHost = value; NotifyPropertyChanged(); }
    }
    private string _prodKeysDownloadUrl = "ftp://{IP}:5000/sdmc:/switch/prod.keys";
    private string _titleKeysDownloadUrl = "ftp://{IP}:5000/sdmc:/switch/title.keys";
    private bool _alwaysReloadKeysBeforeOpen = false;

    private string _titlePageUrl = "https://tinfoil.media/Title/{TitleId}";
    private string _titleInfoApiUrl = "https://tinfoil.media/api/title/{TitleId}";
    private TitleInfoProvider _titleInfoProvider;
    private string _titleDbRegion = "DE.de";
    private string _nLibApiUrl = "https://api.nlib.cc/nx/{TitleId}?lang={Language}";
    private string _lastUsedDir = "";
    private bool _allowNczBlocklessCompressionOpening = true;
    private bool _acceptMissingDeltaFragments = true;
    private bool _injectTicketKeys = true;
    private bool _saveTicketKeys;
    public bool SaveTicketKeys { get => _saveTicketKeys; set { _saveTicketKeys = value; NotifyPropertyChanged(); } }
    private AppTheme _theme = AppTheme.System;
    private bool _rememberWindowPlacement = true;

    public string AppLanguage
    {
        get => _appLanguage;
        set
        {
            _appLanguage = value;
            NotifyPropertyChanged();
        }
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public AppTheme Theme
    {
        get => _theme;
        set { _theme = value; NotifyPropertyChanged(); }
    }

    public bool RememberWindowPlacement
    {
        get => _rememberWindowPlacement;
        set { _rememberWindowPlacement = value; NotifyPropertyChanged(); }
    }

    [JsonIgnore]
    IWindowPlacementSettings IAppSettings.MainWindowPlacement => MainWindowPlacement;

    public WindowPlacementSettings MainWindowPlacement { get; set; } = new();

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

    public bool InjectTicketKeys
    {
        get => _injectTicketKeys;
        set
        {
            _injectTicketKeys = value;
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

    public TitleInfoProvider TitleInfoProvider
    {
        get => _titleInfoProvider;
        set { _titleInfoProvider = value; NotifyPropertyChanged(); }
    }

    public string TitleDbRegion
    {
        get => _titleDbRegion;
        set { _titleDbRegion = value; NotifyPropertyChanged(); }
    }

    public string NLibApiUrl
    {
        get => _nLibApiUrl;
        set { _nLibApiUrl = value; NotifyPropertyChanged(); }
    }

    public RenamingOptions RenamingOptions { get; set; } = new();

    [JsonIgnore]
    public int ProgressBufferSize { get; } = 4 * 1024 * 1024;

}

public class WindowPlacementSettings : NotifyPropertyChangedBase, IWindowPlacementSettings
{
    private int _left;
    private int _top;
    private int _width;
    private int _height;
    private bool _isMaximized;

    public int Left { get => _left; set { _left = value; NotifyPropertyChanged(); } }
    public int Top { get => _top; set { _top = value; NotifyPropertyChanged(); } }
    public int Width { get => _width; set { _width = value; NotifyPropertyChanged(); } }
    public int Height { get => _height; set { _height = value; NotifyPropertyChanged(); } }
    public bool IsMaximized { get => _isMaximized; set { _isMaximized = value; NotifyPropertyChanged(); } }

    [JsonIgnore]
    public bool IsDefined => _width > 0 && _height > 0;
}

public class RenamingOptions : NotifyPropertyChangedBase, IRenamingOptions
{
    private string? _fileFilters = "*.nsp;*.nsz;*.xci;*.xcz";
    private bool _includeSubdirectories = true;
    private string _applicationPattern = "{WAppTitle} [{TitleId:U}][v{PatchNum}].{Ext:L}";
    private string _patchPattern = "{WAppTitle} [{TitleId:U}][v{PatchNum}].{Ext:L}";
    private string _addonPattern = "{WAppTitle} - {WTitle} [{TitleId:U}][v{PatchNum}].{Ext:L}";
    private bool _isSimulation = true;
    private string _invalidFileNameCharsReplacement = "꞉";
    private bool _replaceWhiteSpaceChars = false;
    private string _whiteSpaceCharsReplacement = "_";
    private string _lastRenamePath = "";
    private string _targetDirectory = "";
    public string TargetDirectory
    {
        get => _targetDirectory;
        set { _targetDirectory = value; NotifyPropertyChanged(); }
    }
    private bool _autoCloseOpenedFile = true;

    public bool AutoCloseOpenedFile
    {
        get => _autoCloseOpenedFile;
        set
        {
            _autoCloseOpenedFile = value;
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

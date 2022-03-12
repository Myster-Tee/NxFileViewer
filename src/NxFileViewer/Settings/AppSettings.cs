using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer.Settings;

public class AppSettings : IAppSettings
{
    private string? _appLanguage;
    private string _lastSaveDir = "";
    private string _lastOpenedFile = "";
    private string _keysFilePath = "";
    private string _consoleKeysFilePath = "";
    private string _titleKeysFilePath = "";
    private LogLevel _logLevel;
    private string _prodKeysDownloadUrl = "";
    private string _titleKeysDownloadUrl = "";
    private bool _alwaysReloadKeysBeforeOpen = false;

    private string _titlePageUrl = "";
    private string _applicationPattern = "";
    private string _patchPattern = "";
    private string _addonPattern = "";
    private string _titleInfoApiUrl = "";
    private string? _renamingFileFilters;

    public event PropertyChangedEventHandler? PropertyChanged;


    public string? AppLanguage
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
        get => _lastSaveDir;
        set
        {
            _lastSaveDir = value;
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
        get => _keysFilePath;
        set
        {
            _keysFilePath = value;
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

    public string TitleKeysFilePath
    {
        get => _titleKeysFilePath;
        set
        {
            _titleKeysFilePath = value;
            NotifyPropertyChanged();
        }
    }

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

    public string TitleInfoApiUrl
    {
        get => _titleInfoApiUrl;
        set
        {
            _titleInfoApiUrl = value;
            NotifyPropertyChanged();
        }
    }

    public string? RenamingFileFilters
    {
        get => _renamingFileFilters;
        set
        {
            _renamingFileFilters = value;
            NotifyPropertyChanged();
        }
    }

    public int ProgressBufferSize { get; set; } = 4 * 1024 * 1024;

    protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null!)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}
using System.Reflection;
using Emignatik.NxFileViewer.Utils.MVVM;
using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer.Settings;

public class AppSettingsWrapper : NotifyPropertyChangedBase, IAppSettingsWrapper
{
    private ISerializedSettings _serializedModel = new SerializeSettings();

    public ISerializedSettings SerializedModel
    {
        get => _serializedModel;
        set
        {
            _serializedModel = value;
            NotifyAllSettingsChanged();
        }
    }

    private void NotifyAllSettingsChanged()
    {
        var propertyInfos = typeof(ISerializedSettings).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var propertyInfo in propertyInfos)
        {
            NotifyPropertyChanged(propertyInfo.Name);
        }
    }

    public string AppLanguage
    {
        get => _serializedModel.AppLanguage;
        set
        {
            _serializedModel.AppLanguage = value;
            NotifyPropertyChanged();
        }
    }

    public string LastUsedDir
    {
        get => _serializedModel.LastUsedDir;
        set
        {
            _serializedModel.LastUsedDir = value;
            NotifyPropertyChanged();
        }
    }

    public string LastOpenedFile
    {
        get => _serializedModel.LastOpenedFile;
        set
        {
            _serializedModel.LastOpenedFile = value;
            NotifyPropertyChanged();
        }
    }

    public string ProdKeysFilePath
    {
        get => _serializedModel.ProdKeysFilePath;
        set
        {
            _serializedModel.ProdKeysFilePath = value;
            NotifyPropertyChanged();
        }
    }

    public string ConsoleKeysFilePath
    {
        get => _serializedModel.ConsoleKeysFilePath;
        set
        {
            _serializedModel.ConsoleKeysFilePath = value;
            NotifyPropertyChanged();
        }
    }

    public string TitleKeysFilePath
    {
        get => _serializedModel.TitleKeysFilePath;
        set
        {
            _serializedModel.TitleKeysFilePath = value;
            NotifyPropertyChanged();
        }
    }

    public LogLevel LogLevel
    {
        get => _serializedModel.LogLevel;
        set
        {
            _serializedModel.LogLevel = value;
            NotifyPropertyChanged();
        }
    }

    public string ProdKeysDownloadUrl
    {
        get => _serializedModel.ProdKeysDownloadUrl;
        set
        {
            _serializedModel.ProdKeysDownloadUrl = value;
            NotifyPropertyChanged();
        }
    }

    public string TitleKeysDownloadUrl
    {
        get => _serializedModel.TitleKeysDownloadUrl;
        set
        {
            _serializedModel.TitleKeysDownloadUrl = value;
            NotifyPropertyChanged();
        }
    }

    public bool AlwaysReloadKeysBeforeOpen
    {
        get => _serializedModel.AlwaysReloadKeysBeforeOpen;
        set
        {
            _serializedModel.AlwaysReloadKeysBeforeOpen = value;
            NotifyPropertyChanged();
        }
    }

    public string TitlePageUrl
    {
        get => _serializedModel.TitlePageUrl;
        set
        {
            _serializedModel.TitlePageUrl = value;
            NotifyPropertyChanged();
        }
    }

    public string ApplicationPattern
    {
        get => _serializedModel.ApplicationPattern;
        set
        {
            _serializedModel.ApplicationPattern = value;
            NotifyPropertyChanged();
        }
    }

    public string PatchPattern
    {
        get => _serializedModel.PatchPattern;
        set
        {
            _serializedModel.PatchPattern = value;
            NotifyPropertyChanged();
        }
    }

    public string AddonPattern
    {
        get => _serializedModel.AddonPattern;
        set
        {
            _serializedModel.AddonPattern = value;
            NotifyPropertyChanged();
        }
    }

    public string TitleInfoApiUrl
    {
        get => _serializedModel.TitleInfoApiUrl;
        set
        {
            _serializedModel.TitleInfoApiUrl = value;
            NotifyPropertyChanged();
        }
    }

    public string? RenamingFileFilters
    {
        get => _serializedModel.RenamingFileFilters;
        set
        {
            _serializedModel.RenamingFileFilters = value;
            NotifyPropertyChanged();
        }
    }

    public bool RenameIncludeSubdirectories
    {
        get => _serializedModel.RenameIncludeSubdirectories;
        set
        {
            _serializedModel.RenameIncludeSubdirectories = value;
            NotifyPropertyChanged();
        }
    }

    public bool RenameSimulation
    {
        get => _serializedModel.RenameSimulation;
        set
        {
            _serializedModel.RenameSimulation = value;
            NotifyPropertyChanged();
        }
    }

    public string RenameInvalidWindowsCharsReplacement
    {
        get => _serializedModel.RenameInvalidWindowsCharsReplacement;
        set
        {
            _serializedModel.RenameInvalidWindowsCharsReplacement = value;
            NotifyPropertyChanged();
        }
    }

    public bool ReplaceWhiteSpaceChars
    {
        get => _serializedModel.ReplaceWhiteSpaceChars;
        set
        {
            _serializedModel.ReplaceWhiteSpaceChars = value;
            NotifyPropertyChanged();
        }
    }


    public int ProgressBufferSize { get; set; } = 4 * 1024 * 1024;

}
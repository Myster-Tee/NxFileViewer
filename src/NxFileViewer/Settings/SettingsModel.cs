using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer.Settings;

/// <summary>
/// Model for the serialization of the settings
/// </summary>
public class SettingsModel
{
    public string? AppLanguage { get; set; }

    public string? LastOpenedFile { get; set; }

    public string? LastUsedDir { get; set; }

    public string? ProdKeysFilePath { get; set; }

    public string? TitleKeysFilePath { get; set; }

    public string? ConsoleKeysFilePath { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public LogLevel? LogLevel { get; set; } = Microsoft.Extensions.Logging.LogLevel.Information;

    public string? ProdKeysDownloadUrl { get; set; }

    public string? TitleKeysDownloadUrl { get; set; }

    public bool? AlwaysReloadKeysBeforeOpen { get; set; }

    public string? TitlePageUrl { get; set; } = "https://tinfoil.io/Title/{0}";

    public string? ApplicationPattern { get; set; } = "{FirstTitleName} [{TitleIdU}] [v{VersionNum}].{PackageTypeL}";

    public string? PatchPattern { get; set; } = "{FirstTitleName} [{TitleIdU}] [v{VersionNum}].{PackageTypeL}";

}
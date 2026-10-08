using System.IO;
using Emignatik.NxFileViewer.Models.Overview;

namespace Emignatik.NxFileViewer.Services.Integrity;

public sealed record BatchIntegrityResult(
    string FilePath,
    string FileType,
    string PackageType,
    string Structure,
    string Compression,
    NcasIntegrity Integrity,
    string? Error)
{
    public bool IsNand { get; init; }
    public string? NandDetails { get; init; }
    public bool? NamingMatches { get; init; }
    public string? ProposedPath { get; init; }
    public string? NamingError { get; init; }
    public string? Diagnostic => Error ?? NamingError;
    public string NamingStatus => NamingError != null ? Localization.LocalizationManager.Instance.Current.Keys.RenamingTool_StatusError
        : NamingMatches == true ? Localization.LocalizationManager.Instance.Current.Keys.BatchNaming_Matches
        : NamingMatches == false ? Localization.LocalizationManager.Instance.Current.Keys.BatchNaming_Differs : "—";
    public string? SourceFingerprint { get; init; }
    public string Title { get; init; } = "";
    public string TitleId { get; init; } = "";
    public string Publisher { get; init; } = "";
    public string Version { get; init; } = "";
    public long? VersionNumber => long.TryParse(Version, out var value) ? value : null;
    public string DisplayVersion { get; init; } = "";
    public string SystemVersion { get; init; } = "";
    public string MasterKey { get; init; } = "";
    public string BuildId { get; init; } = "";
    public string Distribution { get; init; } = "";
    public string Languages { get; init; } = "";
    public long? FileSize { get; init; }
    public double? CompressionRatio { get; init; }
    public string? ConversionStatus { get; init; }
    public bool ConversionFailed { get; init; }
    public string? ConvertedPath { get; init; }
    public long? SourceSize { get; init; }
    public long? OutputSize { get; init; }
    public bool HasMissingKeys { get; init; }
    public bool IsFirmware { get; init; }
    public string? FirmwareDetails { get; init; }
    public string FileName => Path.GetFileName(FilePath.TrimEnd('/'));
}

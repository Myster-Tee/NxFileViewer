using System.Collections.Generic;
using Emignatik.NxFileViewer.Localization.Keys;

namespace Emignatik.NxFileViewer.Services.Integrity;

/// <summary>Displays persisted metadata without reopening a source file or archive.</summary>
public static class BatchStoredOverview
{
    public static IReadOnlyList<KeyValuePair<string, string>> Build(BatchIntegrityResult result, ILocalizationKeys keys) =>
        new KeyValuePair<string, string>[]
        {
            new(keys.Title_FileInfo_FileType, result.FileType),
            new(keys.PackageStructure_Title, result.Structure),
            new(keys.Title_FileInfo_Compression, result.Compression),
            new(keys.Title_FileInfo_Integrity, result.Integrity.ToString()),
            new(keys.FileInfo_FileSize, result.FileSize.HasValue ? result.FileSize.Value.ToString("N0") + " B" : "—"),
            new(keys.FileInfo_CompressionRatio, result.CompressionRatio?.ToString("P1") ?? "—"),
            new(keys.AppTitle, result.Title),
            new(keys.CnmtOverview_TitleId, result.TitleId),
            new(keys.Publisher, result.Publisher),
            new(keys.CnmtOverview_TitleVersion, result.Version),
            new(keys.DisplayVersion, result.DisplayVersion),
            new(keys.CnmtOverview_MinimumSystemVersion, result.SystemVersion),
            new(keys.CnmtOverview_MasterKey, result.MasterKey),
            new(keys.CnmtOverview_BuildID, result.BuildId),
            new(keys.CnmtOverview_Distribution, result.Distribution),
            new(keys.AvailableLanguages, result.Languages),
            new(keys.BatchIntegrity_Error, result.Diagnostic ?? "")
        };
}

using System;
using System.Linq;
using Emignatik.NxFileViewer.Models.Overview;

namespace Emignatik.NxFileViewer.Services.Integrity;

public static class BatchResultFilter
{
    public static bool Matches(BatchIntegrityResult result, string search, string fileType, string integrity, bool onlyErrors, string naming = "")
    {
        if (naming.Length > 0 && naming != NamingState(result)) return false;
        if (onlyErrors && result.Integrity == NcasIntegrity.Original && !result.ConversionFailed && result.NamingError == null) return false;
        if (fileType.Length > 0 && !result.FileType.Equals(fileType, StringComparison.OrdinalIgnoreCase) &&
            !result.FileType.StartsWith(fileType + " (", StringComparison.OrdinalIgnoreCase) &&
            !result.FileType.EndsWith(" (" + fileType + ")", StringComparison.OrdinalIgnoreCase)) return false;
        if (integrity.Length > 0 && result.Integrity.ToString() != integrity) return false;
        if (string.IsNullOrWhiteSpace(search)) return true;
        var text = string.Join(" ", result.FilePath, result.Title, result.TitleId, result.Publisher,
            result.Version, result.DisplayVersion, result.SystemVersion, result.MasterKey, result.BuildId,
            result.Distribution, result.Languages, result.FileType, result.PackageType, result.Structure,
            result.Compression, result.Integrity, result.Error, result.ConversionStatus, result.FirmwareDetails,
            result.NamingStatus, result.NamingError, result.ProposedPath, result.NandDetails);
        return search.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries)
            .All(word => text.Contains(word, StringComparison.OrdinalIgnoreCase));
    }

    public static string NamingState(BatchIntegrityResult result) => result.NamingError != null ? "Error"
        : result.NamingMatches == true ? "Matches" : result.NamingMatches == false ? "Differs" : "Unchecked";
}

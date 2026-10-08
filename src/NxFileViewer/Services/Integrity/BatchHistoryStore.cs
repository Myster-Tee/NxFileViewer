using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Emignatik.NxFileViewer.FileLoading;
using Emignatik.NxFileViewer.Models.Overview;
namespace Emignatik.NxFileViewer.Services.Integrity;

public sealed record BatchHistoryEntry(Guid Id, DateTime UpdatedUtc, string Source, bool IncludeSubdirectories,
    bool IncludeArchives, bool VerifyIntegrity, bool IgnoreMissingDeltaFragments, string State,
    BatchIntegrityResult[] Results, Dictionary<string, string> Fingerprints)
{
    public string Label => $"{UpdatedUtc.ToLocalTime():g} — {Source} ({Results.Length}) — {State}";
    public bool CanSkip(string path, bool ignoreMissingDeltaFragments)
    {
        if (IgnoreMissingDeltaFragments != ignoreMissingDeltaFragments) return false;
        var result = Results.FirstOrDefault(r => r.FilePath.Equals(path, StringComparison.OrdinalIgnoreCase));
        if (result == null || result.HasMissingKeys || result.Integrity is NcasIntegrity.Error or NcasIntegrity.InProgress ||
            (VerifyIntegrity && result.Integrity == NcasIntegrity.Unchecked)) return false;
        return Fingerprints.TryGetValue(path, out var saved) && saved == BatchHistoryStore.Fingerprint(path);
    }
}
public sealed class BatchHistoryStore(string path)
{
    public static string DefaultPath => Path.Combine(AppContext.BaseDirectory, "History", "batch-integrity.json");
    public IReadOnlyList<BatchHistoryEntry> Load()
    {
        if (!File.Exists(path)) return Array.Empty<BatchHistoryEntry>();
        return (JsonSerializer.Deserialize<BatchHistoryEntry[]>(File.ReadAllText(path)) ?? Array.Empty<BatchHistoryEntry>())
            .Where(e => e != null && e.Results != null && e.Fingerprints != null && !string.IsNullOrWhiteSpace(e.Source))
            .Select(e => e with { Fingerprints = new Dictionary<string, string>(e.Fingerprints, StringComparer.OrdinalIgnoreCase) })
            .OrderByDescending(e => e.UpdatedUtc).Take(5).ToArray();
    }
    public void Save(BatchHistoryEntry entry)
    {
        var entries = Load().Where(e => e.Id != entry.Id).Prepend(entry).Where(e => e != null && e.Results != null && e.Fingerprints != null && !string.IsNullOrWhiteSpace(e.Source))
            .Select(e => e with { Fingerprints = new Dictionary<string, string>(e.Fingerprints, StringComparer.OrdinalIgnoreCase) })
            .OrderByDescending(e => e.UpdatedUtc).Take(5).ToArray();
        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(path))!);
        var temp = path + "." + Guid.NewGuid().ToString("N") + ".tmp";
        try { File.WriteAllText(temp, JsonSerializer.Serialize(entries)); File.Move(temp, path, true); }
        finally { if (File.Exists(temp)) File.Delete(temp); }
    }
    public static string? Fingerprint(string path)
    {
        try
        {
            path = PackageZip.ArchivePath(path);
            if (File.Exists(path))
            {
                var parts = Services.Nand.NandDetection.SplitFiles(path);
                if (parts.Count > 1) return string.Join("|", parts.Select(part =>
                { var item = new FileInfo(part); return $"{item.Name}:{item.Length}:{item.LastWriteTimeUtc.Ticks}"; }));
                var file = new FileInfo(path); return $"{file.Length}:{file.LastWriteTimeUtc.Ticks}";
            }
            if (Directory.Exists(path)) return string.Join("|", Directory.GetFiles(path, "*", SearchOption.AllDirectories)
                .OrderBy(p => p, StringComparer.OrdinalIgnoreCase).Select(p =>
                { var file = new FileInfo(p); return $"{Path.GetRelativePath(path,p)}:{file.Length}:{file.LastWriteTimeUtc.Ticks}"; }));
        }
        catch (IOException) { }
        catch (UnauthorizedAccessException) { }
        return null;
    }
}

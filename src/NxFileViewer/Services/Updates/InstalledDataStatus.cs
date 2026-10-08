using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using Emignatik.NxFileViewer.Services.Integrity;

namespace Emignatik.NxFileViewer.Services.Updates;

public static class InstalledDataStatus
{
    public static string? HighestFirmwareVersion(IEnumerable<string> manifests)
    {
        Version? highest = null;
        foreach (var manifest in manifests)
        {
            try
            {
                _ = new FirmwareIntegrityVerifier(new[] { manifest });
                using var json = JsonDocument.Parse(manifest);
                var root = json.RootElement;
                var label = root.TryGetProperty("tag", out var tag) ? tag.GetString() : root.GetProperty("name").GetString();
                var match = Regex.Match(label ?? "", @"\d+\.\d+\.\d+");
                if (match.Success && Version.TryParse(match.Value, out var version) && (highest == null || version > highest)) highest = version;
            }
            catch (Exception ex) when (ex is JsonException or InvalidDataException or InvalidOperationException or KeyNotFoundException or ArgumentException) { }
        }
        return highest?.ToString();
    }

    public static string? LocalFirmwareVersion(string applicationDirectory)
    {
        var directory = Path.Combine(applicationDirectory, "fw", "hashes");
        try
        {
            if (!Directory.Exists(directory)) return null;
            var manifests = new List<string>();
            foreach (var path in Directory.EnumerateFiles(directory, "*.json"))
            {
                try { manifests.Add(File.ReadAllText(path)); }
                catch (IOException) { }
                catch (UnauthorizedAccessException) { }
            }
            return HighestFirmwareVersion(manifests);
        }
        catch (IOException) { return null; }
        catch (UnauthorizedAccessException) { return null; }
    }

    public static DateTime? TitleDbDate(string applicationDirectory, string region)
    {
        if (!Regex.IsMatch(region, @"^[A-Z]{2}\.[a-z]{2}\z")) return null;
        var path = Path.Combine(applicationDirectory, "Cache", "TitleDB", region + ".json");
        try { return File.Exists(path) ? File.GetLastWriteTimeUtc(path) : null; }
        catch (IOException) { return null; }
        catch (UnauthorizedAccessException) { return null; }
    }
}

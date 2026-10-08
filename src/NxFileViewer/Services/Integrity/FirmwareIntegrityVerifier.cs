using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Models.Overview;
using Emignatik.NxFileViewer.FileLoading;

namespace Emignatik.NxFileViewer.Services.Integrity;

/// <summary>Checks firmware against the bundled fw/hashes manifests without keys or extraction.</summary>
public sealed class FirmwareIntegrityVerifier
{
    private readonly List<Reference> _references = new();
    private readonly HashSet<string> _knownNames = new(StringComparer.OrdinalIgnoreCase);
    public FirmwareIntegrityVerifier(string? referenceDirectory = null)
    {
        referenceDirectory ??= Path.Combine(AppContext.BaseDirectory, "fw", "hashes");
        Load(Directory.EnumerateFiles(referenceDirectory, "*.json").Select(File.ReadAllText));
    }

    public FirmwareIntegrityVerifier(IEnumerable<string> manifests) => Load(manifests);

    private void Load(IEnumerable<string> manifests)
    {
        foreach (var manifest in manifests)
        {
            using var json = JsonDocument.Parse(manifest);
            var root = json.RootElement;
            var files = new Dictionary<string, Fingerprint>(StringComparer.OrdinalIgnoreCase);
            foreach (var entry in root.GetProperty("files").EnumerateObject())
            {
                var hash = entry.Value.GetProperty("sha256").GetString()!;
                var size = entry.Value.GetProperty("size").GetInt64();
                if (hash.Length != 64 || !hash.All(Uri.IsHexDigit) || size < 0 || Path.GetFileName(entry.Name) != entry.Name)
                    throw new InvalidDataException("Invalid firmware reference.");
                files.Add(entry.Name, new Fingerprint(size, hash));
                _knownNames.Add(entry.Name);
            }
            if (files.Count == 0 || files.Count != root.GetProperty("file_count").GetInt32())
                throw new InvalidDataException("Invalid firmware file count.");
            _references.Add(new Reference(root.GetProperty("name").GetString()!, files));
        }
        if (_references.Count == 0) throw new InvalidDataException("No firmware hash references available.");
    }

    public bool IsFirmwareFolder(string path) => Directory.EnumerateFiles(path).Any(p => _knownNames.Contains(Path.GetFileName(p)));
    public bool IsFirmwareZip(string path)
    {
        try
        {
            return PackageZip.GetFileEntries(path).Any(e => _knownNames.Contains(Path.GetFileName(e.Replace('\\', '/'))));
        }
        catch (InvalidDataException) { return Path.GetFileName(path).Contains("firmware", StringComparison.OrdinalIgnoreCase); }
    }

    public BatchIntegrityResult Verify(string path, CancellationToken cancellationToken = default, Action<double>? progress = null)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var actual = new Dictionary<string, Fingerprint>(StringComparer.OrdinalIgnoreCase);
        var duplicates = new List<string>();
        void Read(string name, Stream stream, long size)
        {
            using var hash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
            var buffer = new byte[128 * 1024];
            long length = 0;
            int read;
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                read = stream.Read(buffer, 0, buffer.Length);
                if (read == 0) break;
                hash.AppendData(buffer, 0, read);
                length += read;
            }
            if (length != size) throw new InvalidDataException($"Truncated firmware file: {name}");
            if (!actual.TryAdd(name, new Fingerprint(length, Convert.ToHexString(hash.GetHashAndReset())))) duplicates.Add(name);
        }
        if (Directory.Exists(path))
        {
            var files = Directory.GetFiles(path).Where(p => p.EndsWith(".nca", StringComparison.OrdinalIgnoreCase)).ToArray();
            for (var i = 0; i < files.Length; i++)
            {
                using var stream = File.OpenRead(files[i]);
                Read(Path.GetFileName(files[i]), stream, stream.Length);
                progress?.Invoke((double)(i + 1) / files.Length);
            }
        }
        else
        {
            var count = PackageZip.GetFileEntries(path).Count(e => e.EndsWith(".nca", StringComparison.OrdinalIgnoreCase));
            var index = 0;
            PackageZip.ReadEntries(path, name => name.EndsWith(".nca", StringComparison.OrdinalIgnoreCase), (name, stream, size) =>
            {
                Read(Path.GetFileName(name.Replace('\\', '/')), stream, size);
                progress?.Invoke((double)++index / count);
            }, cancellationToken);
        }
        // Hash once, then compare all versions: shared files alone never prove completeness.
        var comparisons = _references.Select(r => Compare(r, actual))
            .OrderByDescending(c => c.Matching).ThenByDescending(c => c.Overlap)
            .ThenBy(c => c.Missing.Length + c.Extra.Length).ToArray();
        var exact = comparisons.Where(c => c.Matching == c.Reference.Files.Count && c.Missing.Length == 0 && c.Extra.Length == 0 && c.Renamed.Count == 0).ToArray();
        var best = exact.FirstOrDefault() ?? comparisons[0];
        var keys = LocalizationManager.Instance.Current.Keys;
        if (best.Overlap == 0 && best.Matching == 0)
            return Result(path, "?", NcasIntegrity.Error, keys.Firmware_Unknown, keys.Firmware_Unknown);
        var expected = best.Reference.Files;
        var missing = best.Missing;
        var extra = best.Extra;
        var changed = best.Changed;
        var valid = exact.Length > 0 && duplicates.Count == 0;
        var version = valid ? string.Join(" / ", exact.Select(c => c.Reference.Name)) : best.Reference.Name;
        var summary = string.Format(keys.Firmware_Summary, best.Matching, expected.Count, missing.Length, changed.Length, extra.Length, duplicates.Count)
            + " " + string.Format(keys.Firmware_RenamedSummary, best.Renamed.Count);
        var details = summary + Environment.NewLine + string.Join(Environment.NewLine,
            missing.Select(n => keys.Firmware_Missing + ": " + n)
                .Concat(changed.Select(n => keys.Firmware_Changed + ": " + n))
                .Concat(extra.Select(n => keys.Firmware_Extra + ": " + n))
                .Concat(best.Renamed.Select(n => keys.Firmware_Renamed + ": " + n.ActualName + " → " + n.ExpectedName))
                .Concat(duplicates.Select(n => keys.Firmware_Duplicate + ": " + n)));
        progress?.Invoke(1);
        return Result(path, version, valid ? NcasIntegrity.Original : NcasIntegrity.Error, valid ? null : summary, details);
    }
    private static bool Equal(Fingerprint a, Fingerprint b) => a.Size == b.Size && string.Equals(a.Hash, b.Hash, StringComparison.OrdinalIgnoreCase);
    private static Comparison Compare(Reference reference, Dictionary<string, Fingerprint> actual)
    {
        var missing = reference.Files.Keys.Except(actual.Keys, StringComparer.OrdinalIgnoreCase).Order(StringComparer.OrdinalIgnoreCase).ToList();
        var extra = actual.Keys.Except(reference.Files.Keys, StringComparer.OrdinalIgnoreCase).Order(StringComparer.OrdinalIgnoreCase).ToList();
        var remaining = extra.GroupBy(name => new Fingerprint(actual[name].Size, actual[name].Hash.ToUpperInvariant()))
            .ToDictionary(group => group.Key, group => new Queue<string>(group));
        var renamed = new List<RenamedFile>();
        foreach (var expectedName in missing.ToArray())
        {
            var fingerprint = reference.Files[expectedName];
            if (!remaining.TryGetValue(new Fingerprint(fingerprint.Size, fingerprint.Hash.ToUpperInvariant()), out var candidates) || candidates.Count == 0) continue;
            // Consume each actual file once; duplicate content must not conceal another missing NCA.
            var actualName = candidates.Dequeue();
            renamed.Add(new(actualName, expectedName));
        }
        var renamedExpected = renamed.Select(r => r.ExpectedName).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var renamedActual = renamed.Select(r => r.ActualName).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var matching = reference.Files.Count(e => actual.TryGetValue(e.Key, out var a) && Equal(a, e.Value)) + renamed.Count;
        var changed = reference.Files.Where(e => actual.TryGetValue(e.Key, out var a) && !Equal(a, e.Value)).Select(e => e.Key).ToArray();
        return new(reference, matching, reference.Files.Keys.Count(actual.ContainsKey),
            missing.Where(n => !renamedExpected.Contains(n)).ToArray(), extra.Where(n => !renamedActual.Contains(n)).ToArray(), changed, renamed);
    }
    private static BatchIntegrityResult Result(string path, string version, NcasIntegrity integrity, string? error, string details) =>
        new(path, Directory.Exists(path) ? "Folder" : Path.GetExtension(path).TrimStart('.').ToUpperInvariant(), "Firmware", version, "SHA-256", integrity, error) { IsFirmware = true, FirmwareDetails = details };

    public BatchIntegrityResult IdentifyNca(string path, CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();
        using var stream = File.OpenRead(path);
        if (!_references.Any(r => r.Files.Values.Any(f => f.Size == stream.Length)))
            return Result(path, "?", NcasIntegrity.Unchecked, null, LocalizationManager.Instance.Current.Keys.Firmware_Unknown) with { IsFirmware = false };
        using var hash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
        var buffer = new byte[128 * 1024];
        int count;
        while ((count = stream.Read(buffer)) > 0)
        {
            token.ThrowIfCancellationRequested();
            hash.AppendData(buffer, 0, count);
        }
        var fingerprint = new Fingerprint(stream.Length, Convert.ToHexString(hash.GetHashAndReset()));
        // Content determines membership, even if the NCA was renamed. Shared NCAs list every matching version.
        var versions = _references.Where(r => r.Files.Values.Any(f => Equal(f, fingerprint))).Select(r => r.Name).Distinct().ToArray();
        var keys = LocalizationManager.Instance.Current.Keys;
        return Result(path, versions.Length == 0 ? "?" : string.Join(" / ", versions),
            versions.Length == 0 ? NcasIntegrity.Unchecked : NcasIntegrity.Original, null,
            versions.Length == 0 ? keys.Firmware_Unknown : keys.Firmware_NcaMatches + Environment.NewLine + string.Join(Environment.NewLine, versions))
            with { IsFirmware = versions.Length > 0 };
    }
    private sealed record Fingerprint(long Size, string Hash);
    private sealed record Reference(string Name, Dictionary<string, Fingerprint> Files);
    private sealed record RenamedFile(string ActualName, string ExpectedName);
    private sealed record Comparison(Reference Reference, int Matching, int Overlap, string[] Missing, string[] Extra, string[] Changed, IReadOnlyList<RenamedFile> Renamed);
}

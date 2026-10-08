using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using SharpCompress.Archives.SevenZip;

namespace Emignatik.NxFileViewer.FileLoading;

public static class PackageZip
{
    public const string Separator = "::";
    public static bool IsMember(string path) => path.Contains(Separator, StringComparison.Ordinal);
    public static string ArchivePath(string path) => path.Split(Separator, 2)[0];
    public static string MemberPath(string archive, string entry) => archive + Separator + entry;
    public static string DisplayFileType(string path)
    {
        var type = path.EndsWith('/') ? "Folder" : Path.GetExtension(path).TrimStart('.').ToUpperInvariant();
        if (!IsMember(path)) return type;
        var archiveType = Path.GetExtension(ArchivePath(path)).TrimStart('.').ToUpperInvariant();
        return type + " (" + (archiveType == "7Z" ? "7z" : archiveType) + ")";
    }
    public static bool IsArchive(string path) => Path.GetExtension(path).ToLowerInvariant() is ".zip" or ".7z";
    public static IReadOnlyList<string> GetFileEntries(string path)
    {
        if (Path.GetExtension(path).Equals(".7z", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                using var archive = SevenZipArchive.OpenArchive(path);
                return archive.Entries.Where(e => !e.IsDirectory).Select(e => e.Key!).ToArray();
            }
            catch (SharpCompress.Common.SharpCompressException ex) { throw new InvalidDataException("Invalid or unsupported 7z archive.", ex); }
        }
        using var zip = ZipFile.OpenRead(path);
        return zip.Entries.Where(e => !e.FullName.Replace('\\', '/').EndsWith('/')).Select(e => e.FullName).ToArray();
    }
    // Read solid 7z entries sequentially, reusing the decoder across the whole archive.
    public static void ReadEntries(string path, Func<string, bool> include, Action<string, Stream, long> read,
        CancellationToken token = default, bool stopAfterMatch = false)
    {
        if (Path.GetExtension(path).Equals(".7z", StringComparison.OrdinalIgnoreCase))
        {
            using var archive = SevenZipArchive.OpenArchive(path);
            using var reader = archive.ExtractAllEntries();
            while (reader.MoveToNextEntry())
            {
                token.ThrowIfCancellationRequested();
                if (reader.Entry.IsDirectory) continue;
                using var input = reader.OpenEntryStream();
                if (include(reader.Entry.Key!))
                {
                    read(reader.Entry.Key!, input, reader.Entry.Size);
                    if (stopAfterMatch) return;
                }
                else
                {
                    var buffer = new byte[128 * 1024];
                    while (input.Read(buffer, 0, buffer.Length) > 0) token.ThrowIfCancellationRequested();
                }
            }
            return;
        }
        using var zip = ZipFile.OpenRead(path);
        foreach (var entry in zip.Entries.Where(e => !e.FullName.Replace('\\', '/').EndsWith('/') && include(e.FullName)))
        {
            token.ThrowIfCancellationRequested();
            using var input = entry.Open();
            read(entry.FullName, input, entry.Length);
        }
    }
    private const int MaximumArchiveDepth = 8;
    public static IReadOnlyList<string> GetEntries(string path, bool includeNca = true, CancellationToken token = default, bool includeFirmware = false) =>
        GetEntriesRecursive(path, includeNca, token, 0, includeFirmware);

    private static IReadOnlyList<string> GetEntriesRecursive(string path, bool includeNca, CancellationToken token, int depth, bool includeFirmware)
    {
        token.ThrowIfCancellationRequested();
        if (depth >= MaximumArchiveDepth) throw new InvalidDataException("Archive nesting exceeds the supported depth of 8.");
        var names = GetFileEntries(path);
        var nandNames = new HashSet<string>(StringComparer.Ordinal);
        var nandCandidates = names.Where(Services.Nand.NandDetection.IsCandidateName).ToHashSet(StringComparer.Ordinal);
        foreach (var candidate in nandCandidates) ValidateName(candidate);
        var reusedNandSession = false;
        if (nandCandidates.Count > 0)
        {
            var info = new FileInfo(path);
            var sessionPrefix = info.FullName + "::" + info.Length + "::" + info.LastWriteTimeUtc.Ticks + "::";
            lock (SessionLock)
            {
                var existing = Sessions.FirstOrDefault(pair => pair.Key.StartsWith(sessionPrefix, StringComparison.OrdinalIgnoreCase)).Value;
                if (existing != null)
                {
                    foreach (var candidate in nandCandidates)
                        if (existing.Files.TryGetValue(candidate, out var file) && Services.Nand.NandDetection.Detect(file, token) != null) nandNames.Add(candidate);
                    reusedNandSession = true;
                }
            }
        }
        if (nandCandidates.Count > 0 && !reusedNandSession)
            ReadEntries(path, nandCandidates.Contains, (name, input, size) =>
            {
                token.ThrowIfCancellationRequested();
                // Header probing is bounded; full NAND GPT headers sit after the two BOOT partitions.
                using var prefix = new MemoryStream();
                var buffer = new byte[128 * 1024];
                var remaining = Math.Min(size, 0x1804400L);
                while (remaining > 0)
                {
                    token.ThrowIfCancellationRequested();
                    var count = input.Read(buffer, 0, (int)Math.Min(remaining, buffer.Length));
                    if (count == 0) break;
                    prefix.Write(buffer, 0, count);
                    remaining -= count;
                }
                if (Services.Nand.NandDetection.Detect(prefix, name, token) != null) nandNames.Add(name);
            }, token);
        var firmwareFolders = includeFirmware ? names.Where(name => name.EndsWith(".nca", StringComparison.OrdinalIgnoreCase))
            .Select(name => name.Replace('\\', '/')).Where(name => name.Contains('/'))
            .Where(name => Path.GetFileNameWithoutExtension(name).Length >= 28 && Path.GetFileNameWithoutExtension(name).All(Uri.IsHexDigit)
                || name.Contains("firmware", StringComparison.OrdinalIgnoreCase))
            .Select(name => name[..(name.LastIndexOf('/') + 1)]).Distinct(StringComparer.Ordinal).ToArray() : Array.Empty<string>();
        var entries = names.Where(e => (IsSupported(e, includeNca) && (!Services.Nand.NandDetection.IsCandidateName(e) || nandNames.Contains(e)) || IsArchive(e)) &&
            !Services.Nand.NandDetection.IsContinuationName(e, first => names.Any(name => name.Replace('\\', '/').Equals(first.Replace('\\', '/'), StringComparison.OrdinalIgnoreCase)))).ToArray();
        foreach (var entry in entries) ValidateName(entry);
        if (entries.Distinct(StringComparer.Ordinal).Count() != entries.Length)
            throw new InvalidDataException("Duplicate package names in archive.");
        var packages = new List<string>(firmwareFolders);
        foreach (var entry in entries)
        {
            token.ThrowIfCancellationRequested();
            if (!IsArchive(entry))
            {
                if (!entry.EndsWith(".nca", StringComparison.OrdinalIgnoreCase) || !firmwareFolders.Any(folder => entry.Replace('\\', '/').StartsWith(folder, StringComparison.Ordinal)
                    && !entry.Replace('\\', '/')[folder.Length..].Contains('/'))) packages.Add(entry);
                continue;
            }
            using var nested = Extract(path, entry, token);
            var nestedNames = GetFileEntries(nested.FilePath);
            if (includeFirmware && nestedNames.Any(name => name.EndsWith(".nca", StringComparison.OrdinalIgnoreCase))
                && !nestedNames.Any(name => IsSupported(name, false) || IsArchive(name)))
            {
                packages.Add(entry);
                continue;
            }
            foreach (var member in GetEntriesRecursive(nested.FilePath, includeNca, token, depth + 1, includeFirmware))
                packages.Add(entry + Separator + member);
        }
        return packages;
    }
    private static bool IsSupported(string name, bool includeNca) => Path.GetExtension(name).ToLowerInvariant() is
        ".nsp" or ".nsz" or ".xci" or ".xcz" || (includeNca && name.EndsWith(".nca", StringComparison.OrdinalIgnoreCase)) ||
        Services.Nand.NandDetection.IsCandidateName(name);
    private static void ValidateName(string name)
    {
        var normalized = name.Replace('\\', '/');
        if (normalized.StartsWith('/') || normalized.Split('/').Any(p => p is ".." or "." || p.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0))
            throw new InvalidDataException("Invalid package name in ZIP.");
    }
    private sealed class ArchiveSession
    {
        public required string Directory { get; init; }
        public Dictionary<string, string> Files { get; } = new(StringComparer.Ordinal);
        public int References;
    }
    private static readonly Dictionary<string, ArchiveSession> Sessions = new(StringComparer.OrdinalIgnoreCase);
    private static readonly object SessionLock = new();
    public static ExtractedPackage Extract(string archive, string entry, CancellationToken token = default, string? tempRoot = null)
    {
        token.ThrowIfCancellationRequested();
        if (entry.Contains(Separator, StringComparison.Ordinal))
        {
            var chain = entry.Split(Separator);
            if (chain.Length > MaximumArchiveDepth) throw new InvalidDataException("Archive nesting exceeds the supported depth of 8.");
            foreach (var name in chain) ValidateName(name);
            if (!IsArchive(chain[0])) throw new InvalidDataException("Nested entry is not an archive.");
            var outer = Extract(archive, chain[0], token, tempRoot);
            try
            {
                var inner = Extract(outer.FilePath, string.Join(Separator, chain.Skip(1)), token, tempRoot);
                return new ExtractedPackage(Path.GetDirectoryName(inner.FilePath)!, inner.FilePath, () =>
                {
                    try { inner.Dispose(); } finally { outer.Dispose(); }
                });
            }
            catch { outer.Dispose(); throw; }
        }
        ValidateName(entry);
        var info = new FileInfo(archive);
        var root = Path.GetFullPath(tempRoot ?? Path.Combine(AppContext.BaseDirectory, "Temp", "ZIP"));
        var key = info.FullName + "::" + info.Length + "::" + info.LastWriteTimeUtc.Ticks + "::" + root;
        lock (SessionLock)
        {
            if (!Sessions.TryGetValue(key, out var session))
            {
                var names = GetFileEntries(archive);
                foreach (var name in names) ValidateName(name);
                if (names.Distinct(StringComparer.Ordinal).Count() != names.Count || (!entry.EndsWith('/') && (!names.Contains(entry) || !(IsSupported(entry, true) || IsArchive(entry)))))
                    throw new InvalidDataException("Package entry not found or ambiguous.");
                Directory.CreateDirectory(root);
                if ((File.GetAttributes(root) & FileAttributes.ReparsePoint) != 0) throw new IOException("Archive temporary directory is a link.");
                session = new ArchiveSession { Directory = Path.Combine(root, Guid.NewGuid().ToString("N")) };
                Directory.CreateDirectory(session.Directory);
                try
                {
                    var index = 0;
                    var directories = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    ReadEntries(archive, _ => true, (name, input, size) =>
                    {
                        token.ThrowIfCancellationRequested();
                        // Preserve sibling relationships for split NAND dumps while keeping archive paths out of disk paths.
                        var parent = Path.GetDirectoryName(name.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar)) ?? "";
                        if (!directories.TryGetValue(parent, out var directory))
                        {
                            directory = Path.Combine(session.Directory, (index++).ToString());
                            Directory.CreateDirectory(directory);
                            directories.Add(parent, directory);
                        }
                        var file = Path.Combine(directory, name.Replace('\\', '/').Split('/')[^1]);
                        using var output = new FileStream(file, FileMode.CreateNew, FileAccess.Write, FileShare.None);
                        var buffer = new byte[1024 * 1024];
                        long total = 0;
                        int count;
                        while ((count = input.Read(buffer)) != 0)
                        {
                            token.ThrowIfCancellationRequested();
                            total += count;
                            if (total > size) throw new InvalidDataException("Invalid archive entry size.");
                            output.Write(buffer, 0, count);
                        }
                        if (total != size) throw new InvalidDataException("Truncated archive entry.");
                        session.Files.Add(name, file);
                    }, token);
                    Sessions.Add(key, session);
                }
                catch { Directory.Delete(session.Directory, recursive: true); throw; }
            }
            if (entry.EndsWith('/') && !session.Files.ContainsKey(entry))
            {
                var members = session.Files.Where(pair => pair.Key.Replace('\\', '/').StartsWith(entry, StringComparison.Ordinal)
                    && !pair.Key.Replace('\\', '/')[entry.Length..].Contains('/') && pair.Key.EndsWith(".nca", StringComparison.OrdinalIgnoreCase)).ToArray();
                if (members.Length == 0) throw new InvalidDataException("Firmware folder not found in archive.");
                var folder = Path.Combine(session.Directory, "firmware-" + Guid.NewGuid().ToString("N"));
                Directory.CreateDirectory(folder);
                try
                {
                    foreach (var member in members)
                    {
                        token.ThrowIfCancellationRequested();
                        File.Copy(member.Value, Path.Combine(folder, Path.GetFileName(member.Value)));
                    }
                    session.Files.Add(entry, folder);
                }
                catch
                {
                    Directory.Delete(folder, recursive: true);
                    if (session.References == 0)
                    {
                        Sessions.Remove(key);
                        Directory.Delete(session.Directory, recursive: true);
                    }
                    throw;
                }
            }
            if (!(IsSupported(entry, true) || IsArchive(entry) || entry.EndsWith('/')) || !session.Files.TryGetValue(entry, out var path))
                throw new InvalidDataException("Package entry not found.");
            session.References++;
            var retained = session;
            return new ExtractedPackage(retained.Directory, path, () =>
            {
                lock (SessionLock)
                {
                    if (--retained.References == 0)
                    {
                        Sessions.Remove(key);
                        Directory.Delete(retained.Directory, recursive: true);
                    }
                }
            });
        }
    }
}

public sealed class ExtractedPackage : IDisposable
{
    private readonly string _directory;
    private Action? _release;
    private int _disposed;
    internal ExtractedPackage(string directory, string filePath, Action? release = null) { _directory = directory; FilePath = filePath; _release = release; }
    public string FilePath { get; }
    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0) return;
        var release = Interlocked.Exchange(ref _release, null);
        if (release != null) { release(); return; }
        // Only the unique, directly created directory is removed; archive paths are never extracted.
        if (Directory.Exists(_directory)) Directory.Delete(_directory, recursive: true);
    }
}

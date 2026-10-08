using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Emignatik.NxFileViewer.Services.Updates;

public sealed record ViewerRelease(Version Version, string AssetName, Uri DownloadUrl, string? Digest)
{
    public string Tag { get; init; } = "";
    public bool IsPrerelease { get; init; }
    public string DisplayVersion => Tag.Length > 0 ? Tag : Version.ToString();
}
public sealed record PreparedViewerUpdate(string Directory, string Executable);

public sealed class ViewerUpdateService
{
    public const string Repository = "sandmaennchen5/fork_NxFileViewer";
    private static readonly HttpClient Client = CreateClient();
    private readonly HttpClient _client;
    public ViewerUpdateService(HttpClient? client = null) => _client = client ?? Client;
    private static HttpClient CreateClient()
    {
        var client = new HttpClient { Timeout = TimeSpan.FromMinutes(5) };
        client.DefaultRequestHeaders.UserAgent.ParseAdd("NxFileViewer-Updater/1.0");
        return client;
    }
    public async Task<ViewerRelease?> CheckAsync(Version current, string architecture, CancellationToken token, bool includePrereleases = false, string? currentTag = null, bool selfContained = false)
    {
        if (includePrereleases)
        {
            var candidates = new List<ViewerRelease>();
            for (var page = 1; ; page++)
            {
                using var listing = await _client.GetAsync($"https://api.github.com/repos/{Repository}/releases?per_page=100&page={page}", token).ConfigureAwait(false);
                if (listing.StatusCode == HttpStatusCode.NotFound) return null;
                listing.EnsureSuccessStatusCode();
                using var releases = JsonDocument.Parse(await listing.Content.ReadAsStringAsync(token).ConfigureAwait(false));
                foreach (var item in releases.RootElement.EnumerateArray())
                {
                    // A release for another architecture must not hide compatible releases.
                    var candidate = SelectRelease(item, current, architecture, true, skipMissingPackage: true, currentTag: currentTag, selfContained: selfContained);
                    if (candidate != null) candidates.Add(candidate);
                }
                if (releases.RootElement.GetArrayLength() < 100) break;
            }
            return candidates.OrderByDescending(r => ReleaseVersion(r)).ThenBy(r => r.IsPrerelease).FirstOrDefault();
        }
        using var response = await _client.GetAsync($"https://api.github.com/repos/{Repository}/releases/latest", token).ConfigureAwait(false);
        if (response.StatusCode == HttpStatusCode.NotFound) return null;
        response.EnsureSuccessStatusCode();
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync(token).ConfigureAwait(false));
        return SelectRelease(json.RootElement, current, architecture, currentTag: currentTag, selfContained: selfContained);
    }
    public static ViewerRelease? SelectRelease(JsonElement release, Version current, string architecture, bool includePrereleases = false, bool skipMissingPackage = false, string? currentTag = null, bool selfContained = false)
    {
        if (architecture is not ("x64" or "x86")) throw new NotSupportedException("Unsupported update architecture.");
        if (release.GetProperty("draft").GetBoolean()) return null;
        var tag = release.GetProperty("tag_name").GetString()!;
        if (tag.Contains('/') || tag.Contains('\\')) return null;
        var prerelease = release.GetProperty("prerelease").GetBoolean() || tag.Split('+')[0].Contains('-');
        if (prerelease && !includePrereleases) return null;
        if (!ViewerVersion.TryParse(tag, out var candidateVersion)) return null;
        var version = candidateVersion.Numeric;
        var installedVersion = ViewerVersion.TryParse(currentTag, out var parsedCurrent) && parsedCurrent.Numeric == Normalize(current)
            ? parsedCurrent : ViewerVersion.FromNumeric(current);
        if (candidateVersion.CompareTo(installedVersion) <= 0) return null;
        var variant = selfContained ? "_self-contained" : "";
        var name = $"NxFileViewer_v{version.Major}.{version.Minor}.{version.Build}{variant}_{architecture}.zip";
        var assets = release.GetProperty("assets").EnumerateArray().ToArray();
        var taggedName = $"NxFileViewer_v{tag.TrimStart('v', 'V')}{variant}_{architecture}.zip";
        var asset = assets.FirstOrDefault(a => a.GetProperty("name").GetString() == name);
        if (asset.ValueKind == JsonValueKind.Undefined) asset = assets.FirstOrDefault(a => a.GetProperty("name").GetString() == taggedName);
        if (asset.ValueKind == JsonValueKind.Undefined)
        {
            if (skipMissingPackage) return null;
            throw new InvalidDataException("Missing release package: " + name);
        }
        name = asset.GetProperty("name").GetString()!;
        var url = new Uri(asset.GetProperty("browser_download_url").GetString()!);
        ValidateDownloadUrl(url);
        var digest = asset.TryGetProperty("digest", out var hash) ? hash.GetString() : null;
        return new(version, name, url, digest) { Tag = tag, IsPrerelease = prerelease };
    }
    private static ViewerVersion ReleaseVersion(ViewerRelease release) =>
        ViewerVersion.TryParse(release.Tag, out var parsed) ? parsed : ViewerVersion.FromNumeric(release.Version);
    private static Version Normalize(Version version) => new(version.Major, version.Minor, Math.Max(0, version.Build));
    private static void ValidateDownloadUrl(Uri url)
    {
        if (url.Scheme != "https" || url.Host != "github.com" || !url.AbsolutePath.StartsWith("/" + Repository + "/releases/download/", StringComparison.Ordinal))
            throw new InvalidDataException("Unexpected update download URL.");
    }
    public static string ExpectedHash(string? digest)
    {
        if (digest == null || !digest.StartsWith("sha256:", StringComparison.OrdinalIgnoreCase) || digest.Length != 71 ||
            !digest[7..].All(Uri.IsHexDigit)) throw new InvalidDataException("Release has no valid SHA-256 digest. Re-upload its application ZIP on GitHub.");
        return digest[7..];
    }
    public async Task<PreparedViewerUpdate> PrepareAsync(ViewerRelease release, string applicationDirectory, CancellationToken token)
    {
        ValidateDownloadUrl(release.DownloadUrl);
        var expectedHash = ExpectedHash(release.Digest);
        var updates = Path.Combine(Path.GetFullPath(applicationDirectory), "Updates");
        Directory.CreateDirectory(updates);
        if ((File.GetAttributes(updates) & FileAttributes.ReparsePoint) != 0) throw new IOException("Updates must be a local application subdirectory.");
        var stage = Path.Combine(updates, Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(stage);
        var archive = Path.Combine(stage, "release.zip");
        try
        {
            using var response = await _client.GetAsync(release.DownloadUrl, HttpCompletionOption.ResponseHeadersRead, token).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            await using (var input = await response.Content.ReadAsStreamAsync(token).ConfigureAwait(false))
            await using (var output = File.Create(archive))
            {
                var buffer = new byte[81920];
                long total = 0;
                int read;
                while ((read = await input.ReadAsync(buffer, token).ConfigureAwait(false)) > 0)
                {
                    total += read;
                    if (total > 256L * 1024 * 1024) throw new InvalidDataException("Update archive exceeds size limit.");
                    await output.WriteAsync(buffer.AsMemory(0, read), token).ConfigureAwait(false);
                }
            }
            await using (var input = File.OpenRead(archive))
            {
                var actual = Convert.ToHexString(await SHA256.HashDataAsync(input, token).ConfigureAwait(false));
                if (!actual.Equals(expectedHash, StringComparison.OrdinalIgnoreCase)) throw new InvalidDataException("Update SHA-256 mismatch.");
            }
            var exe = ExtractExecutable(archive, stage, release.AssetName, token);
            using (var executable = File.OpenRead(exe))
            using (var pe = new PEReader(executable))
            {
                var expectedMachine = release.AssetName.EndsWith("_x64.zip", StringComparison.Ordinal) ? Machine.Amd64 : Machine.I386;
                if (pe.PEHeaders.CoffHeader.Machine != expectedMachine ||
                    (pe.PEHeaders.CoffHeader.Characteristics & Characteristics.Dll) != 0 || pe.PEHeaders.CorHeader != null) throw new InvalidDataException("Update executable architecture mismatch.");
            }
            var info = System.Diagnostics.FileVersionInfo.GetVersionInfo(exe);
            if (!string.IsNullOrEmpty(release.Tag) &&
                (!ViewerVersion.TryParse(release.Tag, out var expectedVersion) ||
                 !ViewerVersion.TryParse(info.ProductVersion, out var actualVersion) ||
                 expectedVersion.CompareTo(actualVersion) != 0))
                throw new InvalidDataException("Update executable preview version does not match release tag.");
            if (new Version(info.FileMajorPart, info.FileMinorPart, info.FileBuildPart) != Normalize(release.Version))
                throw new InvalidDataException("Update executable version does not match release.");
            File.Delete(archive);
            return new(stage, exe);
        }
        catch
        {
            // Delete only the files we created, never any application data.
            foreach (var file in new[] { archive, Path.Combine(stage, "new.exe") })
                try { File.Delete(file); } catch (Exception ex) when (ex is IOException or UnauthorizedAccessException) { }
            throw;
        }
    }
    public static string ExtractExecutable(string archive, string stage, string assetName, CancellationToken token)
    {
        using var zip = ZipFile.OpenRead(archive);
        var folder = Path.GetFileNameWithoutExtension(assetName);
        // Compress-Archive on Windows writes backslashes; ZIP tools also use forward slashes.
        var files = zip.Entries.Where(e => !e.FullName.Replace('\\', '/').EndsWith('/')).ToArray();
        var executablePath = files.Length == 1 ? files[0].FullName.Replace('\\', '/') : null;
        if (files.Length != 1 || (executablePath != folder + "/NxFileViewer.exe" && executablePath != "NxFileViewer.exe") ||
            files[0].Length == 0 || files[0].Length > 256L * 1024 * 1024)
            throw new InvalidDataException("Expected a single-file NxFileViewer release ZIP.");
        token.ThrowIfCancellationRequested();
        // Use a fixed filename rather than any archive-supplied path.
        var destination = Path.Combine(stage, "new.exe");
        using var input = files[0].Open();
        using var output = new FileStream(destination, FileMode.CreateNew, FileAccess.Write);
        var buffer = new byte[81920];
        int read;
        long total = 0;
        while ((read = input.Read(buffer)) > 0)
        {
            token.ThrowIfCancellationRequested();
            total += read;
            if (total > files[0].Length || total > 256L * 1024 * 1024) throw new InvalidDataException("Update executable exceeds declared size.");
            output.Write(buffer, 0, read);
        }
        if (total != files[0].Length) throw new InvalidDataException("Incomplete update executable.");
        return destination;
    }
}

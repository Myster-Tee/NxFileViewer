using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Services.BackgroundTask;
using Emignatik.NxFileViewer.Settings;
using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer.Services.Nsz;

/// <summary>Versioned official binaries; activation happens only after digest and CLI compatibility checks.</summary>
public sealed class NszPluginManager(IAppSettings settings, ILogger<NszPluginManager> logger,
    string? rootDirectory = null, HttpClient? httpClient = null, Action<string, CancellationToken>? compatibilityCheck = null)
{
    private static readonly HttpClient Client = CreateClient();
    private readonly System.Collections.Generic.Dictionary<string, (long Length, long Modified)> _checkedExecutables = new(StringComparer.OrdinalIgnoreCase);
    private static HttpClient CreateClient()
    {
        var client = new HttpClient { Timeout = TimeSpan.FromMinutes(5) };
        client.DefaultRequestHeaders.UserAgent.ParseAdd("NxFileViewer-NSZ/1.0");
        return client;
    }
    public string RootDirectory => rootDirectory ?? Path.Combine(AppContext.BaseDirectory, "Plugins", "NSZ");
    private string StatePath => Path.Combine(RootDirectory, "active.json");
    private sealed record State(string Executable, string Version, string? PreviousExecutable = null, string? PreviousVersion = null,
        string? SourceAssetName = null, string? SourceDigest = null, string? ExecutableDigest = null);
    private State? ReadState()
    {
        if (!File.Exists(StatePath)) return null;
        var state = JsonSerializer.Deserialize<State>(File.ReadAllText(StatePath))!;
        string Resolve(string path)
        {
            var root = Path.GetFullPath(RootDirectory).TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
            var resolved = Path.GetFullPath(path, root);
            if (!resolved.StartsWith(root, StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException("Managed NSZ executable must be inside the application plugin directory.");
            return resolved;
        }
        return state with { Executable = Resolve(state.Executable),
            PreviousExecutable = state.PreviousExecutable == null ? null : Resolve(state.PreviousExecutable) };
    }
    public string? InstalledVersion => string.IsNullOrWhiteSpace(settings.NszExecutablePath) ? ReadState()?.Version : null;
    public string ExecutablePath => string.IsNullOrWhiteSpace(settings.NszExecutablePath)
        ? ReadState()?.Executable ?? "" : Path.GetFullPath(settings.NszExecutablePath);
    public string GuiExecutablePath
    {
        get { try { var path = ExecutablePath; return File.Exists(path) && Path.GetFileName(path).StartsWith("nsz-gui", StringComparison.OrdinalIgnoreCase) ? path : ""; } catch { return ""; } }
    }
    public string Status
    {
        get
        {
            var keys = LocalizationManager.Instance.Current.Keys;
            try
            {
                var state = string.IsNullOrWhiteSpace(settings.NszExecutablePath) ? ReadState() : null;
                var executable = ExecutablePath;
                if (!File.Exists(executable)) return keys.Nsz_NotInstalled;
                return keys.Nsz_Installed + " — " + (state?.Version ?? keys.Nsz_CustomExecutable) +
                    Environment.NewLine + executable;
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException or ArgumentException)
            { return keys.Nsz_NotInstalled; }
        }
    }
    public bool CanRollback => string.IsNullOrWhiteSpace(settings.NszExecutablePath) && File.Exists(ReadState()?.PreviousExecutable);

    public void Rollback()
    {
        var state = ReadState();
        if (!CanRollback || state == null) throw new InvalidOperationException("No previous NSZ version.");
        WriteState(new(state.PreviousExecutable!, state.PreviousVersion!, state.Executable, state.Version));
    }

    private void WriteState(State state)
    {
        Directory.CreateDirectory(RootDirectory);
        var temporary = StatePath + "." + Guid.NewGuid().ToString("N") + ".tmp";
        // Relative paths allow the whole application directory to be moved.
        state = state with { Executable = Path.GetRelativePath(Path.GetFullPath(RootDirectory), Path.GetFullPath(state.Executable)),
            PreviousExecutable = state.PreviousExecutable == null ? null : Path.GetRelativePath(Path.GetFullPath(RootDirectory), Path.GetFullPath(state.PreviousExecutable)) };
        File.WriteAllText(temporary, JsonSerializer.Serialize(state));
        File.Move(temporary, StatePath, overwrite: true);
    }

    public void Prepare(IProgressReporter progress, CancellationToken token)
    {
        if (!string.IsNullOrWhiteSpace(settings.NszExecutablePath))
        {
            (compatibilityCheck ?? CheckManagedCompatibility)(ExecutablePath, token);
            return;
        }
        if (settings.NszCheckUpdates || !File.Exists(ExecutablePath))
        {
            try { UpdateAsync(progress, token).GetAwaiter().GetResult(); }
            catch (Exception ex) when (!token.IsCancellationRequested && File.Exists(ExecutablePath) &&
                ex is HttpRequestException or IOException or JsonException or OperationCanceledException or InvalidOperationException or System.Collections.Generic.KeyNotFoundException)
            {
                // An installed version remains usable offline. The UI log makes the fallback visible.
                logger.LogWarning(ex, "NSZ update unavailable; using installed version.");
                progress.SetText(LocalizationManager.Instance.Current.Keys.Nsz_OfflineFallback);
            }
        }
        (compatibilityCheck ?? CheckManagedCompatibility)(ExecutablePath, token);
        progress.SetMode(false);
    }

    public async Task UpdateAsync(IProgressReporter progress, CancellationToken token)
    {
        progress.SetMode(true);
        progress.SetText(LocalizationManager.Instance.Current.Keys.Nsz_Updating);
        var client = httpClient ?? Client;
        using var response = await client.GetAsync("https://api.github.com/repos/nicoboss/nsz/releases/latest", token).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        using var release = JsonDocument.Parse(await response.Content.ReadAsStringAsync(token).ConfigureAwait(false));
        var root = release.RootElement;
        if (root.GetProperty("draft").GetBoolean() || root.GetProperty("prerelease").GetBoolean())
            throw new InvalidDataException("NSZ release is not stable.");
        var tag = root.GetProperty("tag_name").GetString()!;
        var arch = RuntimeInformation.OSArchitecture == Architecture.Arm64 ? "arm64" : "x64";
        if (!Environment.Is64BitOperatingSystem) throw new PlatformNotSupportedException("NSZ requires 64-bit Windows.");
        var expectedName = $"nsz-cli-windows-{arch}.exe";
        var asset = root.GetProperty("assets").EnumerateArray().FirstOrDefault(a => a.GetProperty("name").GetString() == expectedName);
        if (asset.ValueKind == JsonValueKind.Undefined) throw new InvalidDataException("No compatible Windows NSZ CLI asset.");
        var digest = asset.GetProperty("digest").GetString();
        if (digest == null || !digest.StartsWith("sha256:", StringComparison.Ordinal))
            throw new InvalidDataException("NSZ asset has no SHA-256 digest.");
        var state = ReadState();
        if (state?.Version == tag && File.Exists(state.Executable))
        {
            var sourceAsset = root.GetProperty("assets").EnumerateArray().FirstOrDefault(a =>
                a.GetProperty("name").GetString() == (state.SourceAssetName ?? expectedName));
            var sourceDigest = sourceAsset.ValueKind == JsonValueKind.Undefined ? null : sourceAsset.GetProperty("digest").GetString();
            if (sourceDigest != null && sourceDigest == (state.SourceDigest ?? digest))
            {
                await using var existing = File.OpenRead(state.Executable);
                var hash = Convert.ToHexString(await SHA256.HashDataAsync(existing, token).ConfigureAwait(false));
                if (hash.Equals(state.ExecutableDigest ?? digest[7..], StringComparison.OrdinalIgnoreCase))
                { progress.SetMode(false); return; }
            }
        }
        var url = new Uri(asset.GetProperty("browser_download_url").GetString()!);
        if (url.Scheme != "https" || url.Host != "github.com" || !url.AbsolutePath.StartsWith("/nicoboss/nsz/releases/download/", StringComparison.Ordinal))
            throw new InvalidDataException("Unexpected NSZ download URL.");
        Directory.CreateDirectory(RootDirectory);
        var versionDirectory = Path.Combine(RootDirectory, "version-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(versionDirectory);
        var executable = Path.Combine(versionDirectory, expectedName);
        try
        {
            await DownloadVerifiedAsync(client, asset, executable, token).ConfigureAwait(false);
            var sourceName = expectedName;
            var sourceDigest = digest;
            try { (compatibilityCheck ?? CheckManagedCompatibility)(executable, token); }
            catch (NszRuntimeStartupException) when (arch == "x64")
            {
                // The official GUI build also supports CLI arguments; do not launch it without --help.
                var guiName = "nsz-gui-windows-x64.zip";
                var guiAsset = root.GetProperty("assets").EnumerateArray().FirstOrDefault(a => a.GetProperty("name").GetString() == guiName);
                if (guiAsset.ValueKind == JsonValueKind.Undefined) throw;
                logger.LogWarning("Standalone NSZ CLI Python runtime failed; checking the official GUI package in CLI mode.");
                var archivePath = Path.Combine(versionDirectory, guiName);
                await DownloadVerifiedAsync(client, guiAsset, archivePath, token).ConfigureAwait(false);
                using (var archive = ZipFile.OpenRead(archivePath))
                {
                    var candidates = archive.Entries.Where(e => e.FullName.Replace('\\', '/').Split('/')[^1] == "nsz-gui-windows-x64.exe").ToArray();
                    if (candidates.Length != 1) throw new InvalidDataException("Unexpected NSZ GUI package layout.");
                    var guiDirectory = Path.Combine(versionDirectory, "gui");
                    // Preserve runtime DLLs and supporting files shipped beside the executable.
                    archive.ExtractToDirectory(guiDirectory);
                    executable = Path.Combine(guiDirectory, candidates[0].FullName.Replace('/', Path.DirectorySeparatorChar));
                }
                File.Delete(archivePath);
                (compatibilityCheck ?? CheckManagedCompatibility)(executable, token);
                sourceName = guiName;
                sourceDigest = guiAsset.GetProperty("digest").GetString();
            }
            token.ThrowIfCancellationRequested();
            await using var activated = File.OpenRead(executable);
            var executableDigest = Convert.ToHexString(await SHA256.HashDataAsync(activated, token).ConfigureAwait(false));
            WriteState(new(executable, tag, state?.Executable, state?.Version, sourceName, sourceDigest, executableDigest));
        }
        catch
        {
            try { Directory.Delete(versionDirectory, recursive: true); }
            catch (IOException) { }
            catch (UnauthorizedAccessException) { }
            throw;
        }
        finally { progress.SetMode(false); }
    }

    private async Task DownloadVerifiedAsync(HttpClient client, JsonElement asset, string destination, CancellationToken token)
    {
        var digest = asset.GetProperty("digest").GetString();
        if (digest == null || !digest.StartsWith("sha256:", StringComparison.Ordinal) || digest.Length != 71)
            throw new InvalidDataException("NSZ asset has no SHA-256 digest.");
        var url = new Uri(asset.GetProperty("browser_download_url").GetString()!);
        if (url.Scheme != "https" || url.Host != "github.com" || !url.AbsolutePath.StartsWith("/nicoboss/nsz/releases/download/", StringComparison.Ordinal))
            throw new InvalidDataException("Unexpected NSZ download URL.");
        var cacheDirectory = Path.Combine(RootDirectory, "Downloads");
        Directory.CreateDirectory(cacheDirectory);
        var cached = Path.Combine(cacheDirectory, digest[7..].ToLowerInvariant() + ".download");
        if (File.Exists(cached))
        {
            await using var previous = File.OpenRead(cached);
            var cachedHash = Convert.ToHexString(await SHA256.HashDataAsync(previous, token).ConfigureAwait(false));
            if (cachedHash.Equals(digest[7..], StringComparison.OrdinalIgnoreCase))
            {
                logger.LogInformation("NSZ: reusing verified download {Asset}", asset.GetProperty("name").GetString());
                File.Copy(cached, destination, overwrite: true);
                return;
            }
        }
        using var download = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, token).ConfigureAwait(false);
        download.EnsureSuccessStatusCode();
        await using (var output = File.Create(destination))
            await download.Content.CopyToAsync(output, token).ConfigureAwait(false);
        await using var input = File.OpenRead(destination);
        var actual = Convert.ToHexString(await SHA256.HashDataAsync(input, token).ConfigureAwait(false));
        if (!actual.Equals(digest[7..], StringComparison.OrdinalIgnoreCase)) throw new InvalidDataException("NSZ SHA-256 mismatch.");
        var cacheStaging = cached + "." + Guid.NewGuid().ToString("N") + ".tmp";
        try { File.Copy(destination, cacheStaging); File.Move(cacheStaging, cached, overwrite: true); }
        finally { if (File.Exists(cacheStaging)) File.Delete(cacheStaging); }
    }

    private void CheckManagedCompatibility(string executable, CancellationToken token)
    {
        var file = new FileInfo(executable);
        var stamp = (file.Length, file.LastWriteTimeUtc.Ticks);
        if (_checkedExecutables.TryGetValue(file.FullName, out var checkedStamp) && checkedStamp == stamp) return;
        logger.LogInformation("NSZ startup check: {Executable}; first startup may take up to 120 seconds.", executable);
        try
        {
            CheckCompatibility(executable, token, line => logger.LogInformation("NSZ startup: {Output}", line));
            _checkedExecutables[file.FullName] = stamp;
            logger.LogInformation("NSZ startup check passed.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "NSZ startup check failed: {Executable}", executable);
            throw;
        }
    }
    public static void CheckCompatibility(string executable, CancellationToken token, Action<string>? diagnostic = null)
    {
        if (!File.Exists(executable)) throw new FileNotFoundException(LocalizationManager.Instance.Current.Keys.Nsz_NotInstalled);
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(token);
        timeout.CancelAfter(TimeSpan.FromSeconds(120));
        string help;
        try { help = NszProcess.Run(executable, new[] { "--help" }, Path.GetDirectoryName(executable)!, timeout.Token, onDiagnostic: diagnostic); }
        catch (OperationCanceledException ex) when (!token.IsCancellationRequested)
        { throw new IOException("NSZ startup timed out after 120 seconds: " + executable, ex); }
        if (new[] { "--keys", "--output", "--keep", "--verify", "--solid", "--block", "--bs", "-C", "-D" }.Any(flag => !help.Contains(flag, StringComparison.Ordinal)))
            throw new InvalidDataException(LocalizationManager.Instance.Current.Keys.Nsz_Incompatible);
    }
}

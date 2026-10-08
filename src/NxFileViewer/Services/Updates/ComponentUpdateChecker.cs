using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Services.Nand;
using Emignatik.NxFileViewer.Services.Nsz;
using Emignatik.NxFileViewer.Settings;

namespace Emignatik.NxFileViewer.Services.Updates;

public sealed record ComponentUpdateResult(string Name, string Status, bool UpdateAvailable);

public sealed class ComponentUpdateChecker(NszPluginManager nsz, NandPluginManager nand, IAppSettings settings,
    HttpClient? httpClient = null, string? applicationDirectory = null)
{
    public async Task<string> CheckAsync(CancellationToken token) => string.Join(Environment.NewLine, (await CheckResultsAsync(token)).Select(r => r.Name + ": " + r.Status));

    public async Task<ComponentUpdateResult[]> CheckResultsAsync(CancellationToken token)
    {
        using var owned = httpClient == null ? new HttpClient { Timeout = TimeSpan.FromSeconds(20) } : null;
        var client = httpClient ?? owned!;
        var directory = applicationDirectory ?? AppContext.BaseDirectory;
        var keys = LocalizationManager.Instance.Current.Keys;
        async Task<ComponentUpdateResult> Check(string label, Func<Task<(string Status, bool Available)>> action)
        {
            try { var result = await action(); return new(label, result.Status, result.Available); }
            catch (OperationCanceledException) when (token.IsCancellationRequested) { throw; }
            catch (Exception) { return new(label, keys.Update_Failed, false); }
        }
        async Task<JsonDocument> Get(string url)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.UserAgent.ParseAdd("NxFileViewer-Update/1.0");
            request.Headers.CacheControl = new System.Net.Http.Headers.CacheControlHeaderValue { NoCache = true };
            using var response = await client.SendAsync(request, token);
            response.EnsureSuccessStatusCode();
            return JsonDocument.Parse(await response.Content.ReadAsStringAsync(token));
        }
        async Task<(string Status, bool Available)> Plugin(string executable, string? version, string repository)
        {
            if (!File.Exists(executable)) return (keys.Component_NotInstalled, false);
            if (version == null) return (keys.Component_CustomVersion, false);
            using var release = await Get("https://api.github.com/repos/" + repository + "/releases/latest");
            var latest = release.RootElement.GetProperty("tag_name").GetString()!;
            var available = IsNewer(version, latest);
            return (available ? string.Format(keys.Update_Available, latest) : keys.Update_Current, available);
        }
        var checks = new List<Task<ComponentUpdateResult>>
        {
            Check("NSZ", () => Plugin(nsz.ExecutablePath, nsz.InstalledVersion, "nicoboss/nsz")),
            Check("NxNandManager", () => Plugin(nand.ExecutablePath, nand.InstalledVersion, "THZoria/NxNandManager"))
        };
        foreach (var region in new[] { settings.TitleDbRegion, "US.en" }.Distinct())
        {
            var captured = region;
            checks.Add(Check("Title DB " + region, async () =>
            {
                if (!Regex.IsMatch(captured, @"^[A-Z]{2}\.[a-z]{2}\z")) throw new InvalidDataException();
                using var entry = await Get("https://api.github.com/repos/blawar/titledb/contents/" + captured + ".json?ref=master");
                var path = Path.Combine(directory, "Cache", "TitleDB", captured + ".json");
                var available = !MatchesGitBlob(path, entry.RootElement.GetProperty("sha").GetString()!);
                return (available ? keys.Component_UpdateAvailable : keys.Update_Current, available);
            }));
        }
        checks.Add(Check(keys.DataUpdate_Firmware, async () =>
        {
            using var listing = await Get("https://api.github.com/repos/sandmaennchen5/fork_NxFileViewer/contents/fw/hashes?ref=master");
            var entries = listing.RootElement.EnumerateArray().Where(e => e.GetProperty("type").GetString() == "file" && e.GetProperty("name").GetString()!.EndsWith(".json", StringComparison.OrdinalIgnoreCase)).ToArray();
            if (entries.Length == 0) throw new InvalidDataException();
            foreach (var entry in entries)
            {
                token.ThrowIfCancellationRequested();
                var name = entry.GetProperty("name").GetString()!;
                if (Path.GetFileName(name) != name) throw new InvalidDataException();
                if (!MatchesGitBlob(Path.Combine(directory, "fw", "hashes", name), entry.GetProperty("sha").GetString()!)) return (keys.Component_UpdateAvailable, true);
            }
            return (keys.Update_Current, false);
        }));
        return await Task.WhenAll(checks);
    }

    public static bool IsNewer(string installed, string latest)
    {
        static Version? Parse(string value) => Version.TryParse(Regex.Match(value, @"\d+(?:\.\d+){1,3}").Value, out var version) ? version : null;
        var before = Parse(installed); var after = Parse(latest);
        return before != null && after != null ? after > before : !string.Equals(installed.TrimStart('v', 'V'), latest.TrimStart('v', 'V'), StringComparison.OrdinalIgnoreCase);
    }

    public static bool MatchesGitBlob(string path, string sha)
    {
        if (sha.Length != 40 || !sha.All(Uri.IsHexDigit)) throw new InvalidDataException("Invalid Git blob hash.");
        if (!File.Exists(path)) return false;
        using var file = File.OpenRead(path);
        using var hash = IncrementalHash.CreateHash(HashAlgorithmName.SHA1);
        hash.AppendData(Encoding.ASCII.GetBytes("blob " + file.Length + "\0"));
        var buffer = new byte[65536];
        int read;
        while ((read = file.Read(buffer)) > 0) hash.AppendData(buffer.AsSpan(0, read));
        return Convert.ToHexString(hash.GetHashAndReset()).Equals(sha, StringComparison.OrdinalIgnoreCase);
    }
}

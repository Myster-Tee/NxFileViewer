using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Emignatik.NxFileViewer.Services.Integrity;

/// <summary>Downloads a consistent set of manifests into memory only. No disk cache.</summary>
public static class GitHubFirmwareReferences
{
    public static async Task<FirmwareIntegrityVerifier> LoadAsync(HttpClient client, CancellationToken cancellationToken)
    {
        var manifests = await LoadManifestsAsync(client, cancellationToken).ConfigureAwait(false);
        return new FirmwareIntegrityVerifier(manifests.Values);
    }

    public static async Task<IReadOnlyDictionary<string, string>> LoadManifestsAsync(HttpClient client, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get,
            "https://api.github.com/repos/sandmaennchen5/fork_NxFileViewer/contents/fw/hashes?ref=master");
        request.Headers.UserAgent.ParseAdd("NxFileViewer/3.0.4");
        request.Headers.CacheControl = new System.Net.Http.Headers.CacheControlHeaderValue { NoCache = true };
        using var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        using var listing = JsonDocument.Parse(await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false));
        var entries = listing.RootElement.EnumerateArray()
            .Where(e => e.GetProperty("type").GetString() == "file" && e.GetProperty("name").GetString()!.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
            .Select(e => (Name: e.GetProperty("name").GetString()!, Sha: e.GetProperty("sha").GetString()!)).ToArray();
        using var gate = new SemaphoreSlim(4);
        var manifests = await Task.WhenAll(entries.Select(async entry =>
        {
            if (Path.GetFileName(entry.Name) != entry.Name || entry.Sha.Length != 40 || !entry.Sha.All(Uri.IsHexDigit))
                throw new InvalidDataException("Invalid GitHub firmware listing.");
            await gate.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                using var download = new HttpRequestMessage(HttpMethod.Get,
                    "https://raw.githubusercontent.com/sandmaennchen5/fork_NxFileViewer/master/fw/hashes/" + Uri.EscapeDataString(entry.Name));
                download.Headers.CacheControl = new System.Net.Http.Headers.CacheControlHeaderValue { NoCache = true };
                using var data = await client.SendAsync(download, cancellationToken).ConfigureAwait(false);
                data.EnsureSuccessStatusCode();
                var bytes = await data.Content.ReadAsByteArrayAsync(cancellationToken).ConfigureAwait(false);
                // Reject files changed between the directory listing and download.
                using var hash = IncrementalHash.CreateHash(HashAlgorithmName.SHA1);
                hash.AppendData(Encoding.ASCII.GetBytes("blob " + bytes.Length + "\0"));
                hash.AppendData(bytes);
                if (!Convert.ToHexString(hash.GetHashAndReset()).Equals(entry.Sha, StringComparison.OrdinalIgnoreCase))
                    throw new InvalidDataException("GitHub firmware reference changed during download.");
                return (entry.Name, Manifest: Encoding.UTF8.GetString(bytes).TrimStart('\uFEFF'));
            }
            finally { gate.Release(); }
        })).ConfigureAwait(false);
        var result = manifests.ToDictionary(m => m.Name, m => m.Manifest, StringComparer.OrdinalIgnoreCase);
        _ = new FirmwareIntegrityVerifier(result.Values); // Validate the whole set before exposing it.
        return result;
    }
}

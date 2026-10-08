using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer.Services.OnlineServices;

internal sealed class TitleDbCatalog(string cacheDirectory, ILogger? logger)
{
    private readonly SemaphoreSlim _gate = new(1, 1);
    private readonly Dictionary<string, (Dictionary<string, OnlineTitleInfo> Titles, DateTime RefreshAfter)> _catalogs = new();

    public async Task<OnlineTitleInfo?> FindAsync(string region, string titleId, HttpClient client)
    {
        if (!Regex.IsMatch(region, "^[A-Z]{2}\\.[a-z]{2}$"))
            throw new ArgumentException("Invalid TitleDB region.", nameof(region));
        await _gate.WaitAsync();
        try
        {
            var now = DateTime.UtcNow;
            if (_catalogs.TryGetValue(region, out var cached) && now < cached.RefreshAfter)
                return cached.Titles.GetValueOrDefault(titleId);

            var path = Path.Combine(cacheDirectory, region + ".json");
            Dictionary<string, OnlineTitleInfo>? titles = cached.Titles;
            DateTime refreshed = DateTime.MinValue;
            if (titles == null && File.Exists(path))
            {
                try
                {
                    titles = Parse(await File.ReadAllTextAsync(path));
                    refreshed = File.GetLastWriteTimeUtc(path);
                }
                catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException)
                {
                    logger?.LogWarning("Unable to read TitleDB cache: {Reason}", ex.Message);
                }
            }
            if (titles == null || now - refreshed >= TimeSpan.FromDays(1))
            {
                try
                {
                    var json = await client.GetStringAsync($"https://raw.githubusercontent.com/blawar/titledb/master/{region}.json");
                    titles = Parse(json); // Never replace a good cache with invalid JSON.
                    refreshed = now;
                    try
                    {
                        Directory.CreateDirectory(cacheDirectory);
                        var tempPath = path + "." + Guid.NewGuid().ToString("N") + ".tmp";
                        try
                        {
                            await File.WriteAllTextAsync(tempPath, json);
                            File.Move(tempPath, path, overwrite: true);
                        }
                        finally { if (File.Exists(tempPath)) File.Delete(tempPath); }
                    }
                    catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
                    {
                        logger?.LogWarning("Unable to save TitleDB cache: {Reason}", ex.Message);
                    }
                }
                catch (Exception ex) when (titles != null && ex is HttpRequestException or TaskCanceledException or JsonException)
                {
                    logger?.LogWarning("TitleDB update unavailable: {Reason}. Using cached titles.", ex.Message);
                    _catalogs[region] = (titles!, now.AddMinutes(5));
                    return titles!.GetValueOrDefault(titleId);
                }
            }
            _catalogs[region] = (titles!, refreshed.AddDays(1));
            return titles!.GetValueOrDefault(titleId);
        }
        finally { _gate.Release(); }
    }

    public async Task<int> RefreshAsync(string region, HttpClient client, CancellationToken token)
    {
        if (!Regex.IsMatch(region, "^[A-Z]{2}\\.[a-z]{2}$")) throw new ArgumentException("Invalid TitleDB region.");
        await _gate.WaitAsync(token);
        try
        {
            var json = await client.GetStringAsync($"https://raw.githubusercontent.com/blawar/titledb/master/{region}.json", token);
            var titles = Parse(json);
            Directory.CreateDirectory(cacheDirectory);
            var path = Path.Combine(cacheDirectory, region + ".json");
            var temporary = path + "." + Guid.NewGuid().ToString("N") + ".tmp";
            try
            {
                await File.WriteAllTextAsync(temporary, json, token);
                token.ThrowIfCancellationRequested();
                File.Move(temporary, path, overwrite: true);
                _catalogs[region] = (titles, DateTime.UtcNow.AddDays(1));
                return titles.Count;
            }
            finally { if (File.Exists(temporary)) File.Delete(temporary); }
        }
        finally { _gate.Release(); }
    }

    private static Dictionary<string, OnlineTitleInfo> Parse(string json)
    {
        using var document = JsonDocument.Parse(json);
        if (document.RootElement.ValueKind != JsonValueKind.Object)
            throw new JsonException("TitleDB must contain an object indexed by NSU ID.");
        var titles = new Dictionary<string, OnlineTitleInfo>(StringComparer.OrdinalIgnoreCase);
        foreach (var entry in document.RootElement.EnumerateObject())
        {
            var value = entry.Value;
            if (value.ValueKind != JsonValueKind.Object)
                continue;
            var id = GetString(value, "id");
            var name = GetString(value, "name");
            if (!Regex.IsMatch(id, "^[a-fA-F0-9]{16}$") || string.IsNullOrWhiteSpace(name))
                continue;
            titles.TryAdd(id, new OnlineTitleInfo
            {
                Id = id, Name = name, Publisher = GetString(value, "publisher"),
                Description = GetString(value, "description"), IconUrl = GetString(value, "iconUrl")
            });
        }
        if (titles.Count == 0)
            throw new JsonException("TitleDB contains no usable title entries.");
        return titles;
    }

    private static string GetString(JsonElement value, string property) =>
        value.TryGetProperty(property, out var field) && field.ValueKind == JsonValueKind.String ? field.GetString()! : "";
}

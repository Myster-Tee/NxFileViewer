using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using Emignatik.NxFileViewer.Localization;
using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer.Services.Integrity;

public interface IFirmwareReferenceProvider
{
    (FirmwareIntegrityVerifier? Verifier, string Notice) Load(CancellationToken token);
}

public sealed class FirmwareReferenceProvider(ILogger logger) : IFirmwareReferenceProvider
{
    public (FirmwareIntegrityVerifier? Verifier, string Notice) Load(CancellationToken token)
    {
        var keys = LocalizationManager.Instance.Current.Keys;
        using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(15) };
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(token);
        timeout.CancelAfter(TimeSpan.FromSeconds(60));
        try { return (GitHubFirmwareReferences.LoadAsync(client, timeout.Token).GetAwaiter().GetResult(), keys.Firmware_OnlineSource); }
        catch (Exception ex) when (!token.IsCancellationRequested && IsReferenceFailure(ex))
        {
            logger.LogWarning(ex, "Online firmware references unavailable.");
            try { return (new FirmwareIntegrityVerifier(), keys.Firmware_OfflineSource); }
            catch (Exception offline) when (IsReferenceFailure(offline))
            {
                logger.LogWarning(offline, "Offline firmware references unavailable.");
                return (null, keys.Firmware_NoReferences);
            }
        }
    }
    private static bool IsReferenceFailure(Exception ex) => ex is HttpRequestException or OperationCanceledException or
        IOException or UnauthorizedAccessException or System.Text.Json.JsonException or InvalidOperationException or
        System.Collections.Generic.KeyNotFoundException or ArgumentException;
}

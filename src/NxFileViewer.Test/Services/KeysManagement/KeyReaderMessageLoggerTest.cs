using System;
using Emignatik.NxFileViewer.Localization.Keys;
using Emignatik.NxFileViewer.Services.KeysManagement;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Emignatik.NxFileViewer.Test.Services.KeysManagement;

public class KeyReaderMessageLoggerTest
{
    [Theory]
    [InlineData("device_key_4x")]
    [InlineData("eticket_rsa_kek_personalized")]
    [InlineData("eticket_rsa_kek_source")]
    [InlineData("eticket_rsa_kekek_source")]
    [InlineData("save_mac_key")]
    [InlineData("ssl_rsa_kek_personalized")]
    [InlineData("ssl_rsa_kek_source")]
    [InlineData("ssl_rsa_kekek_source")]
    [InlineData("ssl_rsa_key")]
    [InlineData("package1_kek_08")]
    [InlineData("package1_mac_kek_08")]
    public void UnknownKeyIsInformationalAndOnlyNamesTheUnusedKey(string key)
    {
        var logger = new CaptureLogger();
        KeyReaderMessageLogger.Log(logger, "Failed to match key " + key, new LocalizationKeys_DE().KeysLoading_UnusedKey_Log);
        Assert.Equal(LogLevel.Information, logger.Level);
        Assert.Equal($"Hinweis: Zusätzlicher Schlüssel «{key}» wird von dieser Programmversion nicht verwendet.", logger.Message);
    }

    [Theory]
    [InlineData("Invalid line in key data: invalid test line")]
    [InlineData("Key has invalid length")]
    [InlineData("Failed to match key device_key_4x: invalid value")]
    public void OtherDiagnosticsRemainWarnings(string message)
    {
        var logger = new CaptureLogger();
        KeyReaderMessageLogger.Log(logger, message, new LocalizationKeys_DE().KeysLoading_UnusedKey_Log);
        Assert.Equal(LogLevel.Warning, logger.Level);
        Assert.Equal(message, logger.Message);
    }

    private sealed class CaptureLogger : ILogger
    {
        public LogLevel Level { get; private set; }
        public string? Message { get; private set; }
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel level) => true;
        public void Log<TState>(LogLevel level, EventId id, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            Level = level;
            Message = formatter(state, exception);
        }
    }
}

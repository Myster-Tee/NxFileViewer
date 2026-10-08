using System;
using System.IO;
using Emignatik.NxFileViewer.Services.KeysManagement;
using Xunit;

namespace Emignatik.NxFileViewer.Test.Services.KeysManagement;

public class KeyFileValidatorTest : IDisposable
{
    private readonly string _tempDirectory = Path.Combine(Path.GetTempPath(), $"NxFileViewer.KeyTests.{Guid.NewGuid():N}");

    public KeyFileValidatorTest() => Directory.CreateDirectory(_tempDirectory);

    [Fact]
    public void ValidateProdKeys_ReportsMissingAndInvalidMasterKeys()
    {
        var path = WriteFile("prod.keys", "master_key_00 = 00000000000000000000000000000000\n");

        var result = KeyFileValidator.ValidateProdKeys(path);

        Assert.False(result.IsValid);
        Assert.Contains("master_key_00", result.InvalidKeys);
        Assert.Contains("master_key_15", result.MissingKeys);
        Assert.Contains("master_key_16", result.MissingKeys);
        Assert.DoesNotContain("master_key_00", result.MissingKeys);
    }

    [Fact]
    public void ValidateProdKeys_ReportsMalformedLines()
    {
        var path = WriteFile("prod.keys", "master_key_00 = not-hex\n# comment\n");

        var result = KeyFileValidator.ValidateProdKeys(path);

        Assert.Contains(1, result.InvalidLineNumbers);
    }

    [Fact]
    public void ValidateProdKeys_ReportsUnsupportedMasterKeyRevisions()
    {
        var path = WriteFile("future-prod.keys", "master_key_17 = 00112233445566778899AABBCCDDEEFF\n");

        var result = KeyFileValidator.ValidateProdKeys(path);

        Assert.True(result.HasWarnings);
        Assert.Contains("master_key_17", result.UnsupportedMasterKeys!);
        Assert.DoesNotContain("master_key_17", result.InvalidKeys);
    }

    [Fact]
    public void ValidateProdKeys_RecognizesRevision16AndItsFirmware()
    {
        // Artificial CRC32 collision starting with "TEST", not a cryptographic key.
        var path = WriteFile("synthetic.keys", "master_key_16 = 54455354000000000000000063c1e62c\n");
        var result = KeyFileValidator.ValidateProdKeys(path);
        Assert.DoesNotContain("master_key_16", result.InvalidKeys);
        Assert.DoesNotContain("master_key_16", result.MissingKeys);
        Assert.DoesNotContain("master_key_16", result.UnsupportedMasterKeys!);
        Assert.Equal(0x16, result.HighestValidMasterKeyRevision);
        Assert.Equal("23.0.0", MasterKeyFirmwareMap.GetSupportedFirmware(result.HighestValidMasterKeyRevision!.Value));
    }

    [Fact]
    public void ValidateProdKeys_RejectsIncorrectRevision16Checksum()
    {
        var path = WriteFile("invalid.keys", "master_key_16 = 00000000000000000000000000000000\n");
        var result = KeyFileValidator.ValidateProdKeys(path);
        Assert.Contains("master_key_16", result.InvalidKeys);
        Assert.DoesNotContain("master_key_16", result.UnsupportedMasterKeys!);
        Assert.Null(result.HighestValidMasterKeyRevision);
    }

    [Fact]
    public void ValidateTitleKeys_AcceptsRightsIdAndTitleKeyPairs()
    {
        var path = WriteFile("title.keys", "00112233445566778899AABBCCDDEEFF = FFEEDDCCBBAA99887766554433221100\n");

        var result = KeyFileValidator.ValidateTitleKeys(path);

        Assert.True(result.IsValid);
        Assert.Equal(1, result.ValidEntryCount);
    }

    [Fact]
    public void ValidateTitleKeys_ReportsMalformedLinesAndEmptyFiles()
    {
        var malformed = KeyFileValidator.ValidateTitleKeys(WriteFile("bad-title.keys", "0011 = AABB\n"));
        var empty = KeyFileValidator.ValidateTitleKeys(WriteFile("empty-title.keys", "# empty\n"));

        Assert.False(malformed.IsValid);
        Assert.Contains(1, malformed.InvalidLineNumbers);
        Assert.False(empty.IsValid);
    }

    [Fact]
    public void ValidateKeys_ReportsMissingFilesWithoutThrowing()
    {
        Assert.False(KeyFileValidator.ValidateProdKeys(Path.Combine(_tempDirectory, "missing.keys")).FileExists);
        Assert.False(KeyFileValidator.ValidateTitleKeys(null).FileExists);
    }

    [Theory]
    [InlineData(0x13, "20.0.0")]
    [InlineData(0x14, "21.0.0")]
    [InlineData(0x15, "22.0.0")]
    [InlineData(0x16, "23.0.0")]
    public void FirmwareMap_MapsRecentMasterKeyRevisions(int revision, string firmware)
    {
        Assert.Equal(firmware, MasterKeyFirmwareMap.GetSupportedFirmware(revision));
    }

    private string WriteFile(string name, string contents)
    {
        var path = Path.Combine(_tempDirectory, name);
        File.WriteAllText(path, contents);
        return path;
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDirectory))
            Directory.Delete(_tempDirectory, recursive: true);
    }
}

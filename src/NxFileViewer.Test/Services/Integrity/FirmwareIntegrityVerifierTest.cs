using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading;
using Emignatik.NxFileViewer.Models.Overview;
using Emignatik.NxFileViewer.Services.Integrity;
using Xunit;

namespace Emignatik.NxFileViewer.Test.Services.Integrity;

public sealed class FirmwareIntegrityVerifierTest : IDisposable
{
    private readonly string _root = Path.Combine(Path.GetTempPath(), "NxFirmwareTest-" + Guid.NewGuid());
    private readonly FirmwareIntegrityVerifier _verifier;
    private readonly string _folder;
    public FirmwareIntegrityVerifierTest()
    {
        var refs = Directory.CreateDirectory(Path.Combine(_root, "refs")).FullName;
        _folder = Directory.CreateDirectory(Path.Combine(_root, "firmware")).FullName;
        var a = new byte[] { 1, 2, 3 }; var b = new byte[] { 4, 5 };
        File.WriteAllBytes(Path.Combine(_folder, "a.nca"), a);
        File.WriteAllBytes(Path.Combine(_folder, "b.cnmt.nca"), b);
        File.WriteAllText(Path.Combine(refs, "test.json"), JsonSerializer.Serialize(new {
            name = "test", file_count = 2, files = new System.Collections.Generic.Dictionary<string, object> {
                ["a.nca"] = new { size = a.Length, sha256 = Convert.ToHexString(SHA256.HashData(a)) },
                ["b.cnmt.nca"] = new { size = b.Length, sha256 = Convert.ToHexString(SHA256.HashData(b)) }
            }
        }));
        _verifier = new FirmwareIntegrityVerifier(refs);
    }
    [Fact] public void CompleteFolderMatchesAndIsDetected()
    {
        Assert.True(_verifier.IsFirmwareFolder(_folder));
        var result = _verifier.Verify(_folder, TestContext.Current.CancellationToken);
        Assert.Equal(NcasIntegrity.Original, result.Integrity);
        Assert.True(result.IsFirmware);
        Assert.Equal("test", result.Structure);
    }
    [Fact] public void RepackedZipAndNestedEntriesMatchWithoutExtraction()
    {
        var zip = Path.Combine(_root, "repacked.zip");
        using (var archive = ZipFile.Open(zip, ZipArchiveMode.Create))
            foreach (var file in Directory.GetFiles(_folder)) archive.CreateEntryFromFile(file, "nested/" + Path.GetFileName(file));
        Assert.True(_verifier.IsFirmwareZip(zip));
        Assert.Equal(NcasIntegrity.Original, _verifier.Verify(zip, TestContext.Current.CancellationToken).Integrity);
        Assert.False(Directory.Exists(Path.Combine(_root, "nested")));
    }
    [Theory] [InlineData("missing")] [InlineData("changed")] [InlineData("extra")]
    public void IncompleteOrChangedFirmwareFails(string modification)
    {
        if (modification == "missing") File.Delete(Path.Combine(_folder, "b.cnmt.nca"));
        if (modification == "changed") File.WriteAllBytes(Path.Combine(_folder, "a.nca"), new byte[] { 3, 2, 1 });
        if (modification == "extra") File.WriteAllBytes(Path.Combine(_folder, "extra.nca"), new byte[] { 0 });
        Assert.Equal(NcasIntegrity.Error, _verifier.Verify(_folder, TestContext.Current.CancellationToken).Integrity);
    }
    [Fact] public void DuplicateZipBasenamesFail()
    {
        var zip = Path.Combine(_root, "duplicate.zip");
        using (var archive = ZipFile.Open(zip, ZipArchiveMode.Create))
        {
            foreach (var file in Directory.GetFiles(_folder)) archive.CreateEntryFromFile(file, Path.GetFileName(file));
            archive.CreateEntryFromFile(Path.Combine(_folder, "a.nca"), "other/a.nca");
        }
        Assert.Equal(NcasIntegrity.Error, _verifier.Verify(zip, TestContext.Current.CancellationToken).Integrity);
    }
    [Fact] public void UnknownFilesAreNotDetectedAsFirmware()
    {
        foreach (var file in Directory.GetFiles(_folder)) File.Delete(file);
        File.WriteAllBytes(Path.Combine(_folder, "game.nca"), new byte[] { 1 });
        Assert.False(_verifier.IsFirmwareFolder(_folder));
        Assert.Equal(NcasIntegrity.Error, _verifier.Verify(_folder, TestContext.Current.CancellationToken).Integrity);
    }
    [Fact] public void CancellationPropagates() => Assert.Throws<OperationCanceledException>(() => _verifier.Verify(_folder, new CancellationToken(true)));
    [Theory] [InlineData(false)] [InlineData(true)]
    public void RenamedContentIsReportedOnceWithExpectedFilename(bool zip)
    {
        File.Move(Path.Combine(_folder, "a.nca"), Path.Combine(_folder, "typo.nca"));
        var path = _folder;
        if (zip)
        {
            path = Path.Combine(_root, "renamed.zip");
            using var archive = ZipFile.Open(path, ZipArchiveMode.Create);
            foreach (var file in Directory.GetFiles(_folder)) archive.CreateEntryFromFile(file, "nested/" + Path.GetFileName(file));
        }
        var result = _verifier.Verify(path, TestContext.Current.CancellationToken);
        var keys = Emignatik.NxFileViewer.Localization.LocalizationManager.Instance.Current.Keys;
        Assert.Equal("test", result.Structure);
        Assert.Equal(NcasIntegrity.Error, result.Integrity); // Correct contents still need the expected names.
        Assert.Contains(keys.Firmware_Renamed + ": typo.nca → a.nca", result.FirmwareDetails);
        Assert.DoesNotContain(keys.Firmware_Missing + ": a.nca", result.FirmwareDetails);
        Assert.DoesNotContain(keys.Firmware_Extra + ": typo.nca", result.FirmwareDetails);
        Assert.Contains(string.Format(keys.Firmware_RenamedSummary, 1), result.FirmwareDetails);
        Assert.True(File.Exists(Path.Combine(_folder, "typo.nca"))); // Verification does not rename files.
    }
    [Fact] public void EntirelyRenamedSetStillIdentifiesVersion()
    {
        File.Move(Path.Combine(_folder, "a.nca"), Path.Combine(_folder, "first.nca"));
        File.Move(Path.Combine(_folder, "b.cnmt.nca"), Path.Combine(_folder, "second.nca"));
        var result = _verifier.Verify(_folder, TestContext.Current.CancellationToken);
        var keys = Emignatik.NxFileViewer.Localization.LocalizationManager.Instance.Current.Keys;
        Assert.Equal("test", result.Structure);
        Assert.Contains("first.nca → a.nca", result.FirmwareDetails);
        Assert.Contains("second.nca → b.cnmt.nca", result.FirmwareDetails);
        Assert.Contains(string.Format(keys.Firmware_RenamedSummary, 2), result.FirmwareDetails);
        Assert.Equal(NcasIntegrity.Error, result.Integrity);
    }
    [Fact] public void DifferentHashCannotBeClassifiedAsRenamed()
    {
        File.Delete(Path.Combine(_folder, "a.nca"));
        File.WriteAllBytes(Path.Combine(_folder, "typo.nca"), new byte[] { 3, 2, 1 });
        var result = _verifier.Verify(_folder, TestContext.Current.CancellationToken);
        var keys = Emignatik.NxFileViewer.Localization.LocalizationManager.Instance.Current.Keys;
        Assert.Contains(keys.Firmware_Missing + ": a.nca", result.FirmwareDetails);
        Assert.Contains(keys.Firmware_Extra + ": typo.nca", result.FirmwareDetails);
        Assert.DoesNotContain("typo.nca → a.nca", result.FirmwareDetails);
    }
    [Fact] public void OneRenamedFileCannotSatisfyTwoExpectedFilesWithSameContent()
    {
        var bytes = new byte[] { 1, 2, 3 };
        File.Delete(Path.Combine(_folder, "b.cnmt.nca"));
        File.Move(Path.Combine(_folder, "a.nca"), Path.Combine(_folder, "typo.nca"));
        var fingerprint = new { size = bytes.Length, sha256 = Convert.ToHexString(SHA256.HashData(bytes)) };
        var verifier = new FirmwareIntegrityVerifier(new[] { JsonSerializer.Serialize(new {
            name = "shared-content", file_count = 2, files = new System.Collections.Generic.Dictionary<string, object> {
                ["a.nca"] = fingerprint, ["b.nca"] = fingerprint
            }
        }) });
        var result = verifier.Verify(_folder, TestContext.Current.CancellationToken);
        var keys = Emignatik.NxFileViewer.Localization.LocalizationManager.Instance.Current.Keys;
        Assert.Contains("typo.nca → a.nca", result.FirmwareDetails);
        Assert.Contains(keys.Firmware_Missing + ": b.nca", result.FirmwareDetails);
        Assert.Contains(string.Format(keys.Firmware_RenamedSummary, 1), result.FirmwareDetails);
        Assert.Equal(NcasIntegrity.Error, result.Integrity);
    }
    [Fact] public void DuplicateContentWithNoMissingFilenameRemainsExtra()
    {
        File.Copy(Path.Combine(_folder, "a.nca"), Path.Combine(_folder, "copy.nca"));
        var result = _verifier.Verify(_folder, TestContext.Current.CancellationToken);
        var keys = Emignatik.NxFileViewer.Localization.LocalizationManager.Instance.Current.Keys;
        Assert.Contains(keys.Firmware_Extra + ": copy.nca", result.FirmwareDetails);
        Assert.Contains(string.Format(keys.Firmware_RenamedSummary, 0), result.FirmwareDetails);
    }
    [Fact] public void BundledReferencesAreAvailable() => Assert.NotNull(new FirmwareIntegrityVerifier());
    public void Dispose() => Directory.Delete(_root, true);
}

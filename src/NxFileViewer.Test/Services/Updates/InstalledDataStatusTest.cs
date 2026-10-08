using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Emignatik.NxFileViewer.Services.Updates;
using Xunit;

namespace Emignatik.NxFileViewer.Test.Services.Updates;

public sealed class InstalledDataStatusTest : IDisposable
{
    private readonly string _root = Path.Combine(Path.GetTempPath(), "InstalledDataStatus-" + Guid.NewGuid());
    private static string Manifest(string version) => JsonSerializer.Serialize(new { name = version, tag = version, file_count = 1,
        files = new Dictionary<string, object> { ["test.nca"] = new { size = 1, sha256 = new string('a', 64) } } });

    [Fact]
    public void FirmwareVersionsAreComparedNumericallyAndInvalidReferencesAreIgnored()
    {
        Assert.Equal("23.0.0", InstalledDataStatus.HighestFirmwareVersion(new[] { Manifest("9.0.0"), Manifest("23.0.0"), "{}", "invalid" }));
    }
    [Fact]
    public void LocalStatusHandlesMissingDirectoryAndReadsInstalledReferences()
    {
        Assert.Null(InstalledDataStatus.LocalFirmwareVersion(_root));
        var directory = Path.Combine(_root, "fw", "hashes");
        Directory.CreateDirectory(directory);
        File.WriteAllText(Path.Combine(directory, "22.json"), Manifest("22.5.0r3"));
        Assert.Equal("22.5.0", InstalledDataStatus.LocalFirmwareVersion(_root));
    }
    [Fact]
    public void TitleDbReportsLocalTimestampAndRejectsPathsOutsideCatalog()
    {
        Assert.Null(InstalledDataStatus.TitleDbDate(_root, "DE.de"));
        var directory = Path.Combine(_root, "Cache", "TitleDB");
        Directory.CreateDirectory(directory);
        var path = Path.Combine(directory, "DE.de.json");
        File.WriteAllText(path, "{}");
        File.SetLastWriteTimeUtc(path, new DateTime(2026, 10, 7, 10, 0, 0, DateTimeKind.Utc));
        Assert.Equal(File.GetLastWriteTimeUtc(path), InstalledDataStatus.TitleDbDate(_root, "DE.de"));
        Assert.Null(InstalledDataStatus.TitleDbDate(_root, "../../outside"));
    }
    public void Dispose() { if (Directory.Exists(_root)) Directory.Delete(_root, true); }
}

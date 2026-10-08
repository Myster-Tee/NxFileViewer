using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using Emignatik.NxFileViewer.Services.Integrity;
using Xunit;

namespace Emignatik.NxFileViewer.Test.Services.Integrity;

public sealed class FirmwareReferenceStoreTest : IDisposable
{
    private readonly string _root = Path.Combine(Path.GetTempPath(), "FirmwareStoreTest-" + Guid.NewGuid());
    private static string Manifest => JsonSerializer.Serialize(new { name = "new", file_count = 1,
        files = new Dictionary<string, object> { ["new.nca"] = new { size = 1, sha256 = new string('a',64) } } });
    public FirmwareReferenceStoreTest()
    {
        Directory.CreateDirectory(Path.Combine(_root, "fw", "hashes"));
        File.WriteAllText(Path.Combine(_root, "fw", "hashes", "old.json"), "previous data");
        File.WriteAllText(Path.Combine(_root, "prod.keys"), "private data");
    }
    [Fact]
    public void ManualUpdatePublishesValidatedSetAndKeepsPreviousFolder()
    {
        FirmwareReferenceStore.Save(new Dictionary<string,string> { ["new.json"] = Manifest }, _root, TestContext.Current.CancellationToken);
        Assert.Equal(Manifest, File.ReadAllText(Path.Combine(_root, "fw", "hashes", "new.json")));
        Assert.False(File.Exists(Path.Combine(_root, "fw", "hashes", "old.json")));
        var backup = Assert.Single(Directory.GetDirectories(Path.Combine(_root, "fw"), "hashes-backup-*"));
        Assert.Equal("previous data", File.ReadAllText(Path.Combine(backup, "old.json")));
        Assert.Equal("private data", File.ReadAllText(Path.Combine(_root, "prod.keys")));
    }
    [Fact]
    public void CancellationKeepsPreviousSet()
    {
        Assert.Throws<OperationCanceledException>(() => FirmwareReferenceStore.Save(
            new Dictionary<string,string> { ["new.json"] = Manifest }, _root, new CancellationToken(true)));
        Assert.Equal("previous data", File.ReadAllText(Path.Combine(_root, "fw", "hashes", "old.json")));
        Assert.Empty(Directory.GetDirectories(Path.Combine(_root, "fw"), ".hashes-stage-*"));
    }
    [Fact]
    public void InvalidReferencesCannotReplacePreviousSet()
    {
        Assert.ThrowsAny<Exception>(() => FirmwareReferenceStore.Save(
            new Dictionary<string,string> { ["bad.json"] = "{}" }, _root, TestContext.Current.CancellationToken));
        Assert.Equal("previous data", File.ReadAllText(Path.Combine(_root, "fw", "hashes", "old.json")));
    }
    [Fact]
    public void ManifestNamesCannotEscapeOfflineFolder()
    {
        Assert.Throws<InvalidDataException>(() => FirmwareReferenceStore.Save(
            new Dictionary<string,string> { ["../prod.keys"] = Manifest }, _root, TestContext.Current.CancellationToken));
        Assert.Equal("private data", File.ReadAllText(Path.Combine(_root, "prod.keys")));
    }
    public void Dispose() => Directory.Delete(_root, true);
}

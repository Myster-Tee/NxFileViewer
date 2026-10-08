using System;
using System.IO;
using Emignatik.NxFileViewer.Services.KeysManagement;
using Emignatik.NxFileViewer.Settings;
using Xunit;

namespace Emignatik.NxFileViewer.Test.Services.KeysManagement;

public sealed class TicketKeyStoreTest : IDisposable
{
    private readonly string _root = Path.Combine(Path.GetTempPath(), "TicketKeyStore-" + Guid.NewGuid());
    private const string Id = "00000000000000000000000000000001";
    private const string Key = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
    [Fact]
    public void CreatesFileAndDoesNotDuplicateExistingEntry()
    {
        var path = Path.Combine(_root, "title.keys");
        Assert.Equal(TicketKeySaveResult.Added, TicketKeyStore.AddMissing(path, Id, Key));
        var text = File.ReadAllText(path);
        Assert.Equal(TicketKeySaveResult.AlreadyPresent, TicketKeyStore.AddMissing(path, Id, Key.ToUpperInvariant()));
        Assert.Equal(text, File.ReadAllText(path));
    }
    [Fact]
    public void PreservesCommentsExistingContentAndConflictingKey()
    {
        Directory.CreateDirectory(_root);
        var path = Path.Combine(_root, "title.keys");
        var text = "# comment\r\n" + Id + " = " + Key + " # source";
        File.WriteAllText(path, text);
        Assert.Equal(TicketKeySaveResult.Conflict, TicketKeyStore.AddMissing(path, Id, new string('b', 32)));
        Assert.Equal(text, File.ReadAllText(path));
        Assert.Equal(TicketKeySaveResult.Added, TicketKeyStore.AddMissing(path, new string('2', 32), Key));
        Assert.StartsWith(text + "\r\n", File.ReadAllText(path));
        Assert.Single(Directory.GetFiles(_root));
    }
    [Fact]
    public void DisabledByDefaultAndRejectsMalformedKeyWithoutWriting()
    {
        Assert.False(new AppSettings().SaveTicketKeys);
        Assert.Throws<ArgumentException>(() => TicketKeyStore.AddMissing(Path.Combine(_root, "title.keys"), Id, "invalid"));
        Assert.False(Directory.Exists(_root));
    }
    public void Dispose() { if (Directory.Exists(_root)) Directory.Delete(_root, true); }
}

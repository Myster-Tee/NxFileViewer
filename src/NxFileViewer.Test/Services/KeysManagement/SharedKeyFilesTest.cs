using System;
using System.IO;
using Emignatik.NxFileViewer.Services.KeysManagement;
using Xunit;

namespace Emignatik.NxFileViewer.Test.Services.KeysManagement;

public sealed class SharedKeyFilesTest : IDisposable
{
    private readonly string _root = Path.Combine(Path.GetTempPath(), "SharedKeysTest-" + Guid.NewGuid());
    [Fact]
    public void CopiesWithoutDeletingSourceAndRequiresOverwriteForExistingTarget()
    {
        Directory.CreateDirectory(_root);
        var source = Path.Combine(_root, "source.keys");
        var target = Path.Combine(_root, ".switch", "prod.keys");
        File.WriteAllText(source, "synthetic test data");
        Assert.True(SharedKeyFiles.Copy(source, target, false));
        Assert.True(File.Exists(source));
        File.WriteAllText(source, "updated synthetic data");
        Assert.Throws<IOException>(() => SharedKeyFiles.Copy(source, target, false));
        Assert.Equal("synthetic test data", File.ReadAllText(target));
        Assert.True(SharedKeyFiles.Copy(source, target, true));
        Assert.Equal("updated synthetic data", File.ReadAllText(target));
        Assert.False(SharedKeyFiles.Copy(target, target, true));
        Assert.Single(Directory.GetFiles(Path.GetDirectoryName(target)!));
    }
    public void Dispose() { if (Directory.Exists(_root)) Directory.Delete(_root, true); }
}

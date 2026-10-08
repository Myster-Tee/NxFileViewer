using System;
using System.IO;
using Emignatik.NxFileViewer.Services;
using Xunit;

namespace Emignatik.NxFileViewer.Test.Services;

public sealed class EmptyDirectoryCleanupTest : IDisposable
{
    private readonly string _root = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "EmptyCleanup-" + Guid.NewGuid())).FullName;
    [Fact]
    public void RemovesNestedEmptyDirectoriesAndPreservesFilesAndRoot()
    {
        Directory.CreateDirectory(Path.Combine(_root, "Temp", "ZIP", "empty"));
        Directory.CreateDirectory(Path.Combine(_root, "Logs", "empty"));
        File.WriteAllText(Path.Combine(_root, "Logs", "session.log"), "keep");
        Assert.Equal(4, EmptyDirectoryCleanup.Clean(_root));
        Assert.True(Directory.Exists(_root));
        Assert.Equal("keep", File.ReadAllText(Path.Combine(_root, "Logs", "session.log")));
        Assert.Equal(0, EmptyDirectoryCleanup.Clean(_root));
    }
    [Fact]
    public void PreservesDevelopmentAndPrivateTestDirectories()
    {
        Directory.CreateDirectory(Path.Combine(_root, ".git", "empty"));
        Directory.CreateDirectory(Path.Combine(_root, "test", "empty"));
        Assert.Equal(0, EmptyDirectoryCleanup.Clean(_root));
        Assert.True(Directory.Exists(Path.Combine(_root, "test", "empty")));
    }
    [Fact]
    public void MissingRootIsHarmless() => Assert.Equal(0, EmptyDirectoryCleanup.Clean(Path.Combine(_root, "missing")));
    public void Dispose() => Directory.Delete(_root, true);
}

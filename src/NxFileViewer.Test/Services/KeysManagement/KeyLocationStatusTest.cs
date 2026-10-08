using System;
using System.IO;
using Emignatik.NxFileViewer.Views.Windows;
using Xunit;

namespace Emignatik.NxFileViewer.Test.Services.KeysManagement;

public sealed class KeyLocationStatusTest : IDisposable
{
    private readonly string _root = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "KeyLocations-" + Guid.NewGuid())).FullName;

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ReportsAllLocationsIndependentlyWithoutChangingActiveSelection(bool titleKeys)
    {
        var program = Path.Combine(_root, "program.keys");
        var shared = Path.Combine(_root, "shared.keys");
        var custom = Path.Combine(_root, "custom.keys");
        File.WriteAllText(program, "invalid synthetic line");
        File.WriteAllText(shared, "");
        File.WriteAllText(custom, titleKeys
            ? "00000000000000000000000000000012 = 00000000000000000000000000000000"
            : "header_key = " + new string('0', 64));
        var results = KeyLocationStatus.Inspect(program, shared, custom, shared, titleKeys);
        Assert.Equal(3, results.Count);
        Assert.NotEmpty(results[0].Validation.InvalidLineNumbers);
        Assert.Empty(results[1].Validation.InvalidLineNumbers);
        Assert.Equal(0, results[1].Validation.ValidEntryCount);
        Assert.Equal(1, results[2].Validation.ValidEntryCount);
        Assert.False(results[0].IsActive);
        Assert.True(results[1].IsActive);
        Assert.False(results[2].IsActive);
        File.Delete(shared);
        Assert.False(KeyLocationStatus.Inspect(program, shared, custom, null, titleKeys)[1].Validation.FileExists);
    }

    [Fact]
    public void UnsetCustomPathIsOmittedButMissingDefaultPathsRemainVisible()
    {
        var result = KeyLocationStatus.Inspect(Path.Combine(_root, "prod.keys"), Path.Combine(_root, "home.keys"), "", null, false);
        Assert.Equal(2, result.Count);
        Assert.All(result, location => Assert.False(location.Validation.FileExists));
        var configured = KeyLocationStatus.Inspect(result[0].FilePath, result[1].FilePath, Path.Combine(_root, "missing-custom.keys"), null, false);
        Assert.Equal(3, configured.Count);
        Assert.False(configured[2].Validation.FileExists);
    }

    public void Dispose() => Directory.Delete(_root, true);
}

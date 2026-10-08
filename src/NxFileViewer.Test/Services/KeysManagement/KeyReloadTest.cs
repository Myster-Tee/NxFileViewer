using System;
using System.IO;
using Emignatik.NxFileViewer.Services.KeysManagement;
using Emignatik.NxFileViewer.Settings;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Emignatik.NxFileViewer.Test.Services.KeysManagement;

public sealed class KeyReloadTest : IDisposable
{
    private readonly string _root = Path.Combine(Path.GetTempPath(), "KeyReload-" + Guid.NewGuid());

    [Fact]
    public void ReloadDiscoversNewProgramKeysAndRevalidatesWithoutRestart()
    {
        var app = Directory.CreateDirectory(Path.Combine(_root, "app")).FullName;
        var home = Directory.CreateDirectory(Path.Combine(_root, "home")).FullName;
        var shared = Directory.CreateDirectory(Path.Combine(home, ".switch")).FullName;
        foreach (var name in new[] { "prod.keys", "title.keys" }) File.WriteAllText(Path.Combine(shared, name), "");
        var service = new KeySetProviderService(new AppSettings(), NullLoggerFactory.Instance, app, home);
        var original = service.GetKeySet();
        Assert.Equal(Path.Combine(shared, "prod.keys"), service.ActualProdKeysFilePath);
        Assert.Equal(Path.Combine(shared, "title.keys"), service.ActualTitleKeysFilePath);
        File.WriteAllText(Path.Combine(app, "prod.keys"), "malformed synthetic line");
        File.WriteAllText(Path.Combine(app, "title.keys"), "malformed synthetic line");
        Assert.Same(original, service.GetKeySet());
        Assert.NotSame(original, service.GetKeySet(forceReload: true));
        Assert.Equal(Path.Combine(app, "prod.keys"), service.ActualProdKeysFilePath);
        Assert.Equal(Path.Combine(app, "title.keys"), service.ActualTitleKeysFilePath);
        Assert.NotEmpty(service.ProdKeysValidation.InvalidLineNumbers);
        Assert.NotEmpty(service.TitleKeysValidation.InvalidLineNumbers);
        File.Delete(Path.Combine(app, "prod.keys"));
        File.Delete(Path.Combine(app, "title.keys"));
        service.GetKeySet(forceReload: true);
        Assert.Equal(Path.Combine(shared, "prod.keys"), service.ActualProdKeysFilePath);
        Assert.Empty(service.ProdKeysValidation.InvalidLineNumbers);
    }

    [Fact]
    public void ExplicitSettingsPathRetainsPriorityOnReload()
    {
        var app = Directory.CreateDirectory(Path.Combine(_root, "app")).FullName;
        var home = Directory.CreateDirectory(Path.Combine(_root, "home")).FullName;
        var custom = Path.Combine(_root, "custom.keys");
        File.WriteAllText(custom, "");
        foreach (var name in new[] { "prod.keys", "title.keys" }) File.WriteAllText(Path.Combine(app, name), "");
        var settings = new AppSettings { ProdKeysFilePath = custom, TitleKeysFilePath = custom };
        var service = new KeySetProviderService(settings, NullLoggerFactory.Instance, app, home);
        service.GetKeySet(forceReload: true);
        Assert.Equal(custom, service.ActualProdKeysFilePath);
        Assert.Equal(custom, service.ActualTitleKeysFilePath);
    }

    public void Dispose() { if (Directory.Exists(_root)) Directory.Delete(_root, true); }
}

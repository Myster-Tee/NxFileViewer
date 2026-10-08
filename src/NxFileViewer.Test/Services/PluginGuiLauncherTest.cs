using System;
using System.IO;
using Emignatik.NxFileViewer.Services;
using Emignatik.NxFileViewer.Services.Nsz;
using Emignatik.NxFileViewer.Settings;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Emignatik.NxFileViewer.Test.Services;

public sealed class PluginGuiLauncherTest
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void LaunchUsesExecutableDirectoryAndGuiArguments(bool nand)
    {
        var path = Path.Combine(Path.GetTempPath(), "plugin with spaces", "plugin.exe");
        var info = PluginGuiLauncher.CreateStartInfo(path, nand);
        Assert.Equal(path, info.FileName);
        Assert.Equal(Path.GetDirectoryName(path), info.WorkingDirectory);
        Assert.True(info.UseShellExecute);
        Assert.False(info.RedirectStandardOutput);
        Assert.Equal(nand ? new[] { "--gui" } : Array.Empty<string>(), info.ArgumentList);
    }

    [Fact]
    public void NszGuiRequiresAnExistingGuiBuild()
    {
        var root = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        try
        {
            var settings = new AppSettings();
            var manager = new NszPluginManager(settings, NullLogger<NszPluginManager>.Instance, root);
            Assert.Empty(manager.GuiExecutablePath);
            settings.NszExecutablePath = Path.Combine(root, "nsz-cli-windows-x64.exe");
            File.WriteAllText(settings.NszExecutablePath, "fixture");
            Assert.Empty(manager.GuiExecutablePath);
            settings.NszExecutablePath = Path.Combine(root, "nsz-gui-windows-x64.exe");
            Assert.Empty(manager.GuiExecutablePath);
            File.WriteAllText(settings.NszExecutablePath, "fixture");
            Assert.Equal(settings.NszExecutablePath, manager.GuiExecutablePath);
        }
        finally { Directory.Delete(root, true); }
    }
}

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Emignatik.NxFileViewer.Services.BackgroundTask;
using Emignatik.NxFileViewer.Services.Nand;
using Emignatik.NxFileViewer.Services.KeysManagement;
using Microsoft.Extensions.Logging.Abstractions;
using Emignatik.NxFileViewer.Settings;
using Xunit;

namespace Emignatik.NxFileViewer.Test.Services.Nand;

public sealed class NandCliPluginTest : IDisposable
{
    private readonly string _directory = Path.Combine(Path.GetTempPath(), "nxfv-nand-test-" + Guid.NewGuid().ToString("N"));
    private readonly string _source;
    private readonly AppSettings _settings;
    private readonly Progress _progress = new();
    public NandCliPluginTest()
    {
        Directory.CreateDirectory(_directory);
        _source = Path.Combine(_directory, "source.bin");
        File.WriteAllText(_source, "source unchanged");
        _settings = new AppSettings { NandExecutablePath = _source };
    }

    [Fact]
    public void PartitionsComeFromStorageTypeAndPartitionRowsRatherThanPathsOrMessages()
    {
        var info = "NAND type      : RAWNAND\r\nPath : C:\\USER\\BOOT0.bin\r\nPartitions :\r\n - PRODINFO - 3 MiB encrypted\r\n - SYSTEM - 2 GiB\r\n - BCPKG2-1-Normal-Main - 8 MiB\r\nMissing USER keys";
        Assert.Equal(new[] { "PRODINFO", "SYSTEM", "BCPKG2-1-Normal-Main" }, NandCliPlugin.FindPartitions(info));
        Assert.Equal(new[] { "BOOT0" }, NandCliPlugin.FindPartitions("NAND type : BOOT0\n"));
    }

    [Fact]
    public void InformationUsesSeparateArgumentsAndOptionalBisKeyFile()
    {
        _settings.NandBisKeysPath = _source;
        var process = new FakeProcess((start, _) =>
        {
            Assert.Equal(new[] { "-i", _source, "--info", "-keyset", _source }, start.ArgumentList);
            Assert.False(start.UseShellExecute);
            return "NAND type : BOOT0";
        });
        Assert.Equal("NAND type : BOOT0", new NandCliPlugin(_settings, process).ReadInformation(_source, _progress, CancellationToken.None));
    }

    [Theory]
    [InlineData(true, false)]
    [InlineData(true, true)]
    [InlineData(false, false)]
    public void ActiveProdKeysProvidesBisKeysUnlessOverriddenAndBasicInfoNeedsNoBisKeys(bool hasBisKeys, bool customOverride)
    {
        var prodKeys = Path.Combine(_directory, "prod.keys");
        File.WriteAllText(prodKeys, hasBisKeys ? "bis_key_02 = " + new string('1', 64) : "master_key_00 = " + new string('1', 32));
        _settings.ProdKeysFilePath = prodKeys;
        if (customOverride) _settings.NandBisKeysPath = _source;
        var provider = new KeySetProviderService(_settings, NullLoggerFactory.Instance, _directory, _directory);
        var process = new FakeProcess((start, _) =>
        {
            if (hasBisKeys || customOverride)
                Assert.Equal(new[] { "-i", _source, "--info", "-keyset", customOverride ? _source : provider.ActualProdKeysFilePath! }, start.ArgumentList);
            else Assert.Equal(new[] { "-i", _source, "--info" }, start.ArgumentList);
            return "NAND type : BOOT0";
        });
        var plugin = new NandCliPlugin(_settings, process, keyProvider: provider);
        plugin.ReadInformation(_source, _progress, CancellationToken.None);
        // Changing the viewer's selected file affects the next request without recreating the plugin.
        var secondKeys = Path.Combine(_directory, "second.keys");
        File.Copy(prodKeys, secondKeys);
        _settings.ProdKeysFilePath = secondKeys;
        Assert.Equal(secondKeys, provider.ActualProdKeysFilePath);
        plugin.ReadInformation(_source, _progress, CancellationToken.None);
    }

    [Fact]
    public void ExportPublishesOnlySuccessfulNonEmptyOutputAndPreservesSource()
    {
        var target = Path.Combine(_directory, "partition.bin");
        var process = new FakeProcess((start, _) =>
        {
            Assert.Equal("-part=SYSTEM", start.ArgumentList.Last());
            Assert.DoesNotContain("FORCE", start.ArgumentList);
            Assert.DoesNotContain("-d", start.ArgumentList);
            var temporary = start.ArgumentList[3];
            Assert.NotEqual(target, temporary);
            File.WriteAllText(temporary, "partition data");
            Assert.False(File.Exists(target));
            return "done";
        });
        new NandCliPlugin(_settings, process).Export(_source, target, "SYSTEM", _progress, CancellationToken.None);
        Assert.Equal("partition data", File.ReadAllText(target));
        Assert.Equal("source unchanged", File.ReadAllText(_source));
        Assert.Empty(Directory.GetDirectories(_directory));
    }

    [Theory]
    [InlineData("failure")]
    [InlineData("cancel")]
    [InlineData("empty")]
    [InlineData("collision")]
    public void FailedOrCancelledExportsDiscardTemporaryDataAndNeverReplaceTargets(string mode)
    {
        var target = Path.Combine(_directory, "partition.bin");
        var process = new FakeProcess((start, _) =>
        {
            File.WriteAllText(start.ArgumentList[3], mode == "empty" ? "" : "partial data");
            if (mode == "failure") throw new IOException("failure");
            if (mode == "cancel") throw new OperationCanceledException();
            if (mode == "collision") File.WriteAllText(target, "existing data");
            return "done";
        });
        Assert.ThrowsAny<Exception>(() => new NandCliPlugin(_settings, process).Export(_source, target, "USER", _progress, CancellationToken.None));
        if (mode == "collision") Assert.Equal("existing data", File.ReadAllText(target));
        else Assert.False(File.Exists(target));
        Assert.Empty(Directory.GetDirectories(_directory));
        Assert.Equal("source unchanged", File.ReadAllText(_source));
    }

    [Fact]
    public void ExistingTargetsInvalidPartitionsAndPhysicalSourcesNeverLaunchProcess()
    {
        var process = new FakeProcess((_, _) => throw new Exception("must not launch"));
        var plugin = new NandCliPlugin(_settings, process);
        Assert.Throws<IOException>(() => plugin.Export(_source, _source, "USER", _progress, CancellationToken.None));
        Assert.Throws<ArgumentException>(() => plugin.Export(_source, Path.Combine(_directory, "new.bin"), "USER,PRODINFO", _progress, CancellationToken.None));
        Assert.Throws<IOException>(() => plugin.ReadInformation(@"\\.\PhysicalDrive0", _progress, CancellationToken.None));
        Assert.Equal(0, process.Calls);
    }

    public void Dispose() => Directory.Delete(_directory, true);
    private sealed class FakeProcess(Func<ProcessStartInfo, CancellationToken, string> run) : INandProcessRunner
    {
        public int Calls { get; private set; }
        public string Run(ProcessStartInfo start, IProgressReporter progress, CancellationToken token) { Calls++; return run(start, token); }
    }
    private sealed class Progress : IProgressReporter
    {
        public void SetMode(bool value) { }
        public void SetText(string text) { }
        public void SetPercentage(double value) { }
    }
}

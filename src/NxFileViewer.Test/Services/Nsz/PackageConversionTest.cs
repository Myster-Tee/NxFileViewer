using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Text.Json;
using Emignatik.NxFileViewer.Settings;
using Emignatik.NxFileViewer.Models.Overview;
using Emignatik.NxFileViewer.Services.BackgroundTask;
using Emignatik.NxFileViewer.Services.Integrity;
using Emignatik.NxFileViewer.Services.Nsz;
using Xunit;

namespace Emignatik.NxFileViewer.Test.Services.Nsz;

public sealed class PackageConversionTest : IDisposable
{
    private readonly string _root = Path.Combine(Path.GetTempPath(), "NszConversionTest-" + Guid.NewGuid());
    private readonly FakePlugin _plugin = new();
    private readonly FakeVerifier _verifier = new();
    private readonly Reporter _reporter = new();
    private readonly string _source;
    private readonly string _output;

    public PackageConversionTest()
    {
        Directory.CreateDirectory(_root);
        _source = Path.Combine(_root, "game.nsp");
        _output = Path.Combine(_root, "output");
        File.WriteAllText(_source, "original content");
    }
    private PackageConversionResult Convert() => new PackageConversionService(_plugin, _verifier)
        .Convert(_source, _output, NszOperation.Compress, _reporter, TestContext.Current.CancellationToken);

    [Fact]
    public void SuccessfulOutputIsVerifiedBeforePublishingAndOriginalIsKept()
    {
        _verifier.OnVerify = path =>
        {
            if (path != _source) Assert.False(File.Exists(Path.Combine(_output, "game.nsz")));
        };
        var result = Convert();
        Assert.Equal(new[] { _source, _plugin.StagedOutput! }, _verifier.Paths);
        Assert.Equal("original content", File.ReadAllText(_source));
        Assert.Equal("converted content", File.ReadAllText(result.OutputPath));
        Assert.Equal(result.OutputPath, result.Verification.FilePath);
        Assert.Equal(new FileInfo(_source).Length, result.SourceSize);
        Assert.Equal(new FileInfo(result.OutputPath).Length, result.OutputSize);
        Assert.Empty(Directory.GetDirectories(_output));
    }

    [Theory]
    [InlineData(NcasIntegrity.Corrupted)]
    [InlineData(NcasIntegrity.Modified)]
    [InlineData(NcasIntegrity.Incomplete)]
    [InlineData(NcasIntegrity.Error)]
    [InlineData(NcasIntegrity.NoNca)]
    public void InvalidSourceNeverStartsPlugin(NcasIntegrity integrity)
    {
        _verifier.SourceIntegrity = integrity;
        Assert.Throws<InvalidDataException>(() => Convert());
        Assert.Equal(0, _plugin.Calls);
        Assert.False(Directory.Exists(_output));
        Assert.Equal("original content", File.ReadAllText(_source));
    }

    [Fact]
    public void InvalidOutputIsNotPublishedAndStagingIsRemoved()
    {
        _verifier.OutputIntegrity = NcasIntegrity.Corrupted;
        Assert.Throws<InvalidDataException>(() => Convert());
        Assert.Empty(Directory.GetFileSystemEntries(_output));
        Assert.Equal("original content", File.ReadAllText(_source));
    }

    [Fact]
    public void ExistingTargetIsNotOverwritten()
    {
        Directory.CreateDirectory(_output);
        var target = Path.Combine(_output, "game.nsz");
        File.WriteAllText(target, "existing content");
        Assert.Throws<IOException>(() => Convert());
        Assert.Empty(_verifier.Paths);
        Assert.Equal(0, _plugin.Calls);
        Assert.Equal("existing content", File.ReadAllText(target));
    }

    [Fact]
    public void TargetCreatedDuringConversionIsNotOverwritten()
    {
        _plugin.AfterWrite = () => File.WriteAllText(Path.Combine(_output, "game.nsz"), "other output");
        Assert.Throws<IOException>(() => Convert());
        Assert.Equal("other output", File.ReadAllText(Path.Combine(_output, "game.nsz")));
        Assert.Empty(Directory.GetDirectories(_output));
    }

    private PackageConversionResult ConvertWith(OutputConflictResolution resolution, bool deleteSource = false) =>
        new PackageConversionService(_plugin, _verifier).Convert(_source, _output, NszOperation.Compress,
            _reporter, TestContext.Current.CancellationToken, _ => resolution, deleteSource);

    [Fact]
    public void NumberingSkipsExistingFilesAndDirectories()
    {
        Directory.CreateDirectory(_output);
        File.WriteAllText(Path.Combine(_output, "game.nsz"), "existing");
        Directory.CreateDirectory(Path.Combine(_output, "game (1).nsz"));
        var result = ConvertWith(OutputConflictResolution.Number);
        Assert.Equal(Path.Combine(_output, "game (2).nsz"), result.OutputPath);
        Assert.Equal("converted content", File.ReadAllText(result.OutputPath));
        Assert.Equal("existing", File.ReadAllText(Path.Combine(_output, "game.nsz")));
    }

    [Fact]
    public void ReplacementWaitsForVerification()
    {
        Directory.CreateDirectory(_output);
        var target = Path.Combine(_output, "game.nsz");
        File.WriteAllText(target, "existing");
        _verifier.OnVerify = _ => Assert.Equal("existing", File.ReadAllText(target));
        ConvertWith(OutputConflictResolution.Replace);
        Assert.Equal("converted content", File.ReadAllText(target));
    }

    [Fact]
    public void FailedVerificationPreservesExistingTargetAndSource()
    {
        Directory.CreateDirectory(_output);
        var target = Path.Combine(_output, "game.nsz");
        File.WriteAllText(target, "existing");
        _verifier.OutputIntegrity = NcasIntegrity.Corrupted;
        Assert.Throws<InvalidDataException>(() => ConvertWith(OutputConflictResolution.Replace, true));
        Assert.Equal("existing", File.ReadAllText(target));
        Assert.True(File.Exists(_source));
    }

    [Fact]
    public void CancellationAtConflictDoesNotStartConversion()
    {
        Directory.CreateDirectory(_output);
        File.WriteAllText(Path.Combine(_output, "game.nsz"), "existing");
        Assert.Throws<OperationCanceledException>(() => ConvertWith(OutputConflictResolution.Cancel, true));
        Assert.Equal(0, _plugin.Calls);
        Assert.True(File.Exists(_source));
    }

    [Fact]
    public void NewlyCreatedTargetOffersNumberingAfterVerification()
    {
        _plugin.AfterWrite = () => File.WriteAllText(Path.Combine(_output, "game.nsz"), "other output");
        var result = ConvertWith(OutputConflictResolution.Number);
        Assert.EndsWith("game (1).nsz", result.OutputPath);
        Assert.Equal("other output", File.ReadAllText(Path.Combine(_output, "game.nsz")));
        Assert.Equal(1, _plugin.Calls);
    }

    [Fact]
    public void DeletesSourceOnlyAfterSuccessfulPublication()
    {
        _verifier.OnVerify = _ => Assert.True(File.Exists(_source));
        var result = ConvertWith(OutputConflictResolution.Number, true);
        Assert.True(result.SourceDeleted);
        Assert.Null(result.SourceDeletionError);
        Assert.False(File.Exists(_source));
        Assert.Equal("converted content", File.ReadAllText(result.OutputPath));
        Assert.Equal("original content".Length, result.SourceSize);
    }

    [Fact]
    public void LockedSourceDeletionReportsWarningAndKeepsVerifiedOutput()
    {
        using var lockFile = new FileStream(_source, FileMode.Open, FileAccess.Read, FileShare.Read);
        var result = ConvertWith(OutputConflictResolution.Number, true);
        Assert.False(result.SourceDeleted);
        Assert.NotNull(result.SourceDeletionError);
        Assert.True(File.Exists(_source));
        Assert.Equal("converted content", File.ReadAllText(result.OutputPath));
    }

    [Fact]
    public void CancellationRemovesPartialOutput()
    {
        _plugin.AfterWrite = () => throw new OperationCanceledException();
        Assert.Throws<OperationCanceledException>(() => Convert());
        Assert.Empty(Directory.GetFileSystemEntries(_output));
        Assert.Equal("original content", File.ReadAllText(_source));
    }

    [Fact]
    public void PluginFailureRemovesPartialOutput()
    {
        _plugin.AfterWrite = () => throw new IOException("plugin failed");
        Assert.Throws<IOException>(() => Convert());
        Assert.Empty(Directory.GetFileSystemEntries(_output));
    }

    [Fact]
    public void MissingOutputCannotBeReportedAsSuccess()
    {
        _plugin.WriteOutput = false;
        Assert.Throws<InvalidDataException>(() => Convert());
        Assert.Single(_verifier.Paths);
        Assert.Empty(Directory.GetFileSystemEntries(_output));
    }

    [Theory]
    [InlineData("game.nsp", NszOperation.Compress, ".nsz")]
    [InlineData("game.XCI", NszOperation.Compress, ".xcz")]
    [InlineData("game.nsz", NszOperation.Decompress, ".nsp")]
    [InlineData("game.XCZ", NszOperation.Decompress, ".xci")]
    public void CorrectContainerExtension(string source, NszOperation operation, string expected) =>
        Assert.Equal(expected, PackageConversionService.OutputExtension(source, operation));

    [Fact]
    public void UnsupportedAndWrongDirectionAreRejected()
    {
        Assert.False(PackageConversionService.Supports("firmware.zip", NszOperation.Compress));
        Assert.False(PackageConversionService.Supports("game.nsz", NszOperation.Compress));
        Assert.False(PackageConversionService.Supports("game.nsp", NszOperation.Decompress));
    }

    [Fact]
    public void ArgumentsKeepPathsAsSeparateTokensAndNeverRequestDeletionOrOverwrite()
    {
        var source = Path.Combine(_root, "game & title.nsp");
        var args = NszCliPlugin.BuildArguments(source, "output with spaces", "keys with spaces", NszOperation.Compress, 99);
        Assert.Contains(source, args);
        Assert.Contains("keys with spaces", args);
        Assert.Contains("-K", args);
        Assert.Contains("-V", args);
        Assert.Contains("22", args);
        Assert.DoesNotContain("--rm-source", args);
        Assert.DoesNotContain("-w", args);
        var decompress = NszCliPlugin.BuildArguments(Path.ChangeExtension(source, ".nsz"), _output, "prod.keys", NszOperation.Decompress, 18);
        Assert.Contains("-D", decompress);
        Assert.DoesNotContain("-C", decompress);
    }

    [Theory]
    [InlineData(NszCompressionMode.Auto)]
    [InlineData(NszCompressionMode.Solid)]
    [InlineData(NszCompressionMode.Block)]
    public void CompressionModeUsesExclusiveFlagsAndDetectedKeys(NszCompressionMode mode)
    {
        var keyPath = Path.Combine(_root, "viewer keys", "prod.keys");
        var args = NszCliPlugin.BuildArguments(_source, _output, keyPath, NszOperation.Compress, 18, mode, 22).ToList();
        Assert.Equal(keyPath, args[args.IndexOf("--keys") + 1]);
        Assert.Equal(mode == NszCompressionMode.Solid, args.Contains("--solid"));
        Assert.Equal(mode == NszCompressionMode.Block, args.Contains("--block"));
        Assert.Equal(mode == NszCompressionMode.Block, args.Contains("--bs"));
        if (mode == NszCompressionMode.Block) Assert.Equal("22", args[args.IndexOf("--bs") + 1]);
        Assert.Contains("-K", args);
        Assert.Contains("-V", args);
    }

    [Theory]
    [InlineData(NszCompressionMode.Auto)]
    [InlineData(NszCompressionMode.Solid)]
    [InlineData(NszCompressionMode.Block)]
    public void DecompressionIgnoresCompressionModeAndBlockSize(NszCompressionMode mode)
    {
        var args = NszCliPlugin.BuildArguments("game.nsz", _output, "prod.keys", NszOperation.Decompress, 18, mode, 22);
        Assert.Contains("-D", args);
        Assert.DoesNotContain("--solid", args);
        Assert.DoesNotContain("--block", args);
        Assert.DoesNotContain("--bs", args);
        Assert.DoesNotContain("-l", args);
    }

    [Theory]
    [InlineData(0, "14")]
    [InlineData(20, "20")]
    [InlineData(99, "32")]
    public void BlockSizeExponentIsBounded(int exponent, string expected)
    {
        var args = NszCliPlugin.BuildArguments(_source, _output, "prod.keys", NszOperation.Compress,
            18, NszCompressionMode.Block, exponent).ToList();
        Assert.Equal(expected, args[args.IndexOf("--bs") + 1]);
    }

    [Fact]
    public void CompressionSettingsRoundTripAndOldSettingsKeepUpstreamDefaults()
    {
        var settings = new AppSettings { NszCompressionMode = NszCompressionMode.Block, NszBlockSizeExponent = 22 };
        var restored = JsonSerializer.Deserialize<AppSettings>(JsonSerializer.Serialize(settings))!;
        Assert.Equal(NszCompressionMode.Block, restored.NszCompressionMode);
        Assert.Equal(22, restored.NszBlockSizeExponent);
        var oldSettings = JsonSerializer.Deserialize<AppSettings>("{}")!;
        Assert.Equal(NszCompressionMode.Auto, oldSettings.NszCompressionMode);
        Assert.Equal(20, oldSettings.NszBlockSizeExponent);
        settings.NszCompressionMode = (NszCompressionMode)99;
        Assert.Equal(NszCompressionMode.Auto, settings.NszCompressionMode);
        settings.NszBlockSizeExponent = -1;
        Assert.Equal(14, settings.NszBlockSizeExponent);
    }

    public void Dispose() => Directory.Delete(_root, recursive: true);

    private sealed class FakePlugin : INszPlugin
    {
        public int Calls;
        public string? StagedOutput;
        public bool WriteOutput = true;
        public Action? AfterWrite;
        public void Convert(string source, string outputDirectory, NszOperation operation, IProgressReporter progress, CancellationToken cancellationToken)
        {
            Calls++;
            StagedOutput = Path.Combine(outputDirectory, Path.GetFileNameWithoutExtension(source) + PackageConversionService.OutputExtension(source, operation));
            if (WriteOutput) File.WriteAllText(StagedOutput, "converted content");
            AfterWrite?.Invoke();
        }
    }

    private sealed class FakeVerifier : IConversionVerifier
    {
        public List<string> Paths = new();
        public NcasIntegrity SourceIntegrity = NcasIntegrity.Original;
        public NcasIntegrity OutputIntegrity = NcasIntegrity.Original;
        public Action<string>? OnVerify;
        public BatchIntegrityResult Verify(string path, IProgressReporter progress, CancellationToken cancellationToken)
        {
            Paths.Add(path);
            OnVerify?.Invoke(path);
            return new(path, "NSP", "NSP", "Standard", "None", Paths.Count == 1 ? SourceIntegrity : OutputIntegrity, null);
        }
    }

    private sealed class Reporter : IProgressReporter
    {
        public void SetMode(bool isIndeterminate) { }
        public void SetText(string text) { }
        public void SetPercentage(double value) { }
    }
}

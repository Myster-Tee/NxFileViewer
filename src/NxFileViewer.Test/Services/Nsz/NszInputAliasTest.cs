using System;
using System.IO;
using System.Linq;
using Emignatik.NxFileViewer.Services.Nsz;
using Xunit;

namespace Emignatik.NxFileViewer.Test.Services.Nsz;

public sealed class NszInputAliasTest : IDisposable
{
    private readonly string _root = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "NxAlias-" + Guid.NewGuid())).FullName;

    [Theory]
    [InlineData(".nsp", ".nsz", NszOperation.Compress)]
    [InlineData(".xci", ".xcz", NszOperation.Compress)]
    [InlineData(".nsz", ".nsp", NszOperation.Decompress)]
    [InlineData(".xcz", ".xci", NszOperation.Decompress)]
    public void UnicodeInputUsesAsciiArgumentsAndRestoresOutputName(string inputExtension, string outputExtension, NszOperation operation)
    {
        var source = Path.Combine(_root, "Asterix & Obelix꞉ 日本語" + inputExtension);
        File.WriteAllText(source, "synthetic package");
        var keys = Path.Combine(_root, "synthetic.keys");
        File.WriteAllText(keys, "synthetic keys");
        var staging = Directory.CreateDirectory(Path.Combine(_root, "output")).FullName;
        string inputAlias;
        using (var alias = new NszInputAlias(source, staging, keys, TestContext.Current.CancellationToken))
        {
            var args = NszCliPlugin.BuildArguments(alias.SourceArgument, ".", alias.KeysArgument, operation, 18, useRelativePaths: true);
            Assert.All(args, arg => Assert.All(arg, character => Assert.True(character < 128)));
            Assert.False(Path.IsPathRooted(alias.SourceArgument));
            inputAlias = Path.Combine(staging, alias.SourceArgument);
            Assert.Equal("synthetic package", File.ReadAllText(inputAlias));
            Assert.Equal("synthetic keys", File.ReadAllText(Path.Combine(staging, alias.KeysArgument)));
            File.WriteAllText(Path.Combine(staging, "input" + outputExtension), "converted package");
            alias.RestoreOutputName(operation);
            Assert.Equal("converted package", File.ReadAllText(Path.Combine(staging, Path.ChangeExtension(Path.GetFileName(source), outputExtension))));
        }
        Assert.Equal("synthetic package", File.ReadAllText(source));
        Assert.False(File.Exists(inputAlias));
        Assert.Empty(Directory.GetDirectories(staging));
    }

    [Fact]
    public void MissingOutputIsDetectedEvenWhenExternalProcessReportedSuccess()
    {
        var source = Path.Combine(_root, "game꞉.nsp");
        File.WriteAllText(source, "synthetic package");
        var keys = Path.Combine(_root, "synthetic.keys");
        File.WriteAllText(keys, "synthetic keys");
        var staging = Directory.CreateDirectory(Path.Combine(_root, "output")).FullName;
        using (var alias = new NszInputAlias(source, staging, keys, TestContext.Current.CancellationToken))
            Assert.Throws<InvalidDataException>(() => alias.RestoreOutputName(NszOperation.Compress));
        Assert.Empty(Directory.GetFiles(staging, "*", SearchOption.AllDirectories));
        Assert.True(File.Exists(source));
    }

    public void Dispose() => Directory.Delete(_root, recursive: true);
}

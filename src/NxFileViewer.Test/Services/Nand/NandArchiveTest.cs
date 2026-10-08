using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using Emignatik.NxFileViewer.FileLoading;
using Emignatik.NxFileViewer.Models.Overview;
using Emignatik.NxFileViewer.Models.TreeItems.Impl;
using Emignatik.NxFileViewer.Services.BackgroundTask;
using Emignatik.NxFileViewer.Services.BackgroundTask.RunnableImpl;
using Emignatik.NxFileViewer.Services.Integrity;
using Emignatik.NxFileViewer.Services.Nand;
using Emignatik.NxFileViewer.Settings;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Emignatik.NxFileViewer.Test.Services.Nand;

public sealed class NandArchiveTest : IDisposable
{
    private readonly string _root = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "nand-archive-" + Guid.NewGuid())).FullName;
    private static byte[] Gpt(long offset = 0x200, bool switchNames = true)
    {
        var bytes = new byte[offset + 4096];
        Encoding.ASCII.GetBytes("EFI PART").CopyTo(bytes, offset);
        BitConverter.GetBytes((ulong)2).CopyTo(bytes, offset + 72);
        BitConverter.GetBytes((uint)2).CopyTo(bytes, offset + 80);
        BitConverter.GetBytes((uint)128).CopyTo(bytes, offset + 84);
        Encoding.Unicode.GetBytes(switchNames ? "PRODINFO" : "Windows").CopyTo(bytes, offset + 512 + 56);
        Encoding.Unicode.GetBytes(switchNames ? "SYSTEM" : "Data").CopyTo(bytes, offset + 512 + 128 + 56);
        return bytes;
    }
    [Theory]
    [InlineData(0x200, "RAWNAND")]
    [InlineData(0x800200, "FULL NAND")]
    [InlineData(0x1800200, "FULL NAND")]
    public void SwitchGptIsDetectedWithoutKeysOrPlugin(long offset, string expected)
    {
        var file = Path.Combine(_root, "backup.bin");
        File.WriteAllBytes(file, Gpt(offset));
        var result = NandDetection.Detect(file, TestContext.Current.CancellationToken)!;
        Assert.Equal(expected, result.Type);
        Assert.False(result.NameOnly);
        Assert.Contains("SYSTEM", result.Partitions);
        File.WriteAllBytes(file, Gpt(offset, switchNames: false));
        Assert.Null(NandDetection.Detect(file, TestContext.Current.CancellationToken));
    }
    [Fact]
    public void BootPartitionsAreNotMistakenForEachOthersSplitContinuation()
    {
        File.WriteAllBytes(Path.Combine(_root, "BOOT0"), new byte[512]);
        File.WriteAllBytes(Path.Combine(_root, "BOOT1"), new byte[512]);
        Assert.False(NandDetection.IsContinuation(Path.Combine(_root, "BOOT1")));
        Assert.True(NandDetection.Detect(Path.Combine(_root, "BOOT1"), TestContext.Current.CancellationToken)!.NameOnly);
    }
    [Fact]
    public void LooseBatchGroupsSplitDumpsAndNeverCallsNcaVerification()
    {
        var first = Path.Combine(_root, "rawnand.bin.00");
        File.WriteAllBytes(first, Gpt());
        File.WriteAllBytes(Path.Combine(_root, "rawnand.bin.01"), new byte[512]);
        File.WriteAllBytes(Path.Combine(_root, "random.bin"), new byte[512]);
        var fingerprint = BatchHistoryStore.Fingerprint(first);
        var rows = Batch().Setup(_root, false, verifyIntegrity: true).Run(new Progress(), TestContext.Current.CancellationToken);
        var row = Assert.Single(rows);
        Assert.True(row.IsNand);
        Assert.Equal("NAND", row.FileType);
        Assert.Equal(NcasIntegrity.Unchecked, row.Integrity);
        Assert.Null(row.Error);
        File.AppendAllText(Path.Combine(_root, "rawnand.bin.01"), "changed");
        Assert.NotEqual(fingerprint, BatchHistoryStore.Fingerprint(first));
    }
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ZipAndSolid7zListLoadAndBatchRecognizeNandAndKeepSplitSiblings(bool sevenZip)
    {
        var archive = CreateArchive(sevenZip);
        var entries = PackageZip.GetEntries(archive, includeNca: false, token: TestContext.Current.CancellationToken);
        Assert.Equal(3, entries.Count);
        Assert.Contains("dump/BOOT0", entries);
        Assert.Contains("dump/BOOT1", entries);
        Assert.Contains("dump/rawnand.bin.00", entries);
        Assert.DoesNotContain("dump/rawnand.bin.01", entries);
        string physical;
        using (var file = Loader().Load(PackageZip.MemberPath(archive, "dump/rawnand.bin.00"), TestContext.Current.CancellationToken))
        {
            Assert.IsType<NandFileItem>(file.RootItem);
            Assert.Equal(2, file.NandResult!.Parts);
            physical = file.NandPhysicalPath!;
            Assert.True(File.Exists(Path.Combine(Path.GetDirectoryName(physical)!, "rawnand.bin.01")));
            Assert.Equal(3, file.ArchiveEntries.Count);
            Assert.Equal(3, PackageZip.GetEntries(archive, includeNca: false, token: TestContext.Current.CancellationToken).Count);
        }
        Assert.False(File.Exists(physical));
        var rows = Batch().Setup(archive, false, verifyIntegrity: true).Run(new Progress(), TestContext.Current.CancellationToken);
        Assert.Equal(3, rows.Count);
        Assert.All(rows, row => { Assert.True(row.IsNand); Assert.Null(row.Error); Assert.Equal(NcasIntegrity.Unchecked, row.Integrity); Assert.Contains(sevenZip ? "7Z" : "ZIP", row.FileType); });
    }
    [Fact]
    public void MixedZipSkipsUnrecognizedBinAndKeepsGameAndNandEntries()
    {
        var archive = Path.Combine(_root, "mixed.zip");
        using (var zip = ZipFile.Open(archive, ZipArchiveMode.Create))
        {
            using (var entry = zip.CreateEntry("random.bin").Open()) entry.Write(new byte[512]);
            using (var entry = zip.CreateEntry("backup.bin").Open()) entry.Write(Gpt());
            using (var entry = zip.CreateEntry("game.nsp").Open()) entry.Write(Encoding.ASCII.GetBytes("PFS0"));
        }
        Assert.Equal(new[] { "backup.bin", "game.nsp" }, PackageZip.GetEntries(archive, includeNca: false, token: TestContext.Current.CancellationToken));
        using var first = Loader().Load(archive, TestContext.Current.CancellationToken);
        Assert.NotNull(first.NandResult);
    }
    [Fact]
    public void ArchiveExtractionRejectsUnsafeNandNamesAndCancellation()
    {
        var archive = Path.Combine(_root, "bad.zip");
        using (var zip = ZipFile.Open(archive, ZipArchiveMode.Create))
        using (var entry = zip.CreateEntry("../PRODINFO.bin").Open()) entry.Write(new byte[512]);
        Assert.Throws<InvalidDataException>(() => PackageZip.GetEntries(archive, token: TestContext.Current.CancellationToken));
        Assert.ThrowsAny<OperationCanceledException>(() => PackageZip.GetEntries(archive, token: new CancellationToken(true)));
    }
    private string CreateArchive(bool sevenZip)
    {
        if (sevenZip)
        {
            var file = Path.Combine(_root, "nand.7z");
            // Synthetic solid archive containing only zero-filled partitions and a small Switch-shaped GPT.
            File.WriteAllBytes(file, Convert.FromBase64String("N3q8ryccAAT6sc1g8gAAAAAAAAAiAAAAAAAAAP4Dg9TgFf8AT10AAG/9//+jt/9HPcVnjtXbVy/4kHWz+Ny1sAEmU/ZO2htPHdSLjBabyrddsopI6OE9U5HR8M7KXCXedic+TxpxAGzEv8IY5ObYlCvrGfgL+gAAAIEzB64P0xvCvUDAkNL/dKEfpyvffRVnecgIaKma3GU5UN2Dwkvtnh6NfIESfj04NfeawcI/P6wm+JpDXCYqa8mf24lbd7ZcTR2YGxxdohPB0Xz5L2QNmCeaJqqTYCS4myKIMFefiaDgWrHulMbD0+vSr2PmHIkmWIFWFTx4XMlohAGFEpReNEBQ2rBCOQHuinf2oyCrytPdABcGVwEJgJsABwsBAAEjAwEBBV0AEAAADIESCgHscoRhAAA="));
            return file;
        }
        var archive = Path.Combine(_root, "nand.zip");
        using var zip = ZipFile.Open(archive, ZipArchiveMode.Create);
        using (var entry = zip.CreateEntry("dump/rawnand.bin.00").Open()) entry.Write(Gpt());
        foreach (var name in new[] { "dump/rawnand.bin.01", "dump/BOOT0", "dump/BOOT1" })
        { using var entry = zip.CreateEntry(name).Open(); entry.Write(new byte[512]); }
        return archive;
    }
    private static IFileLoader Loader() => (IFileLoader)Activator.CreateInstance(
        typeof(IFileLoader).Assembly.GetType("Emignatik.NxFileViewer.FileLoading.FileLoader")!,
        NullLoggerFactory.Instance, new PackageTypeAnalyzer(NullLoggerFactory.Instance), new DummyItems(), new FileOverviewLoader(NullLoggerFactory.Instance), null)!;
    private static VerifyDirectoryIntegrityRunnable Batch() => new(Loader(), null!, new AppSettings(), NullLogger<VerifyDirectoryIntegrityRunnable>.Instance);
    private sealed class DummyItems : IFileItemLoader
    {
        public event MissingKeyExceptionHandler? MissingKey { add { } remove { } }
        public NspItem LoadNsp(string path) => throw new InvalidOperationException();
        public XciItem LoadXci(string path) => throw new InvalidOperationException();
        public StandaloneNcaFileItem LoadNca(string path) => throw new InvalidOperationException();
    }
    private sealed class Progress : IProgressReporter
    {
        public void SetText(string text) { }
        public void SetMode(bool value) { }
        public void SetPercentage(double value) { }
    }
    public void Dispose() => Directory.Delete(_root, true);
}

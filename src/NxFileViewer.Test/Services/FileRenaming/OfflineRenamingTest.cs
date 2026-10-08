using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Emignatik.NxFileViewer.FileLoading.QuickFileInfoLoading;
using Emignatik.NxFileViewer.Models;
using Emignatik.NxFileViewer.Services.FileOpening;
using Emignatik.NxFileViewer.Services.FileRenaming;
using Emignatik.NxFileViewer.Services.FileRenaming.Models;
using Emignatik.NxFileViewer.Services.FileRenaming.Models.PatternParts;
using Emignatik.NxFileViewer.Services.OnlineServices;
using Emignatik.NxFileViewer.Settings;
using LibHac.Ns;
using LibHac.Tools.Ncm;
using Xunit;

namespace Emignatik.NxFileViewer.Test.Services.FileRenaming;

public class OfflineRenamingTest
{
    [Theory]
    [InlineData(PatternKeyword.OnlineTitleName)]
    [InlineData(PatternKeyword.OnlineAppTitleName)]
    public async Task SimulationUsesLocalTitleWhenServerReturns503(PatternKeyword keyword)
    {
        var metadata = new byte[0x60];
        metadata[0xC] = 0x80; // Application CNMT, no content entries.
        metadata[0xE] = 0x10;
        using var cnmtStream = new MemoryStream(metadata);
        var nacpBytes = new byte[0x4000];
        Encoding.UTF8.GetBytes("Until Then").CopyTo(nacpBytes, 0);
        nacpBytes[0x302C] = 1; // American English supported.
        var content = new Content(new Cnmt(cnmtStream))
        {
            NacpData = new NacpData(MemoryMarshal.Read<ApplicationControlProperty>(nacpBytes))
        };
        using var client = new HttpClient(new UnavailableHandler());
        var online = new CachedOnlineTitleInfoService(new OnlineTitleInfoService(new AppSettings(), httpClient: client));
        var renamer = new FileRenamerService(new PackageLoader(content), online, new UnusedFileOpeningService());
        var settings = new NamingSettings();
        settings.ApplicationPattern.Add(new DynamicTextPatternPart(keyword, StringOperator.Untouched));
        var result = await renamer.RenameFileAsync(Path.Combine(Path.GetTempPath(), "old.nsz"), false, settings, true, null, CancellationToken.None);
        Assert.Null(result.Exception);
        Assert.True(result.IsSimulation);
        Assert.Equal("Until Then", result.NewFileName);
        var directory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        var original = Path.Combine(directory, "old.nsz");
        File.WriteAllText(original, "synthetic input");
        try
        {
            RenamingResult? reported = null;
            var results = await renamer.RenameFromDirectoryAsync(directory, "*.nsz", false, false, settings, true,
                null, new NoProgress(), CancellationToken.None, row => reported = row);
            Assert.Same(Assert.Single(results), reported);
            Assert.Equal("old.nsz", reported!.OldFileName);
            Assert.Equal("Until Then", reported.NewFileName);
            Assert.True(reported.IsSimulation);
            Assert.True(File.Exists(original));
            Assert.False(File.Exists(Path.Combine(directory, "Until Then")));
            settings.TargetDirectory = Path.Combine(directory, "target");
            settings.ApplicationPattern.Insert(0, new StaticTextPatternPart("GAME/DLC/"));
            var preview = await renamer.RenameFileAsync(original, false, settings, true, null, CancellationToken.None);
            var target = Path.Combine(settings.TargetDirectory, "GAME", "DLC", "Until Then");
            Assert.Equal(target, preview.NewFilePath);
            Assert.Equal("QUELL::old.nsz", preview.OldPathDisplay);
            Assert.Equal("ZIEL::" + Path.Combine("GAME", "DLC", "Until Then"), preview.NewPathDisplay);
            settings.TargetDirectory = directory + Path.DirectorySeparatorChar;
            var sameRoot = await renamer.RenameFileAsync(original, false, settings, true, null, CancellationToken.None);
            Assert.Equal("QUELL::" + Path.Combine("GAME", "DLC", "Until Then"), sameRoot.NewPathDisplay);
            settings.TargetDirectory = Path.Combine(directory, "target");
            Assert.Equal(original, preview.OldFilePath);
            Assert.False(Directory.Exists(settings.TargetDirectory));
            var moved = await renamer.RenameFileAsync(original, false, settings, false, null, CancellationToken.None);
            Assert.Null(moved.Exception);
            Assert.Equal(original, moved.OldFilePath);
            Assert.False(File.Exists(original));
            Assert.Equal("synthetic input", File.ReadAllText(target));
            var matching = await renamer.RenameFileAsync(target, false, settings, true, null, CancellationToken.None);
            Assert.Null(matching.Exception);
            Assert.False(matching.IsRenamed);

            File.WriteAllText(original, "second input");
            var collision = await renamer.RenameFileAsync(original, false, settings, false, null, CancellationToken.None);
            Assert.IsType<IOException>(collision.Exception);
            Assert.Equal(target, collision.NewFilePath);
            Assert.Equal("second input", File.ReadAllText(original));
            Assert.Equal("synthetic input", File.ReadAllText(target));
            settings.ApplicationPattern[0] = new StaticTextPatternPart("../");
            var escape = await renamer.RenameFileAsync(original, false, settings, false, null, CancellationToken.None);
            Assert.IsType<IOException>(escape.Exception);
            Assert.True(File.Exists(original));

        }
        finally { Directory.Delete(directory, true); }

    }

    private sealed class NoProgress : Emignatik.NxFileViewer.Services.BackgroundTask.IProgressReporter
    {
        public void SetMode(bool isIndeterminate) { }
        public void SetText(string text) { }
        public void SetPercentage(double percentage) { }
    }

    private sealed class PackageLoader(Content content) : IPackageInfoLoader
    {
        public PackageInfo GetPackageInfo(string path) => new() { Contents = [content], AccuratePackageType = AccuratePackageType.NSZ };
    }
    private sealed class UnavailableHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
            Task.FromResult(new HttpResponseMessage(HttpStatusCode.ServiceUnavailable));
    }
    private sealed class UnusedFileOpeningService : IFileOpeningService
    {
        public event OpenedFileChangedHandler OpenedFileChanged { add { } remove { } }
        public NxFile? OpenedFile => throw new InvalidOperationException("Simulation must not access opened files.");
        public Task SafeOpenFile(string path) => throw new InvalidOperationException();
        public void SafeClose() => throw new InvalidOperationException();
    }
}

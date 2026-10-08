using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Emignatik.NxFileViewer.Logging;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Emignatik.NxFileViewer.Test.Logging;

public sealed class SessionFileLogTest : IDisposable
{
    private readonly string _root = Path.Combine(Path.GetTempPath(), "NxSessionLog-" + Guid.NewGuid());

    [Fact]
    public void SevenLaunchesRetainFiveSessionsAndLeaveUnrelatedFiles()
    {
        Directory.CreateDirectory(_root);
        File.WriteAllText(Path.Combine(_root, "other.log"), "keep");
        for (var start = 0; start < 7; start++)
        {
            using var log = new SessionFileLog(_root);
            log.Write(LogLevel.Information, "launch " + start);
        }
        var files = Directory.GetFiles(_root, "NxFileViewer-*.log");
        Assert.Equal(5, files.Length);
        var contents = string.Join("\n", files.Select(File.ReadAllText));
        Assert.DoesNotContain("launch 0", contents);
        Assert.DoesNotContain("launch 1", contents);
        Assert.Contains("launch 6", contents);
        Assert.Equal("keep", File.ReadAllText(Path.Combine(_root, "other.log")));
    }

    [Fact]
    public void ConcurrentMessagesAreImmediatelyReadableAndKeysAreRedacted()
    {
        using var log = new SessionFileLog(_root);
        Parallel.For(0, 100, i => log.Write(LogLevel.Warning, "Meldung 日本語 " + i));
        log.Write(LogLevel.Information, "TitleID Key «00000000000000000000000000000012=0123456789ABCDEF0123456789ABCDEF»");
        log.Write(LogLevel.Error, "master_key_16 = 0123456789ABCDEF0123456789ABCDEF\nException details");
        using var reader = new StreamReader(new FileStream(Directory.GetFiles(_root).Single(), FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
        var content = reader.ReadToEnd();
        Assert.Equal(100, content.Split('\n').Count(line => line.Contains("[Warning] Meldung 日本語 ")));
        Assert.DoesNotContain("0123456789ABCDEF0123456789ABCDEF", content);
        Assert.Contains("master_key_16 = [REDACTED]", content);
        Assert.Contains("Exception details", content);
    }

    [Fact]
    public void UnwritableDestinationDoesNotBreakLogging()
    {
        Directory.CreateDirectory(_root);
        var file = Path.Combine(_root, "not-a-directory");
        File.WriteAllText(file, "keep");
        using var log = new SessionFileLog(file);
        log.Write(LogLevel.Error, "still usable");
        Assert.Equal("keep", File.ReadAllText(file));
    }

    [Fact]
    public void RetentionCanBeConfiguredAfterSettingsLoadAndReducedWhileRunning()
    {
        for (var start = 0; start < 8; start++)
        {
            using var session = new SessionFileLog(_root, 10);
            session.Write(LogLevel.Information, "launch " + start);
        }
        using var current = new SessionFileLog(_root, int.MaxValue);
        Assert.Equal(9, Directory.GetFiles(_root).Length);
        current.Trim(7);
        Assert.Equal(7, Directory.GetFiles(_root).Length);
        current.Trim(2);
        Assert.Equal(2, Directory.GetFiles(_root).Length);
    }

    public void Dispose()
    {
        if (Directory.Exists(_root)) Directory.Delete(_root, true);
    }
}

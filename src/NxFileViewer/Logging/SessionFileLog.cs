using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer.Logging;

/// <summary>One flushed log per launch with configurable session retention.</summary>
public sealed class SessionFileLog : IDisposable
{
    private readonly object _gate = new();
    private StreamWriter? _writer;
    private readonly string _directory;
    private static readonly Regex KeyValue = new(
        @"(?i)(\b(?:[a-z_]+key[a-z_0-9]*|[0-9a-f]{32})\s*[=|]\s*)[0-9a-f]{32,}",
        RegexOptions.Compiled);

    public SessionFileLog(string directory, int retentionCount = 5)
    {
        _directory = directory;
        try
        {
            Directory.CreateDirectory(directory);
            var path = Path.Combine(directory, $"NxFileViewer-{DateTime.UtcNow:yyyyMMdd-HHmmss-fffffff}-{Guid.NewGuid():N}.log");
            _writer = new StreamWriter(new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.Read), new UTF8Encoding(false)) { AutoFlush = true };
            Write(LogLevel.Information, "NxFileViewer session started.");
            Trim(retentionCount);
        }
        catch (IOException) { Dispose(); }
        catch (UnauthorizedAccessException) { Dispose(); }
    }

    public void Trim(int retentionCount)
    {
        lock (_gate)
        {
            try
            {
                foreach (var old in Directory.EnumerateFiles(_directory, "NxFileViewer-*.log")
                             .OrderByDescending(Path.GetFileName, StringComparer.Ordinal).Skip(Math.Max(1, retentionCount)))
                {
                    try { File.Delete(old); }
                    catch (IOException) { /* Another running instance may still own this log. */ }
                    catch (UnauthorizedAccessException) { }
                }
            }
            catch (IOException) { }
            catch (UnauthorizedAccessException) { }
        }
    }

    public void Write(LogLevel level, string message)
    {
        lock (_gate)
        {
            try
            {
                _writer?.WriteLine($"[{DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss.fff zzz}] [{level}] {KeyValue.Replace(message, "$1[REDACTED]")}");
            }
            catch (IOException) { Dispose(); }
            catch (UnauthorizedAccessException) { Dispose(); }
        }
    }

    public void Dispose()
    {
        lock (_gate)
        {
            var writer = _writer;
            _writer = null;
            try { writer?.Dispose(); }
            catch (IOException) { }
        }
    }
}

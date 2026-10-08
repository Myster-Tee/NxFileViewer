using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Emignatik.NxFileViewer.Services.BackgroundTask;
using Emignatik.NxFileViewer.Services.Nand;
using Xunit;

namespace Emignatik.NxFileViewer.Test.Services.Nand;

public sealed class NandProcessRunnerTest
{
    [Fact]
    public void DrainsBothStreamsAndReturnsInformation()
    {
        var start = Start("[Console]::Out.Write(('x' * 200000)); [Console]::Error.Write(('y' * 200000)); [Console]::Out.Write('NAND type : BOOT0')");
        var output = new NandProcessRunner().Run(start, new Progress(), CancellationToken.None);
        Assert.EndsWith("NAND type : BOOT0", output);
        Assert.DoesNotContain("yyy", output);
    }

    [Fact]
    public void NonzeroExitDoesNotExposeRawDiagnostics()
    {
        var start = Start("[Console]::Error.Write('secret diagnostic'); exit 7");
        var error = Assert.Throws<IOException>(() => new NandProcessRunner().Run(start, new Progress(), CancellationToken.None));
        Assert.Contains("7", error.Message);
        Assert.DoesNotContain("secret", error.Message);
    }

    [Fact]
    public void CancellationStopsRunningProcess()
    {
        using var cancellation = new CancellationTokenSource(TimeSpan.FromSeconds(1));
        var timer = Stopwatch.StartNew();
        Assert.ThrowsAny<OperationCanceledException>(() => new NandProcessRunner().Run(Start("Start-Sleep -Seconds 20"), new Progress(), cancellation.Token));
        Assert.True(timer.Elapsed < TimeSpan.FromSeconds(10));
    }

    private static ProcessStartInfo Start(string script)
    {
        var start = new ProcessStartInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "WindowsPowerShell", "v1.0", "powershell.exe"))
        {
            UseShellExecute = false, CreateNoWindow = true,
            RedirectStandardOutput = true, RedirectStandardError = true, RedirectStandardInput = true
        };
        start.ArgumentList.Add("-NoProfile");
        start.ArgumentList.Add("-NonInteractive");
        start.ArgumentList.Add("-Command");
        start.ArgumentList.Add(script);
        return start;
    }
    private sealed class Progress : IProgressReporter
    {
        public void SetMode(bool value) { }
        public void SetText(string text) { }
        public void SetPercentage(double value) { }
    }
}

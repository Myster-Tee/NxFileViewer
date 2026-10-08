using System;
using System.IO;
using System.Collections.Generic;
using Emignatik.NxFileViewer.Services.Nsz;
using Emignatik.NxFileViewer.Services.BackgroundTask;
using Emignatik.NxFileViewer.Services.WindowPlacement;
using Emignatik.NxFileViewer.Settings;
using Xunit;

namespace Emignatik.NxFileViewer.Test.Services.Nsz;

public sealed class NszProgressUpdateTest
{
    [Theory]
    [InlineData("25.5%|abc| [00:02<00:15, 12.5MB/s]", 0.255)]
    [InlineData("100%", 1)]
    [InlineData("3,5 %", 0.035)]
    public void ExtractsNumericProgressWithoutRawOutput(string line, double expected)
    {
        Assert.True(NszProgressUpdate.TryParse(line, out var value));
        Assert.Equal(expected, value!.Fraction, 6);
        Assert.DoesNotContain("abc", value.Details);
    }
    [Theory]
    [InlineData("123%")]
    [InlineData("-1%")]
    [InlineData("master_key_00 = 0123456789abcdef0123456789abcdef")]
    public void IgnoresInvalidProgressAndKeyLines(string line) => Assert.False(NszProgressUpdate.TryParse(line, out _));

    [Fact]
    public void DetailsOnlyIncludeSafeSpeedAndEta()
    {
        Assert.True(NszProgressUpdate.TryParse("50% secret=0123456789abcdef <00:12 8.2 MiB/s", out var value));
        Assert.Contains("8.2 MiB/s", value!.Details);
        Assert.Contains("ETA 00:12", value.Details);
        Assert.DoesNotContain("secret", value.Details);
        Assert.DoesNotContain("0123456789abcdef", value.Details);
    }

    [Fact]
    public void MapsStageProgressIntoBatchProgress()
    {
        var sink = new Reporter();
        var batch = new NszProgressScope(sink, "[2/2]", 0.5, 0.5);
        var stage = new NszProgressScope(batch, "2/3", 1.0 / 3, 1.0 / 3);
        stage.SetPercentage(0.5);
        stage.SetText("Compress");
        Assert.Equal(0.75, sink.Value, 6);
        Assert.Contains("[2/2]", sink.Text);
        Assert.Contains("2/3", sink.Text);
    }

    [Theory]
    [InlineData(false, true, false, true)]
    [InlineData(true, true, false, false)]
    [InlineData(true, true, true, true)]
    [InlineData(true, false, false, true)]
    public void StartsMaximizedUnlessValidSavedPlacementSaysNormal(bool defined, bool remember, bool maximized, bool expected)
    {
        var settings = new AppSettings { RememberWindowPlacement = remember };
        if (defined) { settings.MainWindowPlacement.Width = 1200; settings.MainWindowPlacement.Height = 800; }
        settings.MainWindowPlacement.IsMaximized = maximized;
        var roundTrip = System.Text.Json.JsonSerializer.Deserialize<AppSettings>(System.Text.Json.JsonSerializer.Serialize(settings))!;
        Assert.Equal(expected, WindowPlacementService.ShouldStartMaximized(roundTrip));
    }

    [Fact]
    public void ProcessReportsCarriageReturnProgressUpdates()
    {
        var updates = new List<NszProgressUpdate>();
        NszProcess.Run(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "WindowsPowerShell", "v1.0", "powershell.exe"),
            new[] { "-NoProfile", "-NonInteractive", "-Command",
                "[Console]::Write('25%'); [Console]::Write([char]13); Start-Sleep -Milliseconds 250; [Console]::Error.Write('100%'); [Console]::Error.Write([char]13)" },
            Path.GetTempPath(), TestContext.Current.CancellationToken, update => updates.Add(update));
        Assert.Contains(updates, update => update.Fraction == 0.25);
        Assert.Contains(updates, update => update.Fraction == 1);
    }

    private sealed class Reporter : IProgressReporter
    {
        public double Value;
        public string Text = "";
        public void SetMode(bool value) { }
        public void SetText(string text) => Text = text;
        public void SetPercentage(double value) => Value = value;
    }
}
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Emignatik.NxFileViewer.Services.BackgroundTask;

namespace Emignatik.NxFileViewer.Services.Nsz;

public sealed record NszProgressUpdate(double Fraction, string Details)
{
    public static bool TryParse(string line, out NszProgressUpdate? update)
    {
        update = null;
        var percentage = Regex.Match(line, @"(?<![\d.\-])(?<value>\d{1,3}(?:[.,]\d+)?)\s*%");
        if (!percentage.Success || !double.TryParse(percentage.Groups["value"].Value.Replace(',', '.'),
            NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var value) || value > 100) return false;
        // Only whitelisted numeric progress details leave the process boundary.
        var speed = Regex.Match(line, @"\b\d+(?:[.,]\d+)?\s*[kKMGT]?i?[bB]/s\b");
        var eta = Regex.Match(line, @"<(?<time>\d{1,3}:\d{2}(?::\d{2})?)");
        var details = value.ToString("0.0", CultureInfo.InvariantCulture) + " %";
        if (speed.Success) details += " | " + speed.Value;
        if (eta.Success) details += " | ETA " + eta.Groups["time"].Value;
        update = new(value / 100, details);
        return true;
    }
}

public sealed class NszProgressScope(IProgressReporter parent, string prefix, double offset, double scale) : IProgressReporter
{
    private string _text = "";
    private double? _fraction;
    public void SetMode(bool isIndeterminate) { if (isIndeterminate) _fraction = null; parent.SetMode(isIndeterminate); ReportText(); }
    public void SetText(string text) { _text = text; ReportText(); }
    public void SetPercentage(double value)
    {
        _fraction = Math.Clamp(value, 0, 1);
        parent.SetPercentage(offset + scale * _fraction.Value);
        ReportText();
    }
    private void ReportText() => parent.SetText(prefix + (_fraction.HasValue ? " [" + (_fraction.Value * 100).ToString("0.0", CultureInfo.CurrentCulture) + " %]" : " […]") + " — " + _text);
}
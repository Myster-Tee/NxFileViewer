using System;
using Emignatik.NxFileViewer.Services.Updates;
using Xunit;

namespace Emignatik.NxFileViewer.Test.Services;

public sealed class UpdateAvailabilitySummaryTest
{
    [Fact]
    public void HomeSummaryIncludesOnlyAvailableComponents()
    {
        var summary = UpdateAvailabilitySummary.Format(null, new[]
        {
            new ComponentUpdateResult("NSZ", "current", false),
            new ComponentUpdateResult("NxNandManager", "failed", false),
            new ComponentUpdateResult("Title DB US.en", "available", true),
            new ComponentUpdateResult("Hashes", "available", true)
        });
        Assert.Equal("Title DB US.en: available" + Environment.NewLine + "Hashes: available", summary);
    }

    [Fact]
    public void HomeSummaryIsEmptyWithoutUpdatesAndNamesViewerWhenAvailable()
    {
        Assert.Empty(UpdateAvailabilitySummary.Format(null, Array.Empty<ComponentUpdateResult>()));
        Assert.StartsWith("NxFileViewer:", UpdateAvailabilitySummary.Format("5.0", Array.Empty<ComponentUpdateResult>()));
    }
}

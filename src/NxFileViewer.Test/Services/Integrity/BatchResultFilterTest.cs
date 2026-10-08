using Emignatik.NxFileViewer.Models.Overview;
using Emignatik.NxFileViewer.Services.Integrity;
using Xunit;

namespace Emignatik.NxFileViewer.Test.Services.Integrity;

public sealed class BatchResultFilterTest
{
    [Theory]
    [InlineData(true, null, "Matches", true)]
    [InlineData(false, null, "Differs", true)]
    [InlineData(null, null, "Unchecked", true)]
    [InlineData(true, "failure", "Error", true)]
    [InlineData(false, null, "Matches", false)]
    [InlineData(null, null, "Differs", false)]
    public void FiltersNamingState(bool? matches, string? error, string filter, bool expected) =>
        Assert.Equal(expected, BatchResultFilter.Matches(Result with { NamingMatches = matches, NamingError = error }, "until", "NSZ", "Original", false, filter));

    private static readonly BatchIntegrityResult Result = new("games/game.nsz", "NSZ", "NSZ", "Cdn", "Blockless", NcasIntegrity.Original, null)
    { Title = "Until Then", TitleId = "010019C023004000", Publisher = "Maximum Entertainment", SystemVersion = "20.0.1.0" };

    [Theory]
    [InlineData("until maximum", "NSZ", "Original", false, true)]
    [InlineData("010019c023004000", "", "", false, true)]
    [InlineData("20.0.1.0", "", "", false, true)]
    [InlineData("until unknown", "", "", false, false)]
    [InlineData("", "NSP", "", false, false)]
    [InlineData("", "NSZ", "Error", false, false)]
    [InlineData("", "", "", true, false)]
    [InlineData("   ", "", "", false, true)]
    public void CombinesSearchTypeIntegrityAndErrorFilters(string search, string type, string integrity, bool errors, bool expected) =>
        Assert.Equal(expected, BatchResultFilter.Matches(Result, search, type, integrity, errors));

    [Fact] public void ConversionFailureRemainsVisibleInErrorFilter() =>
        Assert.True(BatchResultFilter.Matches(Result with { ConversionFailed = true }, "", "", "", true));
    [Theory]
    [InlineData("NSZ (ZIP)", "NSZ", true)]
    [InlineData("NSZ (ZIP)", "ZIP", true)]
    [InlineData("NSZ (7z)", "7Z", true)]
    [InlineData("NSZ (7z)", "NSZ", true)]
    [InlineData("NSZ (ZIP)", "NSP", false)]
    [InlineData("NSZ (ZIP)", "7Z", false)]
    public void ArchiveMembersCanBeFilteredByPackageTypeAndArchiveOrigin(string displayedType, string filter, bool expected) =>
        Assert.Equal(expected, BatchResultFilter.Matches(Result with { FileType = displayedType }, "", filter, "", false));
}

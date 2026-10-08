using Emignatik.NxFileViewer.Services.PackageAnalysis;
using LibHac.Tools.Fs;
using Xunit;

namespace Emignatik.NxFileViewer.Test.Services.PackageAnalysis;

public class PackageStructureAnalyzerTest
{
    [Theory]
    [InlineData("title.tik")]
    [InlineData("title.cert")]
    public void TicketOrCertificateAloneIsIncomplete(string name) =>
        Assert.Equal(PackageStructure.Incomplete, PackageStructureAnalyzer.AnalyzeNspNames(new[] { "program.nca", name }));

    [Fact]
    public void AnalyzeNspNames_ClassifiesSceneRelease()
    {
        var names = new[] { "program.nca", "legalinfo.xml", "nacp.xml", "programinfo.xml", "cardspec.xml" };
        Assert.Equal(PackageStructure.Scene, PackageStructureAnalyzer.AnalyzeNspNames(names));
    }

    [Fact]
    public void AnalyzeNspNames_ClassifiesHomebrew()
    {
        var names = new[] { "program.nca", "authoringtoolinfo.xml" };
        Assert.Equal(PackageStructure.Homebrew, PackageStructureAnalyzer.AnalyzeNspNames(names));
    }

    [Fact]
    public void AnalyzeNspNames_ClassifiesCdnRip()
    {
        var names = new[] { "program.ncz", "title.tik", "title.cert" };
        Assert.Equal(PackageStructure.Cdn, PackageStructureAnalyzer.AnalyzeNspNames(names));
    }

    [Fact]
    public void AnalyzeNspNames_ClassifiesConvertedPackage()
    {
        var names = new[] { "program.nca", "control.xml" };
        Assert.Equal(PackageStructure.Converted, PackageStructureAnalyzer.AnalyzeNspNames(names));
    }

    [Fact]
    public void AnalyzeNspNames_ClassifiesNcaOnlyPackageAsIncomplete()
    {
        var names = new[] { "meta.nca", "program.ncz" };
        Assert.Equal(PackageStructure.Incomplete, PackageStructureAnalyzer.AnalyzeNspNames(names));
    }

    [Fact]
    public void AnalyzeXciPartitions_ClassifiesSceneRelease()
    {
        var partitions = new[] { XciPartitionType.Update, XciPartitionType.Normal, XciPartitionType.Secure };
        Assert.Equal(PackageStructure.Scene, PackageStructureAnalyzer.AnalyzeXciPartitions(partitions));
    }

    [Fact]
    public void AnalyzeXciPartitions_ClassifiesSecureOnlyAsConverted()
    {
        Assert.Equal(PackageStructure.Converted,
            PackageStructureAnalyzer.AnalyzeXciPartitions(new[] { XciPartitionType.Secure }));
    }

    [Fact]
    public void AnalyzeXciPartitions_ClassifiesMissingPartitionsAsIncomplete()
    {
        Assert.Equal(PackageStructure.Incomplete,
            PackageStructureAnalyzer.AnalyzeXciPartitions(new[] { XciPartitionType.Normal, XciPartitionType.Secure }));
    }
}

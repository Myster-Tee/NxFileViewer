using Emignatik.NxFileViewer.Services.FileRenaming;
using Xunit;

namespace Emignatik.NxFileViewer.Test.Services.FileRenaming;

public class FileNameTextNormalizerTest
{
    [Fact]
    public void RestoresMissingSeparatorBetweenWords()
    {
        var result = FileNameTextNormalizer.RestoreMissingWordSeparators(
            "The Legend of ZeldaBreath of the Wild DLC Pack 2");

        Assert.Equal("The Legend of Zelda Breath of the Wild DLC Pack 2", result);
    }

    [Theory]
    [InlineData("Nintendo eShop")]
    [InlineData("iPhone Case")]
    [InlineData("Already separated")]
    public void PreservesExpectedNames(string value)
    {
        Assert.Equal(value, FileNameTextNormalizer.RestoreMissingWordSeparators(value));
    }
}

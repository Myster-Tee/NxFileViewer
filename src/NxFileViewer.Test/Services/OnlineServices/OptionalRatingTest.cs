using System.Text.Json;
using Emignatik.NxFileViewer.Services.OnlineServices;
using Xunit;
namespace Emignatik.NxFileViewer.Test.Services.OnlineServices;
public class OptionalRatingTest
{
    [Theory]
    [InlineData("4.5", 4.5)]
    [InlineData("\"4.5\"", 4.5)]
    [InlineData("\"4,5\"", 4.5)]
    [InlineData("null", 0)]
    [InlineData("\"\"", 0)]
    [InlineData("\"N/A\"", 0)]
    [InlineData("\"NaN\"", 0)]
    [InlineData("1e999", 0)]
    [InlineData("false", 0)]
    [InlineData("{}", 0)]
    [InlineData("[1,2]", 0)]
    public void OptionalRatingDoesNotDiscardTitleOrFollowingFields(string rating, double expected)
    {
        var info = JsonSerializer.Deserialize<OnlineTitleInfo>("{\"rating\":" + rating + ",\"name\":\"Example\",\"publisher\":\"Publisher\"}")!;
        Assert.Equal(expected, info.Rating);
        Assert.Equal("Example", info.Name);
        Assert.Equal("Publisher", info.Publisher);
    }
}
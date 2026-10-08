using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Emignatik.NxFileViewer.Services.OnlineServices;
using Emignatik.NxFileViewer.Settings;
using Xunit;

namespace Emignatik.NxFileViewer.Test.Services.OnlineServices;

public class OnlineTitleInfoServiceTest
{
    [Theory]
    [InlineData(503, "Service Unavailable")]
    [InlineData(404, "Not found")]
    [InlineData(200, "<html>Server unavailable</html>")]
    [InlineData(200, "{}")]
    public async Task UnavailableOrInvalidResponseAllowsLocalFallback(int status, string body)
    {
        using var client = new HttpClient(new ResponseHandler(() => new HttpResponseMessage((HttpStatusCode)status) { Content = new StringContent(body) }));
        var service = new OnlineTitleInfoService(new AppSettings(), httpClient: client);
        Assert.Null(await service.GetTitleInfoAsync("010019C023004000"));
    }

    [Fact]
    public async Task ValidResponsePreservesOnlineName()
    {
        using var client = new HttpClient(new ResponseHandler(() => new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{\"name\":\"Until Then\"}") }));
        var service = new OnlineTitleInfoService(new AppSettings(), httpClient: client);
        Assert.Equal("Until Then", (await service.GetTitleInfoAsync("010019C023004000"))!.Name);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task NetworkFailureOrTimeoutAllowsLocalFallback(bool timeout)
    {
        using var client = new HttpClient(new ResponseHandler(() => throw (timeout ? new TaskCanceledException() : new HttpRequestException())));
        var service = new OnlineTitleInfoService(new AppSettings(), httpClient: client);
        Assert.Null(await service.GetTitleInfoAsync("010019C023004000"));
    }

    private sealed class ResponseHandler(Func<HttpResponseMessage> response) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) => Task.FromResult(response());
    }
}

using Autodesk.Authentication;
using Autodesk.Authentication.Helpers.Models;
using Autodesk.Common.HttpClientLibrary.Middleware;
using Autodesk.Common.HttpClientLibrary.Middleware.Options;
using Microsoft.Kiota.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sdk_Tests;

namespace Tests.HttpClient;
[TestClass]
public class HttpClientTest
{
    public AuthenticationClient AuthClient { get; init; }

    public HttpClientTest()
    {
        AuthClient = InitializeAuthClient();
    }

    [ClassInitialize]
    public static void StartMockServer(TestContext context)
    {
        APSmockServer.StartMockServer();
    }

    [ClassCleanup]
    public static void StopMockServer()
    {
        APSmockServer.Dispose();
    }

    [TestMethod]
    //Test the Http error handler
    public async Task ShouldThrowAnExceptionWhenInvalidCredentials()
    {
        Settings config = Settings.Load();

        try
        {
            await AuthClient.Helper.GetTwoLeggedToken(config.APS_CLIENT_ID, string.Empty, [AuthenticationScopeDefaults.DataWrite, AuthenticationScopeDefaults.DataRead]);
        }
        catch (ApiException ex)
        {
            Assert.IsNotNull(ex.Message);
            return;
        }

        Assert.Fail();

    }

    [TestMethod]
    //Test the rate limit handler
    public async Task ShouldSlowerDownByRateLimit()
    {
        var requestTimeWindow = TimeSpan.FromSeconds(1);
        var rateLimit = (maxConcurrentRequests: 10, timeWindow: requestTimeWindow);
        var httpClient = Autodesk.Common.HttpClientLibrary.HttpClientFactory.Create(rateLimit);

        var startingAt = DateTime.Now;

        var tasks = new List<Task<HttpResponseMessage>>();
        for (int i = 0; i < 20; i++)
        {
            //Response delay from the mock server 2ms
            var resp = httpClient.GetAsync("http://localhost:4200/ratelimit/test?testparam=1");
            tasks.Add(resp);

        }

        await Task.WhenAll(tasks);

        var endAt = DateTime.Now;

        Assert.IsTrue((endAt - startingAt).TotalMilliseconds >= requestTimeWindow.TotalMilliseconds);

    }

    [TestMethod]
    //Test the rate limit handler
    public async Task ShouldIgnoreRateLimit()
    {
        var requestTimeWindow = TimeSpan.FromSeconds(1);

        // By default the rate limiting is disabled
        var httpClient = Autodesk.Common.HttpClientLibrary.HttpClientFactory.Create();

        var startingAt = DateTime.Now;

        var tasks = new List<Task>();
        for (int i = 0; i < 20; i++)
        {
            //Response delay from the mock server 2ms
            var resp = httpClient.GetAsync("http://localhost:4200/ratelimit/test?testparam=1");
            tasks.Add(resp);

        }

        await Task.WhenAll(tasks);

        var endAt = DateTime.Now;

        Assert.IsTrue((endAt - startingAt).TotalMilliseconds < requestTimeWindow.TotalMilliseconds);

    }

    [TestMethod]
    //Test the error handler
    public async Task ShouldThrowException()
    {
        var httpClient = Autodesk.Common.HttpClientLibrary.HttpClientFactory.Create();

        var ex = await Assert.ThrowsExceptionAsync<ApiException>(async () =>
        {
            await httpClient.GetAsync("http://localhost:4200/error/test?testparam=1");
        });

        // Validate that the exception message or data contains "context"
        Assert.IsNotNull(ex.Message);
        Assert.IsTrue(ex.Data.Contains("context"), "Exception data should contain 'context' entry");
    }

    [TestMethod]
    //Test the add query parameter handler
    public async Task ShouldAddQueryParameter()
    {
        //Introduce a query parameter in the url with the queryParameter middleware
        var queryParamHandler = new QueryParameterHandlerOption();
        queryParamHandler.QueryParameters.Add("testparam", "1");

        var httpClient = Autodesk.Common.HttpClientLibrary.HttpClientFactory.Create(null, [queryParamHandler]);

        // The query parameter is not part of the url, it is added by the middleware
        var resp = await httpClient.GetAsync("http://localhost:4200/queryParam/");

        Assert.IsTrue(resp.IsSuccessStatusCode, "Response should be successful");
    }

    [TestMethod]
    public async Task ShouldThrowOnNon2xxWithDefaultPattern()
    {
        var noRedirect = new HttpClientHandler { AllowAutoRedirect = false };
        var httpClient = Autodesk.Common.HttpClientLibrary.HttpClientFactory.Create(noRedirect);

        var ex = await Assert.ThrowsExceptionAsync<ApiException>(async () =>
        {
            await httpClient.GetAsync("http://localhost:4200/status/302");
        });

        Assert.IsNotNull(ex.Message);
    }

    [TestMethod]
    public async Task ShouldNotThrowWhenCustomPatternAllows400()
    {
        // 400 is non-2xx but the custom pattern explicitly permits it — no exception should be thrown.
        var pattern = new System.Text.RegularExpressions.Regex(@"^(2\d{2}|400)$");
        var errorOption = new CustomErrorHandlerOption
        {
            CustomErrorHandler = ctx => CustomErrorHandlerOption.DefaultCustomErrorHandler(ctx, pattern)
        };
        var httpClient = Autodesk.Common.HttpClientLibrary.HttpClientFactory.Create(null, [errorOption]);

        var resp = await httpClient.GetAsync("http://localhost:4200/error/test?testparam=1");

        Assert.AreEqual(400, (int)resp.StatusCode);
    }

    [TestMethod]
    public async Task ShouldReturnIsErrorOutcomeWithoutThrowing()
    {
        // Delegate returns a non-throwing HandlerOutcome with IsError=true instead of throwing.
        var errorOption = new CustomErrorHandlerOption
        {
            CustomErrorHandler = ctx =>
            {
                if ((int)ctx.StatusCode is >= 200 and < 300)
                    return HandlerOutcome.Ok;

                return new HandlerOutcome(IsError: true, Description: $"HTTP {(int)ctx.StatusCode}");
            }
        };
        var httpClient = Autodesk.Common.HttpClientLibrary.HttpClientFactory.Create(null, [errorOption]);

        // The response is returned rather than thrown — caller inspects the status code.
        var resp = await httpClient.GetAsync("http://localhost:4200/error/test?testparam=1");

        Assert.IsFalse(resp.IsSuccessStatusCode);
    }

    private AuthenticationClient InitializeAuthClient()
    {
        var httpClient = Autodesk.Common.HttpClientLibrary.HttpClientFactory.Create();
        httpClient.BaseAddress = new Uri(APSmockServer.GetMockAPSserviceUrl(AdskService.Authentication));

        var authClient = new AuthenticationClient(httpClient);

        return authClient;
    }
}

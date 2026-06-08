using Autodesk.Common.HttpClientLibrary.Middleware;
using Autodesk.Common.HttpClientLibrary.Middleware.Options;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;
using static Microsoft.Kiota.Http.HttpClientLibrary.KiotaClientFactory;

namespace Autodesk.Common.HttpClientLibrary;
public static class HttpClientFactory
{
    /// <summary>
    /// Create a resilient Http Client without rate limiting.
    /// </summary>
    /// <returns>Http client including default middleware without rate limit</returns>
    public static System.Net.Http.HttpClient Create()
    {
        return Create(null, null);
    }

    /// <summary>
    /// Create a resilient Http Client with rate limit settings.
    /// </summary>
    /// <param name="rateLimit">Maximum calls per endpoint in a specific timeframe</param>
    /// <returns>Http client including default middleware with rate limit</returns>
    public static System.Net.Http.HttpClient Create((int maxConcurrentRequests, TimeSpan timeWindow)? rateLimit)
    {
        var rateLimitHandlerOption = new RateLimitingHandlerOption();
        if (rateLimit.HasValue)
        {
            rateLimitHandlerOption.SetRateLimit(rateLimit.Value.maxConcurrentRequests, rateLimit.Value.timeWindow);
        }

        return Create(null, [rateLimitHandlerOption]);
    }

    /// <summary>
    /// Initializes the <see cref="HttpClient"/> with the default configuration and middlewares including a authentication middleware using the <see cref="IAuthenticationProvider"/> if provided.
    /// </summary>
    /// <param name="finalHandler">The final <see cref="HttpMessageHandler"/> in the http pipeline. Can be configured for proxies, auto-decompression and auto-redirects </param>
    /// <param name="optionsForHandlers">A array of <see cref="IRequestOption"/> objects passed to the default handlers.</param>
    /// <returns>The <see cref="HttpClient"/> with the default middlewares.</returns>
    public static HttpClient Create(HttpMessageHandler? finalHandler = null, IRequestOption[]? optionsForHandlers = null)
    {
        var rateLimitHandlerOption = (RateLimitingHandlerOption?)optionsForHandlers?.FirstOrDefault(o => o.GetType() == typeof(RateLimitingHandlerOption)) ?? new RateLimitingHandlerOption();
        var errorHandlerOption = (CustomErrorHandlerOption?)optionsForHandlers?.FirstOrDefault(o => o.GetType() == typeof(CustomErrorHandlerOption)) ?? new CustomErrorHandlerOption();
        var queryParameterHandlerOption = (QueryParameterHandlerOption?)optionsForHandlers?.FirstOrDefault(o => o.GetType() == typeof(QueryParameterHandlerOption)) ?? new QueryParameterHandlerOption();

        var handlers = CreateDefaultHandlers(optionsForHandlers);
        handlers.Add(new RateLimitingHandler(rateLimitHandlerOption));
        handlers.Add(new QueryParameterHandler(queryParameterHandlerOption));
        handlers.Add(new CustomErrorHandler(errorHandlerOption));

        var defaultFinalHandler = finalHandler ?? GetDefaultHttpMessageHandler();
        var httpMessageHandler =
                    ChainHandlersCollectionAndGetFirstLink(defaultFinalHandler, [.. handlers])
                    ?? defaultFinalHandler;

        return httpMessageHandler != null ? new HttpClient(httpMessageHandler) : new HttpClient();
    }

    /// <summary>
    /// Creates a new HttpClientRequestAdapter with an AccessTokenProvider. The adapter will use the default error and decompression handlers.
    /// </summary>
    /// <param name="getAccessToken"></param>
    /// <param name="httpClient"></param>
    /// <returns>Request adapter for SDK client instances</returns>
    public static HttpClientRequestAdapter CreateAdapter(Func<Task<string>> getAccessToken, HttpClient? httpClient)
    {
        var auth = new BaseBearerTokenAuthenticationProvider(new AccessTokenProvider(getAccessToken));

        httpClient ??= Create();

        return new HttpClientRequestAdapter(auth, null, null, httpClient); ;
    }

    /// <summary>
    /// Gets the default handler types.
    /// </summary>
    /// <returns>A list of all the default handlers</returns>
    /// <remarks>Order matters</remarks>
    public static IList<ActivatableType> GetDefaultHandlerActivatableTypes()
    {
        var nativeDefaultHandlers = KiotaClientFactory.GetDefaultHandlerActivatableTypes();

        return [.. nativeDefaultHandlers, new(typeof(RateLimitingHandler)), new(typeof(CustomErrorHandler))];
    }
}
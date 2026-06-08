using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using Autodesk.Common.HttpClientLibrary.Middleware.Options;
using Microsoft.Kiota.Http.HttpClientLibrary;
using Microsoft.Kiota.Http.HttpClientLibrary.Extensions;

namespace Autodesk.Common.HttpClientLibrary.Middleware;

/// <summary>
/// Error handler for the HttpClient that will throw an exception if the response is not successful.
/// </summary>
public class CustomErrorHandler : DelegatingHandler
{
    private readonly CustomErrorHandlerOption _errorHandlerOptions;

    public CustomErrorHandler(CustomErrorHandlerOption? errorHandlerOptions = null)
    {
        _errorHandlerOptions = errorHandlerOptions ?? new CustomErrorHandlerOption() { Enabled = true };
    }
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Activity? activity = null;
        if (request.GetRequestOption<ObservabilityOptions>() is { } obsOptions)
        {
            var activitySource = ActivitySourceRegistry.DefaultInstance.GetOrCreateActivitySource(obsOptions.TracerInstrumentationName);
            activity = activitySource.StartActivity($"{nameof(CustomErrorHandler)}_{nameof(SendAsync)}");
            activity?.SetTag("com.autodesk.handler.errorhandler.enable", _errorHandlerOptions.Enabled);
            activity?.SetTag("http.request.method", request.Method.Method);
            activity?.SetTag("url.full", request.RequestUri?.ToString());
            activity?.SetTag("server.address", request.RequestUri?.Host);
            activity?.SetTag("http.request.body.size", request.Content?.Headers.ContentLength);
        }

        try
        {
            var response = await base.SendAsync(request, cancellationToken);

            // Record neutral facts about the exchange. A non-2xx status is NOT treated as an
            // error here — that decision belongs to the CustomErrorHandler delegate.
            activity?.SetTag("http.response.status_code", (int)response.StatusCode);
            activity?.SetTag("http.response.body.size", response.Content?.Headers.ContentLength);
            if (response.Headers.TryGetValues("x-ads-request-id", out var requestId))
            {
                activity?.SetTag("autodesk.request_id", string.Join(",", requestId));
            }
            if (response.Headers.RetryAfter is { } retryAfter)
            {
                activity?.SetTag("http.response.retry_after", retryAfter.ToString());
            }

            // Check if the request has a specific ErrorHandlerOptions set, otherwise use the default options
            var errorOptions = request.GetRequestOption<CustomErrorHandlerOption>() ?? _errorHandlerOptions;

            if (!errorOptions.Enabled)
            {
                activity?.SetTag("com.autodesk.handler.errorhandler.outcome", "skipped");
                return response;
            }

            // Build a context backed by the live (buffered) HttpContent streams. These remain valid
            // for the duration of the delegate call. If the delegate throws and lets the exception
            // escape, it is responsible for swapping in detached streams via `with { … }` so they
            // survive disposal of the request/response (the default handler does this).
            var httpContext = await HttpContext.CreateAsync(request, response, cancellationToken).ConfigureAwait(false);

            try
            {
                var outcome = errorOptions.CustomErrorHandler(httpContext);

                if (outcome.IsError)
                {
                    activity?.SetStatus(ActivityStatusCode.Error, outcome.Description);
                }
            }
            catch (Exception ex)
            {
                // The delegate (and only the delegate) decides what counts as an error.
                activity?.SetTag("com.autodesk.handler.errorhandler.outcome", "threw");
                activity?.SetTag("error.type", ex.GetType().FullName);
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                activity?.AddEvent(new ActivityEvent("exception", tags: new ActivityTagsCollection
                {
                    ["exception.type"] = ex.GetType().FullName,
                    ["exception.message"] = ex.Message,
                    ["exception.stacktrace"] = ex.ToString(),
                }));

                // The delegate threw, so the caller will never receive the response.
                // Dispose it ourselves to release the connection and underlying buffers.
                // Any stream the consumer needs to keep must already be a detached copy
                // attached to the exception (the default handler does this).
                response.Dispose();
                throw;
            }

            return response;
        }
        finally
        {
            activity?.Dispose();
        }
    }
}

/// <summary>
/// Contains context information about an HTTP request/response pair, passed to
/// <see cref="CustomErrorHandlerOption.CustomErrorHandler"/> and attached to the
/// thrown <see cref="HttpRequestException"/> when a response is treated as an error.
/// </summary>
public record HttpContext
{
    public HttpContext() { }

    /// <summary>
    /// Builds a context from a request/response pair. Both contents are buffered via
    /// <see cref="HttpContent.LoadIntoBufferAsync()"/> so the streams in the context can be read
    /// multiple times. The streams returned by <see cref="HttpContent.ReadAsStreamAsync()"/> are
    /// owned by the request/response — they remain valid only as long as those are not disposed.
    /// To produce a context whose streams survive disposal, copy them to fresh
    /// <see cref="MemoryStream"/>s and use <c>with { … }</c>.
    /// </summary>
    public static async Task<HttpContext> CreateAsync(
        HttpRequestMessage request,
        HttpResponseMessage response,
        CancellationToken cancellationToken = default)
    {
        if (request.Content is { Headers.ContentLength: > 0 })
        {
            await request.Content.LoadIntoBufferAsync().ConfigureAwait(false);
        }

        if (response.Content is not null)
        {
            await response.Content.LoadIntoBufferAsync().ConfigureAwait(false);
        }

        return new HttpContext
        {
            RequestUri = request.RequestUri,
            RequestMethod = request.Method,
            RequestHeaders = request.Headers,
            RequestContent = await CopyToStreamAsync(request.Content, cancellationToken).ConfigureAwait(false),
            StatusCode = response.StatusCode,
            ResponseHeaders = response.Headers,
            ResponseContent = await CopyToStreamAsync(response.Content, cancellationToken).ConfigureAwait(false)

        };
    }

    public Uri? RequestUri { get; init; }
    public HttpMethod? RequestMethod { get; init; }
    public HttpRequestHeaders? RequestHeaders { get; init; }
    public Stream? RequestContent { get; init; }
    public HttpStatusCode StatusCode { get; init; }
    public HttpResponseHeaders? ResponseHeaders { get; init; }
    public Stream? ResponseContent { get; init; }
    public string? ResponseContentAsString
    {
        get
        {
            if (ResponseContent is null)
            {
                return null;
            }

            return ReadAndCompactJson(ResponseContent);
        }
    }

    private static async Task<Stream> CopyToStreamAsync(
            HttpContent? httpContent,
            CancellationToken cancellationToken
        )
    {
        if (httpContent is null or { Headers.ContentLength: 0 })
        {
            return Stream.Null;
        }

        var stream = new MemoryStream();
        await httpContent.LoadIntoBufferAsync().ConfigureAwait(false);

        await httpContent.CopyToAsync(stream, cancellationToken).ConfigureAwait(false);

        if (stream.CanSeek)
        {
            stream.Position = 0;
        }

        return stream;
    }
    /// <summary>
    /// Reads <paramref name="stream"/> as text and, if it parses as JSON, returns a whitespace-stripped
    /// version suitable for inclusion in an exception message. Leaves the stream rewound for re-reading.
    /// </summary>
    private static string ReadAndCompactJson(Stream stream)
    {
        if (stream == Stream.Null)
        {
            return string.Empty;
        }

        stream.Position = 0;
        using var reader = new StreamReader(stream, leaveOpen: true);
        var body = reader.ReadToEnd().TrimStart();
        stream.Position = 0;

        if (body.StartsWith('{') || body.StartsWith('['))
        {
            try
            {
                using var jsonDoc = System.Text.Json.JsonDocument.Parse(body);
                return System.Text.Json.JsonSerializer.Serialize(jsonDoc.RootElement);
            }
            catch
            {
                // Not valid JSON; fall through and return the raw body.
            }
        }

        return body;
    }
}


using System.Net.Http.Headers;
using Autodesk.Common.HttpClientLibrary.Middleware.Options;
using Microsoft.Kiota.Http.HttpClientLibrary.Extensions;

namespace Autodesk.Common.HttpClientLibrary.Middleware;

/// <summary>
/// Error handler for the HttpClient that will throw an exception if the response is not successful.
/// </summary>
public class ErrorHandler : DelegatingHandler
{
    private readonly ErrorHandlerOption _errorHandlerOptions;

    public ErrorHandler(ErrorHandlerOption? errorHandlerOptions = null)
    {
        _errorHandlerOptions = errorHandlerOptions ?? new ErrorHandlerOption() { Enabled = true };
    }
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var requestContentStream = await CopyToStreamAsync(request.Content, cancellationToken).ConfigureAwait(false);

        var response = await base.SendAsync(request, cancellationToken);

        // Check if the request has a specific ErrorHandlerOptions set, otherwise use the default options
        var errorOptions = request.GetRequestOption<ErrorHandlerOption>() ?? _errorHandlerOptions;

        if (errorOptions.Enabled && !response.IsSuccessStatusCode)
        {
            // Buffer the response content so it remains readable after the exception propagates
            var responseContentStream = await CopyToStreamAsync(response.Content, cancellationToken).ConfigureAwait(false);

            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

            throw new HttpRequestException(
                $"Request to '{request.RequestUri}' failed with status code '{response.StatusCode}'. Response body: '{responseBody}'",
                null,
                response.StatusCode)
            {
                Data = { ["context"] = new ErrorContext
                    {
                        RequestUri = request.RequestUri ,
                        RequestMethod = request.Method,
                        RequestHeaders = request.Headers,
                        RequestContent = requestContentStream,
                        ResponseHeaders = response.Headers,
                        ResponseContent = responseContentStream }
                }
            };
        }

        return response;
    }
    static async Task<Stream> CopyToStreamAsync(
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

#if NET5_0_OR_GREATER
        await httpContent.CopyToAsync(stream, cancellationToken).ConfigureAwait(false);
#else
            await httpContent.CopyToAsync(stream).ConfigureAwait(false);
#endif

        if (stream.CanSeek)
        {
            stream.Position = 0;
        }

        return stream;
    }
}

/// <summary>
/// Contains context information about a failed HTTP request.
/// </summary>
public record ErrorContext
{
    public Uri? RequestUri { get; init; }
    public HttpMethod?  RequestMethod { get; init; }
    public HttpRequestHeaders? RequestHeaders { get; init; }
    public  Stream? RequestContent { get; init; }
    public HttpResponseHeaders? ResponseHeaders { get; init; }
    public  Stream? ResponseContent { get; init; }
}
using System.Text.RegularExpressions;
using Autodesk.Common.HttpClientLibrary.Middleware;
using Microsoft.Kiota.Abstractions;

namespace Autodesk.Common.HttpClientLibrary.Middleware.Options;

public class CustomErrorHandlerOption : IRequestOption
{
    private static readonly Regex DefaultPattern = new(@"^2\d{2}$");

    /// <summary>
    /// Default error handler: returns <see cref="HandlerOutcome.Ok"/> for 2xx responses, otherwise
    /// throws an <see cref="HttpRequestException"/>. Before throwing, it copies the (buffered) response
    /// stream into a detached <see cref="MemoryStream"/> so the <see cref="HttpContext"/> remains
    /// usable after the framework disposes the response. The request body is not detached
    /// (potentially large uploads) — <see cref="HttpContext.RequestContent"/> on the exception's
    /// context is therefore <see cref="Stream.Null"/>. The exception's <see cref="System.Exception.Data"/>
    /// contains the detached <see cref="HttpContext"/> under the <c>"context"</c> key.
    /// </summary>
    public static readonly Func<HttpContext, Regex, HandlerOutcome> DefaultCustomErrorHandler = (ctx, validStatusPattern) =>
    {
        if (validStatusPattern.IsMatch(((int)ctx.StatusCode).ToString()))
        {
            return HandlerOutcome.Ok;
        }

        throw new ApiException(
            $"Request to '{ctx.RequestUri}' failed with status code '{ctx.StatusCode}'. Response body: '{ctx.ResponseContentAsString}'.")
        {
            Data = { ["context"] = ctx }
        };

    };

    /// <summary>
    /// If <c>true</c>, the middleware invokes <see cref="CustomErrorHandler"/> for every response.
    /// If <c>false</c>, the middleware passes the response through unchanged.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Hook invoked for every response. The delegate receives a fully populated <see cref="HttpContext"/>
    /// (status code, headers, buffered request and response bodies) and decides what to do: log, emit
    /// metrics, transform, throw, or simply report a non-throwing verdict via the returned
    /// <see cref="HandlerOutcome"/>. The middleware applies the outcome to the current
    /// <see cref="System.Diagnostics.Activity"/> (Error status when <see cref="HandlerOutcome.IsError"/>
    /// is true) but makes no judgment of its own. Defaults to <see cref="DefaultCustomErrorHandler"/>.
    /// </summary>
    public Func<HttpContext, HandlerOutcome> CustomErrorHandler { get; set; } = (context) => DefaultCustomErrorHandler(context, DefaultPattern);

    /// <summary>
    /// Copies <paramref name="source"/> into a fresh <see cref="MemoryStream"/> that survives
    /// disposal of the original <see cref="HttpContent"/>.
    /// </summary>
    private static Stream DetachStream(Stream? source)
    {
        if (source is null || source == Stream.Null)
        {
            return Stream.Null;
        }

        if (source.CanSeek)
        {
            source.Position = 0;
        }

        var copy = new MemoryStream();
        source.CopyTo(copy);
        copy.Position = 0;
        return copy;
    }

 
}

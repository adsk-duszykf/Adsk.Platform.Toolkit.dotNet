using Autodesk.Common.HttpClientLibrary.Middleware.Options;
using Microsoft.Kiota.Http.HttpClientLibrary.Extensions;

namespace Autodesk.Common.HttpClientLibrary.Middleware;

/// <summary>
/// HTTP middleware that automatically appends or modifies query parameters on outgoing requests.
/// This handler intercepts requests in the HTTP client pipeline and merges additional query parameters
/// from <see cref="QueryParameterHandlerOption"/> with any existing parameters in the request URL.
/// Useful for injecting common parameters like API versions, region codes, or tracking identifiers
/// across all API calls without requiring manual modification of each request.
/// </summary>
public class QueryParameterHandler : DelegatingHandler
{
    private readonly QueryParameterHandlerOption _queryParameterHandlerOptions;

    public QueryParameterHandler(QueryParameterHandlerOption? queryParameterOptions = null)
    {
        _queryParameterHandlerOptions = queryParameterOptions ?? new QueryParameterHandlerOption();
    }
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var queryParameterHandlerOptions = request.GetRequestOption<QueryParameterHandlerOption>() ?? _queryParameterHandlerOptions;

        if (queryParameterHandlerOptions.QueryParameters.Count != 0)
        {
            var uriBuilder = new UriBuilder(request.RequestUri!);
            var query = System.Web.HttpUtility.ParseQueryString(uriBuilder.Query);

            foreach (var parameter in queryParameterHandlerOptions.QueryParameters)
            {
                query[parameter.Key] = parameter.Value;
            }

            uriBuilder.Query = query.ToString();
            request.RequestUri = uriBuilder.Uri;
        }

        var response = await base.SendAsync(request, cancellationToken);

        return response;
    }

}

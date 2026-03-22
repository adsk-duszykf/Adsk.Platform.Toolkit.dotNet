using System.Runtime.CompilerServices;
using Autodesk.Automation.Da.UsEast.V3.Shares;
using Microsoft.Kiota.Abstractions;
using static Autodesk.Automation.Da.UsEast.V3.Shares.SharesRequestBuilder;

namespace Autodesk.Automation.Managers;

/// <summary>
/// Manager for Shares operations — lists AppBundles and Activities shared by this app.
/// </summary>
public class SharesManager
{
    private readonly BaseAutomationClient _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="SharesManager"/> class.
    /// </summary>
    /// <param name="api">The API client</param>
    public SharesManager(BaseAutomationClient api)
    {
        _api = api;
    }

    /// <summary>
    /// Lists all Shares (AppBundles and Activities) shared by this app with automatic pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /da/us-east/v3/shares
    /// APS docs: https://aps.autodesk.com/en/docs/design-automation/v3/reference/http/shares-GET
    /// </remarks>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="SharesGetResponse_data"/> share items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (var share in client.SharesManager.ListSharesAsync())
    /// {
    ///     Console.WriteLine($"{share.Id} ({share.Type})");
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<SharesGetResponse_data> ListSharesAsync(
        RequestConfiguration<SharesRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? page = requestConfiguration?.QueryParameters?.Page;

        while (true)
        {
            var response = await _api.Da.UsEast.V3.Shares
                .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                    r.QueryParameters.Page = page;
                }, cancellationToken);

            if (response?.Data is not { Count: > 0 })
                yield break;

            foreach (var item in response.Data)
            {
                yield return item;
            }

            if (string.IsNullOrEmpty(response.PaginationToken))
                yield break;

            page = response.PaginationToken;
        }
    }
}

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Autodesk.BuildingConnected;
using Autodesk.BuildingConnected.Construction.Buildingconnected.V2.OpportunityProjectPairs;
using Autodesk.BuildingConnected.Construction.Buildingconnected.V2.OpportunityProjectPairs.Item;
using Microsoft.Kiota.Abstractions;
using static Autodesk.BuildingConnected.Construction.Buildingconnected.V2.OpportunityProjectPairs.OpportunityProjectPairsRequestBuilder;

namespace Autodesk.BuildingConnected.Managers;

/// <summary>
/// Manager for BuildingConnected opportunity-project pair operations — list, get, create, and update.
/// </summary>
public class OpportunityProjectPairsManager
{
    private readonly BaseBuildingConnectedClient _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="OpportunityProjectPairsManager"/> class.
    /// </summary>
    /// <param name="api">The BuildingConnected API client.</param>
    public OpportunityProjectPairsManager(BaseBuildingConnectedClient api)
    {
        _api = api;
    }

    /// <summary>
    /// Lists opportunity-project pairs with automatic cursor-based pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/buildingconnected/v2/opportunity-project-pairs
    /// APS docs: https://aps.autodesk.com/en/docs/buildingconnected/v2/reference/http/buildingconnected-opportunity-project-pairs-GET
    /// The OpenAPI surface used for Kiota does not yet declare a <c>cursorState</c> query parameter on this operation; after the first page, continuation uses
    /// <see cref="OpportunityProjectPairsGetResponse_pagination.CursorState"/> together with the filter and limit values from <paramref name="requestConfiguration"/>.
    /// </remarks>
    /// <param name="requestConfiguration">(Optional) Configuration for the request, including filter and limit query parameters.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="OpportunityProjectPairsGetResponse_results"/>.</returns>
    /// <example>
    /// <code>
    /// await foreach (var pair in client.OpportunityProjectPairsManager.ListOpportunityProjectPairsAsync())
    /// {
    ///     Console.WriteLine(pair);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<OpportunityProjectPairsGetResponse_results> ListOpportunityProjectPairsAsync(
        RequestConfiguration<OpportunityProjectPairsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? cursor = null;

        while (true)
        {
            OpportunityProjectPairsGetResponse? response;

            if (cursor is null)
            {
                response = await _api.Construction.Buildingconnected.V2.OpportunityProjectPairs
                    .GetAsync(r =>
                    {
                        r.Headers = requestConfiguration?.Headers ?? r.Headers;
                        r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                        r.Options = requestConfiguration?.Options ?? r.Options;
                    }, cancellationToken);
            }
            else
            {
                string listUrl = BuildOpportunityProjectPairsPageUrl(
                    requestConfiguration?.QueryParameters,
                    cursor);
                response = await _api.Construction.Buildingconnected.V2.OpportunityProjectPairs
                    .WithUrl(listUrl)
                    .GetAsync(r =>
                    {
                        r.Headers = requestConfiguration?.Headers ?? r.Headers;
                        r.Options = requestConfiguration?.Options ?? r.Options;
                    }, cancellationToken);
            }

            if (response?.Results is not { Count: > 0 })
                yield break;

            foreach (OpportunityProjectPairsGetResponse_results item in response.Results)
            {
                yield return item;
            }

            if (string.IsNullOrEmpty(response.Pagination?.CursorState))
                yield break;

            cursor = response.Pagination.CursorState;
        }
    }

    /// <summary>
    /// Gets an opportunity-project pair by id.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/buildingconnected/v2/opportunity-project-pairs/{opportunityProjectPairId}
    /// APS docs: https://aps.autodesk.com/en/docs/buildingconnected/v2/reference/http/buildingconnected-opportunity-project-pairs-opportunityProjectPairId-GET
    /// </remarks>
    /// <param name="opportunityProjectPairId">The opportunity-project pair id.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>A <see cref="WithOpportunityProjectPairGetResponse"/> instance, or <c>null</c> if the response is empty.</returns>
    /// <example>
    /// <code>
    /// WithOpportunityProjectPairGetResponse? pair = await client.OpportunityProjectPairsManager.GetOpportunityProjectPairAsync("pair-id-here");
    /// </code>
    /// </example>
    public async Task<WithOpportunityProjectPairGetResponse?> GetOpportunityProjectPairAsync(
        string opportunityProjectPairId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Buildingconnected.V2.OpportunityProjectPairs[opportunityProjectPairId]
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Creates an opportunity-project pair.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /construction/buildingconnected/v2/opportunity-project-pairs
    /// APS docs: https://aps.autodesk.com/en/docs/buildingconnected/v2/reference/http/buildingconnected-opportunity-project-pairs-POST
    /// </remarks>
    /// <param name="body">The request body.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>A <see cref="OpportunityProjectPairsPostResponse"/> instance, or <c>null</c> if the response is empty.</returns>
    /// <example>
    /// <code>
    /// OpportunityProjectPairsPostResponse? created = await client.OpportunityProjectPairsManager.CreateOpportunityProjectPairAsync(new OpportunityProjectPairsPostRequestBody());
    /// </code>
    /// </example>
    public async Task<OpportunityProjectPairsPostResponse?> CreateOpportunityProjectPairAsync(
        OpportunityProjectPairsPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Buildingconnected.V2.OpportunityProjectPairs
            .PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Updates an opportunity-project pair (relinks a project per API semantics).
    /// </summary>
    /// <remarks>
    /// Wraps: PATCH /construction/buildingconnected/v2/opportunity-project-pairs/{opportunityProjectPairId}
    /// APS docs: https://aps.autodesk.com/en/docs/buildingconnected/v2/reference/http/buildingconnected-opportunity-project-pairs-opportunityProjectPairId-PATCH
    /// </remarks>
    /// <param name="id">The opportunity-project pair id.</param>
    /// <param name="body">The patch body.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>A <see cref="WithOpportunityProjectPairPatchResponse"/> instance, or <c>null</c> if the response is empty.</returns>
    /// <example>
    /// <code>
    /// WithOpportunityProjectPairPatchResponse? updated = await client.OpportunityProjectPairsManager.UpdateOpportunityProjectPairAsync("pair-id-here", new WithOpportunityProjectPairPatchRequestBody());
    /// </code>
    /// </example>
    public async Task<WithOpportunityProjectPairPatchResponse?> UpdateOpportunityProjectPairAsync(
        string id,
        WithOpportunityProjectPairPatchRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Buildingconnected.V2.OpportunityProjectPairs[id]
            .PatchAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    private static string BuildOpportunityProjectPairsPageUrl(
        OpportunityProjectPairsRequestBuilderGetQueryParameters? queryParameters,
        string cursorState)
    {
        // Matches <see cref="BaseBuildingConnectedClient"/> default when BaseUrl is not set on the adapter.
        const string root = "https://developer.api.autodesk.com";
        List<string> parts = new List<string>();
        if (!string.IsNullOrEmpty(queryParameters?.FilteropportunityId))
        {
            parts.Add($"filter%5BopportunityId%5D={Uri.EscapeDataString(queryParameters.FilteropportunityId)}");
        }

        if (!string.IsNullOrEmpty(queryParameters?.FilterprojectId))
        {
            parts.Add($"filter%5BprojectId%5D={Uri.EscapeDataString(queryParameters.FilterprojectId)}");
        }

        if (!string.IsNullOrEmpty(queryParameters?.FilterupdatedAt))
        {
            parts.Add($"filter%5BupdatedAt%5D={Uri.EscapeDataString(queryParameters.FilterupdatedAt)}");
        }

        if (queryParameters?.Limit is int limit)
        {
            parts.Add($"limit={limit}");
        }

        parts.Add($"cursorState={Uri.EscapeDataString(cursorState)}");
        return $"{root}/construction/buildingconnected/v2/opportunity-project-pairs?{string.Join("&", parts)}";
    }
}

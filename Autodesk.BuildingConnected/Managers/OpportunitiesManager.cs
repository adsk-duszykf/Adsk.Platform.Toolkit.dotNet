using System.Runtime.CompilerServices;
using Autodesk.BuildingConnected.Construction.Buildingconnected.V2.Opportunities;
using Autodesk.BuildingConnected.Construction.Buildingconnected.V2.Opportunities.Item;
using Autodesk.BuildingConnected.Construction.Buildingconnected.V2.Opportunities.Item.Comments;
using Autodesk.BuildingConnected.Construction.Buildingconnected.V2.Opportunities.Item.Comments.Item;
using Microsoft.Kiota.Abstractions;
using static Autodesk.BuildingConnected.Construction.Buildingconnected.V2.Opportunities.Item.Comments.CommentsRequestBuilder;
using static Autodesk.BuildingConnected.Construction.Buildingconnected.V2.Opportunities.OpportunitiesRequestBuilder;

namespace Autodesk.BuildingConnected.Managers;

/// <summary>
/// Manager for BuildingConnected BidBoard opportunity and opportunity comment operations.
/// </summary>
public class OpportunitiesManager
{
    private readonly BaseBuildingConnectedClient _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="OpportunitiesManager"/> class.
    /// </summary>
    /// <param name="api">The BuildingConnected API client.</param>
    public OpportunitiesManager(BaseBuildingConnectedClient api)
    {
        _api = api;
    }

    /// <summary>
    /// Lists opportunities available to the user&apos;s company with cursor-based pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/buildingconnected/v2/opportunities
    /// APS docs: https://aps.autodesk.com/en/docs/buildingconnected/v2/reference/http/buildingconnected-opportunities-GET
    /// </remarks>
    /// <param name="requestConfiguration">(Optional) Query parameters (including <see cref="OpportunitiesRequestBuilderGetQueryParameters.CursorState"/> for the first page), headers, and middleware options.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="OpportunitiesGetResponse_results"/> across all pages.</returns>
    /// <example>
    /// <code>
    /// await foreach (OpportunitiesGetResponse_results opportunity in client.OpportunitiesManager.ListOpportunitiesAsync())
    /// {
    ///     Console.WriteLine(opportunity.Id);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<OpportunitiesGetResponse_results> ListOpportunitiesAsync(
        RequestConfiguration<OpportunitiesRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? cursor = requestConfiguration?.QueryParameters?.CursorState;

        while (true)
        {
            OpportunitiesGetResponse? response = await _api.Construction.Buildingconnected.V2.Opportunities
                .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                    r.QueryParameters.CursorState = cursor;
                }, cancellationToken);

            if (response?.Results is not { Count: > 0 })
                yield break;

            foreach (OpportunitiesGetResponse_results item in response.Results)
            {
                yield return item;
            }

            if (string.IsNullOrEmpty(response.Pagination?.CursorState))
                yield break;

            cursor = response.Pagination.CursorState;
        }
    }

    /// <summary>
    /// Retrieves a single opportunity by identifier.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/buildingconnected/v2/opportunities/{opportunityId}
    /// APS docs: https://aps.autodesk.com/en/docs/buildingconnected/v2/reference/http/buildingconnected-opportunities-opportunityId-GET
    /// </remarks>
    /// <param name="opportunityId">The ID of the opportunity.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for headers and middleware options.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>A <see cref="WithOpportunityGetResponse"/> for the opportunity, or <c>null</c> if not found.</returns>
    /// <example>
    /// <code>
    /// WithOpportunityGetResponse? opportunity = await client.OpportunitiesManager.GetOpportunityAsync("opportunity-id");
    /// </code>
    /// </example>
    public async Task<WithOpportunityGetResponse?> GetOpportunityAsync(
        string opportunityId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Buildingconnected.V2.Opportunities[opportunityId]
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Manually creates a new opportunity in BidBoard.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /construction/buildingconnected/v2/opportunities
    /// APS docs: https://aps.autodesk.com/en/docs/buildingconnected/v2/reference/http/buildingconnected-opportunities-POST
    /// </remarks>
    /// <param name="body">The opportunity creation payload.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>A <see cref="OpportunitiesPostResponse"/> describing the created opportunity.</returns>
    /// <example>
    /// <code>
    /// OpportunitiesPostResponse? created =
    ///     await client.OpportunitiesManager.CreateOpportunityAsync(new OpportunitiesPostRequestBody());
    /// </code>
    /// </example>
    public async Task<OpportunitiesPostResponse?> CreateOpportunityAsync(
        OpportunitiesPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Buildingconnected.V2.Opportunities
            .PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Updates an existing opportunity.
    /// </summary>
    /// <remarks>
    /// Wraps: PATCH /construction/buildingconnected/v2/opportunities/{opportunityId}
    /// APS docs: https://aps.autodesk.com/en/docs/buildingconnected/v2/reference/http/buildingconnected-opportunities-opportunityId-PATCH
    /// </remarks>
    /// <param name="opportunityId">The ID of the opportunity.</param>
    /// <param name="body">The patch payload.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>A <see cref="WithOpportunityPatchResponse"/> with updated opportunity data.</returns>
    /// <example>
    /// <code>
    /// WithOpportunityPatchResponse? updated = await client.OpportunitiesManager.UpdateOpportunityAsync(
    ///     "opportunity-id",
    ///     new WithOpportunityPatchRequestBody());
    /// </code>
    /// </example>
    public async Task<WithOpportunityPatchResponse?> UpdateOpportunityAsync(
        string opportunityId,
        WithOpportunityPatchRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Buildingconnected.V2.Opportunities[opportunityId]
            .PatchAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Deletes an existing opportunity.
    /// </summary>
    /// <remarks>
    /// Wraps: DELETE /construction/buildingconnected/v2/opportunities/{opportunityId}
    /// APS docs: https://aps.autodesk.com/en/docs/buildingconnected/v2/reference/http/buildingconnected-opportunities-opportunityId-DELETE
    /// </remarks>
    /// <param name="opportunityId">The ID of the opportunity.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>A <see cref="Task"/> that completes when the opportunity is deleted.</returns>
    /// <example>
    /// <code>
    /// await client.OpportunitiesManager.DeleteOpportunityAsync("opportunity-id");
    /// </code>
    /// </example>
    public async Task DeleteOpportunityAsync(
        string opportunityId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        await _api.Construction.Buildingconnected.V2.Opportunities[opportunityId]
            .DeleteAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Lists comments for an opportunity with cursor-based pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/buildingconnected/v2/opportunities/{opportunityId}/comments
    /// APS docs: https://aps.autodesk.com/en/docs/buildingconnected/v2/reference/http/buildingconnected-opportunities-opportunityId-comments-GET
    /// </remarks>
    /// <param name="opportunityId">The ID of the opportunity.</param>
    /// <param name="requestConfiguration">(Optional) Query parameters (including <see cref="CommentsRequestBuilderGetQueryParameters.CursorState"/> for the first page), headers, and middleware options.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="CommentsGetResponse_results"/> across all pages.</returns>
    /// <example>
    /// <code>
    /// await foreach (CommentsGetResponse_results comment in client.OpportunitiesManager.ListOpportunityCommentsAsync("opportunity-id"))
    /// {
    ///     Console.WriteLine(comment.Id);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<CommentsGetResponse_results> ListOpportunityCommentsAsync(
        string opportunityId,
        RequestConfiguration<CommentsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? cursor = requestConfiguration?.QueryParameters?.CursorState;

        while (true)
        {
            CommentsGetResponse? response = await _api.Construction.Buildingconnected.V2.Opportunities[opportunityId].Comments
                .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                    r.QueryParameters.CursorState = cursor;
                }, cancellationToken);

            if (response?.Results is not { Count: > 0 })
                yield break;

            foreach (CommentsGetResponse_results item in response.Results)
            {
                yield return item;
            }

            if (string.IsNullOrEmpty(response.Pagination?.CursorState))
                yield break;

            cursor = response.Pagination.CursorState;
        }
    }

    /// <summary>
    /// Retrieves a single comment on an opportunity.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/buildingconnected/v2/opportunities/{opportunityId}/comments/{commentId}
    /// APS docs: https://aps.autodesk.com/en/docs/buildingconnected/v2/reference/http/buildingconnected-opportunities-opportunityId-comments-commentId-GET
    /// </remarks>
    /// <param name="opportunityId">The ID of the opportunity.</param>
    /// <param name="commentId">The ID of the comment.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for headers and middleware options.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>A <see cref="WithCommentGetResponse"/> for the comment, or <c>null</c> if not found.</returns>
    /// <example>
    /// <code>
    /// WithCommentGetResponse? comment =
    ///     await client.OpportunitiesManager.GetOpportunityCommentAsync("opportunity-id", "comment-id");
    /// </code>
    /// </example>
    public async Task<WithCommentGetResponse?> GetOpportunityCommentAsync(
        string opportunityId,
        string commentId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Buildingconnected.V2.Opportunities[opportunityId].Comments[commentId]
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }
}

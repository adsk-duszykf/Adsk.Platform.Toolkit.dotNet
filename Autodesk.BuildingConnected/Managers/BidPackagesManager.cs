using System.Runtime.CompilerServices;
using Autodesk.BuildingConnected.Construction.Buildingconnected.V2.BidPackages;
using Autodesk.BuildingConnected.Construction.Buildingconnected.V2.BidPackages.Item;
using Autodesk.BuildingConnected.Construction.Buildingconnected.V2.BidPackagesBatchCreate;
using Autodesk.BuildingConnected.Construction.Buildingconnected.V2.BidPackagesBatchDelete;
using Autodesk.BuildingConnected.Construction.Buildingconnected.V2.BidPackagesBatchPatch;
using Microsoft.Kiota.Abstractions;
using static Autodesk.BuildingConnected.Construction.Buildingconnected.V2.BidPackages.BidPackagesRequestBuilder;

namespace Autodesk.BuildingConnected.Managers;

/// <summary>
/// Manager for BuildingConnected Pro bid package operations, including batch endpoints.
/// </summary>
public class BidPackagesManager
{
    private readonly BaseBuildingConnectedClient _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="BidPackagesManager"/> class.
    /// </summary>
    /// <param name="api">The BuildingConnected API client.</param>
    public BidPackagesManager(BaseBuildingConnectedClient api)
    {
        _api = api;
    }

    /// <summary>
    /// Lists bid packages for projects owned by the current user&apos;s company with cursor-based pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/buildingconnected/v2/bid-packages
    /// APS docs: https://aps.autodesk.com/en/docs/buildingconnected/v2/reference/http/buildingconnected-bid-packages-GET
    /// </remarks>
    /// <param name="requestConfiguration">(Optional) Query parameters (including <see cref="BidPackagesRequestBuilderGetQueryParameters.CursorState"/> for the first page), headers, and middleware options.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="BidPackagesGetResponse_results"/> across all pages.</returns>
    /// <example>
    /// <code>
    /// await foreach (BidPackagesGetResponse_results package in client.BidPackagesManager.ListBidPackagesAsync())
    /// {
    ///     Console.WriteLine(package.Id);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<BidPackagesGetResponse_results> ListBidPackagesAsync(
        RequestConfiguration<BidPackagesRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? cursor = requestConfiguration?.QueryParameters?.CursorState;

        while (true)
        {
            BidPackagesGetResponse? response = await _api.Construction.Buildingconnected.V2.BidPackages
                .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                    r.QueryParameters.CursorState = cursor;
                }, cancellationToken);

            if (response?.Results is not { Count: > 0 })
                yield break;

            foreach (BidPackagesGetResponse_results item in response.Results)
            {
                yield return item;
            }

            if (string.IsNullOrEmpty(response.Pagination?.CursorState))
                yield break;

            cursor = response.Pagination.CursorState;
        }
    }

    /// <summary>
    /// Retrieves a single bid package by identifier.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/buildingconnected/v2/bid-packages/{bidPackageId}
    /// APS docs: https://aps.autodesk.com/en/docs/buildingconnected/v2/reference/http/buildingconnected-bid-packages-bidPackageId-GET
    /// </remarks>
    /// <param name="bidPackageId">The bid package identifier.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for headers and middleware options (default query parameters).</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>A <see cref="WithBidPackageGetResponse"/> for the bid package, or <c>null</c> if not found.</returns>
    /// <example>
    /// <code>
    /// WithBidPackageGetResponse? package = await client.BidPackagesManager.GetBidPackageAsync("bid-package-id");
    /// </code>
    /// </example>
    public async Task<WithBidPackageGetResponse?> GetBidPackageAsync(
        string bidPackageId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Buildingconnected.V2.BidPackages[bidPackageId]
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Creates a bid package on a BuildingConnected project owned by the current user&apos;s company.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /construction/buildingconnected/v2/bid-packages
    /// APS docs: https://aps.autodesk.com/en/docs/buildingconnected/v2/reference/http/buildingconnected-bid-packages-POST
    /// </remarks>
    /// <param name="body">The bid package creation payload.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>A <see cref="BidPackagesPostResponse"/> describing the created bid package.</returns>
    /// <example>
    /// <code>
    /// BidPackagesPostResponse? created = await client.BidPackagesManager.CreateBidPackageAsync(new BidPackagesPostRequestBody());
    /// </code>
    /// </example>
    public async Task<BidPackagesPostResponse?> CreateBidPackageAsync(
        BidPackagesPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Buildingconnected.V2.BidPackages
            .PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Updates fields on an existing bid package.
    /// </summary>
    /// <remarks>
    /// Wraps: PATCH /construction/buildingconnected/v2/bid-packages/{bidPackageId}
    /// APS docs: https://aps.autodesk.com/en/docs/buildingconnected/v2/reference/http/buildingconnected-bid-packages-bidPackageId-PATCH
    /// </remarks>
    /// <param name="bidPackageId">The bid package identifier.</param>
    /// <param name="body">The patch payload.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>A <see cref="WithBidPackagePatchResponse"/> with updated bid package data.</returns>
    /// <example>
    /// <code>
    /// WithBidPackagePatchResponse? updated = await client.BidPackagesManager.UpdateBidPackageAsync(
    ///     "bid-package-id",
    ///     new WithBidPackagePatchRequestBody());
    /// </code>
    /// </example>
    public async Task<WithBidPackagePatchResponse?> UpdateBidPackageAsync(
        string bidPackageId,
        WithBidPackagePatchRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Buildingconnected.V2.BidPackages[bidPackageId]
            .PatchAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Deletes a bid package that is in draft state.
    /// </summary>
    /// <remarks>
    /// Wraps: DELETE /construction/buildingconnected/v2/bid-packages/{bidPackageId}
    /// APS docs: https://aps.autodesk.com/en/docs/buildingconnected/v2/reference/http/buildingconnected-bid-packages-bidPackageId-DELETE
    /// </remarks>
    /// <param name="bidPackageId">The bid package identifier.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>A <see cref="Task"/> that completes when the bid package is deleted.</returns>
    /// <example>
    /// <code>
    /// await client.BidPackagesManager.DeleteBidPackageAsync("bid-package-id");
    /// </code>
    /// </example>
    public async Task DeleteBidPackageAsync(
        string bidPackageId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        await _api.Construction.Buildingconnected.V2.BidPackages[bidPackageId]
            .DeleteAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Creates multiple bid packages in one request.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /construction/buildingconnected/v2/bid-packages:batch-create
    /// APS docs: https://aps.autodesk.com/en/docs/buildingconnected/v2/reference/http/buildingconnected-bid-packages-batch-create-POST
    /// </remarks>
    /// <param name="body">The batch-create payload.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>A <see cref="BidPackagesBatchCreatePostResponse"/> with batch results.</returns>
    /// <example>
    /// <code>
    /// BidPackagesBatchCreatePostResponse? batch = await client.BidPackagesManager.BatchCreateBidPackagesAsync(
    ///     new BidPackagesBatchCreatePostRequestBody());
    /// </code>
    /// </example>
    public async Task<BidPackagesBatchCreatePostResponse?> BatchCreateBidPackagesAsync(
        BidPackagesBatchCreatePostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Buildingconnected.V2.BidPackagesBatchCreate
            .PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Updates multiple bid packages in one request.
    /// </summary>
    /// <remarks>
    /// Wraps: PATCH /construction/buildingconnected/v2/bid-packages:batch-patch
    /// APS docs: https://aps.autodesk.com/en/docs/buildingconnected/v2/reference/http/buildingconnected-bid-packages-batch-patch-PATCH
    /// </remarks>
    /// <param name="body">The batch-patch payload.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>A <see cref="BidPackagesBatchPatchPatchResponse"/> with batch results.</returns>
    /// <example>
    /// <code>
    /// BidPackagesBatchPatchPatchResponse? batch = await client.BidPackagesManager.BatchPatchBidPackagesAsync(
    ///     new BidPackagesBatchPatchPatchRequestBody());
    /// </code>
    /// </example>
    public async Task<BidPackagesBatchPatchPatchResponse?> BatchPatchBidPackagesAsync(
        BidPackagesBatchPatchPatchRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Buildingconnected.V2.BidPackagesBatchPatch
            .PatchAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Deletes multiple bid packages in one request (draft bid packages only).
    /// </summary>
    /// <remarks>
    /// Wraps: POST /construction/buildingconnected/v2/bid-packages:batch-delete
    /// APS docs: https://aps.autodesk.com/en/docs/buildingconnected/v2/reference/http/buildingconnected-bid-packages-batch-delete-POST
    /// </remarks>
    /// <param name="bidPackageIds">The list of bid package identifiers to delete.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>A <see cref="Task"/> that completes when the API returns no content.</returns>
    /// <example>
    /// <code>
    /// List&lt;string&gt; ids = ["pkg-1", "pkg-2"];
    /// await client.BidPackagesManager.BatchDeleteBidPackagesAsync(ids);
    /// </code>
    /// </example>
    public async Task BatchDeleteBidPackagesAsync(
        List<string> bidPackageIds,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        await _api.Construction.Buildingconnected.V2.BidPackagesBatchDelete
            .PostAsync(bidPackageIds, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }
}

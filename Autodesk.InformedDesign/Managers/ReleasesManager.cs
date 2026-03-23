using Autodesk.InformedDesign.IndustrializedConstruction.InformedDesign.V1.Releases;
using Autodesk.InformedDesign.IndustrializedConstruction.InformedDesign.V1.Releases.Item;
using Microsoft.Kiota.Abstractions;
using System.Runtime.CompilerServices;

namespace Autodesk.InformedDesign.Managers;

/// <summary>
/// Manager for Releases operations.
/// </summary>
public class ReleasesManager
{
    private readonly BaseInformedDesignClient _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReleasesManager"/> class.
    /// </summary>
    /// <param name="api">The Informed Design API client.</param>
    public ReleasesManager(BaseInformedDesignClient api)
    {
        _api = api;
    }

    /// <summary>
    /// Returns a paginated list of Releases for a Product based on the <c>limit</c> and <c>offset</c> query string parameters specified in the request.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /industrialized-construction/informed-design/v1/releases
    /// APS docs: https://aps.autodesk.com/en/docs/informed-design/v1/reference/quick_reference/informed-design-api-getreleases-GET    /// </remarks>
    /// <param name="requestConfiguration">(Optional) Configuration for the request such as headers, query parameters, and middleware options.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token to use when cancelling requests.</param>
    /// <returns>An async sequence of <see cref="ReleasesGetResponse_results"/> across all pages.</returns>
    /// <example>
    /// <code>
    /// await foreach (ReleasesGetResponse_results release in client.ReleasesManager.ListReleasesAsync())
    /// {
    ///     _ = release;
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<ReleasesGetResponse_results> ListReleasesAsync(
        RequestConfiguration<ReleasesRequestBuilder.ReleasesRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        int offset = requestConfiguration?.QueryParameters?.Offset ?? 0;
        while (true)
        {
            var response = await _api.IndustrializedConstruction.InformedDesign.V1.Releases
                .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                    r.QueryParameters.Offset = offset;
                }, cancellationToken);
            if (response?.Results is not { Count: > 0 })
                yield break;
            foreach (var item in response.Results)
                yield return item;
            if (string.IsNullOrEmpty(response.Pagination?.NextUrl))
                yield break;
            offset += response.Results.Count;
        }
    }

    /// <summary>
    /// Creates a new Release for a Product.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /industrialized-construction/informed-design/v1/releases
    /// APS docs: https://aps.autodesk.com/en/docs/informed-design/v1/reference/quick_reference/informed-design-api-postreleases-POST    /// </remarks>
    /// <param name="body">The request body.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request such as headers, query parameters, and middleware options.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token to use when cancelling requests.</param>
    /// <returns>The <see cref="ReleasesPostResponse"/> from the API, or <c>null</c> if the response was empty.</returns>
    /// <example>
    /// <code>
    /// ReleasesPostRequestBody body = new();
    /// ReleasesPostResponse? created = await client.ReleasesManager.CreateReleaseAsync(body);
    /// </code>
    /// </example>
    public async Task<ReleasesPostResponse?> CreateReleaseAsync(
        ReleasesPostRequestBody body,
        RequestConfiguration<ReleasesRequestBuilder.ReleasesRequestBuilderPostQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.IndustrializedConstruction.InformedDesign.V1.Releases
            .PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Returns the details of a Release. The Release is specified by the client provided <c>releaseId</c> URI parameter.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /industrialized-construction/informed-design/v1/releases/{releaseId}
    /// APS docs: https://aps.autodesk.com/en/docs/informed-design/v1/reference/quick_reference/informed-design-api-getrelease-GET    /// </remarks>
    /// <param name="releaseId">The unique identifier of the release.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request such as headers, query parameters, and middleware options.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token to use when cancelling requests.</param>
    /// <returns>The <see cref="WithReleaseGetResponse"/> for the release, or <c>null</c> if not found.</returns>
    /// <example>
    /// <code>
    /// Guid releaseId = Guid.Parse("00000000-0000-0000-0000-000000000000");
    /// WithReleaseGetResponse? release = await client.ReleasesManager.GetReleaseAsync(releaseId);
    /// </code>
    /// </example>
    public async Task<WithReleaseGetResponse?> GetReleaseAsync(
        Guid releaseId,
        RequestConfiguration<WithReleaseItemRequestBuilder.WithReleaseItemRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var response = await _api.IndustrializedConstruction.InformedDesign.V1.Releases[releaseId]
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
        return response;
    }

    /// <summary>
    /// Updates Release of a Product. The Release is specified by the client provided <c>releaseId</c> URI parameter.
    /// </summary>
    /// <remarks>
    /// Wraps: PATCH /industrialized-construction/informed-design/v1/releases/{releaseId}
    /// APS docs: https://aps.autodesk.com/en/docs/informed-design/v1/reference/quick_reference/informed-design-api-patchrelease-PATCH    /// </remarks>
    /// <param name="releaseId">The unique identifier of the release.</param>
    /// <param name="body">The request body.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request such as headers, query parameters, and middleware options.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token to use when cancelling requests.</param>
    /// <returns>The <see cref="WithReleasePatchResponse"/> from the API, or <c>null</c> if the response was empty.</returns>
    /// <example>
    /// <code>
    /// Guid releaseId = Guid.Parse("00000000-0000-0000-0000-000000000000");
    /// WithReleasePatchRequestBody body = new();
    /// WithReleasePatchResponse? updated = await client.ReleasesManager.UpdateReleaseAsync(releaseId, body);
    /// </code>
    /// </example>
    public async Task<WithReleasePatchResponse?> UpdateReleaseAsync(
        Guid releaseId,
        WithReleasePatchRequestBody body,
        RequestConfiguration<WithReleaseItemRequestBuilder.WithReleaseItemRequestBuilderPatchQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.IndustrializedConstruction.InformedDesign.V1.Releases[releaseId]
            .PatchAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Deletes a Release of a Product, and any Variants or Outputs for that Release. The Release is specified by the client provided <c>releaseId</c> URI parameter.
    /// </summary>
    /// <remarks>
    /// Wraps: DELETE /industrialized-construction/informed-design/v1/releases/{releaseId}
    /// APS docs: https://aps.autodesk.com/en/docs/informed-design/v1/reference/quick_reference/informed-design-api-deleterelease-DELETE    /// </remarks>
    /// <param name="releaseId">The unique identifier of the release.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request such as headers, query parameters, and middleware options.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token to use when cancelling requests.</param>
    /// <returns>A <see cref="Task"/> that completes when the delete operation finishes.</returns>
    /// <example>
    /// <code>
    /// Guid releaseId = Guid.Parse("00000000-0000-0000-0000-000000000000");
    /// await client.ReleasesManager.DeleteReleaseAsync(releaseId);
    /// </code>
    /// </example>
    public async Task DeleteReleaseAsync(
        Guid releaseId,
        RequestConfiguration<WithReleaseItemRequestBuilder.WithReleaseItemRequestBuilderDeleteQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        await _api.IndustrializedConstruction.InformedDesign.V1.Releases[releaseId]
            .DeleteAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }
}

using Autodesk.InformedDesign.IndustrializedConstruction.InformedDesign.V1.Downloads;
using Autodesk.InformedDesign.IndustrializedConstruction.InformedDesign.V1.Downloads.Item;
using Microsoft.Kiota.Abstractions;

namespace Autodesk.InformedDesign.Managers;

/// <summary>
/// Manager for Downloads operations
/// </summary>
public class DownloadsManager
{
    private readonly BaseInformedDesignClient _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="DownloadsManager"/> class.
    /// </summary>
    /// <param name="api">The API client</param>
    public DownloadsManager(BaseInformedDesignClient api)
    {
        _api = api;
    }

    /// <summary>
    /// Creates a new download outputs request and returns the download URLs with additional metadata for the requested outputs (Subject to token charges).
    /// </summary>
    /// <remarks>
    /// Wraps: POST /industrialized-construction/informed-design/v1/downloads
    /// APS docs: https://aps.autodesk.com/en/docs/informed-design/v1/reference/quick_reference/informed-design-api-postdownloads-POST    /// </remarks>
    /// <param name="body">The request body.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request such as headers, query parameters, and middleware options.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token to use when cancelling requests.</param>
    /// <returns>The <see cref="DownloadsPostResponse"/> from the API, or <c>null</c> if the response was empty.</returns>
    /// <example>
    /// <code>
    /// DownloadsPostRequestBody body = new();
    /// DownloadsPostResponse? created = await client.DownloadsManager.CreateDownloadAsync(body);
    /// </code>
    /// </example>
    public async Task<DownloadsPostResponse?> CreateDownloadAsync(
        DownloadsPostRequestBody body,
        RequestConfiguration<DownloadsRequestBuilder.DownloadsRequestBuilderPostQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.IndustrializedConstruction.InformedDesign.V1.Downloads
            .PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Gets a download outputs request by ID.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /industrialized-construction/informed-design/v1/downloads/{downloadRequestId}
    /// APS docs: https://aps.autodesk.com/en/docs/informed-design/v1/reference/quick_reference/informed-design-api-getdownloadoutputsrequestbyid-GET    /// </remarks>
    /// <param name="downloadRequestId">The unique identifier of the download request.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request such as headers, query parameters, and middleware options.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token to use when cancelling requests.</param>
    /// <returns>The <see cref="WithDownloadRequestGetResponse"/> for the download request, or <c>null</c> if not found.</returns>
    /// <example>
    /// <code>
    /// Guid downloadRequestId = Guid.Parse("00000000-0000-0000-0000-000000000000");
    /// WithDownloadRequestGetResponse? download = await client.DownloadsManager.GetDownloadAsync(downloadRequestId);
    /// </code>
    /// </example>
    public async Task<WithDownloadRequestGetResponse?> GetDownloadAsync(
        Guid downloadRequestId,
        RequestConfiguration<WithDownloadRequestItemRequestBuilder.WithDownloadRequestItemRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.IndustrializedConstruction.InformedDesign.V1.Downloads[downloadRequestId]
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }
}

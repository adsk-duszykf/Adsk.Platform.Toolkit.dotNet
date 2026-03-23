using Autodesk.InformedDesign.IndustrializedConstruction.InformedDesign.V1.Uploads;
using Autodesk.InformedDesign.IndustrializedConstruction.InformedDesign.V1.Uploads.Item;
using Microsoft.Kiota.Abstractions;
using System.Runtime.CompilerServices;

namespace Autodesk.InformedDesign.Managers;

/// <summary>
/// Manager for Uploads operations
/// </summary>
public class UploadsManager
{
    private readonly BaseInformedDesignClient _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="UploadsManager"/> class.
    /// </summary>
    /// <param name="api">The API client</param>
    public UploadsManager(BaseInformedDesignClient api)
    {
        _api = api;
    }

    /// <summary>
    /// Returns a paginated list of Upload Requests based on the <c>limit</c>, <c>offset</c> and <c>filter</c> query string parameters specified in the request.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /industrialized-construction/informed-design/v1/uploads
    /// APS docs: https://aps.autodesk.com/en/docs/informed-design/v1/reference/quick_reference/informed-design-api-getuploads-GET    /// </remarks>
    /// <param name="requestConfiguration">(Optional) Configuration for the request such as headers, query parameters, and middleware options.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token to use when cancelling requests.</param>
    /// <returns>An async sequence of <see cref="UploadsGetResponse_results"/> across all pages.</returns>
    /// <example>
    /// <code>
    /// await foreach (UploadsGetResponse_results upload in client.UploadsManager.ListUploadsAsync())
    /// {
    ///     _ = upload;
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<UploadsGetResponse_results> ListUploadsAsync(
        RequestConfiguration<UploadsRequestBuilder.UploadsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var offset = requestConfiguration?.QueryParameters?.Offset ?? 0;
        while (true)
        {
            var response = await _api.IndustrializedConstruction.InformedDesign.V1.Uploads
                .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                    if (r.QueryParameters is not null)
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
    /// Creates a new Upload Request to upload outputs to the client specified location. <strong>POST /uploads will charge / consume tokens per output once enabled.</strong>
    /// </summary>
    /// <remarks>
    /// Wraps: POST /industrialized-construction/informed-design/v1/uploads
    /// APS docs: https://aps.autodesk.com/en/docs/informed-design/v1/reference/quick_reference/informed-design-api-postuploads-POST    /// </remarks>
    /// <param name="body">The request body.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request such as headers, query parameters, and middleware options.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token to use when cancelling requests.</param>
    /// <returns>The <see cref="UploadsPostResponse"/> from the API, or <c>null</c> if the response was empty.</returns>
    /// <example>
    /// <code>
    /// UploadsPostRequestBody body = new();
    /// UploadsPostResponse? created = await client.UploadsManager.CreateUploadAsync(body);
    /// </code>
    /// </example>
    public async Task<UploadsPostResponse?> CreateUploadAsync(
        UploadsPostRequestBody body,
        RequestConfiguration<UploadsRequestBuilder.UploadsRequestBuilderPostQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.IndustrializedConstruction.InformedDesign.V1.Uploads
            .PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Returns the details of an Upload Request. The Upload Request is specified by the client provided <c>uploadRequestId</c> URI parameter.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /industrialized-construction/informed-design/v1/uploads/{uploadRequestId}
    /// APS docs: https://aps.autodesk.com/en/docs/informed-design/v1/reference/quick_reference/informed-design-api-getupload-GET    /// </remarks>
    /// <param name="uploadRequestId">The unique identifier of the upload request.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request such as headers, query parameters, and middleware options.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token to use when cancelling requests.</param>
    /// <returns>The <see cref="WithUploadRequestGetResponse"/> for the upload request, or <c>null</c> if not found.</returns>
    /// <example>
    /// <code>
    /// Guid uploadRequestId = Guid.Parse("00000000-0000-0000-0000-000000000000");
    /// WithUploadRequestGetResponse? upload = await client.UploadsManager.GetUploadAsync(uploadRequestId);
    /// </code>
    /// </example>
    public async Task<WithUploadRequestGetResponse?> GetUploadAsync(
        Guid uploadRequestId,
        RequestConfiguration<WithUploadRequestItemRequestBuilder.WithUploadRequestItemRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.IndustrializedConstruction.InformedDesign.V1.Uploads[uploadRequestId]
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }
}

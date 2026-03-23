using Autodesk.InformedDesign.IndustrializedConstruction.InformedDesign.V1.Outputs;
using Autodesk.InformedDesign.IndustrializedConstruction.InformedDesign.V1.Outputs.Item;
using Autodesk.InformedDesign.IndustrializedConstruction.InformedDesign.V1.OutputsRequest.Item;
using Microsoft.Kiota.Abstractions;
using System.Runtime.CompilerServices;

namespace Autodesk.InformedDesign.Managers;

/// <summary>
/// Manager for Outputs operations
/// </summary>
public class OutputsManager
{
    private readonly BaseInformedDesignClient _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="OutputsManager"/> class.
    /// </summary>
    /// <param name="api">The API client</param>
    public OutputsManager(BaseInformedDesignClient api)
    {
        _api = api;
    }

    /// <summary>
    /// Returns a paginated list of Outputs of a Release or a Variant based on the <c>limit</c>, <c>offset</c> and <c>filter</c> query string parameters specified in the request.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /industrialized-construction/informed-design/v1/outputs
    /// APS docs: https://aps.autodesk.com/en/docs/informed-design/v1/reference/quick_reference/informed-design-api-getoutputs-GET    /// </remarks>
    /// <param name="requestConfiguration">(Optional) Configuration for the request such as headers, query parameters, and middleware options.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token to use when cancelling requests.</param>
    /// <returns>An async sequence of <see cref="OutputsGetResponse_results"/> across all pages (one deserialized <c>results</c> payload per page as modeled by Kiota).</returns>
    /// <example>
    /// <code>
    /// await foreach (OutputsGetResponse_results output in client.OutputsManager.ListOutputsAsync())
    /// {
    ///     _ = output;
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<OutputsGetResponse_results> ListOutputsAsync(
        RequestConfiguration<OutputsRequestBuilder.OutputsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var offset = requestConfiguration?.QueryParameters?.Offset ?? 0;
        while (true)
        {
            var response = await _api.IndustrializedConstruction.InformedDesign.V1.Outputs
                .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                    if (r.QueryParameters is not null)
                        r.QueryParameters.Offset = offset;
                }, cancellationToken);
            if (response?.Results is null)
                yield break;
            yield return response.Results;
            if (string.IsNullOrEmpty(response.Pagination?.NextUrl))
                yield break;
            offset += 1;
        }
    }

    /// <summary>
    /// Create Outputs of a Variant.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /industrialized-construction/informed-design/v1/outputs
    /// APS docs: https://aps.autodesk.com/en/docs/informed-design/v1/reference/quick_reference/informed-design-api-postoutputs-POST    /// </remarks>
    /// <param name="body">The request body.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request such as headers, query parameters, and middleware options.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token to use when cancelling requests.</param>
    /// <returns>The <see cref="OutputsPostResponse"/> from the API, or <c>null</c> if the response was empty.</returns>
    /// <example>
    /// <code>
    /// OutputsPostRequestBody body = new();
    /// OutputsPostResponse? created = await client.OutputsManager.CreateOutputsAsync(body);
    /// </code>
    /// </example>
    public async Task<OutputsPostResponse?> CreateOutputsAsync(
        OutputsPostRequestBody body,
        RequestConfiguration<OutputsRequestBuilder.OutputsRequestBuilderPostQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.IndustrializedConstruction.InformedDesign.V1.Outputs
            .PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Returns the details of an Output of a Variant. The Output is specified by the client provided <c>outputId</c> URI parameter.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /industrialized-construction/informed-design/v1/outputs/{outputId}
    /// APS docs: https://aps.autodesk.com/en/docs/informed-design/v1/reference/quick_reference/informed-design-api-getoutput-GET    /// </remarks>
    /// <param name="outputId">The unique identifier of the output.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request such as headers, query parameters, and middleware options.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token to use when cancelling requests.</param>
    /// <returns>The <see cref="WithOutputGetResponse"/> for the output, or <c>null</c> if not found.</returns>
    /// <example>
    /// <code>
    /// Guid outputId = Guid.Parse("00000000-0000-0000-0000-000000000000");
    /// WithOutputGetResponse? output = await client.OutputsManager.GetOutputAsync(outputId);
    /// </code>
    /// </example>
    public async Task<WithOutputGetResponse?> GetOutputAsync(
        Guid outputId,
        RequestConfiguration<WithOutputItemRequestBuilder.WithOutputItemRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.IndustrializedConstruction.InformedDesign.V1.Outputs[outputId]
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Deletes an Output of a Variant. The Output is specified by the client provided <c>outputId</c> URI parameter.
    /// </summary>
    /// <remarks>
    /// Wraps: DELETE /industrialized-construction/informed-design/v1/outputs/{outputId}
    /// APS docs: https://aps.autodesk.com/en/docs/informed-design/v1/reference/quick_reference/informed-design-api-deleteoutput-DELETE    /// </remarks>
    /// <param name="outputId">The unique identifier of the output.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request such as headers, query parameters, and middleware options.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token to use when cancelling requests.</param>
    /// <returns>A <see cref="Task"/> that completes when the delete operation finishes.</returns>
    /// <example>
    /// <code>
    /// Guid outputId = Guid.Parse("00000000-0000-0000-0000-000000000000");
    /// await client.OutputsManager.DeleteOutputAsync(outputId);
    /// </code>
    /// </example>
    public async Task DeleteOutputAsync(
        Guid outputId,
        RequestConfiguration<WithOutputItemRequestBuilder.WithOutputItemRequestBuilderDeleteQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        await _api.IndustrializedConstruction.InformedDesign.V1.Outputs[outputId]
            .DeleteAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Returns the details of an Outputs Request. The Outputs Request is specified by the client provided <c>outputsRequestId</c> URI parameter.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /industrialized-construction/informed-design/v1/outputs-request/{outputsRequestId}
    /// APS docs: https://aps.autodesk.com/en/docs/informed-design/v1/reference/quick_reference/informed-design-api-getoutputsrequest-GET    /// </remarks>
    /// <param name="outputsRequestId">The unique identifier of the outputs request.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request such as headers, query parameters, and middleware options.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token to use when cancelling requests.</param>
    /// <returns>The <see cref="WithOutputsRequestGetResponse"/> for the request, or <c>null</c> if not found.</returns>
    /// <example>
    /// <code>
    /// Guid outputsRequestId = Guid.Parse("00000000-0000-0000-0000-000000000000");
    /// WithOutputsRequestGetResponse? request = await client.OutputsManager.GetOutputsRequestAsync(outputsRequestId);
    /// </code>
    /// </example>
    public async Task<WithOutputsRequestGetResponse?> GetOutputsRequestAsync(
        Guid outputsRequestId,
        RequestConfiguration<WithOutputsRequestItemRequestBuilder.WithOutputsRequestItemRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.IndustrializedConstruction.InformedDesign.V1.OutputsRequest[outputsRequestId]
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }
}

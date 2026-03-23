using Autodesk.InformedDesign.IndustrializedConstruction.InformedDesign.V1.Variants;
using Autodesk.InformedDesign.IndustrializedConstruction.InformedDesign.V1.Variants.Item;
using Microsoft.Kiota.Abstractions;
using System.Runtime.CompilerServices;

namespace Autodesk.InformedDesign.Managers;

/// <summary>
/// Manager for Variants operations
/// </summary>
public class VariantsManager
{
    private readonly BaseInformedDesignClient _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="VariantsManager"/> class.
    /// </summary>
    /// <param name="api">The API client</param>
    public VariantsManager(BaseInformedDesignClient api)
    {
        _api = api;
    }

    /// <summary>
    /// Returns a paginated list of Variants of a Release based on the <c>limit</c> and <c>offset</c> query string parameters specified in the request.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /industrialized-construction/informed-design/v1/variants
    /// APS docs: https://aps.autodesk.com/en/docs/informed-design/v1/reference/quick_reference/informed-design-api-getvariants-GET    /// </remarks>
    /// <param name="requestConfiguration">(Optional) Configuration for the request such as headers, query parameters, and middleware options.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token to use when cancelling requests.</param>
    /// <returns>An async sequence of <see cref="VariantsGetResponse_results"/> across all pages.</returns>
    /// <example>
    /// <code>
    /// await foreach (VariantsGetResponse_results variant in client.VariantsManager.ListVariantsAsync())
    /// {
    ///     _ = variant;
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<VariantsGetResponse_results> ListVariantsAsync(
        RequestConfiguration<VariantsRequestBuilder.VariantsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var offset = requestConfiguration?.QueryParameters?.Offset ?? 0;
        while (true)
        {
            var response = await _api.IndustrializedConstruction.InformedDesign.V1.Variants
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
    /// Creates a new Variant of a Release.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /industrialized-construction/informed-design/v1/variants
    /// APS docs: https://aps.autodesk.com/en/docs/informed-design/v1/reference/quick_reference/informed-design-api-postvariants-POST    /// </remarks>
    /// <param name="body">The request body.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request such as headers, query parameters, and middleware options.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token to use when cancelling requests.</param>
    /// <returns>The <see cref="VariantsPostResponse"/> from the API, or <c>null</c> if the response was empty.</returns>
    /// <example>
    /// <code>
    /// VariantsPostRequestBody body = new();
    /// VariantsPostResponse? created = await client.VariantsManager.CreateVariantAsync(body);
    /// </code>
    /// </example>
    public async Task<VariantsPostResponse?> CreateVariantAsync(
        VariantsPostRequestBody body,
        RequestConfiguration<VariantsRequestBuilder.VariantsRequestBuilderPostQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.IndustrializedConstruction.InformedDesign.V1.Variants
            .PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Returns the details of a Variant of a Release. The Variant is specified by the client provided <c>variantId</c> URI parameter.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /industrialized-construction/informed-design/v1/variants/{variantId}
    /// APS docs: https://aps.autodesk.com/en/docs/informed-design/v1/reference/quick_reference/informed-design-api-getvariant-GET    /// </remarks>
    /// <param name="variantId">The unique identifier of the variant.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request such as headers, query parameters, and middleware options.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token to use when cancelling requests.</param>
    /// <returns>The <see cref="WithVariantGetResponse"/> for the variant, or <c>null</c> if not found.</returns>
    /// <example>
    /// <code>
    /// Guid variantId = Guid.Parse("00000000-0000-0000-0000-000000000000");
    /// WithVariantGetResponse? variant = await client.VariantsManager.GetVariantAsync(variantId);
    /// </code>
    /// </example>
    public async Task<WithVariantGetResponse?> GetVariantAsync(
        Guid variantId,
        RequestConfiguration<WithVariantItemRequestBuilder.WithVariantItemRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.IndustrializedConstruction.InformedDesign.V1.Variants[variantId]
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Deletes a Variant of a Release, and all of its Outputs. The Variant is specified by the client provided <c>variantId</c> URI parameter.
    /// </summary>
    /// <remarks>
    /// Wraps: DELETE /industrialized-construction/informed-design/v1/variants/{variantId}
    /// APS docs: https://aps.autodesk.com/en/docs/informed-design/v1/reference/quick_reference/informed-design-api-deletevariant-DELETE    /// </remarks>
    /// <param name="variantId">The unique identifier of the variant.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request such as headers, query parameters, and middleware options.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token to use when cancelling requests.</param>
    /// <returns>A <see cref="Task"/> that completes when the delete operation finishes.</returns>
    /// <example>
    /// <code>
    /// Guid variantId = Guid.Parse("00000000-0000-0000-0000-000000000000");
    /// await client.VariantsManager.DeleteVariantAsync(variantId);
    /// </code>
    /// </example>
    public async Task DeleteVariantAsync(
        Guid variantId,
        RequestConfiguration<WithVariantItemRequestBuilder.WithVariantItemRequestBuilderDeleteQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        await _api.IndustrializedConstruction.InformedDesign.V1.Variants[variantId]
            .DeleteAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }
}

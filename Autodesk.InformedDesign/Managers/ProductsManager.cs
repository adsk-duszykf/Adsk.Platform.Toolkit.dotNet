using Autodesk.InformedDesign.IndustrializedConstruction.InformedDesign.V1.Products;
using Autodesk.InformedDesign.IndustrializedConstruction.InformedDesign.V1.Products.Item;
using Autodesk.InformedDesign.IndustrializedConstruction.InformedDesign.V1.Products.Item.CompleteUpload;
using Autodesk.InformedDesign.IndustrializedConstruction.InformedDesign.V1.Products.Item.DownloadUrl;
using Autodesk.InformedDesign.IndustrializedConstruction.InformedDesign.V1.Products.Item.Upload;
using Autodesk.InformedDesign.IndustrializedConstruction.InformedDesign.V1.Products.Item.UploadUrls;
using Microsoft.Kiota.Abstractions;
using System.Runtime.CompilerServices;

namespace Autodesk.InformedDesign.Managers;

/// <summary>
/// Manager for Products operations.
/// </summary>
public class ProductsManager
{
    private readonly BaseInformedDesignClient _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProductsManager"/> class.
    /// </summary>
    /// <param name="api">The Informed Design API client.</param>
    public ProductsManager(BaseInformedDesignClient api)
    {
        _api = api;
    }

    /// <summary>
    /// Returns a paginated list of Dynamic Content Definitions / Products based on the <c>limit</c> and <c>offset</c> query string parameters specified in the request.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /industrialized-construction/informed-design/v1/products
    /// APS docs: https://aps.autodesk.com/en/docs/informed-design/v1/reference/quick_reference/informed-design-api-getproducts-GET    /// </remarks>
    /// <param name="requestConfiguration">(Optional) Configuration for the request such as headers, query parameters, and middleware options.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token to use when cancelling requests.</param>
    /// <returns>An async sequence of <see cref="ProductsGetResponse_results"/> across all pages.</returns>
    /// <example>
    /// <code>
    /// await foreach (ProductsGetResponse_results product in client.ProductsManager.ListProductsAsync())
    /// {
    ///     _ = product;
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<ProductsGetResponse_results> ListProductsAsync(
        RequestConfiguration<ProductsRequestBuilder.ProductsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        int offset = requestConfiguration?.QueryParameters?.Offset ?? 0;
        while (true)
        {
            var response = await _api.IndustrializedConstruction.InformedDesign.V1.Products
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
    /// Creates a new Dynamic Content Definition / Product.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /industrialized-construction/informed-design/v1/products
    /// APS docs: https://aps.autodesk.com/en/docs/informed-design/v1/reference/quick_reference/informed-design-api-postproducts-POST    /// </remarks>
    /// <param name="body">The request body.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request such as headers, query parameters, and middleware options.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token to use when cancelling requests.</param>
    /// <returns>The <see cref="ProductsPostResponse"/> from the API, or <c>null</c> if the response was empty.</returns>
    /// <example>
    /// <code>
    /// ProductsPostRequestBody body = new();
    /// ProductsPostResponse? created = await client.ProductsManager.CreateProductAsync(body);
    /// </code>
    /// </example>
    public async Task<ProductsPostResponse?> CreateProductAsync(
        ProductsPostRequestBody body,
        RequestConfiguration<ProductsRequestBuilder.ProductsRequestBuilderPostQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.IndustrializedConstruction.InformedDesign.V1.Products
            .PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Returns the details of a Dynamic Content Definition / Product specified by the client provided <c>productId</c> URI parameter.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /industrialized-construction/informed-design/v1/products/{productId}
    /// APS docs: https://aps.autodesk.com/en/docs/informed-design/v1/reference/quick_reference/informed-design-api-getproduct-GET    /// </remarks>
    /// <param name="productId">The unique identifier of the product.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request such as headers, query parameters, and middleware options.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token to use when cancelling requests.</param>
    /// <returns>The <see cref="WithProductGetResponse"/> for the product, or <c>null</c> if not found.</returns>
    /// <example>
    /// <code>
    /// Guid productId = Guid.Parse("00000000-0000-0000-0000-000000000000");
    /// WithProductGetResponse? product = await client.ProductsManager.GetProductAsync(productId);
    /// </code>
    /// </example>
    public async Task<WithProductGetResponse?> GetProductAsync(
        Guid productId,
        RequestConfiguration<WithProductItemRequestBuilder.WithProductItemRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var response = await _api.IndustrializedConstruction.InformedDesign.V1.Products[productId]
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
        return response;
    }

    /// <summary>
    /// Updates a Dynamic Content Definition / Product specified by the client provided <c>productId</c> URI parameter.
    /// </summary>
    /// <remarks>
    /// Wraps: PATCH /industrialized-construction/informed-design/v1/products/{productId}
    /// APS docs: https://aps.autodesk.com/en/docs/informed-design/v1/reference/quick_reference/informed-design-api-patchproduct-PATCH    /// </remarks>
    /// <param name="productId">The unique identifier of the product.</param>
    /// <param name="body">The request body.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request such as headers, query parameters, and middleware options.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token to use when cancelling requests.</param>
    /// <returns>The <see cref="WithProductPatchResponse"/> from the API, or <c>null</c> if the response was empty.</returns>
    /// <example>
    /// <code>
    /// Guid productId = Guid.Parse("00000000-0000-0000-0000-000000000000");
    /// WithProductPatchRequestBody body = new();
    /// WithProductPatchResponse? updated = await client.ProductsManager.UpdateProductAsync(productId, body);
    /// </code>
    /// </example>
    public async Task<WithProductPatchResponse?> UpdateProductAsync(
        Guid productId,
        WithProductPatchRequestBody body,
        RequestConfiguration<WithProductItemRequestBuilder.WithProductItemRequestBuilderPatchQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.IndustrializedConstruction.InformedDesign.V1.Products[productId]
            .PatchAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Deletes a Dynamic Content Definition / Product specified by the client provided <c>productId</c> URI parameter, and all of its Releases, Variants and Outputs.
    /// </summary>
    /// <remarks>
    /// Wraps: DELETE /industrialized-construction/informed-design/v1/products/{productId}
    /// APS docs: https://aps.autodesk.com/en/docs/informed-design/v1/reference/quick_reference/informed-design-api-deleteproduct-DELETE    /// </remarks>
    /// <param name="productId">The unique identifier of the product.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request such as headers, query parameters, and middleware options.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token to use when cancelling requests.</param>
    /// <returns>A <see cref="Task"/> that completes when the delete operation finishes.</returns>
    /// <example>
    /// <code>
    /// Guid productId = Guid.Parse("00000000-0000-0000-0000-000000000000");
    /// await client.ProductsManager.DeleteProductAsync(productId);
    /// </code>
    /// </example>
    public async Task DeleteProductAsync(
        Guid productId,
        RequestConfiguration<WithProductItemRequestBuilder.WithProductItemRequestBuilderDeleteQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        await _api.IndustrializedConstruction.InformedDesign.V1.Products[productId]
            .DeleteAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Returns upload URLs to upload a file to a Product data set. The Product is specified by the client provided <c>productId</c> URI parameter.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /industrialized-construction/informed-design/v1/products/{productId}/upload-urls
    /// APS docs: https://aps.autodesk.com/en/docs/informed-design/v1/reference/quick_reference/informed-design-api-postuploadurls-POST    /// </remarks>
    /// <param name="productId">The unique identifier of the product.</param>
    /// <param name="body">The request body.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request such as headers, query parameters, and middleware options.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token to use when cancelling requests.</param>
    /// <returns>The <see cref="UploadUrlsPostResponse"/> from the API, or <c>null</c> if the response was empty.</returns>
    /// <example>
    /// <code>
    /// Guid productId = Guid.Parse("00000000-0000-0000-0000-000000000000");
    /// UploadUrlsPostRequestBody body = new();
    /// UploadUrlsPostResponse? urls = await client.ProductsManager.GetUploadUrlsAsync(productId, body);
    /// </code>
    /// </example>
    public async Task<UploadUrlsPostResponse?> GetUploadUrlsAsync(
        Guid productId,
        UploadUrlsPostRequestBody body,
        RequestConfiguration<UploadUrlsRequestBuilder.UploadUrlsRequestBuilderPostQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.IndustrializedConstruction.InformedDesign.V1.Products[productId].UploadUrls
            .PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Marks a Product data set upload as complete. The Product is specified by the client provided <c>productId</c> URI parameter.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /industrialized-construction/informed-design/v1/products/{productId}/complete-upload
    /// APS docs: https://aps.autodesk.com/en/docs/informed-design/v1/reference/quick_reference/informed-design-api-postcompleteupload-POST    /// </remarks>
    /// <param name="productId">The unique identifier of the product.</param>
    /// <param name="body">The request body.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request such as headers, query parameters, and middleware options.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token to use when cancelling requests.</param>
    /// <returns>The <see cref="CompleteUploadPostResponse"/> from the API, or <c>null</c> if the response was empty.</returns>
    /// <example>
    /// <code>
    /// Guid productId = Guid.Parse("00000000-0000-0000-0000-000000000000");
    /// CompleteUploadPostRequestBody body = new();
    /// CompleteUploadPostResponse? result = await client.ProductsManager.CompleteUploadAsync(productId, body);
    /// </code>
    /// </example>
    public async Task<CompleteUploadPostResponse?> CompleteUploadAsync(
        Guid productId,
        CompleteUploadPostRequestBody body,
        RequestConfiguration<CompleteUploadRequestBuilder.CompleteUploadRequestBuilderPostQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.IndustrializedConstruction.InformedDesign.V1.Products[productId].CompleteUpload
            .PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Get a signed URL to download Product content that was uploaded. The Product is specified by the client provided <c>productId</c> URI parameter.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /industrialized-construction/informed-design/v1/products/{productId}/download-url
    /// APS docs: https://aps.autodesk.com/en/docs/informed-design/v1/reference/quick_reference/informed-design-api-getdownloadurl-GET    /// </remarks>
    /// <param name="productId">The unique identifier of the product.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request such as headers, query parameters, and middleware options.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token to use when cancelling requests.</param>
    /// <returns>The <see cref="DownloadUrlGetResponse"/> from the API, or <c>null</c> if the response was empty.</returns>
    /// <example>
    /// <code>
    /// Guid productId = Guid.Parse("00000000-0000-0000-0000-000000000000");
    /// DownloadUrlGetResponse? download = await client.ProductsManager.GetDownloadUrlAsync(productId);
    /// </code>
    /// </example>
    public async Task<DownloadUrlGetResponse?> GetDownloadUrlAsync(
        Guid productId,
        RequestConfiguration<DownloadUrlRequestBuilder.DownloadUrlRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var response = await _api.IndustrializedConstruction.InformedDesign.V1.Products[productId].DownloadUrl
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
        return response;
    }

    /// <summary>
    /// Deletes Product content uploaded from the upload endpoints. The Product is specified by the client provided <c>productId</c> URI parameter.
    /// </summary>
    /// <remarks>
    /// Wraps: DELETE /industrialized-construction/informed-design/v1/products/{productId}/upload
    /// APS docs: https://aps.autodesk.com/en/docs/informed-design/v1/reference/quick_reference/informed-design-api-deleteupload-DELETE    /// </remarks>
    /// <param name="productId">The unique identifier of the product.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request such as headers, query parameters, and middleware options.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token to use when cancelling requests.</param>
    /// <returns>A <see cref="Task"/> that completes when the delete operation finishes.</returns>
    /// <example>
    /// <code>
    /// Guid productId = Guid.Parse("00000000-0000-0000-0000-000000000000");
    /// await client.ProductsManager.DeleteUploadAsync(productId);
    /// </code>
    /// </example>
    public async Task DeleteUploadAsync(
        Guid productId,
        RequestConfiguration<UploadRequestBuilder.UploadRequestBuilderDeleteQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        await _api.IndustrializedConstruction.InformedDesign.V1.Products[productId].Upload
            .DeleteAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }
}

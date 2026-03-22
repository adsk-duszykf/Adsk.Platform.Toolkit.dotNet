using System.Runtime.CompilerServices;
using Autodesk.BIM360.Bim360.Assets.V1.ErrorCodes;
using Autodesk.BIM360.Bim360.Assets.V1.ErrorCodes.Item;
using Autodesk.BIM360.Bim360.Assets.V1.Projects.Item.AssetStatuses;
using Autodesk.BIM360.Bim360.Assets.V1.Projects.Item.AssetStatusesBatchGet;
using Autodesk.BIM360.Bim360.Assets.V1.Projects.Item.Categories;
using AssetRelationships = Autodesk.BIM360.Bim360.Assets.V1.Projects.Item.Assets.Item.Relationships;
using CategoryRelationships = Autodesk.BIM360.Bim360.Assets.V1.Projects.Item.Categories.Item.Relationships;
using Autodesk.BIM360.Bim360.Assets.V1.Projects.Item.Categories.Item.StatusStepSet.Item;
using Autodesk.BIM360.Bim360.Assets.V1.Projects.Item.CategoriesBatchGet;
using Autodesk.BIM360.Bim360.Assets.V1.Projects.Item.CustomAttributes;
using CategoryStatusStepSetsBatchGet = Autodesk.BIM360.Bim360.Assets.V1.Projects.Item.CategoryStatusStepSets.StatusStepSetsBatchGet;
using Autodesk.BIM360.Bim360.Assets.V1.Projects.Item.CustomAttributes.Item;
using Autodesk.BIM360.Bim360.Assets.V1.Projects.Item.CustomAttributesBatchGet;
using Autodesk.BIM360.Bim360.Assets.V1.Projects.Item.RelationshipsDelete;
using Autodesk.BIM360.Bim360.Assets.V1.Projects.Item.StatusStepSets;
using Autodesk.BIM360.Bim360.Assets.V2.Projects.Item.Assets;
using Autodesk.BIM360.Bim360.Assets.V2.Projects.Item.AssetsBatchCreate;
using Autodesk.BIM360.Bim360.Assets.V2.Projects.Item.AssetsBatchDelete;
using Autodesk.BIM360.Bim360.Assets.V2.Projects.Item.AssetsBatchGet;
using Autodesk.BIM360.Bim360.Assets.V2.Projects.Item.AssetsBatchPatch;
using CategoryCustomAttributes = Autodesk.BIM360.Bim360.Assets.V1.Projects.Item.Categories.Item.CustomAttributes;
using ProjectCustomAttributes = Autodesk.BIM360.Bim360.Assets.V1.Projects.Item.CustomAttributes;
using ProjectStatusStepSetsBatchGet = Autodesk.BIM360.Bim360.Assets.V1.Projects.Item.StatusStepSetsBatchGet;
using Microsoft.Kiota.Abstractions;

namespace Autodesk.BIM360.Managers;

/// <summary>
/// Manager for BIM 360 Assets — asset records (v2), categories, status sets, asset statuses, custom attributes, error codes, and legacy relationship endpoints.
/// </summary>
public class AssetsManager
{
    private readonly BaseBIM360client _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="AssetsManager"/> class.
    /// </summary>
    /// <param name="api">The API client</param>
    public AssetsManager(BaseBIM360client api)
    {
        _api = api;
    }

    /// <summary>
    /// Searches for and returns all specified assets within a project with automatic pagination (cursor).
    /// </summary>
    /// <remarks>
    /// Wraps: GET /bim360/assets/v2/projects/{projectId}/assets
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/assets-assets-v2-GET
    /// </remarks>
    /// <param name="projectId">The BIM 360 project ID (UUID or b.{UUID})</param>
    /// <param name="requestConfiguration">(Optional) Configuration (filter, sort, limit, includeCustomAttributes, includeDeleted)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{AssetsGetResponse_results}"/> of asset rows, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (AssetsGetResponse_results asset in client.AssetsManager.ListAssetsAsync("b.project-guid"))
    /// {
    ///     Console.WriteLine(asset.ClientAssetId);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<AssetsGetResponse_results> ListAssetsAsync(
        string projectId,
        RequestConfiguration<AssetsRequestBuilder.AssetsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? cursorState = requestConfiguration?.QueryParameters?.CursorState;

        while (true)
        {
            AssetsGetResponse? response = await _api.Bim360.Assets.V2.Projects[projectId]
                .Assets
                .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                    r.QueryParameters.CursorState = cursorState;
                }, cancellationToken);

            if (response?.Results is not { Count: > 0 })
                yield break;

            foreach (AssetsGetResponse_results item in response.Results)
            {
                yield return item;
            }

            cursorState = response.Pagination?.CursorState;
            if (string.IsNullOrEmpty(cursorState))
                yield break;
        }
    }

    /// <summary>
    /// Creates a set of new assets.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /bim360/assets/v2/projects/{projectId}/assets:batch-create
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/assets-assets-batch-create-POST-v2
    /// </remarks>
    /// <param name="projectId">The project ID</param>
    /// <param name="body">The batch create request body</param>
    /// <param name="requestConfiguration">(Optional) Configuration</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>The batch create response</returns>
    /// <example>
    /// <code>
    /// AssetsBatchCreatePostResponse? result = await client.AssetsManager.BatchCreateAssetsAsync("b.project-guid", new AssetsBatchCreatePostRequestBody());
    /// </code>
    /// </example>
    public async Task<AssetsBatchCreatePostResponse?> BatchCreateAssetsAsync(
        string projectId,
        AssetsBatchCreatePostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Assets.V2.Projects[projectId]
            .AssetsBatchCreate
            .PostAsync(body, r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Returns a specified set of assets by ID.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /bim360/assets/v2/projects/{projectId}/assets:batch-get
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/assets-assets-batch-get-v2-POST
    /// </remarks>
    public async Task<AssetsBatchGetPostResponse?> BatchGetAssetsAsync(
        string projectId,
        AssetsBatchGetPostRequestBody body,
        RequestConfiguration<AssetsBatchGetRequestBuilder.AssetsBatchGetRequestBuilderPostQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Assets.V2.Projects[projectId]
            .AssetsBatchGet
            .PostAsync(body, r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Updates a set of one or more assets.
    /// </summary>
    /// <remarks>
    /// Wraps: PATCH /bim360/assets/v2/projects/{projectId}/assets:batch-patch
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/assets-assets-batch-patch-PATCH-v2
    /// </remarks>
    public async Task<AssetsBatchPatchPatchResponse?> BatchPatchAssetsAsync(
        string projectId,
        AssetsBatchPatchPatchRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Assets.V2.Projects[projectId]
            .AssetsBatchPatch
            .PatchAsync(body, r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Deletes one or more assets.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /bim360/assets/v2/projects/{projectId}/assets:batch-delete
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/assets-assets-batch-delete-v2-POST
    /// </remarks>
    public async Task BatchDeleteAssetsAsync(
        string projectId,
        AssetsBatchDeletePostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        await _api.Bim360.Assets.V2.Projects[projectId]
            .AssetsBatchDelete
            .PostAsync(body, r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Searches for and returns categories in a project.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /bim360/assets/v1/projects/{projectId}/categories
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/assets-categories-GET
    /// </remarks>
    public async Task<CategoriesGetResponse?> GetCategoriesAsync(
        string projectId,
        RequestConfiguration<CategoriesRequestBuilder.CategoriesRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Assets.V1.Projects[projectId]
            .Categories
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Creates a new category.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /bim360/assets/v1/projects/{projectId}/categories
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/assets-categories-POST
    /// </remarks>
    public async Task<CategoriesPostResponse?> CreateCategoryAsync(
        string projectId,
        CategoriesPostRequestBody body,
        RequestConfiguration<CategoriesRequestBuilder.CategoriesRequestBuilderPostQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Assets.V1.Projects[projectId]
            .Categories
            .PostAsync(body, r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Returns a specified set of categories by ID.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /bim360/assets/v1/projects/{projectId}/categories:batch-get
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/assets-categories-batch-get-POST
    /// </remarks>
    public async Task<CategoriesBatchGetPostResponse?> BatchGetCategoriesAsync(
        string projectId,
        CategoriesBatchGetPostRequestBody body,
        RequestConfiguration<CategoriesBatchGetRequestBuilder.CategoriesBatchGetRequestBuilderPostQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Assets.V1.Projects[projectId]
            .CategoriesBatchGet
            .PostAsync(body, r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Assigns a status set to a category (HTTP PUT in the underlying API).
    /// </summary>
    /// <remarks>
    /// Wraps: PUT /bim360/assets/v1/projects/{projectId}/categories/{categoryId}/status-step-set/{statusStepSetId}
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/assets-categories-category-id-status-step-set-status-step-set-id-PUT
    /// </remarks>
    public async Task<WithStatusStepSetPutResponse?> SetCategoryStatusStepSetAsync(
        string projectId,
        string categoryId,
        Guid statusStepSetId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Assets.V1.Projects[projectId]
            .Categories[categoryId]
            .StatusStepSet[statusStepSetId]
            .PutAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Returns the custom attribute assignments for a specified category.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /bim360/assets/v1/projects/{projectId}/categories/{categoryId}/custom-attributes
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/assets-categories-category-id-custom-attributes-GET
    /// </remarks>
    public async Task<CategoryCustomAttributes.CustomAttributesGetResponse?> GetCategoryCustomAttributesAsync(
        string projectId,
        string categoryId,
        RequestConfiguration<CategoryCustomAttributes.CustomAttributesRequestBuilder.CustomAttributesRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Assets.V1.Projects[projectId]
            .Categories[categoryId]
            .CustomAttributes
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Assigns an asset custom attribute to a category (HTTP PUT in the underlying API).
    /// </summary>
    /// <remarks>
    /// Wraps: PUT /bim360/assets/v1/projects/{projectId}/categories/{categoryId}/custom-attributes/{customAttributeId}
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/assets-categories-category-id-custom-attributes-custom-attribute-id-PUT
    /// </remarks>
    public async Task<CategoryCustomAttributes.Item.WithCustomAttributePutResponse?> AssignCustomAttributeToCategoryAsync(
        string projectId,
        string categoryId,
        Guid customAttributeId,
        RequestConfiguration<CategoryCustomAttributes.Item.WithCustomAttributeItemRequestBuilder.WithCustomAttributeItemRequestBuilderPutQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Assets.V1.Projects[projectId]
            .Categories[categoryId]
            .CustomAttributes[customAttributeId]
            .PutAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Searches for and returns all status sets in a project with automatic pagination (cursor).
    /// </summary>
    /// <remarks>
    /// Wraps: GET /bim360/assets/v1/projects/{projectId}/status-step-sets
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/assets-status-step-sets-GET
    /// </remarks>
    public async IAsyncEnumerable<StatusStepSetsGetResponse_results> ListStatusStepSetsAsync(
        string projectId,
        RequestConfiguration<StatusStepSetsRequestBuilder.StatusStepSetsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? cursorState = requestConfiguration?.QueryParameters?.CursorState;

        while (true)
        {
            StatusStepSetsGetResponse? response = await _api.Bim360.Assets.V1.Projects[projectId]
                .StatusStepSets
                .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                    r.QueryParameters.CursorState = cursorState;
                }, cancellationToken);

            if (response?.Results is not { Count: > 0 })
                yield break;

            foreach (StatusStepSetsGetResponse_results item in response.Results)
            {
                yield return item;
            }

            cursorState = response.Pagination?.CursorState;
            if (string.IsNullOrEmpty(cursorState))
                yield break;
        }
    }

    /// <summary>
    /// Creates a new status set.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /bim360/assets/v1/projects/{projectId}/status-step-sets
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/assets-status-step-sets-POST
    /// </remarks>
    public async Task<StatusStepSetsPostResponse?> CreateStatusStepSetAsync(
        string projectId,
        StatusStepSetsPostRequestBody body,
        RequestConfiguration<StatusStepSetsRequestBuilder.StatusStepSetsRequestBuilderPostQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Assets.V1.Projects[projectId]
            .StatusStepSets
            .PostAsync(body, r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Returns a specified set of status step sets by ID.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /bim360/assets/v1/projects/{projectId}/status-step-sets:batch-get
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/assets-status-step-sets-batch-get-POST
    /// </remarks>
    public async Task<ProjectStatusStepSetsBatchGet.StatusStepSetsBatchGetPostResponse?> BatchGetStatusStepSetsAsync(
        string projectId,
        ProjectStatusStepSetsBatchGet.StatusStepSetsBatchGetPostRequestBody body,
        RequestConfiguration<ProjectStatusStepSetsBatchGet.StatusStepSetsBatchGetRequestBuilder.StatusStepSetsBatchGetRequestBuilderPostQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Assets.V1.Projects[projectId]
            .StatusStepSetsBatchGet
            .PostAsync(body, r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Returns status step sets for specified categories.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /bim360/assets/v1/projects/{projectId}/category-status-step-sets/status-step-sets:batch-get
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/assets-category-status-step-sets-status-step-sets-batch-get-POST
    /// </remarks>
    public async Task<CategoryStatusStepSetsBatchGet.StatusStepSetsBatchGetPostResponse?> BatchGetCategoryStatusStepSetsAsync(
        string projectId,
        CategoryStatusStepSetsBatchGet.StatusStepSetsBatchGetPostRequestBody body,
        RequestConfiguration<CategoryStatusStepSetsBatchGet.StatusStepSetsBatchGetRequestBuilder.StatusStepSetsBatchGetRequestBuilderPostQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Assets.V1.Projects[projectId]
            .CategoryStatusStepSets
            .StatusStepSetsBatchGet
            .PostAsync(body, r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Searches for and returns all asset statuses in a project with automatic pagination (cursor).
    /// </summary>
    /// <remarks>
    /// Wraps: GET /bim360/assets/v1/projects/{projectId}/asset-statuses
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/assets-asset-statuses-GET
    /// </remarks>
    public async IAsyncEnumerable<AssetStatusesGetResponse_results> ListAssetStatusesAsync(
        string projectId,
        RequestConfiguration<AssetStatusesRequestBuilder.AssetStatusesRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? cursorState = requestConfiguration?.QueryParameters?.CursorState;

        while (true)
        {
            AssetStatusesGetResponse? response = await _api.Bim360.Assets.V1.Projects[projectId]
                .AssetStatuses
                .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                    r.QueryParameters.CursorState = cursorState;
                }, cancellationToken);

            if (response?.Results is not { Count: > 0 })
                yield break;

            foreach (AssetStatusesGetResponse_results item in response.Results)
            {
                yield return item;
            }

            cursorState = response.Pagination?.CursorState;
            if (string.IsNullOrEmpty(cursorState))
                yield break;
        }
    }

    /// <summary>
    /// Creates a new asset status.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /bim360/assets/v1/projects/{projectId}/asset-statuses
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/assets-asset-statuses-POST
    /// </remarks>
    public async Task<AssetStatusesPostResponse?> CreateAssetStatusAsync(
        string projectId,
        AssetStatusesPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Assets.V1.Projects[projectId]
            .AssetStatuses
            .PostAsync(body, r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Returns a specified set of asset statuses by ID.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /bim360/assets/v1/projects/{projectId}/asset-statuses:batch-get
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/assets-asset-statuses-batch-get-POST
    /// </remarks>
    public async Task<AssetStatusesBatchGetPostResponse?> BatchGetAssetStatusesAsync(
        string projectId,
        AssetStatusesBatchGetPostRequestBody body,
        RequestConfiguration<AssetStatusesBatchGetRequestBuilder.AssetStatusesBatchGetRequestBuilderPostQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Assets.V1.Projects[projectId]
            .AssetStatusesBatchGet
            .PostAsync(body, r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Searches for and returns all custom attributes in a project with automatic pagination (cursor).
    /// </summary>
    /// <remarks>
    /// Wraps: GET /bim360/assets/v1/projects/{projectId}/custom-attributes
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/assets-custom-attributes-GET
    /// </remarks>
    public async IAsyncEnumerable<ProjectCustomAttributes.CustomAttributesGetResponse_results> ListCustomAttributesAsync(
        string projectId,
        RequestConfiguration<ProjectCustomAttributes.CustomAttributesRequestBuilder.CustomAttributesRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? cursorState = requestConfiguration?.QueryParameters?.CursorState;

        while (true)
        {
            ProjectCustomAttributes.CustomAttributesGetResponse? response = await _api.Bim360.Assets.V1.Projects[projectId]
                .CustomAttributes
                .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                    r.QueryParameters.CursorState = cursorState;
                }, cancellationToken);

            if (response?.Results is not { Count: > 0 })
                yield break;

            foreach (ProjectCustomAttributes.CustomAttributesGetResponse_results item in response.Results)
            {
                yield return item;
            }

            cursorState = response.Pagination?.CursorState;
            if (string.IsNullOrEmpty(cursorState))
                yield break;
        }
    }

    /// <summary>
    /// Creates a new asset custom attribute.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /bim360/assets/v1/projects/{projectId}/custom-attributes
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/assets-custom-attributes-POST
    /// </remarks>
    public async Task<CustomAttributesPostResponse?> CreateCustomAttributeAsync(
        string projectId,
        CustomAttributesPostRequestBody body,
        RequestConfiguration<ProjectCustomAttributes.CustomAttributesRequestBuilder.CustomAttributesRequestBuilderPostQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Assets.V1.Projects[projectId]
            .CustomAttributes
            .PostAsync(body, r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Returns a specified set of custom attributes by ID.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /bim360/assets/v1/projects/{projectId}/custom-attributes:batch-get
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/assets-custom-attributes-batch-get-POST
    /// </remarks>
    public async Task<CustomAttributesBatchGetPostResponse?> BatchGetCustomAttributesAsync(
        string projectId,
        CustomAttributesBatchGetPostRequestBody body,
        RequestConfiguration<CustomAttributesBatchGetRequestBuilder.CustomAttributesBatchGetRequestBuilderPostQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Assets.V1.Projects[projectId]
            .CustomAttributesBatchGet
            .PostAsync(body, r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Updates an asset custom attribute.
    /// </summary>
    /// <remarks>
    /// Wraps: PATCH /bim360/assets/v1/projects/{projectId}/custom-attributes/{customAttributeId}
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/assets-custom-attributes-custom-attribute-id-PATCH
    /// </remarks>
    public async Task<WithCustomAttributePatchResponse?> UpdateCustomAttributeAsync(
        string projectId,
        Guid customAttributeId,
        WithCustomAttributePatchRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Assets.V1.Projects[projectId]
            .CustomAttributes[customAttributeId]
            .PatchAsync(body, r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Retrieves a list of all error codes returned by the Assets API.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /bim360/assets/v1/error-codes
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/assets-error-codes-GET
    /// </remarks>
    public async Task<ErrorCodesGetResponse?> GetErrorCodesAsync(
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Assets.V1.ErrorCodes
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Retrieves details for a specific error code by name.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /bim360/assets/v1/error-codes/{errorCodeName}
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/assets-error-codes-error-code-name-GET
    /// </remarks>
    public async Task<WithErrorCodeNameGetResponse?> GetErrorCodeAsync(
        string errorCodeName,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Assets.V1.ErrorCodes[errorCodeName]
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Creates relationships for an asset (deprecated in APS documentation).
    /// </summary>
    /// <remarks>
    /// Wraps: POST /bim360/assets/v1/projects/{projectId}/assets/{assetId}/relationships
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/assets-assets-asset-id-relationships-POST
    /// </remarks>
    public async Task<AssetRelationships.RelationshipsPostResponse?> CreateAssetRelationshipsAsync(
        string projectId,
        Guid assetId,
        AssetRelationships.RelationshipsPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Assets.V1.Projects[projectId]
            .Assets[assetId]
            .Relationships
            .PostAsync(body, r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Creates relationships for a category (deprecated in APS documentation).
    /// </summary>
    /// <remarks>
    /// Wraps: POST /bim360/assets/v1/projects/{projectId}/categories/{categoryId}/relationships
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/assets-categories-category-id-relationships-POST
    /// </remarks>
    public async Task<CategoryRelationships.RelationshipsPostResponse?> CreateCategoryRelationshipsAsync(
        string projectId,
        string categoryId,
        CategoryRelationships.RelationshipsPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Assets.V1.Projects[projectId]
            .Categories[categoryId]
            .Relationships
            .PostAsync(body, r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Deletes relationships in bulk (deprecated in APS documentation).
    /// </summary>
    /// <remarks>
    /// Wraps: POST /bim360/assets/v1/projects/{projectId}/relationships:delete
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/assets-relationships-delete-POST
    /// </remarks>
    public async Task<RelationshipsDeletePostResponse?> DeleteRelationshipsAsync(
        string projectId,
        RelationshipsDeletePostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Assets.V1.Projects[projectId]
            .RelationshipsDelete
            .PostAsync(body, r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }
}

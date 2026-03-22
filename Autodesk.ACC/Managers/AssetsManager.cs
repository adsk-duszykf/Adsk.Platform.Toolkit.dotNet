using System.Runtime.CompilerServices;
using Autodesk.ACC.Construction.Assets.V1.ErrorCodes;
using Autodesk.ACC.Construction.Assets.V1.ErrorCodes.Item;
using Autodesk.ACC.Construction.Assets.V1.Projects.Item.AssetStatuses;
using Autodesk.ACC.Construction.Assets.V1.Projects.Item.AssetStatusesBatchGet;
using Autodesk.ACC.Construction.Assets.V1.Projects.Item.Categories;
using Autodesk.ACC.Construction.Assets.V1.Projects.Item.Categories.Item.StatusStepSet.Item;
using Autodesk.ACC.Construction.Assets.V1.Projects.Item.CategoriesBatchGet;
using Autodesk.ACC.Construction.Assets.V1.Projects.Item.CustomAttributes;
using CategoryCustomAttributes = Autodesk.ACC.Construction.Assets.V1.Projects.Item.Categories.Item.CustomAttributes;
using ProjectCustomAttributes = Autodesk.ACC.Construction.Assets.V1.Projects.Item.CustomAttributes;
using ProjectStatusStepSetsBatchGet = Autodesk.ACC.Construction.Assets.V1.Projects.Item.StatusStepSetsBatchGet;
using Autodesk.ACC.Construction.Assets.V1.Projects.Item.CustomAttributes.Item;
using Autodesk.ACC.Construction.Assets.V1.Projects.Item.CustomAttributesBatchGet;
using Autodesk.ACC.Construction.Assets.V1.Projects.Item.StatusStepSets;
using Autodesk.ACC.Construction.Assets.V2.Projects.Item.Assets;
using Autodesk.ACC.Construction.Assets.V2.Projects.Item.AssetsBatchCreate;
using Autodesk.ACC.Construction.Assets.V2.Projects.Item.AssetsBatchDelete;
using Autodesk.ACC.Construction.Assets.V2.Projects.Item.AssetsBatchGet;
using Autodesk.ACC.Construction.Assets.V2.Projects.Item.AssetsBatchPatch;
using Microsoft.Kiota.Abstractions;

namespace Autodesk.ACC.Managers;

/// <summary>
/// Manager for Assets operations — manages asset categories, statuses, custom attributes, and asset records.
/// </summary>
public class AssetsManager
{
    private readonly BaseACCclient _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="AssetsManager"/> class.
    /// </summary>
    /// <param name="api">The API client</param>
    public AssetsManager(BaseACCclient api)
    {
        _api = api;
    }

    /// <summary>
    /// Searches for and returns all specified assets within a project with automatic pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/assets/v2/projects/{projectId}/assets
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/assets-assets-v2-GET
    /// </remarks>
    /// <param name="projectId">The Autodesk Construction Cloud project ID (UUID or b.{UUID})</param>
    /// <param name="requestConfiguration">(Optional) Configuration (filter, sort, limit, includeCustomAttributes, includeDeleted)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{AssetsGetResponse_results}"/> of <see cref="AssetsGetResponse_results"/> items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (var asset in client.AssetsManager.ListAssetsAsync("b.project-guid"))
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
            var response = await _api.Construction.Assets.V2.Projects[projectId]
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

            foreach (var item in response.Results)
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
    /// Wraps: POST /construction/assets/v2/projects/{projectId}/assets:batch-create
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/assets-assets-batch-create-POST-v2
    /// </remarks>
    /// <param name="projectId">The project ID</param>
    /// <param name="body">The batch create request body</param>
    /// <param name="requestConfiguration">(Optional) Configuration</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="AssetsBatchCreatePostResponse"/> containing the created assets</returns>
    /// <example>
    /// <code>
    /// AssetsBatchCreatePostResponse? result = await client.AssetsManager.BatchCreateAssetsAsync("b.project-guid", new AssetsBatchCreatePostRequestBody { Assets = [...] });
    /// </code>
    /// </example>
    public async Task<AssetsBatchCreatePostResponse?> BatchCreateAssetsAsync(
        string projectId,
        AssetsBatchCreatePostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Assets.V2.Projects[projectId]
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
    /// Wraps: POST /construction/assets/v2/projects/{projectId}/assets:batch-get
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/assets-assets-batch-get-v2-POST
    /// </remarks>
    /// <param name="projectId">The project ID</param>
    /// <param name="body">The request body containing asset IDs</param>
    /// <param name="requestConfiguration">(Optional) Configuration (includeCustomAttributes, includeDeleted)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="AssetsBatchGetPostResponse"/> containing the requested assets</returns>
    /// <example>
    /// <code>
    /// AssetsBatchGetPostResponse? result = await client.AssetsManager.BatchGetAssetsAsync("b.project-guid", new AssetsBatchGetPostRequestBody { Ids = ["id1", "id2"] });
    /// </code>
    /// </example>
    public async Task<AssetsBatchGetPostResponse?> BatchGetAssetsAsync(
        string projectId,
        AssetsBatchGetPostRequestBody body,
        RequestConfiguration<AssetsBatchGetRequestBuilder.AssetsBatchGetRequestBuilderPostQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Assets.V2.Projects[projectId]
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
    /// Wraps: PATCH /construction/assets/v2/projects/{projectId}/assets:batch-patch
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/assets-assets-batch-patch-PATCH-v2
    /// </remarks>
    /// <param name="projectId">The project ID</param>
    /// <param name="body">The batch patch request body</param>
    /// <param name="requestConfiguration">(Optional) Configuration</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="AssetsBatchPatchPatchResponse"/> containing the updated assets</returns>
    /// <example>
    /// <code>
    /// AssetsBatchPatchPatchResponse? result = await client.AssetsManager.BatchPatchAssetsAsync("b.project-guid", new AssetsBatchPatchPatchRequestBody { ... });
    /// </code>
    /// </example>
    public async Task<AssetsBatchPatchPatchResponse?> BatchPatchAssetsAsync(
        string projectId,
        AssetsBatchPatchPatchRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Assets.V2.Projects[projectId]
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
    /// Wraps: POST /construction/assets/v2/projects/{projectId}/assets:batch-delete
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/assets-assets-batch-delete-v2-POST
    /// </remarks>
    /// <param name="projectId">The project ID</param>
    /// <param name="body">The request body containing asset IDs to delete</param>
    /// <param name="requestConfiguration">(Optional) Configuration</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    /// <example>
    /// <code>
    /// await client.AssetsManager.BatchDeleteAssetsAsync("b.project-guid", new AssetsBatchDeletePostRequestBody { Ids = ["id1"] });
    /// </code>
    /// </example>
    public async Task BatchDeleteAssetsAsync(
        string projectId,
        AssetsBatchDeletePostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        await _api.Construction.Assets.V2.Projects[projectId]
            .AssetsBatchDelete
            .PostAsync(body, r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Searches for and returns all specified categories.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/assets/v1/projects/{projectId}/categories
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/assets-categories-GET
    /// </remarks>
    /// <param name="projectId">The project ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration (filter by isActive, parentId, maxDepth, updatedAt, includeUid)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="CategoriesGetResponse"/> containing the categories</returns>
    /// <example>
    /// <code>
    /// CategoriesGetResponse? categories = await client.AssetsManager.GetCategoriesAsync("b.project-guid");
    /// </code>
    /// </example>
    public async Task<CategoriesGetResponse?> GetCategoriesAsync(
        string projectId,
        RequestConfiguration<CategoriesRequestBuilder.CategoriesRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Assets.V1.Projects[projectId]
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
    /// Wraps: POST /construction/assets/v1/projects/{projectId}/categories
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/assets-categories-POST
    /// </remarks>
    /// <param name="projectId">The project ID</param>
    /// <param name="body">The category creation data</param>
    /// <param name="requestConfiguration">(Optional) Configuration (includeUid)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="CategoriesPostResponse"/> containing the created category</returns>
    /// <example>
    /// <code>
    /// CategoriesPostResponse? category = await client.AssetsManager.CreateCategoryAsync("b.project-guid", new CategoriesPostRequestBody { Name = "Equipment", ParentId = "root" });
    /// </code>
    /// </example>
    public async Task<CategoriesPostResponse?> CreateCategoryAsync(
        string projectId,
        CategoriesPostRequestBody body,
        RequestConfiguration<CategoriesRequestBuilder.CategoriesRequestBuilderPostQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Assets.V1.Projects[projectId]
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
    /// Wraps: POST /construction/assets/v1/projects/{projectId}/categories:batch-get
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/assets-categories-batch-get-POST
    /// </remarks>
    /// <param name="projectId">The project ID</param>
    /// <param name="body">The request body containing category IDs</param>
    /// <param name="requestConfiguration">(Optional) Configuration</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="CategoriesBatchGetPostResponse"/> containing the requested categories</returns>
    /// <example>
    /// <code>
    /// CategoriesBatchGetPostResponse? result = await client.AssetsManager.BatchGetCategoriesAsync("b.project-guid", new CategoriesBatchGetPostRequestBody { Ids = ["1", "2"] });
    /// </code>
    /// </example>
    public async Task<CategoriesBatchGetPostResponse?> BatchGetCategoriesAsync(
        string projectId,
        CategoriesBatchGetPostRequestBody body,
        RequestConfiguration<CategoriesBatchGetRequestBuilder.CategoriesBatchGetRequestBuilderPostQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Assets.V1.Projects[projectId]
            .CategoriesBatchGet
            .PostAsync(body, r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Assigns a status set to a category.
    /// </summary>
    /// <remarks>
    /// Wraps: PUT /construction/assets/v1/projects/{projectId}/categories/{categoryId}/status-step-set/{statusStepSetId}
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/assets-categories-category-id-status-step-set-status-step-set-id-PUT
    /// </remarks>
    /// <param name="projectId">The project ID</param>
    /// <param name="categoryId">The category ID</param>
    /// <param name="statusStepSetId">The status step set ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="WithStatusStepSetPutResponse"/> containing the updated category status step set</returns>
    /// <example>
    /// <code>
    /// WithStatusStepSetPutResponse? result = await client.AssetsManager.SetCategoryStatusStepSetAsync("b.project-guid", "categoryId", statusSetId);
    /// </code>
    /// </example>
    public async Task<WithStatusStepSetPutResponse?> SetCategoryStatusStepSetAsync(
        string projectId,
        string categoryId,
        Guid statusStepSetId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Assets.V1.Projects[projectId]
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
    /// Wraps: GET /construction/assets/v1/projects/{projectId}/categories/{categoryId}/custom-attributes
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/assets-categories-category-id-custom-attributes-GET
    /// </remarks>
    /// <param name="projectId">The project ID</param>
    /// <param name="categoryId">The category ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration (includeInherited)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.ACC.Construction.Assets.V1.Projects.Item.Categories.Item.CustomAttributes.CustomAttributesGetResponse"/> containing the custom attributes for the category</returns>
    /// <example>
    /// <code>
    /// CategoryCustomAttributes.CustomAttributesGetResponse? attrs = await client.AssetsManager.GetCategoryCustomAttributesAsync("b.project-guid", "categoryId");
    /// </code>
    /// </example>
    public async Task<CategoryCustomAttributes.CustomAttributesGetResponse?> GetCategoryCustomAttributesAsync(
        string projectId,
        string categoryId,
        RequestConfiguration<CategoryCustomAttributes.CustomAttributesRequestBuilder.CustomAttributesRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Assets.V1.Projects[projectId]
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
    /// Assigns an Asset custom attribute to a category.
    /// </summary>
    /// <remarks>
    /// Wraps: PUT /construction/assets/v1/projects/{projectId}/categories/{categoryId}/custom-attributes/{customAttributeId}
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/assets-categories-category-id-custom-attributes-custom-attribute-id-PUT
    /// </remarks>
    /// <param name="projectId">The project ID</param>
    /// <param name="categoryId">The category ID</param>
    /// <param name="customAttributeId">The custom attribute ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration (includeInherited)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="CategoryCustomAttributes.Item.WithCustomAttributePutResponse"/> containing the assignment result</returns>
    /// <example>
    /// <code>
    /// CategoryCustomAttributes.Item.WithCustomAttributePutResponse? result = await client.AssetsManager.AssignCustomAttributeToCategoryAsync("b.project-guid", "categoryId", customAttrId);
    /// </code>
    /// </example>
    public async Task<CategoryCustomAttributes.Item.WithCustomAttributePutResponse?> AssignCustomAttributeToCategoryAsync(
        string projectId,
        string categoryId,
        Guid customAttributeId,
        RequestConfiguration<CategoryCustomAttributes.Item.WithCustomAttributeItemRequestBuilder.WithCustomAttributeItemRequestBuilderPutQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Assets.V1.Projects[projectId]
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
    /// Searches for and returns all specified status sets with automatic pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/assets/v1/projects/{projectId}/status-step-sets
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/assets-status-step-sets-GET
    /// </remarks>
    /// <param name="projectId">The project ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration (cursorState, limit, filter, includeDeleted)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{StatusStepSetsGetResponse_results}"/> of <see cref="StatusStepSetsGetResponse_results"/> items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (var ss in client.AssetsManager.ListStatusStepSetsAsync("b.project-guid"))
    /// {
    ///     Console.WriteLine(ss.Name);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<StatusStepSetsGetResponse_results> ListStatusStepSetsAsync(
        string projectId,
        RequestConfiguration<StatusStepSetsRequestBuilder.StatusStepSetsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? cursorState = requestConfiguration?.QueryParameters?.CursorState;

        while (true)
        {
            var response = await _api.Construction.Assets.V1.Projects[projectId]
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

            foreach (var item in response.Results)
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
    /// Wraps: POST /construction/assets/v1/projects/{projectId}/status-step-sets
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/assets-status-step-sets-POST
    /// </remarks>
    /// <param name="projectId">The project ID</param>
    /// <param name="body">The status set creation data</param>
    /// <param name="requestConfiguration">(Optional) Configuration (renameConflicting)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="StatusStepSetsPostResponse"/> containing the created status set</returns>
    /// <example>
    /// <code>
    /// StatusStepSetsPostResponse? ss = await client.AssetsManager.CreateStatusStepSetAsync("b.project-guid", new StatusStepSetsPostRequestBody { Name = "Status Set" });
    /// </code>
    /// </example>
    public async Task<StatusStepSetsPostResponse?> CreateStatusStepSetAsync(
        string projectId,
        StatusStepSetsPostRequestBody body,
        RequestConfiguration<StatusStepSetsRequestBuilder.StatusStepSetsRequestBuilderPostQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Assets.V1.Projects[projectId]
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
    /// Wraps: POST /construction/assets/v1/projects/{projectId}/status-step-sets:batch-get
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/assets-status-step-sets-batch-get-POST
    /// </remarks>
    /// <param name="projectId">The project ID</param>
    /// <param name="body">The request body containing status step set IDs</param>
    /// <param name="requestConfiguration">(Optional) Configuration</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="ProjectStatusStepSetsBatchGet.StatusStepSetsBatchGetPostResponse"/> containing the requested status step sets</returns>
    /// <example>
    /// <code>
    /// ProjectStatusStepSetsBatchGet.StatusStepSetsBatchGetPostResponse? result = await client.AssetsManager.BatchGetStatusStepSetsAsync("b.project-guid", new ProjectStatusStepSetsBatchGet.StatusStepSetsBatchGetPostRequestBody { Ids = [...] });
    /// </code>
    /// </example>
    public async Task<ProjectStatusStepSetsBatchGet.StatusStepSetsBatchGetPostResponse?> BatchGetStatusStepSetsAsync(
        string projectId,
        ProjectStatusStepSetsBatchGet.StatusStepSetsBatchGetPostRequestBody body,
        RequestConfiguration<ProjectStatusStepSetsBatchGet.StatusStepSetsBatchGetRequestBuilder.StatusStepSetsBatchGetRequestBuilderPostQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Assets.V1.Projects[projectId]
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
    /// Wraps: POST /construction/assets/v1/projects/{projectId}/category-status-step-sets/status-step-sets:batch-get
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/assets-category-status-step-sets-status-step-sets-batch-get-POST
    /// </remarks>
    /// <param name="projectId">The project ID</param>
    /// <param name="body">The request body containing category IDs</param>
    /// <param name="requestConfiguration">(Optional) Configuration (includeInherited)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.ACC.Construction.Assets.V1.Projects.Item.CategoryStatusStepSets.StatusStepSetsBatchGet.StatusStepSetsBatchGetPostResponse"/> containing the category-to-status-set mappings</returns>
    /// <example>
    /// <code>
    /// Autodesk.ACC.Construction.Assets.V1.Projects.Item.CategoryStatusStepSets.StatusStepSetsBatchGet.StatusStepSetsBatchGetPostResponse? result = await client.AssetsManager.BatchGetCategoryStatusStepSetsAsync("b.project-guid", new Autodesk.ACC.Construction.Assets.V1.Projects.Item.CategoryStatusStepSets.StatusStepSetsBatchGet.StatusStepSetsBatchGetPostRequestBody { Ids = ["cat1", "cat2"] });
    /// </code>
    /// </example>
    public async Task<Autodesk.ACC.Construction.Assets.V1.Projects.Item.CategoryStatusStepSets.StatusStepSetsBatchGet.StatusStepSetsBatchGetPostResponse?> BatchGetCategoryStatusStepSetsAsync(
        string projectId,
        Autodesk.ACC.Construction.Assets.V1.Projects.Item.CategoryStatusStepSets.StatusStepSetsBatchGet.StatusStepSetsBatchGetPostRequestBody body,
        RequestConfiguration<Autodesk.ACC.Construction.Assets.V1.Projects.Item.CategoryStatusStepSets.StatusStepSetsBatchGet.StatusStepSetsBatchGetRequestBuilder.StatusStepSetsBatchGetRequestBuilderPostQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Assets.V1.Projects[projectId]
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
    /// Searches for and returns all specified asset statuses with automatic pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/assets/v1/projects/{projectId}/asset-statuses
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/assets-asset-statuses-GET
    /// </remarks>
    /// <param name="projectId">The project ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration (cursorState, limit, filter, includeDeleted)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{AssetStatusesGetResponse_results}"/> of <see cref="AssetStatusesGetResponse_results"/> items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (var status in client.AssetsManager.ListAssetStatusesAsync("b.project-guid"))
    /// {
    ///     Console.WriteLine(status.Label);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<AssetStatusesGetResponse_results> ListAssetStatusesAsync(
        string projectId,
        RequestConfiguration<AssetStatusesRequestBuilder.AssetStatusesRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? cursorState = requestConfiguration?.QueryParameters?.CursorState;

        while (true)
        {
            var response = await _api.Construction.Assets.V1.Projects[projectId]
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

            foreach (var item in response.Results)
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
    /// Wraps: POST /construction/assets/v1/projects/{projectId}/asset-statuses
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/assets-asset-statuses-POST
    /// </remarks>
    /// <param name="projectId">The project ID</param>
    /// <param name="body">The asset status creation data</param>
    /// <param name="requestConfiguration">(Optional) Configuration</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="AssetStatusesPostResponse"/> containing the created asset status</returns>
    /// <example>
    /// <code>
    /// AssetStatusesPostResponse? status = await client.AssetsManager.CreateAssetStatusAsync("b.project-guid", new AssetStatusesPostRequestBody { Label = "Ordered", StatusStepSetId = setId });
    /// </code>
    /// </example>
    public async Task<AssetStatusesPostResponse?> CreateAssetStatusAsync(
        string projectId,
        AssetStatusesPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Assets.V1.Projects[projectId]
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
    /// Wraps: POST /construction/assets/v1/projects/{projectId}/asset-statuses:batch-get
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/assets-asset-statuses-batch-get-POST
    /// </remarks>
    /// <param name="projectId">The project ID</param>
    /// <param name="body">The request body containing status IDs</param>
    /// <param name="requestConfiguration">(Optional) Configuration</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="AssetStatusesBatchGetPostResponse"/> containing the requested asset statuses</returns>
    /// <example>
    /// <code>
    /// AssetStatusesBatchGetPostResponse? result = await client.AssetsManager.BatchGetAssetStatusesAsync("b.project-guid", new AssetStatusesBatchGetPostRequestBody { Ids = [...] });
    /// </code>
    /// </example>
    public async Task<AssetStatusesBatchGetPostResponse?> BatchGetAssetStatusesAsync(
        string projectId,
        AssetStatusesBatchGetPostRequestBody body,
        RequestConfiguration<AssetStatusesBatchGetRequestBuilder.AssetStatusesBatchGetRequestBuilderPostQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Assets.V1.Projects[projectId]
            .AssetStatusesBatchGet
            .PostAsync(body, r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Searches for and returns all specified custom attributes with automatic pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/assets/v1/projects/{projectId}/custom-attributes
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/assets-custom-attributes-GET
    /// </remarks>
    /// <param name="projectId">The project ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration (cursorState, limit, filter, includeDeleted)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{CustomAttributesGetResponse_results}"/> of <see cref="Autodesk.ACC.Construction.Assets.V1.Projects.Item.CustomAttributes.CustomAttributesGetResponse_results"/> items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (var ca in client.AssetsManager.ListCustomAttributesAsync("b.project-guid"))
    /// {
    ///     Console.WriteLine(ca.Name);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<ProjectCustomAttributes.CustomAttributesGetResponse_results> ListCustomAttributesAsync(
        string projectId,
        RequestConfiguration<ProjectCustomAttributes.CustomAttributesRequestBuilder.CustomAttributesRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? cursorState = requestConfiguration?.QueryParameters?.CursorState;

        while (true)
        {
            var response = await _api.Construction.Assets.V1.Projects[projectId].CustomAttributes
                .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                    r.QueryParameters.CursorState = cursorState;
                }, cancellationToken);

            if (response?.Results is not { Count: > 0 })
                yield break;

            foreach (var item in response.Results)
            {
                yield return item;
            }

            cursorState = response.Pagination?.CursorState;
            if (string.IsNullOrEmpty(cursorState))
                yield break;
        }
    }

    /// <summary>
    /// Creates a new Asset custom attribute.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /construction/assets/v1/projects/{projectId}/custom-attributes
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/assets-custom-attributes-POST
    /// </remarks>
    /// <param name="projectId">The project ID</param>
    /// <param name="body">The custom attribute creation data</param>
    /// <param name="requestConfiguration">(Optional) Configuration (renameConflicting)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="CustomAttributesPostResponse"/> containing the created custom attribute</returns>
    /// <example>
    /// <code>
    /// CustomAttributesPostResponse? ca = await client.AssetsManager.CreateCustomAttributeAsync("b.project-guid", new CustomAttributesPostRequestBody { Name = "Serial Number", DataType = "text" });
    /// </code>
    /// </example>
    public async Task<CustomAttributesPostResponse?> CreateCustomAttributeAsync(
        string projectId,
        CustomAttributesPostRequestBody body,
        RequestConfiguration<ProjectCustomAttributes.CustomAttributesRequestBuilder.CustomAttributesRequestBuilderPostQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Assets.V1.Projects[projectId]
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
    /// Wraps: POST /construction/assets/v1/projects/{projectId}/custom-attributes:batch-get
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/assets-custom-attributes-batch-get-POST
    /// </remarks>
    /// <param name="projectId">The project ID</param>
    /// <param name="body">The request body containing custom attribute IDs</param>
    /// <param name="requestConfiguration">(Optional) Configuration</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="CustomAttributesBatchGetPostResponse"/> containing the requested custom attributes</returns>
    /// <example>
    /// <code>
    /// CustomAttributesBatchGetPostResponse? result = await client.AssetsManager.BatchGetCustomAttributesAsync("b.project-guid", new CustomAttributesBatchGetPostRequestBody { Ids = [...] });
    /// </code>
    /// </example>
    public async Task<CustomAttributesBatchGetPostResponse?> BatchGetCustomAttributesAsync(
        string projectId,
        CustomAttributesBatchGetPostRequestBody body,
        RequestConfiguration<CustomAttributesBatchGetRequestBuilder.CustomAttributesBatchGetRequestBuilderPostQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Assets.V1.Projects[projectId]
            .CustomAttributesBatchGet
            .PostAsync(body, r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Updates an Asset custom attribute.
    /// </summary>
    /// <remarks>
    /// Wraps: PATCH /construction/assets/v1/projects/{projectId}/custom-attributes/{customAttributeId}
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/assets-custom-attributes-custom-attribute-id-PATCH
    /// </remarks>
    /// <param name="projectId">The project ID</param>
    /// <param name="customAttributeId">The custom attribute ID</param>
    /// <param name="body">The update data</param>
    /// <param name="requestConfiguration">(Optional) Configuration</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="WithCustomAttributePatchResponse"/> containing the updated custom attribute</returns>
    /// <example>
    /// <code>
    /// WithCustomAttributePatchResponse? updated = await client.AssetsManager.UpdateCustomAttributeAsync("b.project-guid", customAttrId, new WithCustomAttributePatchRequestBody { ... });
    /// </code>
    /// </example>
    public async Task<WithCustomAttributePatchResponse?> UpdateCustomAttributeAsync(
        string projectId,
        Guid customAttributeId,
        WithCustomAttributePatchRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Assets.V1.Projects[projectId]
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
    /// Wraps: GET /construction/assets/v1/error-codes
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/assets-error-codes-GET
    /// </remarks>
    /// <param name="requestConfiguration">(Optional) Configuration</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="ErrorCodesGetResponse"/> containing the error codes</returns>
    /// <example>
    /// <code>
    /// ErrorCodesGetResponse? errorCodes = await client.AssetsManager.GetErrorCodesAsync();
    /// </code>
    /// </example>
    public async Task<ErrorCodesGetResponse?> GetErrorCodesAsync(
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Assets.V1.ErrorCodes
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
    /// Wraps: GET /construction/assets/v1/error-codes/{errorCodeName}
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/assets-error-codes-error-code-name-GET
    /// </remarks>
    /// <param name="errorCodeName">The name of the error code (from the errorCode field in error responses)</param>
    /// <param name="requestConfiguration">(Optional) Configuration</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="WithErrorCodeNameGetResponse"/> containing the error code details</returns>
    /// <example>
    /// <code>
    /// WithErrorCodeNameGetResponse? detail = await client.AssetsManager.GetErrorCodeAsync("ASSET_NOT_FOUND");
    /// </code>
    /// </example>
    public async Task<WithErrorCodeNameGetResponse?> GetErrorCodeAsync(
        string errorCodeName,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Assets.V1.ErrorCodes[errorCodeName]
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }
}

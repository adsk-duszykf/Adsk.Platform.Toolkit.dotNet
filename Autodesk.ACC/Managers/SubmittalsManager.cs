using System.Runtime.CompilerServices;
using Autodesk.ACC.Construction.Submittals.V2.Projects.Item.Items;
using Autodesk.ACC.Construction.Submittals.V2.Projects.Item.Items.Item;
using Autodesk.ACC.Construction.Submittals.V2.Projects.Item.ItemsValidateCustomIdentifier;
using Autodesk.ACC.Construction.Submittals.V2.Projects.Item.ItemsNextCustomIdentifier;
using Autodesk.ACC.Construction.Submittals.V2.Projects.Item.Items.Item.Attachments;
using Autodesk.ACC.Construction.Submittals.V2.Projects.Item.Items.Item.Attachments.Item;
using Autodesk.ACC.Construction.Submittals.V2.Projects.Item.Items.Item.Steps;
using Autodesk.ACC.Construction.Submittals.V2.Projects.Item.Items.Item.Steps.Item.Tasks;
using Autodesk.ACC.Construction.Submittals.V2.Projects.Item.Items.Item.Steps.Item.Tasks.WithTaskIdClose;
using Autodesk.ACC.Construction.Submittals.V2.Projects.Item.Items.WithItemIdTransition;
using Autodesk.ACC.Construction.Submittals.V2.Projects.Item.ItemTypes;
using Autodesk.ACC.Construction.Submittals.V2.Projects.Item.Metadata;
using Autodesk.ACC.Construction.Submittals.V2.Projects.Item.Packages;
using Autodesk.ACC.Construction.Submittals.V2.Projects.Item.Responses;
using Autodesk.ACC.Construction.Submittals.V2.Projects.Item.Settings.Mappings;
using Autodesk.ACC.Construction.Submittals.V2.Projects.Item.Specs;
using Autodesk.ACC.Construction.Submittals.V2.Projects.Item.Templates;
using Autodesk.ACC.Construction.Submittals.V2.Projects.Item.Users.Me;
using Microsoft.Kiota.Abstractions;
using static Autodesk.ACC.Construction.Submittals.V2.Projects.Item.Items.ItemsRequestBuilder;
using static Autodesk.ACC.Construction.Submittals.V2.Projects.Item.Packages.PackagesRequestBuilder;
using static Autodesk.ACC.Construction.Submittals.V2.Projects.Item.Specs.SpecsRequestBuilder;
using static Autodesk.ACC.Construction.Submittals.V2.Projects.Item.Responses.ResponsesRequestBuilder;
using static Autodesk.ACC.Construction.Submittals.V2.Projects.Item.Items.Item.Attachments.AttachmentsRequestBuilder;

namespace Autodesk.ACC.Managers;

/// <summary>
/// Manager for Submittals operations — manages submittal packages, items, specs, workflows, and attachments.
/// </summary>
public class SubmittalsManager
{
    private readonly BaseACCclient _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="SubmittalsManager"/> class.
    /// </summary>
    /// <param name="api">The API client</param>
    public SubmittalsManager(BaseACCclient api)
    {
        _api = api;
    }

    /// <summary>
    /// Retrieves all submittal items in a project with automatic pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/submittals/v2/projects/{projectId}/items
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/submittals-items-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports Limit, Offset, Sort, Search, filter parameters)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{ItemsGetResponse_results}"/> of <see cref="ItemsGetResponse_results"/> items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (var item in client.SubmittalsManager.ListItemsAsync(projectId))
    /// {
    ///     Console.WriteLine(item.Title);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<ItemsGetResponse_results> ListItemsAsync(
        Guid projectId,
        RequestConfiguration<ItemsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        int offset = requestConfiguration?.QueryParameters?.Offset ?? 0;
        while (true)
        {
            var response = await _api.Construction.Submittals.V2.Projects[projectId]
                .Items
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
            {
                yield return item;
            }

            if (string.IsNullOrEmpty(response.Pagination?.NextUrl))
                yield break;

            offset += response.Results.Count;
        }
    }

    /// <summary>
    /// Retrieves a single submittal item by ID.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/submittals/v2/projects/{projectId}/items/{itemId}
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/submittals-items-itemId-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="itemId">The ID of the submittal item</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="WithItemGetResponse"/> containing the submittal item details</returns>
    /// <example>
    /// <code>
    /// WithItemGetResponse? item = await client.SubmittalsManager.GetItemAsync(projectId, itemId);
    /// </code>
    /// </example>
    public async Task<WithItemGetResponse?> GetItemAsync(
        Guid projectId,
        string itemId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Submittals.V2.Projects[projectId]
            .Items[itemId]
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Creates a new submittal item in the specified project.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /construction/submittals/v2/projects/{projectId}/items
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/submittals-items-POST
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="body">The item creation data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="ItemsPostResponse"/> containing the created submittal item</returns>
    /// <example>
    /// <code>
    /// ItemsPostResponse? item = await client.SubmittalsManager.CreateItemAsync(projectId, new ItemsPostRequestBody());
    /// </code>
    /// </example>
    public async Task<ItemsPostResponse?> CreateItemAsync(
        Guid projectId,
        ItemsPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Submittals.V2.Projects[projectId]
            .Items
            .PostAsync(body, r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Updates an existing submittal item.
    /// </summary>
    /// <remarks>
    /// Wraps: PATCH /construction/submittals/v2/projects/{projectId}/items/{itemId}
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/submittals-items-itemId-PATCH
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="itemId">The ID of the submittal item</param>
    /// <param name="body">The item update data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="WithItemPatchResponse"/> containing the updated submittal item</returns>
    /// <example>
    /// <code>
    /// WithItemPatchResponse? updated = await client.SubmittalsManager.UpdateItemAsync(projectId, itemId, new WithItemPatchRequestBody());
    /// </code>
    /// </example>
    public async Task<WithItemPatchResponse?> UpdateItemAsync(
        Guid projectId,
        string itemId,
        WithItemPatchRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Submittals.V2.Projects[projectId]
            .Items[itemId]
            .PatchAsync(body, r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Transitions a submittal item to a specified state in the workflow.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /construction/submittals/v2/projects/{projectId}/items/{itemId}:transition
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/submittals-items-itemIdtransition-POST
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="itemId">The ID of the submittal item</param>
    /// <param name="body">The transition data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="WithItemIdTransitionPostResponse"/> containing the transition result</returns>
    /// <example>
    /// <code>
    /// WithItemIdTransitionPostResponse? result = await client.SubmittalsManager.TransitionItemAsync(projectId, itemId, new WithItemIdTransitionPostRequestBody());
    /// </code>
    /// </example>
    public async Task<WithItemIdTransitionPostResponse?> TransitionItemAsync(
        Guid projectId,
        string itemId,
        WithItemIdTransitionPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Submittals.V2.Projects[projectId]
            .Items
            .WithItemIdTransition(itemId)
            .PostAsync(body, r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Retrieves the revision history of a submittal item.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/submittals/v2/projects/{projectId}/items/{itemId}/revisions
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/submittals-revisions-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="itemId">The ID of the submittal item</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports Limit, Offset)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.ACC.Construction.Submittals.V2.Projects.Item.Items.Item.Revisions.RevisionsGetResponse"/> containing the revisions</returns>
    /// <example>
    /// <code>
    /// Autodesk.ACC.Construction.Submittals.V2.Projects.Item.Items.Item.Revisions.RevisionsGetResponse? revisions = await client.SubmittalsManager.GetRevisionsAsync(projectId, itemId);
    /// </code>
    /// </example>
    public async Task<Autodesk.ACC.Construction.Submittals.V2.Projects.Item.Items.Item.Revisions.RevisionsGetResponse?> GetRevisionsAsync(
        Guid projectId,
        string itemId,
        RequestConfiguration<Autodesk.ACC.Construction.Submittals.V2.Projects.Item.Items.Item.Revisions.RevisionsRequestBuilder.RevisionsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Submittals.V2.Projects[projectId]
            .Items[itemId]
            .Revisions
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Validates a custom identifier for a submittal item in a project.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /construction/submittals/v2/projects/{projectId}/items:validate-custom-identifier
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/submittals-itemsvalidate-custom-identifier-POST
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="body">The validation request body</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports specId query parameter)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    /// <example>
    /// <code>
    /// await client.SubmittalsManager.ValidateCustomIdentifierAsync(projectId, new ItemsValidateCustomIdentifierPostRequestBody());
    /// </code>
    /// </example>
    public async Task ValidateCustomIdentifierAsync(
        Guid projectId,
        ItemsValidateCustomIdentifierPostRequestBody body,
        RequestConfiguration<ItemsValidateCustomIdentifierRequestBuilder.ItemsValidateCustomIdentifierRequestBuilderPostQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        await _api.Construction.Submittals.V2.Projects[projectId]
            .ItemsValidateCustomIdentifier
            .PostAsync(body, r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Retrieves the next available custom identifier for submittal items.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/submittals/v2/projects/{projectId}/items:next-custom-identifier
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/submittals-itemsnext-custom-identifier-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports specId query parameter)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="ItemsNextCustomIdentifierGetResponse"/> containing the next custom identifier</returns>
    /// <example>
    /// <code>
    /// ItemsNextCustomIdentifierGetResponse? nextId = await client.SubmittalsManager.GetNextCustomIdentifierAsync(projectId);
    /// </code>
    /// </example>
    public async Task<ItemsNextCustomIdentifierGetResponse?> GetNextCustomIdentifierAsync(
        Guid projectId,
        RequestConfiguration<ItemsNextCustomIdentifierRequestBuilder.ItemsNextCustomIdentifierRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Submittals.V2.Projects[projectId]
            .ItemsNextCustomIdentifier
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Retrieves all submittal item types for the project.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/submittals/v2/projects/{projectId}/item-types
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/submittals-item-types-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports Limit, Offset)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.ACC.Construction.Submittals.V2.Projects.Item.ItemTypes.ItemTypesGetResponse"/> containing the item types</returns>
    /// <example>
    /// <code>
    /// Autodesk.ACC.Construction.Submittals.V2.Projects.Item.ItemTypes.ItemTypesGetResponse? types = await client.SubmittalsManager.GetItemTypesAsync(projectId);
    /// </code>
    /// </example>
    public async Task<Autodesk.ACC.Construction.Submittals.V2.Projects.Item.ItemTypes.ItemTypesGetResponse?> GetItemTypesAsync(
        Guid projectId,
        RequestConfiguration<ItemTypesRequestBuilder.ItemTypesRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Submittals.V2.Projects[projectId]
            .ItemTypes
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Retrieves a single submittal item type by ID.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/submittals/v2/projects/{projectId}/item-types/{id}
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/submittals-item-types-id-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="itemTypeId">The ID of the item type</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.ACC.Construction.Submittals.V2.Projects.Item.ItemTypes.Item.ItemTypesGetResponse"/> containing the item type details</returns>
    /// <example>
    /// <code>
    /// Autodesk.ACC.Construction.Submittals.V2.Projects.Item.ItemTypes.Item.ItemTypesGetResponse? type = await client.SubmittalsManager.GetItemTypeAsync(projectId, itemTypeId);
    /// </code>
    /// </example>
    public async Task<Autodesk.ACC.Construction.Submittals.V2.Projects.Item.ItemTypes.Item.ItemTypesGetResponse?> GetItemTypeAsync(
        Guid projectId,
        string itemTypeId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Submittals.V2.Projects[projectId]
            .ItemTypes[itemTypeId]
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Retrieves project metadata including submittal roles, statuses, and configuration.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/submittals/v2/projects/{projectId}/metadata
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/submittals-metadata-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="MetadataGetResponse"/> containing the project metadata</returns>
    /// <example>
    /// <code>
    /// MetadataGetResponse? metadata = await client.SubmittalsManager.GetMetadataAsync(projectId);
    /// </code>
    /// </example>
    public async Task<MetadataGetResponse?> GetMetadataAsync(
        Guid projectId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Submittals.V2.Projects[projectId]
            .Metadata
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Retrieves all submittal packages in a project with automatic pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/submittals/v2/projects/{projectId}/packages
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/submittals-packages-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports Limit, Offset, Sort, filter parameters)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{PackagesGetResponse_results}"/> of <see cref="PackagesGetResponse_results"/> items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (var pkg in client.SubmittalsManager.ListPackagesAsync(projectId))
    /// {
    ///     Console.WriteLine(pkg.Title);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<PackagesGetResponse_results> ListPackagesAsync(
        Guid projectId,
        RequestConfiguration<PackagesRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        int offset = requestConfiguration?.QueryParameters?.Offset ?? 0;
        while (true)
        {
            var response = await _api.Construction.Submittals.V2.Projects[projectId]
                .Packages
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
            {
                yield return item;
            }

            if (string.IsNullOrEmpty(response.Pagination?.NextUrl))
                yield break;

            offset += response.Results.Count;
        }
    }

    /// <summary>
    /// Retrieves a single submittal package by ID.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/submittals/v2/projects/{projectId}/packages/{id}
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/submittals-packages-id-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="packageId">The ID of the package</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.ACC.Construction.Submittals.V2.Projects.Item.Packages.Item.PackagesGetResponse"/> containing the package details</returns>
    /// <example>
    /// <code>
    /// Autodesk.ACC.Construction.Submittals.V2.Projects.Item.Packages.Item.PackagesGetResponse? pkg = await client.SubmittalsManager.GetPackageAsync(projectId, packageId);
    /// </code>
    /// </example>
    public async Task<Autodesk.ACC.Construction.Submittals.V2.Projects.Item.Packages.Item.PackagesGetResponse?> GetPackageAsync(
        Guid projectId,
        string packageId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Submittals.V2.Projects[projectId]
            .Packages[packageId]
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Retrieves all submittal responses in a project with automatic pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/submittals/v2/projects/{projectId}/responses
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/submittals-responses-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports Limit, Offset)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{ResponsesGetResponse_results}"/> of <see cref="ResponsesGetResponse_results"/> items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (var response in client.SubmittalsManager.ListResponsesAsync(projectId))
    /// {
    ///     Console.WriteLine(response.Value);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<ResponsesGetResponse_results> ListResponsesAsync(
        Guid projectId,
        RequestConfiguration<ResponsesRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        int offset = requestConfiguration?.QueryParameters?.Offset ?? 0;
        while (true)
        {
            var response = await _api.Construction.Submittals.V2.Projects[projectId]
                .Responses
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
            {
                yield return item;
            }

            if (string.IsNullOrEmpty(response.Pagination?.NextUrl))
                yield break;

            offset += response.Results.Count;
        }
    }

    /// <summary>
    /// Retrieves a single submittal response by ID.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/submittals/v2/projects/{projectId}/responses/{id}
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/submittals-responses-id-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="responseId">The ID of the response</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.ACC.Construction.Submittals.V2.Projects.Item.Responses.Item.ResponsesGetResponse"/> containing the response details</returns>
    /// <example>
    /// <code>
    /// Autodesk.ACC.Construction.Submittals.V2.Projects.Item.Responses.Item.ResponsesGetResponse? response = await client.SubmittalsManager.GetResponseAsync(projectId, responseId);
    /// </code>
    /// </example>
    public async Task<Autodesk.ACC.Construction.Submittals.V2.Projects.Item.Responses.Item.ResponsesGetResponse?> GetResponseAsync(
        Guid projectId,
        string responseId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Submittals.V2.Projects[projectId]
            .Responses[responseId]
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Retrieves users, roles, and companies assigned the manager role in the project.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/submittals/v2/projects/{projectId}/settings/mappings
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/submittals-mappings-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports Limit, Offset, filter[autodeskId])</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="MappingsGetResponse"/> containing the mappings</returns>
    /// <example>
    /// <code>
    /// MappingsGetResponse? mappings = await client.SubmittalsManager.GetMappingsAsync(projectId);
    /// </code>
    /// </example>
    public async Task<MappingsGetResponse?> GetMappingsAsync(
        Guid projectId,
        RequestConfiguration<MappingsRequestBuilder.MappingsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Submittals.V2.Projects[projectId]
            .Settings
            .Mappings
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Retrieves all spec sections for the project with automatic pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/submittals/v2/projects/{projectId}/specs
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/submittals-specs-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports Limit, Offset, Sort, Search, filter[identifier])</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{SpecsGetResponse_results}"/> of <see cref="SpecsGetResponse_results"/> items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (var spec in client.SubmittalsManager.ListSpecsAsync(projectId))
    /// {
    ///     Console.WriteLine(spec.Title);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<SpecsGetResponse_results> ListSpecsAsync(
        Guid projectId,
        RequestConfiguration<SpecsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        int offset = requestConfiguration?.QueryParameters?.Offset ?? 0;
        while (true)
        {
            var response = await _api.Construction.Submittals.V2.Projects[projectId]
                .Specs
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
            {
                yield return item;
            }

            if (string.IsNullOrEmpty(response.Pagination?.NextUrl))
                yield break;

            offset += response.Results.Count;
        }
    }

    /// <summary>
    /// Creates a spec section to organize and categorize submittals.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /construction/submittals/v2/projects/{projectId}/specs
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/submittals-specs-POST
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="body">The spec creation data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="SpecsPostResponse"/> containing the created spec section</returns>
    /// <example>
    /// <code>
    /// SpecsPostResponse? spec = await client.SubmittalsManager.CreateSpecAsync(projectId, new SpecsPostRequestBody());
    /// </code>
    /// </example>
    public async Task<SpecsPostResponse?> CreateSpecAsync(
        Guid projectId,
        SpecsPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Submittals.V2.Projects[projectId]
            .Specs
            .PostAsync(body, r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Retrieves a single spec section by ID.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/submittals/v2/projects/{projectId}/specs/{id}
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/submittals-specs-id-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="specId">The ID of the spec section</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.ACC.Construction.Submittals.V2.Projects.Item.Specs.Item.SpecsGetResponse"/> containing the spec section details</returns>
    /// <example>
    /// <code>
    /// Autodesk.ACC.Construction.Submittals.V2.Projects.Item.Specs.Item.SpecsGetResponse? spec = await client.SubmittalsManager.GetSpecAsync(projectId, specId);
    /// </code>
    /// </example>
    public async Task<Autodesk.ACC.Construction.Submittals.V2.Projects.Item.Specs.Item.SpecsGetResponse?> GetSpecAsync(
        Guid projectId,
        string specId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Submittals.V2.Projects[projectId]
            .Specs[specId]
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Retrieves review steps associated with a submittal item.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/submittals/v2/projects/{projectId}/items/{itemId}/steps
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/submittals-steps-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="itemId">The ID of the submittal item</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports Limit, Offset)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="StepsGetResponse"/> containing the review steps</returns>
    /// <example>
    /// <code>
    /// StepsGetResponse? steps = await client.SubmittalsManager.GetStepsAsync(projectId, itemId);
    /// </code>
    /// </example>
    public async Task<StepsGetResponse?> GetStepsAsync(
        Guid projectId,
        string itemId,
        RequestConfiguration<StepsRequestBuilder.StepsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Submittals.V2.Projects[projectId]
            .Items[itemId]
            .Steps
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Retrieves a single review step by ID.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/submittals/v2/projects/{projectId}/items/{itemId}/steps/{stepId}
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/submittals-steps-stepId-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="itemId">The ID of the submittal item</param>
    /// <param name="stepId">The ID of the step</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.ACC.Construction.Submittals.V2.Projects.Item.Items.Item.Steps.Item.WithStepGetResponse"/> containing the step details</returns>
    /// <example>
    /// <code>
    /// Autodesk.ACC.Construction.Submittals.V2.Projects.Item.Items.Item.Steps.Item.WithStepGetResponse? step = await client.SubmittalsManager.GetStepAsync(projectId, itemId, stepId);
    /// </code>
    /// </example>
    public async Task<Autodesk.ACC.Construction.Submittals.V2.Projects.Item.Items.Item.Steps.Item.WithStepGetResponse?> GetStepAsync(
        Guid projectId,
        string itemId,
        string stepId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Submittals.V2.Projects[projectId]
            .Items[itemId]
            .Steps[stepId]
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Retrieves tasks associated with a review step.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/submittals/v2/projects/{projectId}/items/{itemId}/steps/{stepId}/tasks
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/submittals-tasks-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="itemId">The ID of the submittal item</param>
    /// <param name="stepId">The ID of the step</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports Limit, Offset)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="TasksGetResponse"/> containing the tasks</returns>
    /// <example>
    /// <code>
    /// TasksGetResponse? tasks = await client.SubmittalsManager.GetTasksAsync(projectId, itemId, stepId);
    /// </code>
    /// </example>
    public async Task<TasksGetResponse?> GetTasksAsync(
        Guid projectId,
        string itemId,
        string stepId,
        RequestConfiguration<TasksRequestBuilder.TasksRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Submittals.V2.Projects[projectId]
            .Items[itemId]
            .Steps[stepId]
            .Tasks
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Retrieves a single task by ID.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/submittals/v2/projects/{projectId}/items/{itemId}/steps/{stepId}/tasks/{taskId}
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/submittals-tasks-taskId-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="itemId">The ID of the submittal item</param>
    /// <param name="stepId">The ID of the step</param>
    /// <param name="taskId">The ID of the task</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.ACC.Construction.Submittals.V2.Projects.Item.Items.Item.Steps.Item.Tasks.Item.WithTaskGetResponse"/> containing the task details</returns>
    /// <example>
    /// <code>
    /// Autodesk.ACC.Construction.Submittals.V2.Projects.Item.Items.Item.Steps.Item.Tasks.Item.WithTaskGetResponse? task = await client.SubmittalsManager.GetTaskAsync(projectId, itemId, stepId, taskId);
    /// </code>
    /// </example>
    public async Task<Autodesk.ACC.Construction.Submittals.V2.Projects.Item.Items.Item.Steps.Item.Tasks.Item.WithTaskGetResponse?> GetTaskAsync(
        Guid projectId,
        string itemId,
        string stepId,
        string taskId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Submittals.V2.Projects[projectId]
            .Items[itemId]
            .Steps[stepId]
            .Tasks[taskId]
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Closes a task by adding a required review response.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /construction/submittals/v2/projects/{projectId}/items/{itemId}/steps/{stepId}/tasks/{taskId}:close
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/submittals-tasks-taskIdclose-POST
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="itemId">The ID of the submittal item</param>
    /// <param name="stepId">The ID of the step</param>
    /// <param name="taskId">The ID of the task</param>
    /// <param name="body">The close task request body</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="WithTaskIdClosePostResponse"/> containing the close task response</returns>
    /// <example>
    /// <code>
    /// WithTaskIdClosePostResponse? result = await client.SubmittalsManager.CloseTaskAsync(projectId, itemId, stepId, taskId, new WithTaskIdClosePostRequestBody());
    /// </code>
    /// </example>
    public async Task<WithTaskIdClosePostResponse?> CloseTaskAsync(
        Guid projectId,
        string itemId,
        string stepId,
        string taskId,
        WithTaskIdClosePostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Submittals.V2.Projects[projectId]
            .Items[itemId]
            .Steps[stepId]
            .Tasks
            .WithTaskIdClose(taskId)
            .PostAsync(body, r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Retrieves the current user's submittal profile and permissions.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/submittals/v2/projects/{projectId}/users/me
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/submittals-users-me-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="MeGetResponse"/> containing the current user profile</returns>
    /// <example>
    /// <code>
    /// MeGetResponse? me = await client.SubmittalsManager.GetCurrentUserAsync(projectId);
    /// </code>
    /// </example>
    public async Task<MeGetResponse?> GetCurrentUserAsync(
        Guid projectId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Submittals.V2.Projects[projectId]
            .Users
            .Me
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Retrieves review templates available for the project.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/submittals/v2/projects/{projectId}/templates
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/submittals-templates-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports Limit, Offset, Sort)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="TemplatesGetResponse"/> containing the templates</returns>
    /// <example>
    /// <code>
    /// TemplatesGetResponse? templates = await client.SubmittalsManager.GetTemplatesAsync(projectId);
    /// </code>
    /// </example>
    public async Task<TemplatesGetResponse?> GetTemplatesAsync(
        Guid projectId,
        RequestConfiguration<TemplatesRequestBuilder.TemplatesRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Submittals.V2.Projects[projectId]
            .Templates
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Retrieves attachments for a submittal item with automatic pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/submittals/v2/projects/{projectId}/items/{itemId}/attachments
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/submittals-items-itemId-attachments-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="itemId">The ID of the submittal item</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports Limit, Offset, Sort, filter parameters)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{AttachmentsGetResponse_results}"/> of <see cref="AttachmentsGetResponse_results"/> items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (var attachment in client.SubmittalsManager.ListAttachmentsAsync(projectId, itemId))
    /// {
    ///     Console.WriteLine(attachment.Name);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<AttachmentsGetResponse_results> ListAttachmentsAsync(
        Guid projectId,
        string itemId,
        RequestConfiguration<AttachmentsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        int offset = requestConfiguration?.QueryParameters?.Offset ?? 0;
        while (true)
        {
            var response = await _api.Construction.Submittals.V2.Projects[projectId]
                .Items[itemId]
                .Attachments
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
            {
                yield return item;
            }

            if (string.IsNullOrEmpty(response.Pagination?.NextUrl))
                yield break;

            offset += response.Results.Count;
        }
    }

    /// <summary>
    /// Adds an attachment to a submittal item.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /construction/submittals/v2/projects/{projectId}/items/{itemId}/attachments
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/submittals-attachments-POST
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="itemId">The ID of the submittal item</param>
    /// <param name="body">The attachment creation data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="AttachmentsPostResponse"/> containing the created attachment</returns>
    /// <example>
    /// <code>
    /// AttachmentsPostResponse? attachment = await client.SubmittalsManager.CreateAttachmentAsync(projectId, itemId, new AttachmentsPostRequestBody());
    /// </code>
    /// </example>
    public async Task<AttachmentsPostResponse?> CreateAttachmentAsync(
        Guid projectId,
        string itemId,
        AttachmentsPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Submittals.V2.Projects[projectId]
            .Items[itemId]
            .Attachments
            .PostAsync(body, r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Updates the upload status of an attachment.
    /// </summary>
    /// <remarks>
    /// Wraps: PATCH /construction/submittals/v2/projects/{projectId}/items/{itemId}/attachments/{attachmentId}
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/submittals-attachments-attachmentId-PATCH
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="itemId">The ID of the submittal item</param>
    /// <param name="attachmentId">The ID of the attachment</param>
    /// <param name="body">The attachment update data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="WithAttachmentPatchResponse"/> containing the updated attachment</returns>
    /// <example>
    /// <code>
    /// WithAttachmentPatchResponse? updated = await client.SubmittalsManager.UpdateAttachmentAsync(projectId, itemId, attachmentId, new WithAttachmentPatchRequestBody());
    /// </code>
    /// </example>
    public async Task<WithAttachmentPatchResponse?> UpdateAttachmentAsync(
        Guid projectId,
        string itemId,
        string attachmentId,
        WithAttachmentPatchRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Submittals.V2.Projects[projectId]
            .Items[itemId]
            .Attachments[attachmentId]
            .PatchAsync(body, r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }
}

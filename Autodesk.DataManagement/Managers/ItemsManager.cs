using System.Runtime.CompilerServices;
using Autodesk.DataManagement.Data.V1.Projects.Item.Items;
using Autodesk.DataManagement.Data.V1.Projects.Item.Items.Item;
using Autodesk.DataManagement.Data.V1.Projects.Item.Items.Item.Refs;
using Autodesk.DataManagement.Data.V1.Projects.Item.Items.Item.Versions;
using Autodesk.DataManagement.Models;
using Microsoft.Kiota.Abstractions;
using ItemRelationshipRefsBuilder = Autodesk.DataManagement.Data.V1.Projects.Item.Items.Item.Relationships.Refs.RefsRequestBuilder;

namespace Autodesk.DataManagement.Managers;

/// <summary>
/// Manager for Item operations
/// </summary>
public class ItemsManager
{
    private readonly BaseDataManagementClient _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="ItemsManager"/> class.
    /// </summary>
    /// <param name="api">The Data Management API client</param>
    public ItemsManager(BaseDataManagementClient api)
    {
        _api = api;
    }

    #region Get & Update Item

    /// <summary>
    /// Retrieves metadata for a specified item.
    /// Items represent word documents, fusion design files, drawings, spreadsheets, etc.
    /// </summary>
    /// <remarks>API: GET /data/v1/projects/{project_id}/items/{item_id}</remarks>
    /// <param name="projectId">The unique identifier of a project</param>
    /// <param name="itemId">The unique identifier of an item</param>
    /// <param name="requestConfiguration">Optional configuration for the request (includePathInProject)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The item metadata</returns>
    /// <example>
    /// <code>
    /// var item = await client.Items.GetItemAsync("b.my-project-id", "urn:adsk.wipprod:dm.lineage:...");
    /// Console.WriteLine($"Item: {item?.Data?.Attributes?.DisplayName}");
    /// </code>
    /// </example>
    public async Task<Models.Item?> GetItemAsync(
        string projectId,
        string itemId,
        Action<RequestConfiguration<WithItem_ItemRequestBuilder.WithItem_ItemRequestBuilderGetQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Data.V1.Projects[projectId].Items[itemId]
            .GetAsync(requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Updates the properties of the given item.
    /// </summary>
    /// <remarks>API: PATCH /data/v1/projects/{project_id}/items/{item_id}</remarks>
    /// <param name="projectId">The unique identifier of a project</param>
    /// <param name="itemId">The unique identifier of an item</param>
    /// <param name="body">The item update payload</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated item</returns>
    /// <example>
    /// <code>
    /// var updated = await client.Items.UpdateItemAsync("b.my-project-id",
    ///     "urn:adsk.wipprod:dm.lineage:...",
    ///     new ItemRequest { /* ... */ });
    /// </code>
    /// </example>
    public async Task<Models.Item?> UpdateItemAsync(
        string projectId,
        string itemId,
        ItemRequest body,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Data.V1.Projects[projectId].Items[itemId]
            .PatchAsync(body, requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Creates the first version of a file (item). To create additional versions, use the Versions manager.
    /// </summary>
    /// <remarks>API: POST /data/v1/projects/{project_id}/items</remarks>
    /// <param name="projectId">The unique identifier of a project</param>
    /// <param name="body">The item creation payload</param>
    /// <param name="requestConfiguration">Optional configuration for the request (copyFrom)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created item</returns>
    /// <example>
    /// <code>
    /// var item = await client.Items.CreateItemAsync("b.my-project-id",
    ///     new CreateItem { /* ... */ });
    /// Console.WriteLine($"Created item: {item?.Data?.Id}");
    /// </code>
    /// </example>
    public async Task<Models.Item?> CreateItemAsync(
        string projectId,
        CreateItem body,
        Action<RequestConfiguration<ItemsRequestBuilder.ItemsRequestBuilderPostQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Data.V1.Projects[projectId].Items
            .PostAsync(body, requestConfiguration, cancellationToken);
    }

    #endregion

    #region Parent & Tip

    /// <summary>
    /// Returns the parent folder for the given item.
    /// </summary>
    /// <remarks>API: GET /data/v1/projects/{project_id}/items/{item_id}/parent</remarks>
    /// <param name="projectId">The unique identifier of a project</param>
    /// <param name="itemId">The unique identifier of an item</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The parent folder</returns>
    /// <example>
    /// <code>
    /// var parent = await client.Items.GetItemParentFolderAsync("b.my-project-id", "urn:adsk.wipprod:dm.lineage:...");
    /// Console.WriteLine($"Parent folder: {parent?.Data?.Attributes?.Name}");
    /// </code>
    /// </example>
    public async Task<Folder?> GetItemParentFolderAsync(
        string projectId,
        string itemId,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Data.V1.Projects[projectId].Items[itemId].Parent
            .GetAsync(requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Returns the tip (most recent) version for the given item.
    /// </summary>
    /// <remarks>API: GET /data/v1/projects/{project_id}/items/{item_id}/tip</remarks>
    /// <param name="projectId">The unique identifier of a project</param>
    /// <param name="itemId">The unique identifier of an item</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The tip version</returns>
    /// <example>
    /// <code>
    /// var tip = await client.Items.GetItemTipAsync("b.my-project-id", "urn:adsk.wipprod:dm.lineage:...");
    /// Console.WriteLine($"Tip version: {tip?.Data?.Id}");
    /// </code>
    /// </example>
    public async Task<ItemTip?> GetItemTipAsync(
        string projectId,
        string itemId,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Data.V1.Projects[projectId].Items[itemId].Tip
            .GetAsync(requestConfiguration, cancellationToken);
    }

    #endregion

    #region Item Versions (Paginated)

    /// <summary>
    /// Lists all versions for the given item with automatic pagination.
    /// </summary>
    /// <remarks>API: GET /data/v1/projects/{project_id}/items/{item_id}/versions</remarks>
    /// <param name="projectId">The unique identifier of a project</param>
    /// <param name="itemId">The unique identifier of an item</param>
    /// <param name="requestConfiguration">Optional configuration for the request (filter by id, extension type, version number, page limit)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>An async enumerable of version data items across all pages</returns>
    /// <example>
    /// <code>
    /// await foreach (var version in client.Items.ListItemVersionsAsync("b.my-project-id", "urn:adsk.wipprod:dm.lineage:..."))
    /// {
    ///     Console.WriteLine($"v{version.Attributes?.VersionNumber}: {version.Id}");
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<Versions_data> ListItemVersionsAsync(
        string projectId,
        string itemId,
        Action<RequestConfiguration<VersionsRequestBuilder.VersionsRequestBuilderGetQueryParameters>>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        int pageNumber = 0;

        while (true)
        {
            var capturedPage = pageNumber;
            var response = await _api.Data.V1.Projects[projectId].Items[itemId].Versions
                .GetAsync(config =>
                {
                    requestConfiguration?.Invoke(config);
                    config.QueryParameters.Pagenumber = capturedPage;
                }, cancellationToken);

            if (response?.Data != null)
            {
                foreach (var item in response.Data)
                {
                    yield return item;
                }
            }

            if (response?.Links?.Next?.Href == null)
                break;

            pageNumber++;
        }
    }

    #endregion

    #region Refs & Relationships

    /// <summary>
    /// Returns the resources (items, folders, and versions) that have a custom relationship with the given item.
    /// </summary>
    /// <remarks>API: GET /data/v1/projects/{project_id}/items/{item_id}/refs</remarks>
    /// <param name="projectId">The unique identifier of a project</param>
    /// <param name="itemId">The unique identifier of an item</param>
    /// <param name="requestConfiguration">Optional configuration for the request (filter by type, id, extension type)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The item refs</returns>
    /// <example>
    /// <code>
    /// var refs = await client.Items.GetItemRefsAsync("b.my-project-id", "urn:adsk.wipprod:dm.lineage:...");
    /// </code>
    /// </example>
    public async Task<Refs?> GetItemRefsAsync(
        string projectId,
        string itemId,
        Action<RequestConfiguration<RefsRequestBuilder.RefsRequestBuilderGetQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Data.V1.Projects[projectId].Items[itemId].Refs
            .GetAsync(requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Returns the custom relationships associated with the given item.
    /// </summary>
    /// <remarks>API: GET /data/v1/projects/{project_id}/items/{item_id}/relationships/refs</remarks>
    /// <param name="projectId">The unique identifier of a project</param>
    /// <param name="itemId">The unique identifier of an item</param>
    /// <param name="requestConfiguration">Optional configuration for the request (filter by type, id, direction, refType, extension type)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The item relationship refs</returns>
    /// <example>
    /// <code>
    /// var relRefs = await client.Items.GetItemRelationshipRefsAsync("b.my-project-id", "urn:adsk.wipprod:dm.lineage:...");
    /// </code>
    /// </example>
    public async Task<RelationshipRefs?> GetItemRelationshipRefsAsync(
        string projectId,
        string itemId,
        Action<RequestConfiguration<ItemRelationshipRefsBuilder.RefsRequestBuilderGetQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Data.V1.Projects[projectId].Items[itemId].Relationships.Refs
            .GetAsync(requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Creates a custom relationship between an item and another resource (folder, item, or version).
    /// </summary>
    /// <remarks>API: POST /data/v1/projects/{project_id}/items/{item_id}/relationships/refs</remarks>
    /// <param name="projectId">The unique identifier of a project</param>
    /// <param name="itemId">The unique identifier of an item</param>
    /// <param name="body">The relationship ref to create</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <example>
    /// <code>
    /// await client.Items.CreateItemRelationshipRefAsync("b.my-project-id",
    ///     "urn:adsk.wipprod:dm.lineage:...",
    ///     new RelationshipRefsRequest { /* ... */ });
    /// </code>
    /// </example>
    public async Task CreateItemRelationshipRefAsync(
        string projectId,
        string itemId,
        RelationshipRefsRequest body,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        await _api.Data.V1.Projects[projectId].Items[itemId].Relationships.Refs
            .PostAsync(body, requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Returns a collection of links for the given item.
    /// </summary>
    /// <remarks>API: GET /data/v1/projects/{project_id}/items/{item_id}/relationships/links</remarks>
    /// <param name="projectId">The unique identifier of a project</param>
    /// <param name="itemId">The unique identifier of an item</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The item relationship links</returns>
    /// <example>
    /// <code>
    /// var links = await client.Items.GetItemRelationshipLinksAsync("b.my-project-id", "urn:adsk.wipprod:dm.lineage:...");
    /// </code>
    /// </example>
    public async Task<RelationshipLinks?> GetItemRelationshipLinksAsync(
        string projectId,
        string itemId,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Data.V1.Projects[projectId].Items[itemId].Relationships.Links
            .GetAsync(requestConfiguration, cancellationToken);
    }

    #endregion
}

using System.Runtime.CompilerServices;
using Autodesk.Vault.Models;
using Autodesk.Vault.Vaults.Item.Items.Item;
using Microsoft.Kiota.Abstractions;
using static Autodesk.Vault.Vaults.Item.Items.ItemsRequestBuilder;
using static Autodesk.Vault.Vaults.Item.Items.Item.Versions.VersionsRequestBuilder;
using static Autodesk.Vault.Vaults.Item.ItemVersions.ItemVersionsRequestBuilder;
using static Autodesk.Vault.Vaults.Item.ItemVersions.Item.AssociatedFiles.AssociatedFilesRequestBuilder;
using static Autodesk.Vault.Vaults.Item.ItemVersions.Item.BillOfMaterials.BillOfMaterialsRequestBuilder;
using static Autodesk.Vault.Vaults.Item.ItemVersions.Item.Parents.ParentsRequestBuilder;
using ItemChangeOrdersRequestBuilder = Autodesk.Vault.Vaults.Item.Items.Item.ChangeOrders.ChangeOrdersRequestBuilder;

namespace Autodesk.Vault.Managers;

/// <summary>
/// Manager for Items operations
/// </summary>
public class ItemsManager
{
    private readonly BaseVaultClient _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="ItemsManager"/> class.
    /// </summary>
    /// <param name="api">The API client</param>
    public ItemsManager(BaseVaultClient api)
    {
        _api = api;
    }

    #region Items

    /// <summary>
    /// Lists items for a specific vault with automatic cursor-based pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /vaults/{vaultId}/items
    /// </remarks>
    /// <param name="vaultId">The unique identifier of a vault</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports query, filtering, sorting, limit, cursorState)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="Item"/> items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (Item item in client.Items.ListItemsAsync("1"))
    /// {
    ///     Console.WriteLine(item.ItemNumber);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<Item> ListItemsAsync(
        string vaultId,
        RequestConfiguration<ItemsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? cursor = requestConfiguration?.QueryParameters?.CursorState;

        while (true)
        {
            ItemCollection? response = await _api.Vaults[vaultId].Items
                .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                    r.QueryParameters.CursorState = cursor;
                }, cancellationToken);

            if (response?.Results is not { Count: > 0 })
                yield break;

            foreach (Item item in response.Results)
            {
                yield return item;
            }

            if (string.IsNullOrEmpty(response.Pagination?.NextUrl))
                yield break;

            cursor = PaginationHelper.ExtractBookmarkFromNextUrl(response.Pagination.NextUrl);
        }
    }

    /// <summary>
    /// Get an item by its ID.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /vaults/{vaultId}/items/{id}
    /// </remarks>
    /// <param name="vaultId">The unique identifier of a vault</param>
    /// <param name="itemId">The unique identifier of an item</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports releasedOnly)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Item"/> containing the item information</returns>
    /// <example>
    /// <code>
    /// Item? item = await client.Items.GetItemByIdAsync("1", "42");
    /// </code>
    /// </example>
    public async Task<Item?> GetItemByIdAsync(
        string vaultId,
        string itemId,
        RequestConfiguration<ItemsItemRequestBuilder.ItemsItemRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Vaults[vaultId].Items[itemId]
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Lists all versions (history) for a specific item with automatic cursor-based pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /vaults/{vaultId}/items/{id}/versions
    /// </remarks>
    /// <param name="vaultId">The unique identifier of a vault</param>
    /// <param name="itemId">The unique identifier of an item</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports extendedModels, propDefIds, limit, cursorState)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="ItemVersion"/> items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (ItemVersion version in client.Items.ListItemVersionsAsync("1", "42"))
    /// {
    ///     Console.WriteLine(version.ItemNumber);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<ItemVersion> ListItemVersionsAsync(
        string vaultId,
        string itemId,
        RequestConfiguration<VersionsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? cursor = requestConfiguration?.QueryParameters?.CursorState;

        while (true)
        {
            ItemVersionCollection? response = await _api.Vaults[vaultId].Items[itemId].Versions
                .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                    r.QueryParameters.CursorState = cursor;
                }, cancellationToken);

            if (response?.Results is not { Count: > 0 })
                yield break;

            foreach (ItemVersion item in response.Results)
            {
                yield return item;
            }

            if (string.IsNullOrEmpty(response.Pagination?.NextUrl))
                yield break;

            cursor = PaginationHelper.ExtractBookmarkFromNextUrl(response.Pagination.NextUrl);
        }
    }

    /// <summary>
    /// Lists change orders associated with a specific item with automatic cursor-based pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /vaults/{vaultId}/items/{id}/change-orders
    /// </remarks>
    /// <param name="vaultId">The unique identifier of a vault</param>
    /// <param name="itemId">The unique identifier of an item</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports extendedModels, includeClosedECOs, propDefIds, limit, cursorState)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="ChangeOrderCollection.ChangeOrderCollection_results"/> items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (var co in client.Items.ListItemAssociatedChangeOrdersAsync("1", "42"))
    /// {
    ///     Console.WriteLine(co.Name);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<ChangeOrderCollection.ChangeOrderCollection_results> ListItemAssociatedChangeOrdersAsync(
        string vaultId,
        string itemId,
        RequestConfiguration<ItemChangeOrdersRequestBuilder.ChangeOrdersRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? cursor = requestConfiguration?.QueryParameters?.CursorState;

        while (true)
        {
            ChangeOrderCollection? response = await _api.Vaults[vaultId].Items[itemId].ChangeOrders
                .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                    r.QueryParameters.CursorState = cursor;
                }, cancellationToken);

            if (response?.Results is not { Count: > 0 })
                yield break;

            foreach (var item in response.Results)
            {
                yield return item;
            }

            if (string.IsNullOrEmpty(response.Pagination?.NextUrl))
                yield break;

            cursor = PaginationHelper.ExtractBookmarkFromNextUrl(response.Pagination.NextUrl);
        }
    }

    #endregion

    #region Item Versions

    /// <summary>
    /// Lists all item versions for a vault with automatic cursor-based pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /vaults/{vaultId}/item-versions
    /// </remarks>
    /// <param name="vaultId">The unique identifier of a vault</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports filtering, sorting, limit, cursorState)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="ItemVersion"/> items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (ItemVersion version in client.Items.ListAllItemVersionsAsync("1"))
    /// {
    ///     Console.WriteLine(version.ItemNumber);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<ItemVersion> ListAllItemVersionsAsync(
        string vaultId,
        RequestConfiguration<ItemVersionsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? cursor = requestConfiguration?.QueryParameters?.CursorState;

        while (true)
        {
            ItemVersionCollection? response = await _api.Vaults[vaultId].ItemVersions
                .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                    r.QueryParameters.CursorState = cursor;
                }, cancellationToken);

            if (response?.Results is not { Count: > 0 })
                yield break;

            foreach (ItemVersion item in response.Results)
            {
                yield return item;
            }

            if (string.IsNullOrEmpty(response.Pagination?.NextUrl))
                yield break;

            cursor = PaginationHelper.ExtractBookmarkFromNextUrl(response.Pagination.NextUrl);
        }
    }

    /// <summary>
    /// Get an item version by its ID.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /vaults/{vaultId}/item-versions/{id}
    /// </remarks>
    /// <param name="vaultId">The unique identifier of a vault</param>
    /// <param name="itemVersionId">The unique identifier of an item version</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="ItemVersion"/> containing the item version information</returns>
    /// <example>
    /// <code>
    /// ItemVersion? version = await client.Items.GetItemVersionByIdAsync("1", "100");
    /// </code>
    /// </example>
    public async Task<ItemVersion?> GetItemVersionByIdAsync(
        string vaultId,
        string itemVersionId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Vaults[vaultId].ItemVersions[itemVersionId]
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Get associated files for an item version.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /vaults/{vaultId}/item-versions/{id}/associated-files
    /// </remarks>
    /// <param name="vaultId">The unique identifier of a vault</param>
    /// <param name="itemVersionId">The unique identifier of an item version</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports limit, cursorState)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="ItemAssociatedFileVersionCollection"/> containing the associated files</returns>
    /// <example>
    /// <code>
    /// ItemAssociatedFileVersionCollection? files = await client.Items.GetItemVersionAssociatedFilesAsync("1", "100");
    /// </code>
    /// </example>
    public async Task<ItemAssociatedFileVersionCollection?> GetItemVersionAssociatedFilesAsync(
        string vaultId,
        string itemVersionId,
        RequestConfiguration<AssociatedFilesRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Vaults[vaultId].ItemVersions[itemVersionId].AssociatedFiles
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Get bill of materials for an item version.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /vaults/{vaultId}/item-versions/{id}/bill-of-materials
    /// </remarks>
    /// <param name="vaultId">The unique identifier of a vault</param>
    /// <param name="itemVersionId">The unique identifier of an item version</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports bomType, date)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="BOMLinksAndRevisions"/> containing the bill of materials</returns>
    /// <example>
    /// <code>
    /// BOMLinksAndRevisions? bom = await client.Items.GetItemVersionBillOfMaterialsAsync("1", "100");
    /// </code>
    /// </example>
    public async Task<BOMLinksAndRevisions?> GetItemVersionBillOfMaterialsAsync(
        string vaultId,
        string itemVersionId,
        RequestConfiguration<BillOfMaterialsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Vaults[vaultId].ItemVersions[itemVersionId].BillOfMaterials
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Get parent items (where used) for an item version.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /vaults/{vaultId}/item-versions/{id}/parents
    /// </remarks>
    /// <param name="vaultId">The unique identifier of a vault</param>
    /// <param name="itemVersionId">The unique identifier of an item version</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports bomType, date)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="BOMLinksAndRevisions"/> containing the parent items</returns>
    /// <example>
    /// <code>
    /// BOMLinksAndRevisions? parents = await client.Items.GetItemVersionWhereUsedAsync("1", "100");
    /// </code>
    /// </example>
    public async Task<BOMLinksAndRevisions?> GetItemVersionWhereUsedAsync(
        string vaultId,
        string itemVersionId,
        RequestConfiguration<ParentsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Vaults[vaultId].ItemVersions[itemVersionId].Parents
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Get the thumbnail image for an item version.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /vaults/{vaultId}/item-versions/{id}/thumbnail
    /// </remarks>
    /// <param name="vaultId">The unique identifier of a vault</param>
    /// <param name="itemVersionId">The unique identifier of an item version</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Stream"/> containing the thumbnail image</returns>
    /// <example>
    /// <code>
    /// Stream? thumbnail = await client.Items.GetItemVersionThumbnailAsync("1", "100");
    /// </code>
    /// </example>
    public async Task<Stream?> GetItemVersionThumbnailAsync(
        string vaultId,
        string itemVersionId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Vaults[vaultId].ItemVersions[itemVersionId].Thumbnail
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    #endregion
}

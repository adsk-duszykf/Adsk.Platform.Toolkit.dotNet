using Autodesk.Vault.Models;
using Autodesk.Vault.Vaults.Item.Items.Item;
using Autodesk.Vault.Vaults.Item.Items.Item.Versions;
using Microsoft.Kiota.Abstractions;
using static Autodesk.Vault.Vaults.Item.Items.ItemsRequestBuilder;
using static Autodesk.Vault.Vaults.Item.ItemVersions.Item.AssociatedFiles.AssociatedFilesRequestBuilder;
using static Autodesk.Vault.Vaults.Item.ItemVersions.Item.BillOfMaterials.BillOfMaterialsRequestBuilder;
using static Autodesk.Vault.Vaults.Item.ItemVersions.ItemVersionsRequestBuilder;

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
    /// Get items for a specific vault
    /// </summary>
    /// <param name="vaultId">Vault ID</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of items</returns>
    public async Task<ItemCollection?> GetItemsAsync(
        string vaultId,
        Action<RequestConfiguration<ItemsRequestBuilderGetQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _api.Vaults[vaultId].Items
            .GetAsync(requestConfiguration, cancellationToken);

        return result;
    }

    /// <summary>
    /// Get an item by ID
    /// </summary>
    /// <param name="vaultId">Vault ID</param>
    /// <param name="itemId">Item ID</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Item information</returns>
    public async Task<Item?> GetItemByIdAsync(
        string vaultId,
        string itemId,
        Action<RequestConfiguration<ItemsItemRequestBuilder.ItemsItemRequestBuilderGetQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _api.Vaults[vaultId].Items[itemId]
            .GetAsync(requestConfiguration, cancellationToken);

        return result;
    }

    /// <summary>
    /// Get all versions for a specific item
    /// </summary>
    /// <param name="vaultId">Vault ID</param>
    /// <param name="itemId">Item ID</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of item versions</returns>
    public async Task<ItemVersionCollection?> GetItemVersionsAsync(
        string vaultId,
        string itemId,
        Action<RequestConfiguration<VersionsRequestBuilder.VersionsRequestBuilderGetQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _api.Vaults[vaultId].Items[itemId].Versions
            .GetAsync(requestConfiguration, cancellationToken);

        return result;
    }

    #endregion

    #region Item Versions

    /// <summary>
    /// Get all item versions for a vault
    /// </summary>
    /// <param name="vaultId">Vault ID</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of item versions</returns>
    public async Task<ItemVersionCollection?> GetAllItemVersionsAsync(
        string vaultId,
        Action<RequestConfiguration<ItemVersionsRequestBuilderGetQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _api.Vaults[vaultId].ItemVersions
            .GetAsync(requestConfiguration, cancellationToken);

        return result;
    }

    /// <summary>
    /// Get an item version by ID
    /// </summary>
    /// <param name="vaultId">Vault ID</param>
    /// <param name="itemVersionId">Item version ID</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Item version information</returns>
    public async Task<ItemVersion?> GetItemVersionByIdAsync(
        string vaultId,
        string itemVersionId,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _api.Vaults[vaultId].ItemVersions[itemVersionId]
            .GetAsync(requestConfiguration, cancellationToken);

        return result;
    }

    /// <summary>
    /// Get associated files for an item version
    /// </summary>
    /// <param name="vaultId">Vault ID</param>
    /// <param name="itemVersionId">Item version ID</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of associated file versions</returns>
    public async Task<ItemAssociatedFileVersionCollection?> GetItemVersionAssociatedFilesAsync(
        string vaultId,
        string itemVersionId,
        Action<RequestConfiguration<AssociatedFilesRequestBuilderGetQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _api.Vaults[vaultId].ItemVersions[itemVersionId].AssociatedFiles
            .GetAsync(requestConfiguration, cancellationToken);

        return result;
    }

    /// <summary>
    /// Get bill of materials for an item version
    /// </summary>
    /// <param name="vaultId">Vault ID</param>
    /// <param name="itemVersionId">Item version ID</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Bill of materials</returns>
    public async Task<BOMLinksAndRevisions?> GetItemVersionBillOfMaterialsAsync(
        string vaultId,
        string itemVersionId,
        Action<RequestConfiguration<BillOfMaterialsRequestBuilderGetQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _api.Vaults[vaultId].ItemVersions[itemVersionId].BillOfMaterials
            .GetAsync(requestConfiguration, cancellationToken);

        return result;
    }

    /// <summary>
    /// Get item version thumbnail
    /// </summary>
    /// <param name="vaultId">Vault ID</param>
    /// <param name="itemVersionId">Item version ID</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Thumbnail image stream</returns>
    public async Task<Stream?> GetItemVersionThumbnailAsync(
        string vaultId,
        string itemVersionId,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _api.Vaults[vaultId].ItemVersions[itemVersionId].Thumbnail
            .GetAsync(requestConfiguration, cancellationToken);

        return result;
    }

    #endregion
}

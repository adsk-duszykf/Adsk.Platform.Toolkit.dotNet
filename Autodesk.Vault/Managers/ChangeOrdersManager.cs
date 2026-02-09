using Autodesk.Vault.Models;
using Microsoft.Kiota.Abstractions;
using static Autodesk.Vault.Vaults.Item.ChangeOrderComments.Item.Attachments.AttachmentsRequestBuilder;
using static Autodesk.Vault.Vaults.Item.ChangeOrders.ChangeOrdersRequestBuilder;
using static Autodesk.Vault.Vaults.Item.ChangeOrders.Item.AllRelatedFiles.AllRelatedFilesRequestBuilder;
using static Autodesk.Vault.Vaults.Item.ChangeOrders.Item.AssociatedEntities.AssociatedEntitiesRequestBuilder;
using static Autodesk.Vault.Vaults.Item.ChangeOrders.Item.Comments.CommentsRequestBuilder;

namespace Autodesk.Vault.Managers;

/// <summary>
/// Manager for Change Orders operations
/// </summary>
public class ChangeOrdersManager
{
    private readonly BaseVaultClient _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChangeOrdersManager"/> class.
    /// </summary>
    /// <param name="api">The API client</param>
    public ChangeOrdersManager(BaseVaultClient api)
    {
        _api = api;
    }

    /// <summary>
    /// Get list of change orders based on a set of conditions
    /// </summary>
    /// <param name="vaultId">Vault ID</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of change orders</returns>
    public async Task<ChangeOrderCollection?> GetChangeOrdersAsync(
        string vaultId,
        Action<RequestConfiguration<ChangeOrdersRequestBuilderGetQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _api.Vaults[vaultId].ChangeOrders
            .GetAsync(requestConfiguration, cancellationToken);

        return result;
    }

    /// <summary>
    /// Get change order by its ID
    /// </summary>
    /// <param name="vaultId">Vault ID</param>
    /// <param name="changeOrderId">Change order ID</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Change order information</returns>
    public async Task<ChangeOrder?> GetChangeOrderByIdAsync(
        string vaultId,
        string changeOrderId,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _api.Vaults[vaultId].ChangeOrders[changeOrderId]
            .GetAsync(requestConfiguration, cancellationToken);

        return result;
    }

    /// <summary>
    /// Get all change order related Files by its Id. This includes files that are not only tracked by the change order,
    /// but also its associated items file associations as well.
    /// </summary>
    /// <param name="vaultId">Vault ID</param>
    /// <param name="changeOrderId">Change order ID</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of related files</returns>
    public async Task<FileVersionCollection?> GetChangeOrderRelatedFilesAsync(
        string vaultId,
        string changeOrderId,
        Action<RequestConfiguration<AllRelatedFilesRequestBuilderGetQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _api.Vaults[vaultId].ChangeOrders[changeOrderId].AllRelatedFiles
            .GetAsync(requestConfiguration, cancellationToken);

        return result;
    }

    /// <summary>
    /// Get associated entities for a change order
    /// </summary>
    /// <param name="vaultId">Vault ID</param>
    /// <param name="changeOrderId">Change order ID</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of associated entities</returns>
    public async Task<EntityCollection?> GetChangeOrderAssociatedEntitiesAsync(
        string vaultId,
        string changeOrderId,
        Action<RequestConfiguration<AssociatedEntitiesRequestBuilderGetQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _api.Vaults[vaultId].ChangeOrders[changeOrderId].AssociatedEntities
            .GetAsync(requestConfiguration, cancellationToken);

        return result;
    }

    /// <summary>
    /// Get comments for a change order
    /// </summary>
    /// <param name="vaultId">Vault ID</param>
    /// <param name="changeOrderId">Change order ID</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of comments</returns>
    public async Task<ECOCommentCollection?> GetChangeOrderCommentsAsync(
        string vaultId,
        string changeOrderId,
        Action<RequestConfiguration<CommentsRequestBuilderGetQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _api.Vaults[vaultId].ChangeOrders[changeOrderId].Comments
            .GetAsync(requestConfiguration, cancellationToken);

        return result;
    }

    /// <summary>
    /// Get attachments for a change order comment
    /// </summary>
    /// <param name="vaultId">Vault ID</param>
    /// <param name="commentId">Comment ID</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of attachments</returns>
    public async Task<FileVersionCollection?> GetChangeOrderCommentAttachmentsAsync(
        string vaultId,
        string commentId,
        Action<RequestConfiguration<AttachmentsRequestBuilderGetQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _api.Vaults[vaultId].ChangeOrderComments[commentId].Attachments
            .GetAsync(requestConfiguration, cancellationToken);

        return result;
    }
}

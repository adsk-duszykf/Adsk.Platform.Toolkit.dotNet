using System.Runtime.CompilerServices;
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
    /// Get list of change orders based on a set of conditions.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /vaults/{vaultId}/change-orders
    /// </remarks>
    /// <param name="vaultId">The unique identifier of a vault</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports query, filtering, sorting, limit, cursorState)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="ChangeOrderCollection"/> containing the change orders</returns>
    /// <example>
    /// <code>
    /// ChangeOrderCollection? changeOrders = await client.ChangeOrders.GetChangeOrdersAsync("1");
    /// </code>
    /// </example>
    public async Task<ChangeOrderCollection?> GetChangeOrdersAsync(
        string vaultId,
        RequestConfiguration<ChangeOrdersRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Vaults[vaultId].ChangeOrders
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Get a change order by its ID.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /vaults/{vaultId}/change-orders/{id}
    /// </remarks>
    /// <param name="vaultId">The unique identifier of a vault</param>
    /// <param name="changeOrderId">The unique identifier of a change order</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="ChangeOrder"/> containing the change order</returns>
    /// <example>
    /// <code>
    /// ChangeOrder? co = await client.ChangeOrders.GetChangeOrderByIdAsync("1", "42");
    /// </code>
    /// </example>
    public async Task<ChangeOrder?> GetChangeOrderByIdAsync(
        string vaultId,
        string changeOrderId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Vaults[vaultId].ChangeOrders[changeOrderId]
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Get all change order related files by its ID, including files tracked by the change order
    /// and associated items file associations. The result also includes related attachments.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /vaults/{vaultId}/change-orders/{id}/all-related-files
    /// </remarks>
    /// <param name="vaultId">The unique identifier of a vault</param>
    /// <param name="changeOrderId">The unique identifier of a change order</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports releasedOnly, extendedModels, propDefIds, limit, cursorState)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="FileVersionCollection"/> containing the related files</returns>
    /// <example>
    /// <code>
    /// FileVersionCollection? files = await client.ChangeOrders.GetChangeOrderRelatedFilesAsync("1", "42");
    /// </code>
    /// </example>
    public async Task<FileVersionCollection?> GetChangeOrderRelatedFilesAsync(
        string vaultId,
        string changeOrderId,
        RequestConfiguration<AllRelatedFilesRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Vaults[vaultId].ChangeOrders[changeOrderId].AllRelatedFiles
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Get associated entities for a change order.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /vaults/{vaultId}/change-orders/{id}/associated-entities
    /// </remarks>
    /// <param name="vaultId">The unique identifier of a vault</param>
    /// <param name="changeOrderId">The unique identifier of a change order</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports extendedModels, propDefIds, limit, cursorState)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="EntityCollection"/> containing the associated entities</returns>
    /// <example>
    /// <code>
    /// EntityCollection? entities = await client.ChangeOrders.GetChangeOrderAssociatedEntitiesAsync("1", "42");
    /// </code>
    /// </example>
    public async Task<EntityCollection?> GetChangeOrderAssociatedEntitiesAsync(
        string vaultId,
        string changeOrderId,
        RequestConfiguration<AssociatedEntitiesRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Vaults[vaultId].ChangeOrders[changeOrderId].AssociatedEntities
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Lists comments for a change order with automatic cursor-based pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /vaults/{vaultId}/change-orders/{id}/comments
    /// </remarks>
    /// <param name="vaultId">The unique identifier of a vault</param>
    /// <param name="changeOrderId">The unique identifier of a change order</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports limit, cursorState)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="ECOComment"/> items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (ECOComment comment in client.ChangeOrders.ListChangeOrderCommentsAsync("1", "42"))
    /// {
    ///     Console.WriteLine(comment.Comment);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<ECOComment> ListChangeOrderCommentsAsync(
        string vaultId,
        string changeOrderId,
        RequestConfiguration<CommentsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? cursor = requestConfiguration?.QueryParameters?.CursorState;

        while (true)
        {
            ECOCommentCollection? response = await _api.Vaults[vaultId].ChangeOrders[changeOrderId].Comments
                .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                    r.QueryParameters.CursorState = cursor;
                }, cancellationToken);

            if (response?.Results is not { Count: > 0 })
                yield break;

            foreach (ECOComment item in response.Results)
            {
                yield return item;
            }

            if (string.IsNullOrEmpty(response.Pagination?.NextUrl))
                yield break;

            cursor = PaginationHelper.ExtractBookmarkFromNextUrl(response.Pagination.NextUrl);
        }
    }

    /// <summary>
    /// Lists attachments for a change order comment with automatic cursor-based pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /vaults/{vaultId}/change-order-comments/{id}/attachments
    /// </remarks>
    /// <param name="vaultId">The unique identifier of a vault</param>
    /// <param name="commentId">The unique identifier of a change order comment</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports limit, cursorState)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="FileVersionCollection.FileVersionCollection_results"/> items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (var attachment in client.ChangeOrders.ListChangeOrderCommentAttachmentsAsync("1", "99"))
    /// {
    ///     Console.WriteLine(attachment.FileName);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<FileVersionCollection.FileVersionCollection_results> ListChangeOrderCommentAttachmentsAsync(
        string vaultId,
        string commentId,
        RequestConfiguration<AttachmentsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? cursor = requestConfiguration?.QueryParameters?.CursorState;

        while (true)
        {
            FileVersionCollection? response = await _api.Vaults[vaultId].ChangeOrderComments[commentId].Attachments
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
}

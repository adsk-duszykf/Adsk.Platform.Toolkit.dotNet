using System.Runtime.CompilerServices;
using Autodesk.Vault.Models;
using Microsoft.Kiota.Abstractions;
using static Autodesk.Vault.Vaults.Item.Links.LinksRequestBuilder;

namespace Autodesk.Vault.Managers;

/// <summary>
/// Manager for Links operations
/// </summary>
public class LinksManager
{
    private readonly BaseVaultClient _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="LinksManager"/> class.
    /// </summary>
    /// <param name="api">The API client</param>
    public LinksManager(BaseVaultClient api)
    {
        _api = api;
    }

    /// <summary>
    /// Lists links for a specific vault with automatic cursor-based pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /vaults/{vaultId}/links
    /// </remarks>
    /// <param name="vaultId">The unique identifier of a vault</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports filtering, limit, cursorState)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="LinkEntity"/> items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (LinkEntity link in client.Links.ListLinksAsync("1"))
    /// {
    ///     Console.WriteLine(link.Id);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<LinkEntity> ListLinksAsync(
        string vaultId,
        RequestConfiguration<LinksRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? cursor = requestConfiguration?.QueryParameters?.CursorState;

        while (true)
        {
            LinkCollection? response = await _api.Vaults[vaultId].Links
                .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                    r.QueryParameters.CursorState = cursor;
                }, cancellationToken);

            if (response?.Results is not { Count: > 0 })
                yield break;

            foreach (LinkEntity item in response.Results)
            {
                yield return item;
            }

            if (string.IsNullOrEmpty(response.Pagination?.NextUrl))
                yield break;

            cursor = PaginationHelper.ExtractBookmarkFromNextUrl(response.Pagination.NextUrl);
        }
    }

    /// <summary>
    /// Get a link by its ID.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /vaults/{vaultId}/links/{id}
    /// </remarks>
    /// <param name="vaultId">The unique identifier of a vault</param>
    /// <param name="linkId">The unique identifier of a link</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="LinkEntity"/> containing the link information</returns>
    /// <example>
    /// <code>
    /// LinkEntity? link = await client.Links.GetLinkByIdAsync("1", "42");
    /// </code>
    /// </example>
    public async Task<LinkEntity?> GetLinkByIdAsync(
        string vaultId,
        string linkId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Vaults[vaultId].Links[linkId]
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }
}

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
    /// Get links for a specific vault
    /// </summary>
    /// <param name="vaultId">Vault ID</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of links</returns>
    public async Task<LinkCollection?> GetLinksAsync(
        string vaultId,
        Action<RequestConfiguration<LinksRequestBuilderGetQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _api.Vaults[vaultId].Links
            .GetAsync(requestConfiguration, cancellationToken);

        return result;
    }

    /// <summary>
    /// Get a link by ID
    /// </summary>
    /// <param name="vaultId">Vault ID</param>
    /// <param name="linkId">Link ID</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Link information</returns>
    public async Task<LinkEntity?> GetLinkByIdAsync(
        string vaultId,
        string linkId,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _api.Vaults[vaultId].Links[linkId]
            .GetAsync(requestConfiguration, cancellationToken);

        return result;
    }
}

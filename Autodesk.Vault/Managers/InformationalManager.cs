using Autodesk.Vault.Models;
using Autodesk.Vault.ServerInfo;
using Autodesk.Vault.Vaults;
using Autodesk.Vault.Vaults.Item;
using Microsoft.Kiota.Abstractions;
using static Autodesk.Vault.Vaults.VaultsRequestBuilder;

namespace Autodesk.Vault.Managers;

/// <summary>
/// Manager for Informational operations (Server Info, Vaults, API Spec)
/// </summary>
public class InformationalManager
{
    private readonly BaseVaultClient _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="InformationalManager"/> class.
    /// </summary>
    /// <param name="api">The API client</param>
    public InformationalManager(BaseVaultClient api)
    {
        _api = api;
    }

    /// <summary>
    /// Get some metadata information about server such as product version, etc.
    /// </summary>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Server information</returns>
    public async Task<ServerInfoGetResponse?> GetServerInfoAsync(
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _api.ServerInfo
            .GetAsServerInfoGetResponseAsync(requestConfiguration, cancellationToken);

        return result;
    }

    /// <summary>
    /// Get the list of all knowledge vaults on the server without logging in
    /// </summary>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of vaults</returns>
    public async Task<VaultCollection?> GetVaultsAsync(
        Action<RequestConfiguration<VaultsRequestBuilderGetQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _api.Vaults
            .GetAsync(requestConfiguration, cancellationToken);

        return result;
    }

    /// <summary>
    /// Get Knowledge vault based on its ID
    /// </summary>
    /// <param name="vaultId">Vault ID</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Vault information</returns>
    public async Task<Models.Vault?> GetVaultByIdAsync(
        string vaultId,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _api.Vaults[vaultId]
            .GetAsync(requestConfiguration, cancellationToken);

        return result;
    }
}

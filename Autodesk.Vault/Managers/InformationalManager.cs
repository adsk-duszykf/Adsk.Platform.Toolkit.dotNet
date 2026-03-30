using Autodesk.Vault.Models;
using Autodesk.Vault.ServerInfo;
using Microsoft.Kiota.Abstractions;
using static Autodesk.Vault.Vaults.VaultsRequestBuilder;

namespace Autodesk.Vault.Managers;

/// <summary>
/// Manager for Informational operations (Server Info, Vaults)
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
    /// Get metadata information about the server such as product version.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /server-info
    /// </remarks>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="ServerInfoGetResponse"/> containing the server information</returns>
    /// <example>
    /// <code>
    /// ServerInfoGetResponse? info = await client.Informational.GetServerInfoAsync();
    /// Console.WriteLine(info?.ProductVersion);
    /// </code>
    /// </example>
    public async Task<ServerInfoGetResponse?> GetServerInfoAsync(
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.ServerInfo
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Get the list of all Knowledge Vaults on the server without logging in.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /vaults
    /// </remarks>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports limit, cursorState)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="VaultCollection"/> containing the list of vaults</returns>
    /// <example>
    /// <code>
    /// VaultCollection? vaults = await client.Informational.GetVaultsAsync();
    /// </code>
    /// </example>
    public async Task<VaultCollection?> GetVaultsAsync(
        RequestConfiguration<VaultsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Vaults
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Get a Knowledge Vault by its ID.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /vaults/{id}
    /// </remarks>
    /// <param name="vaultId">The unique identifier of a vault</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Models.Vault"/> containing the vault information</returns>
    /// <example>
    /// <code>
    /// Autodesk.Vault.Models.Vault? vault = await client.Informational.GetVaultByIdAsync("1");
    /// Console.WriteLine(vault?.Name);
    /// </code>
    /// </example>
    public async Task<Models.Vault?> GetVaultByIdAsync(
        string vaultId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Vaults[vaultId]
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }
}

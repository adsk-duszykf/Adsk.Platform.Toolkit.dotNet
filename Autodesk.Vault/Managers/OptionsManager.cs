using Autodesk.Vault.Models;
using Autodesk.Vault.SystemOptions;
using Autodesk.Vault.SystemOptions.Item;
using Autodesk.Vault.Vaults.Item.VaultOptions;
using Autodesk.Vault.Vaults.Item.VaultOptions.Item;
using Microsoft.Kiota.Abstractions;
using static Autodesk.Vault.SystemOptions.SystemOptionsRequestBuilder;
using static Autodesk.Vault.Vaults.Item.VaultOptions.VaultOptionsRequestBuilder;

namespace Autodesk.Vault.Managers;

/// <summary>
/// Manager for Options operations (System Options and Vault Options)
/// </summary>
public class OptionsManager
{
    private readonly BaseVaultClient _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="OptionsManager"/> class.
    /// </summary>
    /// <param name="api">The API client</param>
    public OptionsManager(BaseVaultClient api)
    {
        _api = api;
    }

    #region System Options

    /// <summary>
    /// Returns list of options which applies to the entire system
    /// </summary>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of system options</returns>
    public async Task<VaultOptionCollection?> GetSystemOptionsAsync(
        Action<RequestConfiguration<SystemOptionsRequestBuilderGetQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _api.SystemOptions
            .GetAsync(requestConfiguration, cancellationToken);

        return result;
    }

    /// <summary>
    /// Creates a system wide option with input name and value
    /// </summary>
    /// <param name="optionData">System option data</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created system option</returns>
    public async Task<VaultOption?> CreateSystemOptionAsync(
        SystemOptionsPostRequestBody optionData,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _api.SystemOptions
            .PostAsync(optionData, requestConfiguration, cancellationToken);

        return result;
    }

    /// <summary>
    /// Get an option (By ID) which applies to the entire system
    /// </summary>
    /// <param name="optionId">System option ID</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>System option information</returns>
    public async Task<VaultOption?> GetSystemOptionByIdAsync(
        string optionId,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _api.SystemOptions[optionId]
            .GetAsync(requestConfiguration, cancellationToken);

        return result;
    }

    /// <summary>
    /// Update an option (By ID) which applies to the entire system
    /// </summary>
    /// <param name="optionId">System option ID</param>
    /// <param name="optionData">Updated system option data</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated system option</returns>
    public async Task<VaultOption?> UpdateSystemOptionByIdAsync(
        string optionId,
        SystemOptionsPatchRequestBody optionData,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _api.SystemOptions[optionId]
            .PatchAsync(optionData, requestConfiguration, cancellationToken);

        return result;
    }

    /// <summary>
    /// Delete an option (By ID) which applies to the entire system
    /// </summary>
    /// <param name="optionId">System option ID</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public async Task DeleteSystemOptionByIdAsync(
        string optionId,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        await _api.SystemOptions[optionId]
            .DeleteAsync(requestConfiguration, cancellationToken);
    }

    #endregion

    #region Vault Options

    /// <summary>
    /// Get vault options for a specific vault
    /// </summary>
    /// <param name="vaultId">Vault ID</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of vault options</returns>
    public async Task<VaultOptionCollection?> GetVaultOptionsAsync(
        string vaultId,
        Action<RequestConfiguration<VaultOptionsRequestBuilderGetQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _api.Vaults[vaultId].VaultOptions
            .GetAsync(requestConfiguration, cancellationToken);

        return result;
    }

    /// <summary>
    /// Create a vault option
    /// </summary>
    /// <param name="vaultId">Vault ID</param>
    /// <param name="optionData">Vault option data</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created vault option</returns>
    public async Task<VaultOption?> CreateVaultOptionAsync(
        string vaultId,
        VaultOptionsPostRequestBody optionData,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _api.Vaults[vaultId].VaultOptions
            .PostAsync(optionData, requestConfiguration, cancellationToken);

        return result;
    }

    /// <summary>
    /// Get a vault option by ID
    /// </summary>
    /// <param name="vaultId">Vault ID</param>
    /// <param name="optionId">Vault option ID</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Vault option information</returns>
    public async Task<VaultOption?> GetVaultOptionByIdAsync(
        string vaultId,
        string optionId,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _api.Vaults[vaultId].VaultOptions[optionId]
            .GetAsync(requestConfiguration, cancellationToken);

        return result;
    }

    /// <summary>
    /// Update a vault option by ID
    /// </summary>
    /// <param name="vaultId">Vault ID</param>
    /// <param name="optionId">Vault option ID</param>
    /// <param name="optionData">Updated vault option data</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated vault option</returns>
    public async Task<VaultOption?> UpdateVaultOptionByIdAsync(
        string vaultId,
        string optionId,
        VaultOptionsPatchRequestBody optionData,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _api.Vaults[vaultId].VaultOptions[optionId]
            .PatchAsync(optionData, requestConfiguration, cancellationToken);

        return result;
    }

    /// <summary>
    /// Delete a vault option by ID
    /// </summary>
    /// <param name="vaultId">Vault ID</param>
    /// <param name="optionId">Vault option ID</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public async Task DeleteVaultOptionByIdAsync(
        string vaultId,
        string optionId,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        await _api.Vaults[vaultId].VaultOptions[optionId]
            .DeleteAsync(requestConfiguration, cancellationToken);
    }

    #endregion
}

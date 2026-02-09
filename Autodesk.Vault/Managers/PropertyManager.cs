using Autodesk.Vault.Models;
using Autodesk.Vault.Vaults.Item.PropertyDefinitions;
using Autodesk.Vault.Vaults.Item.PropertyDefinitions.Item;
using Microsoft.Kiota.Abstractions;
using static Autodesk.Vault.Vaults.Item.PropertyDefinitions.PropertyDefinitionsRequestBuilder;

namespace Autodesk.Vault.Managers;

/// <summary>
/// Manager for Property Definition operations
/// </summary>
public class PropertyManager
{
    private readonly BaseVaultClient _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyManager"/> class.
    /// </summary>
    /// <param name="api">The API client</param>
    public PropertyManager(BaseVaultClient api)
    {
        _api = api;
    }

    /// <summary>
    /// Get property definitions for a specific vault
    /// </summary>
    /// <param name="vaultId">Vault ID</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of property definitions</returns>
    public async Task<PropertyDefinitionCollection?> GetPropertyDefinitionsAsync(
        string vaultId,
        Action<RequestConfiguration<PropertyDefinitionsRequestBuilderGetQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _api.Vaults[vaultId].PropertyDefinitions
            .GetAsync(requestConfiguration, cancellationToken);

        return result;
    }

    /// <summary>
    /// Get a property definition by ID
    /// </summary>
    /// <param name="vaultId">Vault ID</param>
    /// <param name="propertyDefinitionId">Property definition ID</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Property definition information</returns>
    public async Task<PropertyDefinition?> GetPropertyDefinitionByIdAsync(
        string vaultId,
        string propertyDefinitionId,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _api.Vaults[vaultId].PropertyDefinitions[propertyDefinitionId]
            .GetAsync(requestConfiguration, cancellationToken);

        return result;
    }
}


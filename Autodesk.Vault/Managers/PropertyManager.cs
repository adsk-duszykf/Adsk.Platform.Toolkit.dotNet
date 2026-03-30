using System.Runtime.CompilerServices;
using Autodesk.Vault.Models;
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
    /// Lists property definitions for a specific vault with automatic cursor-based pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /vaults/{vaultId}/property-definitions
    /// </remarks>
    /// <param name="vaultId">The unique identifier of a vault</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports filtering, limit, cursorState)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="PropertyDefinitionCollection.PropertyDefinitionCollection_results"/> items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (var propDef in client.Properties.ListPropertyDefinitionsAsync("1"))
    /// {
    ///     Console.WriteLine(propDef.DisplayName);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<PropertyDefinitionCollection.PropertyDefinitionCollection_results> ListPropertyDefinitionsAsync(
        string vaultId,
        RequestConfiguration<PropertyDefinitionsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? cursor = requestConfiguration?.QueryParameters?.CursorState;

        while (true)
        {
            PropertyDefinitionCollection? response = await _api.Vaults[vaultId].PropertyDefinitions
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

    /// <summary>
    /// Get a property definition by its ID.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /vaults/{vaultId}/property-definitions/{id}
    /// </remarks>
    /// <param name="vaultId">The unique identifier of a vault</param>
    /// <param name="propertyDefinitionId">The unique identifier of a property definition</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="PropertyDefinition"/> containing the property definition</returns>
    /// <example>
    /// <code>
    /// PropertyDefinition? propDef = await client.Properties.GetPropertyDefinitionByIdAsync("1", "42");
    /// Console.WriteLine(propDef?.DisplayName);
    /// </code>
    /// </example>
    public async Task<PropertyDefinition?> GetPropertyDefinitionByIdAsync(
        string vaultId,
        string propertyDefinitionId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Vaults[vaultId].PropertyDefinitions[propertyDefinitionId]
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }
}

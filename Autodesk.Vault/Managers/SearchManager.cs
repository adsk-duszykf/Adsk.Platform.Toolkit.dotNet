using Autodesk.Vault.Models;
using Autodesk.Vault.Vaults.WithVaultIdAdvancedSearch;
using Microsoft.Kiota.Abstractions;
using static Autodesk.Vault.Vaults.Item.SearchResults.SearchResultsRequestBuilder;
using static Autodesk.Vault.Vaults.WithVaultIdAdvancedSearch.WithVaultIdAdvancedSearchRequestBuilder;

namespace Autodesk.Vault.Managers;

/// <summary>
/// Manager for Search operations
/// </summary>
public class SearchManager
{
    private readonly BaseVaultClient _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchManager"/> class.
    /// </summary>
    /// <param name="api">The API client</param>
    public SearchManager(BaseVaultClient api)
    {
        _api = api;
    }

    /// <summary>
    /// Get search results for a specific vault
    /// </summary>
    /// <param name="vaultId">Vault ID</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Search results</returns>
    public async Task<EntityCollection?> GetSearchResultsAsync(
        string vaultId,
        Action<RequestConfiguration<SearchResultsRequestBuilderGetQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _api.Vaults[vaultId].SearchResults
            .GetAsync(requestConfiguration, cancellationToken);

        return result;
    }

    /// <summary>
    /// Perform advanced search in a vault
    /// </summary>
    /// <param name="vaultId">Vault ID</param>
    /// <param name="searchBody">Advanced search criteria</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Search results</returns>
    public async Task<EntityCollection?> PerformAdvancedSearchAsync(
        string vaultId,
        WithVaultIdAdvancedSearchPostRequestBody searchBody,
        Action<RequestConfiguration<WithVaultIdAdvancedSearchRequestBuilderPostQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _api.Vaults.WithVaultIdAdvancedSearch(vaultId)
            .PostAsync(searchBody, requestConfiguration, cancellationToken);

        return result;
    }
}

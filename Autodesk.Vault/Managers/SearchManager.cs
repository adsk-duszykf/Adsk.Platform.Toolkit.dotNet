using System.Runtime.CompilerServices;
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
    /// Search a vault using a simple query string with automatic cursor-based pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /vaults/{vaultId}/search-results
    /// </remarks>
    /// <param name="vaultId">The unique identifier of a vault</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports query, filtering, sorting, limit, cursorState)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="EntityCollection.EntityCollection_results"/> across all pages</returns>
    /// <example>
    /// <code>
    /// await foreach (EntityCollection.EntityCollection_results result in client.Search.GetSearchResultsAsync("1",
    ///     new RequestConfiguration&lt;SearchResultsRequestBuilderGetQueryParameters&gt;
    ///     {
    ///         QueryParameters = new SearchResultsRequestBuilderGetQueryParameters { Q = "assembly" }
    ///     }))
    /// {
    ///     Console.WriteLine(result.FileVersion?.Name ?? result.Folder?.Name);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<EntityCollection.EntityCollection_results> GetSearchResultsAsync(
        string vaultId,
        RequestConfiguration<SearchResultsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? cursor = requestConfiguration?.QueryParameters?.CursorState;

        while (true)
        {
            EntityCollection? response = await _api.Vaults[vaultId].SearchResults
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
    /// Perform an advanced search in a vault using structured search criteria with automatic cursor-based pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /vaults/{vaultId}:advanced-search
    /// </remarks>
    /// <param name="vaultId">The unique identifier of a vault</param>
    /// <param name="searchBody">The advanced search criteria including search conditions and sort criteria</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports limit, cursorState)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="EntityCollection.EntityCollection_results"/> across all pages</returns>
    /// <example>
    /// <code>
    /// await foreach (EntityCollection.EntityCollection_results result in client.Search.PerformAdvancedSearchAsync("1",
    ///     new WithVaultIdAdvancedSearchPostRequestBody
    ///     {
    ///         SearchCriterias = new List&lt;SearchCriteria&gt;
    ///         {
    ///             new SearchCriteria { Operator = SearchCriteria_operator.Contains, SearchString = "assembly" }
    ///         }
    ///     }))
    /// {
    ///     Console.WriteLine(result.FileVersion?.Name ?? result.Folder?.Name);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<EntityCollection.EntityCollection_results> PerformAdvancedSearchAsync(
        string vaultId,
        WithVaultIdAdvancedSearchPostRequestBody searchBody,
        RequestConfiguration<WithVaultIdAdvancedSearchRequestBuilderPostQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? cursor = requestConfiguration?.QueryParameters?.CursorState;

        while (true)
        {
            EntityCollection? response = await _api.Vaults.WithVaultIdAdvancedSearch(vaultId)
                .PostAsync(searchBody, r =>
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

using System.Runtime.CompilerServices;
using Autodesk.Parameters.Parameters.V1.Accounts.Item.Groups.Item.Collections;
using Autodesk.Parameters.Parameters.V1.Accounts.Item.Groups.Item.Collections.Item;
using Microsoft.Kiota.Abstractions;

namespace Autodesk.Parameters.Managers;

/// <summary>
/// Manager for Parameter collection operations
/// </summary>
public class CollectionsManager
{
    private readonly BaseParametersClient _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="CollectionsManager"/> class.
    /// </summary>
    /// <param name="api">The API client</param>
    public CollectionsManager(BaseParametersClient api)
    {
        _api = api;
    }

    /// <summary>
    /// Lists all parameter collections in the specified group with automatic pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /parameters/v1/accounts/{accountId}/groups/{groupId}/collections
    /// APS docs: https://aps.autodesk.com/en/docs/parameters/v1/reference/http/parameters-listparametercollections-GET
    /// </remarks>
    /// <param name="accountId">The Autodesk Construction Cloud account ID</param>
    /// <param name="groupId">The group ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports limit, offset, joinConstructionData)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="CollectionsGetResponse_results"/> items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (var collection in client.CollectionsManager.ListCollectionsAsync(accountId, groupId))
    /// {
    ///     Console.WriteLine($"{collection.Id}: {collection.Title}");
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<CollectionsGetResponse_results> ListCollectionsAsync(
        Guid accountId,
        string groupId,
        RequestConfiguration<CollectionsRequestBuilder.CollectionsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        int offset = requestConfiguration?.QueryParameters?.Offset ?? 0;

        while (true)
        {
            var response = await _api.Parameters.V1.Accounts[accountId].Groups[groupId].Collections
                .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                    r.QueryParameters.Offset = offset;
                }, cancellationToken);

            if (response?.Results is not { Count: > 0 })
                yield break;

            foreach (var item in response.Results)
            {
                yield return item;
            }

            if (string.IsNullOrEmpty(response.Pagination?.NextUrl))
                yield break;

            offset += response.Results.Count;
        }
    }

    /// <summary>
    /// Gets the details of a parameter collection.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /parameters/v1/accounts/{accountId}/groups/{groupId}/collections/{collectionId}
    /// APS docs: https://aps.autodesk.com/en/docs/parameters/v1/reference/http/parameters-getcollection-GET
    /// </remarks>
    /// <param name="accountId">The Autodesk Construction Cloud account ID</param>
    /// <param name="groupId">The group ID</param>
    /// <param name="collectionId">The collection ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="WithCollectionGetResponse"/> containing the collection details</returns>
    /// <example>
    /// <code>
    /// WithCollectionGetResponse? collection = await client.CollectionsManager.GetCollectionAsync(accountId, groupId, "my-collection-id");
    /// Console.WriteLine(collection?.Title);
    /// </code>
    /// </example>
    public async Task<WithCollectionGetResponse?> GetCollectionAsync(
        Guid accountId,
        string groupId,
        string collectionId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Parameters.V1.Accounts[accountId].Groups[groupId].Collections[collectionId]
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Creates a new collection for associating with parameters.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /parameters/v1/accounts/{accountId}/groups/{groupId}/collections
    /// APS docs: https://aps.autodesk.com/en/docs/parameters/v1/reference/http/parameters-createcollection-POST
    /// </remarks>
    /// <param name="accountId">The Autodesk Construction Cloud account ID</param>
    /// <param name="groupId">The group ID</param>
    /// <param name="body">The collection creation payload</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="CollectionsPostResponse"/> containing the created collection details</returns>
    /// <example>
    /// <code>
    /// CollectionsPostResponse? created = await client.CollectionsManager.CreateCollectionAsync(accountId, groupId, body);
    /// Console.WriteLine(created?.Id);
    /// </code>
    /// </example>
    public async Task<CollectionsPostResponse?> CreateCollectionAsync(
        Guid accountId,
        string groupId,
        CollectionsPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Parameters.V1.Accounts[accountId].Groups[groupId].Collections
            .PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Updates the details of an existing parameter collection. The title cannot be empty or null.
    /// </summary>
    /// <remarks>
    /// Wraps: PUT /parameters/v1/accounts/{accountId}/groups/{groupId}/collections/{collectionId}
    /// APS docs: https://aps.autodesk.com/en/docs/parameters/v1/reference/http/parameters-updatecollection-PUT
    /// </remarks>
    /// <param name="accountId">The Autodesk Construction Cloud account ID</param>
    /// <param name="groupId">The group ID</param>
    /// <param name="collectionId">The collection ID</param>
    /// <param name="body">The collection update payload</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="WithCollectionPutResponse"/> containing the updated collection details</returns>
    /// <example>
    /// <code>
    /// WithCollectionPutResponse? updated = await client.CollectionsManager.UpdateCollectionAsync(accountId, groupId, "my-collection-id", body);
    /// </code>
    /// </example>
    public async Task<WithCollectionPutResponse?> UpdateCollectionAsync(
        Guid accountId,
        string groupId,
        string collectionId,
        WithCollectionPutRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Parameters.V1.Accounts[accountId].Groups[groupId].Collections[collectionId]
            .PutAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }
}

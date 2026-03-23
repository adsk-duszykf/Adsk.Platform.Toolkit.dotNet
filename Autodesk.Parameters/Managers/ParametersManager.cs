using System.Runtime.CompilerServices;
using Autodesk.Parameters.Parameters.V1.Accounts.Item.Groups.Item.Collections.Item.Parameters;
using Autodesk.Parameters.Parameters.V1.Accounts.Item.Groups.Item.Collections.Item.ParametersBatchShare;
using Autodesk.Parameters.Parameters.V1.Accounts.Item.Groups.Item.Collections.Item.ParametersBatchUnshare;
using Autodesk.Parameters.Parameters.V1.Accounts.Item.Groups.Item.Collections.Item.ParametersSearch;
using Autodesk.Parameters.Parameters.V1.Accounts.Item.Groups.Item.Collections.Item.ParametersSearchIndices;
using Autodesk.Parameters.Parameters.V1.ParametersRender;
using Autodesk.Parameters.Parameters.V1.Parameters.Item;
using Microsoft.Kiota.Abstractions;

namespace Autodesk.Parameters.Managers;

/// <summary>
/// Manager for Parameter operations including CRUD, sharing, searching, and rendering
/// </summary>
public class ParametersManager
{
    private readonly BaseParametersClient _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="ParametersManager"/> class.
    /// </summary>
    /// <param name="api">The API client</param>
    public ParametersManager(BaseParametersClient api)
    {
        _api = api;
    }

    /// <summary>
    /// Lists parameters in a parameter collection. Optionally, attempt to localize the information according to a given localization fallback list.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /parameters/v1/accounts/{accountId}/groups/{groupId}/collections/{collectionId}/parameters
    /// APS docs: https://aps.autodesk.com/en/docs/parameters/v1/reference/http/parameters-listparameters-GET
    /// </remarks>
    /// <param name="accountId">The Autodesk Construction Cloud account ID</param>
    /// <param name="groupId">The group ID</param>
    /// <param name="collectionId">The collection ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports ids, limit, offset)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="ParametersGetResponse"/> containing the parameter details</returns>
    /// <example>
    /// <code>
    /// ParametersGetResponse? parameters = await client.ParametersManager.GetParametersAsync(accountId, groupId, collectionId);
    /// </code>
    /// </example>
    public async Task<ParametersGetResponse?> GetParametersAsync(
        Guid accountId,
        string groupId,
        string collectionId,
        RequestConfiguration<ParametersRequestBuilder.ParametersRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Parameters.V1.Accounts[accountId].Groups[groupId].Collections[collectionId].Parameters
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Creates new parameter definitions. Definition data is immutable (id, name, dataTypeId, readOnly).
    /// </summary>
    /// <remarks>
    /// Wraps: POST /parameters/v1/accounts/{accountId}/groups/{groupId}/collections/{collectionId}/parameters
    /// APS docs: https://aps.autodesk.com/en/docs/parameters/v1/reference/http/parameters-createparameters-POST
    /// </remarks>
    /// <param name="accountId">The Autodesk Construction Cloud account ID</param>
    /// <param name="groupId">The group ID</param>
    /// <param name="collectionId">The collection ID</param>
    /// <param name="body">The parameter creation payload</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="ParametersPostResponse"/> containing the created parameters</returns>
    /// <example>
    /// <code>
    /// ParametersPostResponse? created = await client.ParametersManager.CreateParametersAsync(accountId, groupId, collectionId, body);
    /// </code>
    /// </example>
    public async Task<ParametersPostResponse?> CreateParametersAsync(
        Guid accountId,
        string groupId,
        string collectionId,
        ParametersPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Parameters.V1.Accounts[accountId].Groups[groupId].Collections[collectionId].Parameters
            .PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Updates a list of parameters (max 10 per request) with description, metadata (isHidden, revitCategoryBindingIds, instanceTypeAssociation, labelIds).
    /// </summary>
    /// <remarks>
    /// Wraps: PATCH /parameters/v1/accounts/{accountId}/groups/{groupId}/collections/{collectionId}/parameters
    /// APS docs: https://aps.autodesk.com/en/docs/parameters/v1/reference/http/parameters-updateparameters-PATCH
    /// </remarks>
    /// <param name="accountId">The Autodesk Construction Cloud account ID</param>
    /// <param name="groupId">The group ID</param>
    /// <param name="collectionId">The collection ID</param>
    /// <param name="body">The parameter update payload</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="ParametersPatchResponse"/> containing the updated parameters</returns>
    /// <example>
    /// <code>
    /// ParametersPatchResponse? updated = await client.ParametersManager.UpdateParametersAsync(accountId, groupId, collectionId, body);
    /// </code>
    /// </example>
    public async Task<ParametersPatchResponse?> UpdateParametersAsync(
        Guid accountId,
        string groupId,
        string collectionId,
        ParametersPatchRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Parameters.V1.Accounts[accountId].Groups[groupId].Collections[collectionId].Parameters
            .PatchAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Gets a parameter definition by ID. Optionally, attempt to localize the information according to a given localization fallback list.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /parameters/v1/parameters/{parameterId}
    /// APS docs: https://aps.autodesk.com/en/docs/parameters/v1/reference/http/parameters-getparameterv1-GET
    /// </remarks>
    /// <param name="parameterId">The parameter ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="WithParameterGetResponse"/> containing the parameter details</returns>
    /// <example>
    /// <code>
    /// WithParameterGetResponse? param = await client.ParametersManager.GetParameterByIdAsync("my-parameter-id");
    /// Console.WriteLine(param?.Name);
    /// </code>
    /// </example>
    public async Task<WithParameterGetResponse?> GetParameterByIdAsync(
        string parameterId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Parameters.V1.Parameters[parameterId]
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Renders the parameters from the source unit/symbol to the target unit/symbol with number format and precision.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /parameters/v1/parameters:render
    /// APS docs: https://aps.autodesk.com/en/docs/parameters/v1/reference/http/parameters-renderparametersv1-POST
    /// </remarks>
    /// <param name="body">The render request payload with source/target units and precision</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="ParametersRenderPostResponse"/> containing the rendered values</returns>
    /// <example>
    /// <code>
    /// ParametersRenderPostResponse? rendered = await client.ParametersManager.RenderParametersAsync(body);
    /// </code>
    /// </example>
    public async Task<ParametersRenderPostResponse?> RenderParametersAsync(
        ParametersRenderPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Parameters.V1.ParametersRender
            .PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Adds the specified parameters to a parameter collection (batch share).
    /// </summary>
    /// <remarks>
    /// Wraps: POST /parameters/v1/accounts/{accountId}/groups/{groupId}/collections/{collectionId}/parameters:batch-share
    /// APS docs: https://aps.autodesk.com/en/docs/parameters/v1/reference/http/parameters-batchshareparameters-POST
    /// </remarks>
    /// <param name="accountId">The Autodesk Construction Cloud account ID</param>
    /// <param name="groupId">The group ID</param>
    /// <param name="collectionId">The collection ID</param>
    /// <param name="body">The batch share payload with parameter IDs</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="ParametersBatchSharePostResponse"/> containing the shared parameters</returns>
    /// <example>
    /// <code>
    /// ParametersBatchSharePostResponse? result = await client.ParametersManager.BatchShareParametersAsync(accountId, groupId, collectionId, body);
    /// </code>
    /// </example>
    public async Task<ParametersBatchSharePostResponse?> BatchShareParametersAsync(
        Guid accountId,
        string groupId,
        string collectionId,
        ParametersBatchSharePostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Parameters.V1.Accounts[accountId].Groups[groupId].Collections[collectionId].ParametersBatchShare
            .PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Removes the specified parameters from a parameter collection (batch unshare).
    /// </summary>
    /// <remarks>
    /// Wraps: POST /parameters/v1/accounts/{accountId}/groups/{groupId}/collections/{collectionId}/parameters:batch-unshare
    /// APS docs: https://aps.autodesk.com/en/docs/parameters/v1/reference/http/parameters-batchunshareparameters-POST
    /// </remarks>
    /// <param name="accountId">The Autodesk Construction Cloud account ID</param>
    /// <param name="groupId">The group ID</param>
    /// <param name="collectionId">The collection ID</param>
    /// <param name="body">The batch unshare payload with parameter IDs</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="ParametersBatchUnsharePostResponse"/> containing the unshared parameters</returns>
    /// <example>
    /// <code>
    /// ParametersBatchUnsharePostResponse? result = await client.ParametersManager.BatchUnshareParametersAsync(accountId, groupId, collectionId, body);
    /// </code>
    /// </example>
    public async Task<ParametersBatchUnsharePostResponse?> BatchUnshareParametersAsync(
        Guid accountId,
        string groupId,
        string collectionId,
        ParametersBatchUnsharePostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Parameters.V1.Accounts[accountId].Groups[groupId].Collections[collectionId].ParametersBatchUnshare
            .PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Searches among the parameters in a parameter collection with automatic pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /parameters/v1/accounts/{accountId}/groups/{groupId}/collections/{collectionId}/parameters:search
    /// APS docs: https://aps.autodesk.com/en/docs/parameters/v1/reference/http/parameters-searchparametersv2-POST
    /// </remarks>
    /// <param name="accountId">The Autodesk Construction Cloud account ID</param>
    /// <param name="groupId">The group ID</param>
    /// <param name="collectionId">The collection ID</param>
    /// <param name="body">The search request payload with filters and sorting</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports limit, offset)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="ParametersSearchPostResponse_results"/> items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (var param in client.ParametersManager.SearchParametersAsync(accountId, groupId, collectionId, searchBody))
    /// {
    ///     Console.WriteLine($"{param.Id}: {param.Name}");
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<ParametersSearchPostResponse_results> SearchParametersAsync(
        Guid accountId,
        string groupId,
        string collectionId,
        ParametersSearchPostRequestBody body,
        RequestConfiguration<ParametersSearchRequestBuilder.ParametersSearchRequestBuilderPostQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        int offset = requestConfiguration?.QueryParameters?.Offset ?? 0;

        while (true)
        {
            var response = await _api.Parameters.V1.Accounts[accountId].Groups[groupId].Collections[collectionId].ParametersSearch
                .PostAsync(body, r =>
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
    /// Gets search indices reporting occurrence counts by various data types (labels, category bindings, etc.).
    /// </summary>
    /// <remarks>
    /// Wraps: POST /parameters/v1/accounts/{accountId}/groups/{groupId}/collections/{collectionId}/parameters:search-indices
    /// APS docs: https://aps.autodesk.com/en/docs/parameters/v1/reference/http/parameters-searchparametersindicesv2-POST
    /// </remarks>
    /// <param name="accountId">The Autodesk Construction Cloud account ID</param>
    /// <param name="groupId">The group ID</param>
    /// <param name="collectionId">The collection ID</param>
    /// <param name="body">The search indices request payload with filters</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="ParametersSearchIndicesPostResponse"/> containing the search index counts</returns>
    /// <example>
    /// <code>
    /// ParametersSearchIndicesPostResponse? indices = await client.ParametersManager.GetSearchIndicesAsync(accountId, groupId, collectionId, body);
    /// </code>
    /// </example>
    public async Task<ParametersSearchIndicesPostResponse?> GetSearchIndicesAsync(
        Guid accountId,
        string groupId,
        string collectionId,
        ParametersSearchIndicesPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Parameters.V1.Accounts[accountId].Groups[groupId].Collections[collectionId].ParametersSearchIndices
            .PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }
}

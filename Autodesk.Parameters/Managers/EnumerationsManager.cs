using System.Runtime.CompilerServices;
using Autodesk.Parameters.Parameters.V1.Enumerations;
using Autodesk.Parameters.Parameters.V1.Enumerations.Item;
using Autodesk.Parameters.Parameters.V1.EnumerationsBatchCreate;
using Autodesk.Parameters.Parameters.V1.EnumerationsBatchUpdate;
using Microsoft.Kiota.Abstractions;

namespace Autodesk.Parameters.Managers;

/// <summary>
/// Manager for Enumeration operations
/// </summary>
public class EnumerationsManager
{
    private readonly BaseParametersClient _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="EnumerationsManager"/> class.
    /// </summary>
    /// <param name="api">The API client</param>
    public EnumerationsManager(BaseParametersClient api)
    {
        _api = api;
    }

    /// <summary>
    /// Lists enumerations in an ACC Account with automatic pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /parameters/v1/enumerations
    /// APS docs: https://aps.autodesk.com/en/docs/parameters/v1/reference/http/parameters-listenumerations-GET
    /// </remarks>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports accountId, ids, limit, offset)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="EnumerationsGetResponse_results"/> items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// var listConfig = new RequestConfiguration&lt;EnumerationsRequestBuilder.EnumerationsRequestBuilderGetQueryParameters&gt;
    /// {
    ///     QueryParameters = new() { AccountId = accountId }
    /// };
    /// await foreach (var enumeration in client.EnumerationsManager.ListEnumerationsAsync(listConfig))
    /// {
    ///     Console.WriteLine($"{enumeration.Id}: {enumeration.Name}");
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<EnumerationsGetResponse_results> ListEnumerationsAsync(
        RequestConfiguration<EnumerationsRequestBuilder.EnumerationsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        int offset = requestConfiguration?.QueryParameters?.Offset ?? 0;

        while (true)
        {
            var response = await _api.Parameters.V1.Enumerations
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
    /// Creates new enumeration definitions.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /parameters/v1/enumerations:batch-create
    /// APS docs: https://aps.autodesk.com/en/docs/parameters/v1/reference/http/parameters-createenumerations-POST
    /// </remarks>
    /// <param name="body">The enumeration creation payload</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="EnumerationsBatchCreatePostResponse"/> containing the created enumerations</returns>
    /// <example>
    /// <code>
    /// EnumerationsBatchCreatePostResponse? created = await client.EnumerationsManager.BatchCreateEnumerationsAsync(body);
    /// </code>
    /// </example>
    public async Task<EnumerationsBatchCreatePostResponse?> BatchCreateEnumerationsAsync(
        EnumerationsBatchCreatePostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Parameters.V1.EnumerationsBatchCreate
            .PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Updates a list of enumerations (max 50 per request) with the given properties, including content.
    /// </summary>
    /// <remarks>
    /// Wraps: PATCH /parameters/v1/enumerations:batch-update
    /// APS docs: https://aps.autodesk.com/en/docs/parameters/v1/reference/http/parameters-updateenumerations-PATCH
    /// </remarks>
    /// <param name="body">The enumeration batch update payload</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports accountId)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="EnumerationsBatchUpdatePatchResponse"/> containing the updated enumerations</returns>
    /// <example>
    /// <code>
    /// EnumerationsBatchUpdatePatchResponse? updated = await client.EnumerationsManager.BatchUpdateEnumerationsAsync(body, config =>
    /// {
    ///     config.QueryParameters.AccountId = accountId;
    /// });
    /// </code>
    /// </example>
    public async Task<EnumerationsBatchUpdatePatchResponse?> BatchUpdateEnumerationsAsync(
        EnumerationsBatchUpdatePatchRequestBody body,
        RequestConfiguration<EnumerationsBatchUpdateRequestBuilder.EnumerationsBatchUpdateRequestBuilderPatchQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Parameters.V1.EnumerationsBatchUpdate
            .PatchAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Updates a single enumeration with the given properties, including content.
    /// </summary>
    /// <remarks>
    /// Wraps: PATCH /parameters/v1/enumerations/{enumerationId}
    /// APS docs: https://aps.autodesk.com/en/docs/parameters/v1/reference/http/parameters-updateenumeration-PATCH
    /// </remarks>
    /// <param name="enumerationId">The ID of the enumeration to update</param>
    /// <param name="body">The enumeration update payload</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports accountId)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="WithEnumerationPatchResponse"/> containing the updated enumeration</returns>
    /// <example>
    /// <code>
    /// WithEnumerationPatchResponse? updated = await client.EnumerationsManager.UpdateEnumerationAsync("my-enum-id", body, config =>
    /// {
    ///     config.QueryParameters.AccountId = accountId;
    /// });
    /// </code>
    /// </example>
    public async Task<WithEnumerationPatchResponse?> UpdateEnumerationAsync(
        string enumerationId,
        WithEnumerationPatchRequestBody body,
        RequestConfiguration<WithEnumerationItemRequestBuilder.WithEnumerationItemRequestBuilderPatchQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Parameters.V1.Enumerations[enumerationId]
            .PatchAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }
}

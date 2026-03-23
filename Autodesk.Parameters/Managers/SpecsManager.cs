using System.Runtime.CompilerServices;
using Autodesk.Parameters.Parameters.V1.Specs;
using Autodesk.Parameters.Parameters.V1.Specs.Item;
using Autodesk.Parameters.Parameters.V1.SpecsBatchCreate;
using Autodesk.Parameters.Parameters.V1.SpecsBatchUpdate;
using Microsoft.Kiota.Abstractions;

namespace Autodesk.Parameters.Managers;

/// <summary>
/// Manager for Spec operations
/// </summary>
public class SpecsManager
{
    private readonly BaseParametersClient _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="SpecsManager"/> class.
    /// </summary>
    /// <param name="api">The API client</param>
    public SpecsManager(BaseParametersClient api)
    {
        _api = api;
    }

    /// <summary>
    /// Lists parameter specs used in the system with automatic pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /parameters/v1/specs
    /// APS docs: https://aps.autodesk.com/en/docs/parameters/v1/reference/http/parameters-listspecsv1-GET
    /// </remarks>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports accountId, ids, limit, offset)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="SpecsGetResponse_results"/> items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (var spec in client.SpecsManager.ListSpecsAsync())
    /// {
    ///     Console.WriteLine($"{spec.Id}: {spec.Name}");
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<SpecsGetResponse_results> ListSpecsAsync(
        RequestConfiguration<SpecsRequestBuilder.SpecsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        int offset = requestConfiguration?.QueryParameters?.Offset ?? 0;

        while (true)
        {
            var response = await _api.Parameters.V1.Specs
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
    /// Creates new specs.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /parameters/v1/specs:batch-create
    /// APS docs: https://aps.autodesk.com/en/docs/parameters/v1/reference/http/parameters-createspecs-POST
    /// </remarks>
    /// <param name="body">The spec creation payload</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="SpecsBatchCreatePostResponse"/> containing the created specs</returns>
    /// <example>
    /// <code>
    /// SpecsBatchCreatePostResponse? created = await client.SpecsManager.BatchCreateSpecsAsync(body);
    /// </code>
    /// </example>
    public async Task<SpecsBatchCreatePostResponse?> BatchCreateSpecsAsync(
        SpecsBatchCreatePostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Parameters.V1.SpecsBatchCreate
            .PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Updates a list of specs (max 10 per request) with the latest version of the enumeration used in the spec.
    /// </summary>
    /// <remarks>
    /// Wraps: PUT /parameters/v1/specs:batch-update
    /// APS docs: https://aps.autodesk.com/en/docs/parameters/v1/reference/http/parameters-updatespecs-PUT
    /// </remarks>
    /// <param name="body">The spec batch update payload</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports accountId)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="SpecsBatchUpdatePutResponse"/> containing the updated specs</returns>
    /// <example>
    /// <code>
    /// SpecsBatchUpdatePutResponse? updated = await client.SpecsManager.BatchUpdateSpecsAsync(body, config =>
    /// {
    ///     config.QueryParameters.AccountId = accountId;
    /// });
    /// </code>
    /// </example>
    public async Task<SpecsBatchUpdatePutResponse?> BatchUpdateSpecsAsync(
        SpecsBatchUpdatePutRequestBody body,
        RequestConfiguration<SpecsBatchUpdateRequestBuilder.SpecsBatchUpdateRequestBuilderPutQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Parameters.V1.SpecsBatchUpdate
            .PutAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Updates a spec with the latest version of the enumeration used in the spec.
    /// </summary>
    /// <remarks>
    /// Wraps: PUT /parameters/v1/specs/{specId}
    /// APS docs: https://aps.autodesk.com/en/docs/parameters/v1/reference/http/parameters-updatespec-PUT
    /// </remarks>
    /// <param name="specId">The spec ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports accountId)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="WithSpecPutResponse"/> containing the updated spec</returns>
    /// <example>
    /// <code>
    /// WithSpecPutResponse? updated = await client.SpecsManager.UpdateSpecAsync("my-spec-id", config =>
    /// {
    ///     config.QueryParameters.AccountId = accountId;
    /// });
    /// </code>
    /// </example>
    public async Task<WithSpecPutResponse?> UpdateSpecAsync(
        string specId,
        RequestConfiguration<WithSpecItemRequestBuilder.WithSpecItemRequestBuilderPutQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Parameters.V1.Specs[specId]
            .PutAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }
}

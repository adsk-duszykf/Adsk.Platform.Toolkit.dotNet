using Autodesk.ACC.DataConnector.DataConnector.V1.Accounts.Item.Requests;
using Autodesk.ACC.DataConnector.DataConnector.V1.Accounts.Item.Requests.Item;
using Microsoft.Kiota.Abstractions;
using static Autodesk.ACC.DataConnector.DataConnector.V1.Accounts.Item.Requests.RequestsRequestBuilder;

namespace Autodesk.ACC.DataConnector.Managers;

/// <summary>
/// Manager for Data Connector Requests operations
/// </summary>
public class RequestsManager
{
    private readonly ApiRequestBuilder _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestsManager"/> class.
    /// </summary>
    /// <param name="api">The API request builder</param>
    public RequestsManager(ApiRequestBuilder api)
    {
        _api = api;
    }

    /// <summary>
    /// Creates a data request for an authenticated user. The user can optionally limit the request to one project.
    /// The user must have executive overview or project administrator permissions.
    /// </summary>
    /// <param name="accountId">The account ID (derived from hub ID by removing 'b.' prefix)</param>
    /// <param name="requestData">The request creation data</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created request information</returns>
    public async Task<RequestsPostResponse?> CreateRequestAsync(
        Guid accountId,
        RequestsPostRequestBody requestData,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _api.DataConnector!.V1.Accounts[accountId]
            .Requests
            .PostAsRequestsPostResponseAsync(requestData, requestConfiguration, cancellationToken);

        return result;
    }

    /// <summary>
    /// Returns an array of data requests that the authenticated user has created in the specified account.
    /// The user must have executive overview or project administrator permissions.
    /// </summary>
    /// <param name="accountId">The account ID (derived from hub ID by removing 'b.' prefix)</param>
    /// <param name="requestConfiguration">Optional configuration for the request including filters, sort, pagination</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Array of data requests</returns>
    public async Task<RequestsGetResponse?> ListRequestsAsync(
        Guid accountId,
        Action<RequestConfiguration<RequestsRequestBuilderGetQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _api.DataConnector!.V1.Accounts[accountId]
            .Requests
            .GetAsRequestsGetResponseAsync(requestConfiguration, cancellationToken);

        return result;
    }

    /// <summary>
    /// Returns information about a specified data request created earlier by the authenticated user.
    /// The user must have executive overview or project administrator permissions.
    /// </summary>
    /// <param name="accountId">The account ID (derived from hub ID by removing 'b.' prefix)</param>
    /// <param name="requestId">The ID of the specified request</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The request information</returns>
    public async Task<WithRequestGetResponse?> GetRequestAsync(
        Guid accountId,
        Guid requestId,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _api.DataConnector!.V1.Accounts[accountId]
            .Requests[requestId]
            .GetAsWithRequestGetResponseAsync(requestConfiguration, cancellationToken);

        return result;
    }

    /// <summary>
    /// Updates the attributes of an existing data request created earlier by the authenticated user.
    /// The user must have executive overview or project administrator permissions.
    /// </summary>
    /// <param name="accountId">The account ID (derived from hub ID by removing 'b.' prefix)</param>
    /// <param name="requestId">The ID of the specified request</param>
    /// <param name="updateData">The request update data</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated request information</returns>
    public async Task<WithRequestPatchResponse?> UpdateRequestAsync(
        Guid accountId,
        Guid requestId,
        WithRequestPatchRequestBody updateData,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _api.DataConnector!.V1.Accounts[accountId]
            .Requests[requestId]
            .PatchAsWithRequestPatchResponseAsync(updateData, requestConfiguration, cancellationToken);

        return result;
    }

    /// <summary>
    /// Deletes a specified data request created by the authenticated user.
    /// The user must have executive overview or project administrator permissions.
    /// </summary>
    /// <param name="accountId">The account ID (derived from hub ID by removing 'b.' prefix)</param>
    /// <param name="requestId">The ID of the specified request</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public async Task DeleteRequestAsync(
        Guid accountId,
        Guid requestId,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        await _api.DataConnector!.V1.Accounts[accountId]
            .Requests[requestId]
            .DeleteAsync(requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Returns an array of data connector jobs associated with a request that was created by the authenticated user.
    /// The user must have project administrator or executive overview permissions.
    /// </summary>
    /// <param name="accountId">The account ID (derived from hub ID by removing 'b.' prefix)</param>
    /// <param name="requestId">The ID of the specified request</param>
    /// <param name="requestConfiguration">Optional configuration for the request including pagination</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Array of jobs associated with the specified request</returns>
    public async Task<DataConnector.V1.Accounts.Item.Requests.Item.Jobs.JobsGetResponse?> GetRequestJobsAsync(
        Guid accountId,
        Guid requestId,
        Action<RequestConfiguration<DataConnector.V1.Accounts.Item.Requests.Item.Jobs.JobsRequestBuilder.JobsRequestBuilderGetQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _api.DataConnector!.V1.Accounts[accountId]
            .Requests[requestId]
            .Jobs
            .GetAsJobsGetResponseAsync(requestConfiguration, cancellationToken);

        return result;
    }
}

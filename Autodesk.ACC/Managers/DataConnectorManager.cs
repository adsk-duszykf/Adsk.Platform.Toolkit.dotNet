using Autodesk.ACC.DataConnector.V1.Accounts.Item.Jobs.Item;
using Autodesk.ACC.DataConnector.V1.Accounts.Item.Jobs.Item.Data.Item;
using Autodesk.ACC.DataConnector.V1.Accounts.Item.Jobs.Item.DataListing;
using Autodesk.ACC.DataConnector.V1.Accounts.Item.Requests;
using Autodesk.ACC.DataConnector.V1.Accounts.Item.Requests.Item;
using Microsoft.Kiota.Abstractions;
using RequestJobsRequestBuilder = Autodesk.ACC.DataConnector.V1.Accounts.Item.Requests.Item.Jobs.JobsRequestBuilder;
using static Autodesk.ACC.DataConnector.V1.Accounts.Item.Requests.RequestsRequestBuilder;
using static Autodesk.ACC.DataConnector.V1.Accounts.Item.Jobs.JobsRequestBuilder;

namespace Autodesk.ACC.Managers;

/// <summary>
/// Manager for Data Connector operations — manages data extraction requests, jobs,
/// and data downloads for ACC accounts.
/// </summary>
public class DataConnectorManager
{
    private readonly BaseACCclient _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="DataConnectorManager"/> class.
    /// </summary>
    /// <param name="api">The API client</param>
    public DataConnectorManager(BaseACCclient api)
    {
        _api = api;
    }

    /// <summary>
    /// Creates a data request for an authenticated user.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /data-connector/v1/accounts/{accountId}/requests
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/data-connector-requests-POST
    /// </remarks>
    /// <param name="accountId">The account ID</param>
    /// <param name="body">The data request creation data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="RequestsPostResponse"/> containing the created data request</returns>
    /// <example>
    /// <code>
    /// RequestsPostResponse? request = await client.DataConnectorManager.CreateRequestAsync("accountId", new RequestsPostRequestBody());
    /// </code>
    /// </example>
    public async Task<RequestsPostResponse?> CreateRequestAsync(
        Guid accountId,
        RequestsPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.DataConnector.V1.Accounts[accountId]
            .Requests
            .PostAsync(body, r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Returns an array of data requests that the authenticated user has created in the specified account.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /data-connector/v1/accounts/{accountId}/requests
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/data-connector-requests-GET
    /// </remarks>
    /// <param name="accountId">The account ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports filtering, sorting, limit, offset)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="RequestsGetResponse"/> containing the data requests</returns>
    /// <example>
    /// <code>
    /// RequestsGetResponse? requests = await client.DataConnectorManager.GetRequestsAsync("accountId");
    /// </code>
    /// </example>
    public async Task<RequestsGetResponse?> GetRequestsAsync(
        Guid accountId,
        RequestConfiguration<RequestsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.DataConnector.V1.Accounts[accountId]
            .Requests
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Returns information about a specified data request.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /data-connector/v1/accounts/{accountId}/requests/{requestId}
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/data-connector-requests-requestId-GET
    /// </remarks>
    /// <param name="accountId">The account ID</param>
    /// <param name="requestId">The request ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="WithRequestGetResponse"/> containing the data request details</returns>
    /// <example>
    /// <code>
    /// WithRequestGetResponse? request = await client.DataConnectorManager.GetRequestAsync("accountId", "requestId");
    /// </code>
    /// </example>
    public async Task<WithRequestGetResponse?> GetRequestAsync(
        Guid accountId,
        Guid requestId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.DataConnector.V1.Accounts[accountId]
            .Requests[requestId]
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Updates the attributes of an existing data request.
    /// </summary>
    /// <remarks>
    /// Wraps: PATCH /data-connector/v1/accounts/{accountId}/requests/{requestId}
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/data-connector-requests-requestId-PATCH
    /// </remarks>
    /// <param name="accountId">The account ID</param>
    /// <param name="requestId">The request ID</param>
    /// <param name="body">The update data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="WithRequestPatchResponse"/> containing the updated data request</returns>
    /// <example>
    /// <code>
    /// WithRequestPatchResponse? updated = await client.DataConnectorManager.UpdateRequestAsync("accountId", "requestId", new WithRequestPatchRequestBody());
    /// </code>
    /// </example>
    public async Task<WithRequestPatchResponse?> UpdateRequestAsync(
        Guid accountId,
        Guid requestId,
        WithRequestPatchRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.DataConnector.V1.Accounts[accountId]
            .Requests[requestId]
            .PatchAsync(body, r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Deletes the specified data request.
    /// </summary>
    /// <remarks>
    /// Wraps: DELETE /data-connector/v1/accounts/{accountId}/requests/{requestId}
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/data-connector-requests-requestId-DELETE
    /// </remarks>
    /// <param name="accountId">The account ID</param>
    /// <param name="requestId">The request ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    /// <example>
    /// <code>
    /// await client.DataConnectorManager.DeleteRequestAsync("accountId", "requestId");
    /// </code>
    /// </example>
    public async Task DeleteRequestAsync(
        Guid accountId,
        Guid requestId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        await _api.DataConnector.V1.Accounts[accountId]
            .Requests[requestId]
            .DeleteAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Returns an array of data connector jobs associated with a request.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /data-connector/v1/accounts/{accountId}/requests/{requestId}/jobs
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/data-connector-requests-requestId-jobs-GET
    /// </remarks>
    /// <param name="accountId">The account ID</param>
    /// <param name="requestId">The request ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports limit, offset, sort)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.ACC.DataConnector.V1.Accounts.Item.Requests.Item.Jobs.JobsGetResponse"/> containing the jobs for the request</returns>
    /// <example>
    /// <code>
    /// Autodesk.ACC.DataConnector.V1.Accounts.Item.Requests.Item.Jobs.JobsGetResponse? jobs = await client.DataConnectorManager.GetRequestJobsAsync("accountId", "requestId");
    /// </code>
    /// </example>
    public async Task<Autodesk.ACC.DataConnector.V1.Accounts.Item.Requests.Item.Jobs.JobsGetResponse?> GetRequestJobsAsync(
        Guid accountId,
        Guid requestId,
        RequestConfiguration<RequestJobsRequestBuilder.JobsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.DataConnector.V1.Accounts[accountId]
            .Requests[requestId]
            .Jobs
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Returns an array of Data Connector jobs spawned by requests from the authenticated user.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /data-connector/v1/accounts/{accountId}/jobs
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/data-connector-jobs-GET
    /// </remarks>
    /// <param name="accountId">The account ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports filtering, sorting, limit, offset, projectId)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.ACC.DataConnector.V1.Accounts.Item.Jobs.JobsGetResponse"/> containing the jobs</returns>
    /// <example>
    /// <code>
    /// Autodesk.ACC.DataConnector.V1.Accounts.Item.Jobs.JobsGetResponse? jobs = await client.DataConnectorManager.GetJobsAsync("accountId");
    /// </code>
    /// </example>
    public async Task<Autodesk.ACC.DataConnector.V1.Accounts.Item.Jobs.JobsGetResponse?> GetJobsAsync(
        Guid accountId,
        RequestConfiguration<JobsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.DataConnector.V1.Accounts[accountId]
            .Jobs
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Returns information about a specified job.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /data-connector/v1/accounts/{accountId}/jobs/{jobId}
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/data-connector-jobs-jobId-GET
    /// </remarks>
    /// <param name="accountId">The account ID</param>
    /// <param name="jobId">The job ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="WithJobGetResponse"/> containing the job details</returns>
    /// <example>
    /// <code>
    /// WithJobGetResponse? job = await client.DataConnectorManager.GetJobAsync("accountId", "jobId");
    /// </code>
    /// </example>
    public async Task<WithJobGetResponse?> GetJobAsync(
        Guid accountId,
        Guid jobId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.DataConnector.V1.Accounts[accountId]
            .Jobs[jobId]
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Cancels the specified running job.
    /// </summary>
    /// <remarks>
    /// Wraps: DELETE /data-connector/v1/accounts/{accountId}/jobs/{jobId}
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/data-connector-jobs-jobId-DELETE
    /// </remarks>
    /// <param name="accountId">The account ID</param>
    /// <param name="jobId">The job ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    /// <example>
    /// <code>
    /// await client.DataConnectorManager.CancelJobAsync("accountId", "jobId");
    /// </code>
    /// </example>
    public async Task CancelJobAsync(
        Guid accountId,
        Guid jobId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        await _api.DataConnector.V1.Accounts[accountId]
            .Jobs[jobId]
            .DeleteAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Returns information about the files contained within the data extract created by a specified job.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /data-connector/v1/accounts/{accountId}/jobs/{jobId}/data-listing
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/data-connector-jobs-jobId-data-listing-GET
    /// </remarks>
    /// <param name="accountId">The account ID</param>
    /// <param name="jobId">The job ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="DataListingGetResponse"/> containing the data listing for the job</returns>
    /// <example>
    /// <code>
    /// DataListingGetResponse? listing = await client.DataConnectorManager.GetDataListingAsync("accountId", "jobId");
    /// </code>
    /// </example>
    public async Task<DataListingGetResponse?> GetDataListingAsync(
        Guid accountId,
        Guid jobId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.DataConnector.V1.Accounts[accountId]
            .Jobs[jobId]
            .DataListing
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Returns a signed URL to retrieve a single specified file from a job's data extract.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /data-connector/v1/accounts/{accountId}/jobs/{jobId}/data/{name}
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/data-connector-jobs-jobId-data-name-GET
    /// </remarks>
    /// <param name="accountId">The account ID</param>
    /// <param name="jobId">The job ID</param>
    /// <param name="name">The name of the data file to download</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="WithNameGetResponse"/> containing the signed URL response</returns>
    /// <example>
    /// <code>
    /// WithNameGetResponse? download = await client.DataConnectorManager.GetDataDownloadUrlAsync("accountId", "jobId", "filename.csv");
    /// </code>
    /// </example>
    public async Task<WithNameGetResponse?> GetDataDownloadUrlAsync(
        Guid accountId,
        Guid jobId,
        Guid name,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.DataConnector.V1.Accounts[accountId]
            .Jobs[jobId]
            .Data[name]
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }
}

using Autodesk.BIM360.DataConnector.V1.Accounts.Item.Jobs.Item;
using Autodesk.BIM360.DataConnector.V1.Accounts.Item.Jobs.Item.Data.Item;
using Autodesk.BIM360.DataConnector.V1.Accounts.Item.Jobs.Item.DataListing;
using Autodesk.BIM360.DataConnector.V1.Accounts.Item.Requests;
using Autodesk.BIM360.DataConnector.V1.Accounts.Item.Requests.Item;
using Microsoft.Kiota.Abstractions;
using RequestJobsRequestBuilder = Autodesk.BIM360.DataConnector.V1.Accounts.Item.Requests.Item.Jobs.JobsRequestBuilder;
using static Autodesk.BIM360.DataConnector.V1.Accounts.Item.Requests.RequestsRequestBuilder;
using static Autodesk.BIM360.DataConnector.V1.Accounts.Item.Jobs.JobsRequestBuilder;

namespace Autodesk.BIM360.Managers;

/// <summary>
/// Manager for Data Connector operations — manages data extraction requests, jobs,
/// and data downloads for BIM 360 accounts.
/// </summary>
public class DataConnectorManager
{
    private readonly BaseBIM360client _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="DataConnectorManager"/> class.
    /// </summary>
    /// <param name="api">The API client</param>
    public DataConnectorManager(BaseBIM360client api)
    {
        _api = api;
    }

    /// <summary>
    /// Creates a data request for an authenticated user.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /data-connector/v1/accounts/{accountId}/requests
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/data-connector-requests-POST
    /// </remarks>
    /// <param name="accountId">The account ID</param>
    /// <param name="body">The data request creation data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="RequestsPostResponse"/> containing the created data request</returns>
    /// <example>
    /// <code>
    /// Guid accountId = Guid.Parse("a4be0c34-a4ab-4ab7-9a4b-1234567890ab");
    /// RequestsPostResponse? request = await client.DataConnectorManager.CreateRequestAsync(accountId, new RequestsPostRequestBody());
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
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/data-connector-requests-GET
    /// </remarks>
    /// <param name="accountId">The account ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports filtering, sorting, limit, offset)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="RequestsGetResponse"/> containing the data requests</returns>
    /// <example>
    /// <code>
    /// Guid accountId = Guid.Parse("a4be0c34-a4ab-4ab7-9a4b-1234567890ab");
    /// RequestsGetResponse? requests = await client.DataConnectorManager.GetRequestsAsync(accountId);
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
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/data-connector-requests-requestId-GET
    /// </remarks>
    /// <param name="accountId">The account ID</param>
    /// <param name="requestId">The request ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="WithRequestGetResponse"/> containing the data request details</returns>
    /// <example>
    /// <code>
    /// Guid accountId = Guid.Parse("a4be0c34-a4ab-4ab7-9a4b-1234567890ab");
    /// Guid requestId = Guid.Parse("b5cf1d45-b5bc-5bc8-0b5c-2345678901bc");
    /// WithRequestGetResponse? request = await client.DataConnectorManager.GetRequestAsync(accountId, requestId);
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
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/data-connector-requests-requestId-PATCH
    /// </remarks>
    /// <param name="accountId">The account ID</param>
    /// <param name="requestId">The request ID</param>
    /// <param name="body">The update data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="WithRequestPatchResponse"/> containing the updated data request</returns>
    /// <example>
    /// <code>
    /// Guid accountId = Guid.Parse("a4be0c34-a4ab-4ab7-9a4b-1234567890ab");
    /// Guid requestId = Guid.Parse("b5cf1d45-b5bc-5bc8-0b5c-2345678901bc");
    /// WithRequestPatchResponse? updated = await client.DataConnectorManager.UpdateRequestAsync(accountId, requestId, new WithRequestPatchRequestBody());
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
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/data-connector-requests-requestId-DELETE
    /// </remarks>
    /// <param name="accountId">The account ID</param>
    /// <param name="requestId">The request ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    /// <example>
    /// <code>
    /// Guid accountId = Guid.Parse("a4be0c34-a4ab-4ab7-9a4b-1234567890ab");
    /// Guid requestId = Guid.Parse("b5cf1d45-b5bc-5bc8-0b5c-2345678901bc");
    /// await client.DataConnectorManager.DeleteRequestAsync(accountId, requestId);
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
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/data-connector-requests-requestId-jobs-GET
    /// </remarks>
    /// <param name="accountId">The account ID</param>
    /// <param name="requestId">The request ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports limit, offset, sort)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.BIM360.DataConnector.V1.Accounts.Item.Requests.Item.Jobs.JobsGetResponse"/> containing the jobs for the request</returns>
    /// <example>
    /// <code>
    /// Guid accountId = Guid.Parse("a4be0c34-a4ab-4ab7-9a4b-1234567890ab");
    /// Guid requestId = Guid.Parse("b5cf1d45-b5bc-5bc8-0b5c-2345678901bc");
    /// Autodesk.BIM360.DataConnector.V1.Accounts.Item.Requests.Item.Jobs.JobsGetResponse? jobs = await client.DataConnectorManager.GetRequestJobsAsync(accountId, requestId);
    /// </code>
    /// </example>
    public async Task<Autodesk.BIM360.DataConnector.V1.Accounts.Item.Requests.Item.Jobs.JobsGetResponse?> GetRequestJobsAsync(
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
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/data-connector-jobs-GET
    /// </remarks>
    /// <param name="accountId">The account ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports filtering, sorting, limit, offset, projectId)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.BIM360.DataConnector.V1.Accounts.Item.Jobs.JobsGetResponse"/> containing the jobs</returns>
    /// <example>
    /// <code>
    /// Guid accountId = Guid.Parse("a4be0c34-a4ab-4ab7-9a4b-1234567890ab");
    /// Autodesk.BIM360.DataConnector.V1.Accounts.Item.Jobs.JobsGetResponse? jobs = await client.DataConnectorManager.GetJobsAsync(accountId);
    /// </code>
    /// </example>
    public async Task<Autodesk.BIM360.DataConnector.V1.Accounts.Item.Jobs.JobsGetResponse?> GetJobsAsync(
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
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/data-connector-jobs-jobId-GET
    /// </remarks>
    /// <param name="accountId">The account ID</param>
    /// <param name="jobId">The job ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="WithJobGetResponse"/> containing the job details</returns>
    /// <example>
    /// <code>
    /// Guid accountId = Guid.Parse("a4be0c34-a4ab-4ab7-9a4b-1234567890ab");
    /// Guid jobId = Guid.Parse("c6d02e56-c6cd-6cd9-1c6d-3456789012cd");
    /// WithJobGetResponse? job = await client.DataConnectorManager.GetJobAsync(accountId, jobId);
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
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/data-connector-jobs-jobId-DELETE
    /// </remarks>
    /// <param name="accountId">The account ID</param>
    /// <param name="jobId">The job ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    /// <example>
    /// <code>
    /// Guid accountId = Guid.Parse("a4be0c34-a4ab-4ab7-9a4b-1234567890ab");
    /// Guid jobId = Guid.Parse("c6d02e56-c6cd-6cd9-1c6d-3456789012cd");
    /// await client.DataConnectorManager.CancelJobAsync(accountId, jobId);
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
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/data-connector-jobs-jobId-data-listing-GET
    /// </remarks>
    /// <param name="accountId">The account ID</param>
    /// <param name="jobId">The job ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="DataListingGetResponse"/> containing the data listing for the job</returns>
    /// <example>
    /// <code>
    /// Guid accountId = Guid.Parse("a4be0c34-a4ab-4ab7-9a4b-1234567890ab");
    /// Guid jobId = Guid.Parse("c6d02e56-c6cd-6cd9-1c6d-3456789012cd");
    /// DataListingGetResponse? listing = await client.DataConnectorManager.GetDataListingAsync(accountId, jobId);
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
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/data-connector-jobs-jobId-data-name-GET
    /// </remarks>
    /// <param name="accountId">The account ID</param>
    /// <param name="jobId">The job ID</param>
    /// <param name="dataName">The data object identifier for the file to download (path segment <c>name</c> in the API)</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="WithNameGetResponse"/> containing the signed URL response</returns>
    /// <example>
    /// <code>
    /// Guid accountId = Guid.Parse("a4be0c34-a4ab-4ab7-9a4b-1234567890ab");
    /// Guid jobId = Guid.Parse("c6d02e56-c6cd-6cd9-1c6d-3456789012cd");
    /// Guid dataName = Guid.Parse("d7e03f67-d7de-7de0-2d7e-4567890123de");
    /// WithNameGetResponse? download = await client.DataConnectorManager.GetDataDownloadUrlAsync(accountId, jobId, dataName);
    /// </code>
    /// </example>
    public async Task<WithNameGetResponse?> GetDataDownloadUrlAsync(
        Guid accountId,
        Guid jobId,
        Guid dataName,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.DataConnector.V1.Accounts[accountId]
            .Jobs[jobId]
            .Data[dataName]
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }
}

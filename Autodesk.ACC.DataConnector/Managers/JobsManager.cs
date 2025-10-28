using Autodesk.ACC.DataConnector.DataConnector.V1.Accounts.Item.Jobs;
using Autodesk.ACC.DataConnector.DataConnector.V1.Accounts.Item.Jobs.Item;
using Microsoft.Kiota.Abstractions;
using static Autodesk.ACC.DataConnector.DataConnector.V1.Accounts.Item.Jobs.JobsRequestBuilder;

namespace Autodesk.ACC.DataConnector.Managers;

/// <summary>
/// Manager for Data Connector Jobs operations
/// </summary>
public class JobsManager
{
    private readonly ApiRequestBuilder _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="JobsManager"/> class.
    /// </summary>
    /// <param name="api">The API request builder</param>
    public JobsManager(ApiRequestBuilder api)
    {
        _api = api;
    }

    /// <summary>
    /// Returns an array of Data Connector jobs spawned by requests from the authenticated user.
    /// The array can contain all jobs associated with a specified project, or all jobs associated with all projects in the user's account.
    /// The user must have project administrator or executive overview permissions.
    /// </summary>
    /// <param name="accountId">The account ID (derived from hub ID by removing 'b.' prefix)</param>
    /// <param name="requestConfiguration">Optional configuration for the request including filters, sort, pagination</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Array of jobs</returns>
    public async Task<JobsGetResponse?> ListJobsAsync(
        Guid accountId,
        Action<RequestConfiguration<JobsRequestBuilderGetQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _api.DataConnector!.V1.Accounts[accountId]
            .Jobs
            .GetAsJobsGetResponseAsync(requestConfiguration, cancellationToken);

        return result;
    }

    /// <summary>
    /// Returns information about a specified job that was spawned by a data request created by the authenticated user.
    /// The user must have project administrator or executive overview permissions.
    /// </summary>
    /// <param name="accountId">The account ID (derived from hub ID by removing 'b.' prefix)</param>
    /// <param name="jobId">The job ID</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The job information</returns>
    public async Task<WithJobGetResponse?> GetJobAsync(
        Guid accountId,
        Guid jobId,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _api.DataConnector!.V1.Accounts[accountId]
            .Jobs[jobId]
            .GetAsWithJobGetResponseAsync(requestConfiguration, cancellationToken);

        return result;
    }

    /// <summary>
    /// Cancels the specified running job spawned by a data request created by the authenticated user.
    /// The user must have project administrator or executive overview permissions.
    /// </summary>
    /// <param name="accountId">The account ID (derived from hub ID by removing 'b.' prefix)</param>
    /// <param name="jobId">The job ID</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public async Task CancelJobAsync(
        Guid accountId,
        Guid jobId,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        await _api.DataConnector!.V1.Accounts[accountId]
            .Jobs[jobId]
            .DeleteAsync(requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Returns an array of information about the files contained within the data extract created by a specified job.
    /// The job must be spawned by a data request that was created by the authenticated user.
    /// The user must have executive overview or project administrator permissions.
    /// </summary>
    /// <param name="accountId">The account ID (derived from hub ID by removing 'b.' prefix)</param>
    /// <param name="jobId">The job ID</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Array of information about data extract files</returns>
    public async Task<DataConnector.V1.Accounts.Item.Jobs.Item.DataListing.DataListingGetResponse?> GetJobDataListingAsync(
        Guid accountId,
        Guid jobId,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _api.DataConnector!.V1.Accounts[accountId]
            .Jobs[jobId]
            .DataListing
            .GetAsDataListingGetResponseAsync(requestConfiguration, cancellationToken);

        return result;
    }
}

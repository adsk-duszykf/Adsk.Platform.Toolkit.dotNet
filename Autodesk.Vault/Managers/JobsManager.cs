using Autodesk.Vault.Models;
using Microsoft.Kiota.Abstractions;

namespace Autodesk.Vault.Managers;

/// <summary>
/// Manager for Jobs operations
/// </summary>
public class JobsManager
{
    private readonly BaseVaultClient _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="JobsManager"/> class.
    /// </summary>
    /// <param name="api">The API client</param>
    public JobsManager(BaseVaultClient api)
    {
        _api = api;
    }

    /// <summary>
    /// Create a new job in the vault
    /// </summary>
    /// <param name="vaultId">Vault ID</param>
    /// <param name="jobData">Job creation data</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created job information</returns>
    public async Task<Job?> CreateJobAsync(
        string vaultId,
        Job jobData,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _api.Vaults[vaultId].Jobs
            .PostAsync(jobData, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);

        return result;
    }

    /// <summary>
    /// Get job queue enabled status
    /// </summary>
    /// <param name="vaultId">Vault ID</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Job queue status</returns>
    public async Task<bool?> GetJobQueueEnabledStatusAsync(
        string vaultId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _api.Vaults[vaultId].Jobs.JobQueueEnabled
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);

        return result;
    }

    /// <summary>
    /// Get a job by ID
    /// </summary>
    /// <param name="vaultId">Vault ID</param>
    /// <param name="jobId">Job ID</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Job information</returns>
    public async Task<Job?> GetJobByIdAsync(
        string vaultId,
        string jobId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _api.Vaults[vaultId].Jobs[jobId]
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);

        return result;
    }
}

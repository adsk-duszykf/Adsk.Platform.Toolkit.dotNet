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
    /// Create a new job in the vault.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /vaults/{vaultId}/jobs
    /// </remarks>
    /// <param name="vaultId">The unique identifier of a vault</param>
    /// <param name="jobData">The job creation data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Job"/> containing the created job information</returns>
    /// <example>
    /// <code>
    /// Job? job = await client.Jobs.CreateJobAsync("1", new Job { });
    /// </code>
    /// </example>
    public async Task<Job?> CreateJobAsync(
        string vaultId,
        Job jobData,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Vaults[vaultId].Jobs
            .PostAsync(jobData, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Get job queue enabled status for a vault.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /vaults/{vaultId}/jobs/job-queue-enabled
    /// </remarks>
    /// <param name="vaultId">The unique identifier of a vault</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="bool"/> indicating whether the job queue is enabled</returns>
    /// <example>
    /// <code>
    /// bool? enabled = await client.Jobs.GetJobQueueEnabledStatusAsync("1");
    /// </code>
    /// </example>
    public async Task<bool?> GetJobQueueEnabledStatusAsync(
        string vaultId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Vaults[vaultId].Jobs.JobQueueEnabled
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Get a job by its ID.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /vaults/{vaultId}/jobs/{id}
    /// </remarks>
    /// <param name="vaultId">The unique identifier of a vault</param>
    /// <param name="jobId">The unique identifier of a job</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Job"/> containing the job information</returns>
    /// <example>
    /// <code>
    /// Job? job = await client.Jobs.GetJobByIdAsync("1", "42");
    /// </code>
    /// </example>
    public async Task<Job?> GetJobByIdAsync(
        string vaultId,
        string jobId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Vaults[vaultId].Jobs[jobId]
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }
}

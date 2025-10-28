using Autodesk.ACC.DataConnector.DataConnector.V1.Accounts.Item.Jobs.Item.Data.Item;
using Microsoft.Kiota.Abstractions;

namespace Autodesk.ACC.DataConnector.Managers;

/// <summary>
/// Manager for Data Connector Data extraction operations
/// </summary>
public class DataManager
{
    private readonly ApiRequestBuilder _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="DataManager"/> class.
    /// </summary>
    /// <param name="api">The API request builder</param>
    public DataManager(ApiRequestBuilder api)
    {
        _api = api;
    }

    /// <summary>
    /// Retrieves a data extract file from a completed job.
    /// Each data extract contains three types of files:
    /// - Schema files (.json) describing the data structure
    /// - Data files (.csv) containing the actual data
    /// - Metadata files describing the extraction
    /// This operation returns a signed URL that can be used to download the file.
    /// </summary>
    /// <param name="accountId">The account ID (derived from hub ID by removing 'b.' prefix)</param>
    /// <param name="jobId">The job ID</param>
    /// <param name="name">Name of the file to retrieve</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Response containing a signed URL to download the file</returns>
    public async Task<WithNameGetResponse?> GetJobDataFileAsync(
        Guid accountId,
        Guid jobId,
        Guid name,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _api.DataConnector!.V1.Accounts[accountId]
            .Jobs[jobId]
            .Data[name]
            .GetAsWithNameGetResponseAsync(requestConfiguration, cancellationToken);

        return result;
    }
}

using Autodesk.DataManagement.Models;
using Microsoft.Kiota.Abstractions;

namespace Autodesk.DataManagement.Managers;

/// <summary>
/// Manager for Data Management Command operations.
/// Commands enable general operations on multiple resources such as checking permissions
/// or listing items across folders.
/// </summary>
public class CommandsManager
{
    private readonly BaseDataManagementClient _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandsManager"/> class.
    /// </summary>
    /// <param name="api">The Data Management API client</param>
    public CommandsManager(BaseDataManagementClient api)
    {
        _api = api;
    }

    /// <summary>
    /// Executes a command on a project. Commands enable general operations on multiple resources,
    /// such as checking permissions to delete versions, items, and folders, or listing items/refs in bulk.
    /// </summary>
    /// <remarks>API: POST /data/v1/projects/{project_id}/commands</remarks>
    /// <param name="projectId">The unique identifier of a project</param>
    /// <param name="body">The command to execute</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The command response</returns>
    /// <example>
    /// <code>
    /// var result = await client.Commands.ExecuteCommandAsync("b.my-project-id",
    ///     new CreateCommand { /* ... */ });
    /// </code>
    /// </example>
    public async Task<Command?> ExecuteCommandAsync(
        string projectId,
        CreateCommand body,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Data.V1.Projects[projectId].Commands
            .PostAsync(body, requestConfiguration, cancellationToken);
    }
}

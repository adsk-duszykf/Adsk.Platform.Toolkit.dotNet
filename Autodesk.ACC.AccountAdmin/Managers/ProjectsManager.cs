using Autodesk.ACC.AccountAdmin.Construction.Admin.V1.Accounts.Item.Projects;
using Autodesk.ACC.AccountAdmin.Construction.Admin.V1.Projects.Item;
using Microsoft.Kiota.Abstractions;
using static Autodesk.ACC.AccountAdmin.Construction.Admin.V1.Accounts.Item.Projects.ProjectsRequestBuilder;
using static Autodesk.ACC.AccountAdmin.Construction.Admin.V1.Projects.Item.WithProjectItemRequestBuilder;

namespace Autodesk.ACC.AccountAdmin.Managers;

/// <summary>
/// Manager for Projects operations
/// </summary>
public class ProjectsManager
{
    private readonly ApiRequestBuilder _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectsManager"/> class.
    /// </summary>
    /// <param name="api">The API request builder</param>
    public ProjectsManager(ApiRequestBuilder api)
    {
        _api = api;
    }

    /// <summary>
    /// Retrieves a list of all projects within a specified ACC account.
    /// Note: For pagination, call the endpoint directly using the Api property.
    /// </summary>
    /// <param name="accountId">The ID of the ACC account (hub ID without 'b.' prefix)</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Projects response including results and pagination information</returns>
    public async Task<ProjectsGetResponse?> ListProjectsAsync(
        string accountId,
        Action<RequestConfiguration<ProjectsRequestBuilderGetQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _api.Construction!.Admin.V1.Accounts[accountId]
            .Projects
            .GetAsProjectsGetResponseAsync(requestConfiguration, cancellationToken);

        return result;
    }

    /// <summary>
    /// Creates a new project in the specified account.
    /// </summary>
    /// <param name="accountId">The ID of the ACC account (hub ID without 'b.' prefix)</param>
    /// <param name="projectData">The project creation data</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created project information</returns>
    public async Task<ProjectsPostResponse?> CreateProjectAsync(
        string accountId,
        ProjectsPostRequestBody projectData,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _api.Construction!.Admin.V1.Accounts[accountId]
            .Projects
            .PostAsProjectsPostResponseAsync(projectData, requestConfiguration, cancellationToken);

        return result;
    }

    /// <summary>
    /// Retrieves detailed information about a specific project.
    /// </summary>
    /// <param name="projectId">The ID of the project (without 'b.' prefix)</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The project information</returns>
    public async Task<WithProjectGetResponse?> GetProjectAsync(
        string projectId,
        Action<RequestConfiguration<WithProjectItemRequestBuilderGetQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _api.Construction!.Admin.V1.Projects[projectId]
            .GetAsWithProjectGetResponseAsync(requestConfiguration, cancellationToken);

        return result;
    }
}


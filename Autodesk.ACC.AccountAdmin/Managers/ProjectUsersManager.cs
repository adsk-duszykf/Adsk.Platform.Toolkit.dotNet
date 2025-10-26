using Autodesk.ACC.AccountAdmin.ACC.Construction.Admin.V1.Projects.Item.Users;
using Autodesk.ACC.AccountAdmin.ACC.Construction.Admin.V1.Projects.Item.Users.Item;
using Autodesk.ACC.AccountAdmin.ACC.Construction.Admin.V2.Projects.Item.UsersImport;
using Microsoft.Kiota.Abstractions;
using static Autodesk.ACC.AccountAdmin.ACC.Construction.Admin.V1.Projects.Item.Users.Item.WithUserItemRequestBuilder;
using static Autodesk.ACC.AccountAdmin.ACC.Construction.Admin.V1.Projects.Item.Users.UsersRequestBuilder;

namespace Autodesk.ACC.AccountAdmin.Managers;

/// <summary>
/// Manager for Project Users operations
/// </summary>
public class ProjectUsersManager
{
    private readonly ApiRequestBuilder _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectUsersManager"/> class.
    /// </summary>
    /// <param name="api">The API request builder</param>
    public ProjectUsersManager(ApiRequestBuilder api)
    {
        _api = api;
    }

    /// <summary>
    /// Retrieves a filtered list of users in a specified project.
    /// Note: For pagination, call the endpoint directly using the Api property.
    /// </summary>
    /// <param name="projectId">The ID of the project (without 'b.' prefix)</param>
    /// <param name="requestConfiguration">Optional configuration for the request including query parameters and headers</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Project users response including results and pagination information</returns>
    public async Task<UsersGetResponse?> ListProjectUsersAsync(
        string projectId,
        Action<RequestConfiguration<UsersRequestBuilderGetQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _api.Construction!.Admin.V1.Projects[projectId]
            .Users
            .GetAsUsersGetResponseAsync(requestConfiguration, cancellationToken);

        return result;
    }

    /// <summary>
    /// Assigns a user to the specified project.
    /// </summary>
    /// <param name="projectId">The ID of the project (without 'b.' prefix)</param>
    /// <param name="userData">The user assignment data</param>
    /// <param name="requestConfiguration">Optional configuration for the request including headers</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created project user information</returns>
    public async Task<UsersPostResponse?> AddProjectUserAsync(
        string projectId,
        UsersPostRequestBody userData,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _api.Construction!.Admin.V1.Projects[projectId]
            .Users
            .PostAsUsersPostResponseAsync(userData, requestConfiguration, cancellationToken);

        return result;
    }

    /// <summary>
    /// Retrieves detailed information about a specific user in a project.
    /// </summary>
    /// <param name="projectId">The ID of the project (without 'b.' prefix)</param>
    /// <param name="userId">The ACC ID or Autodesk ID of the user</param>
    /// <param name="requestConfiguration">Optional configuration for the request including query parameters and headers</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The project user information</returns>
    public async Task<WithUserGetResponse?> GetProjectUserAsync(
        string projectId,
        string userId,
        Action<RequestConfiguration<WithUserItemRequestBuilderGetQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _api.Construction!.Admin.V1.Projects[projectId]
            .Users[userId]
            .GetAsWithUserGetResponseAsync(requestConfiguration, cancellationToken);

        return result;
    }

    /// <summary>
    /// Updates information about a specified user in a project.
    /// </summary>
    /// <param name="projectId">The ID of the project (without 'b.' prefix)</param>
    /// <param name="userId">The ACC ID or Autodesk ID of the user</param>
    /// <param name="userData">The user update data (partial update)</param>
    /// <param name="requestConfiguration">Optional configuration for the request including headers</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated project user information</returns>
    public async Task<WithUserPatchResponse?> UpdateProjectUserAsync(
        string projectId,
        string userId,
        WithUserPatchRequestBody userData,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _api.Construction!.Admin.V1.Projects[projectId]
            .Users[userId]
            .PatchAsWithUserPatchResponseAsync(userData, requestConfiguration, cancellationToken);

        return result;
    }

    /// <summary>
    /// Removes a specified user from a project.
    /// </summary>
    /// <param name="projectId">The ID of the project (without 'b.' prefix)</param>
    /// <param name="userId">The ACC ID or Autodesk ID of the user</param>
    /// <param name="requestConfiguration">Optional configuration for the request including headers</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task that completes when the operation is done</returns>
    public async Task RemoveProjectUserAsync(
        string projectId,
        string userId,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        await _api.Construction!.Admin.V1.Projects[projectId]
            .Users[userId]
            .DeleteAsync(requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Imports multiple users to a project at once (up to 200 users per request).
    /// </summary>
    /// <param name="projectId">The ID of the project (without 'b.' prefix)</param>
    /// <param name="usersData">Array of users to import</param>
    /// <param name="requestConfiguration">Optional configuration for the request including headers</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The import job information</returns>
    public async Task<UsersImportPostResponse?> ImportProjectUsersAsync(
        string projectId,
        UsersImportPostRequestBody usersData,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _api.Construction!.Admin.V2.Projects[projectId]
            .UsersImport
            .PostAsUsersImportPostResponseAsync(usersData, requestConfiguration, cancellationToken);

        return result;
    }
}


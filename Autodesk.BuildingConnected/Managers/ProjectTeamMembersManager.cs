using System.Runtime.CompilerServices;
using Autodesk.BuildingConnected;
using Autodesk.BuildingConnected.Construction.Buildingconnected.V2.ProjectTeamMembers;
using Autodesk.BuildingConnected.Construction.Buildingconnected.V2.ProjectTeamMembers.Item;
using Microsoft.Kiota.Abstractions;
using static Autodesk.BuildingConnected.Construction.Buildingconnected.V2.ProjectTeamMembers.ProjectTeamMembersRequestBuilder;

namespace Autodesk.BuildingConnected.Managers;

/// <summary>
/// Manager for BuildingConnected project team member operations — list, get, create, update, and delete.
/// </summary>
public class ProjectTeamMembersManager
{
    private readonly BaseBuildingConnectedClient _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectTeamMembersManager"/> class.
    /// </summary>
    /// <param name="api">The BuildingConnected API client.</param>
    public ProjectTeamMembersManager(BaseBuildingConnectedClient api)
    {
        _api = api;
    }

    /// <summary>
    /// Lists project team members with automatic cursor-based pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/buildingconnected/v2/project-team-members
    /// APS docs: https://aps.autodesk.com/en/docs/buildingconnected/v2/reference/http/buildingconnected-project-team-members-GET
    /// </remarks>
    /// <param name="requestConfiguration">(Optional) Configuration for the request, including cursor, limit, and filter query parameters.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="ProjectTeamMembersGetResponse_results"/>.</returns>
    /// <example>
    /// <code>
    /// await foreach (var member in client.ProjectTeamMembersManager.ListProjectTeamMembersAsync())
    /// {
    ///     Console.WriteLine(member);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<ProjectTeamMembersGetResponse_results> ListProjectTeamMembersAsync(
        RequestConfiguration<ProjectTeamMembersRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? cursor = requestConfiguration?.QueryParameters?.CursorState;

        while (true)
        {
            ProjectTeamMembersGetResponse? response = await _api.Construction.Buildingconnected.V2.ProjectTeamMembers
                .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                    r.QueryParameters.CursorState = cursor;
                }, cancellationToken);

            if (response?.Results is not { Count: > 0 })
                yield break;

            foreach (ProjectTeamMembersGetResponse_results item in response.Results)
            {
                yield return item;
            }

            if (string.IsNullOrEmpty(response.Pagination?.CursorState))
                yield break;

            cursor = response.Pagination.CursorState;
        }
    }

    /// <summary>
    /// Gets a project team member by id.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/buildingconnected/v2/project-team-members/{memberId}
    /// APS docs: https://aps.autodesk.com/en/docs/buildingconnected/v2/reference/http/buildingconnected-project-team-members-memberId-GET
    /// </remarks>
    /// <param name="memberId">The project team member id.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>A <see cref="WithMemberGetResponse"/> instance, or <c>null</c> if the response is empty.</returns>
    /// <example>
    /// <code>
    /// WithMemberGetResponse? member = await client.ProjectTeamMembersManager.GetProjectTeamMemberAsync("member-id-here");
    /// </code>
    /// </example>
    public async Task<WithMemberGetResponse?> GetProjectTeamMemberAsync(
        string memberId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Buildingconnected.V2.ProjectTeamMembers[memberId]
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Creates a project team member.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /construction/buildingconnected/v2/project-team-members
    /// APS docs: https://aps.autodesk.com/en/docs/buildingconnected/v2/reference/http/buildingconnected-project-team-members-POST
    /// </remarks>
    /// <param name="body">The request body.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>A <see cref="ProjectTeamMembersPostResponse"/> instance, or <c>null</c> if the response is empty.</returns>
    /// <example>
    /// <code>
    /// ProjectTeamMembersPostResponse? created = await client.ProjectTeamMembersManager.CreateProjectTeamMemberAsync(new ProjectTeamMembersPostRequestBody());
    /// </code>
    /// </example>
    public async Task<ProjectTeamMembersPostResponse?> CreateProjectTeamMemberAsync(
        ProjectTeamMembersPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Buildingconnected.V2.ProjectTeamMembers
            .PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Updates a project team member.
    /// </summary>
    /// <remarks>
    /// Wraps: PATCH /construction/buildingconnected/v2/project-team-members/{memberId}
    /// APS docs: https://aps.autodesk.com/en/docs/buildingconnected/v2/reference/http/buildingconnected-project-team-members-memberId-PATCH
    /// </remarks>
    /// <param name="memberId">The project team member id.</param>
    /// <param name="body">The patch body.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>A <see cref="WithMemberPatchResponse"/> instance, or <c>null</c> if the response is empty.</returns>
    /// <example>
    /// <code>
    /// WithMemberPatchResponse? updated = await client.ProjectTeamMembersManager.UpdateProjectTeamMemberAsync("member-id-here", new WithMemberPatchRequestBody());
    /// </code>
    /// </example>
    public async Task<WithMemberPatchResponse?> UpdateProjectTeamMemberAsync(
        string memberId,
        WithMemberPatchRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Buildingconnected.V2.ProjectTeamMembers[memberId]
            .PatchAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Deletes a project team member from their current project.
    /// </summary>
    /// <remarks>
    /// Wraps: DELETE /construction/buildingconnected/v2/project-team-members/{memberId}
    /// APS docs: https://aps.autodesk.com/en/docs/buildingconnected/v2/reference/http/buildingconnected-project-team-members-memberId-DELETE
    /// </remarks>
    /// <param name="memberId">The project team member id.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>A <see cref="Task"/> that completes when the delete request finishes.</returns>
    /// <example>
    /// <code>
    /// await client.ProjectTeamMembersManager.DeleteProjectTeamMemberAsync("member-id-here");
    /// </code>
    /// </example>
    public async Task DeleteProjectTeamMemberAsync(
        string memberId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        await _api.Construction.Buildingconnected.V2.ProjectTeamMembers[memberId]
            .DeleteAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }
}

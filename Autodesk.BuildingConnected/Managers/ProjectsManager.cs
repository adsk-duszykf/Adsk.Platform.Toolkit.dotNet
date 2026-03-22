using System.Runtime.CompilerServices;
using Autodesk.BuildingConnected.Construction.Buildingconnected.V2.Projects;
using Autodesk.BuildingConnected.Construction.Buildingconnected.V2.Projects.Item;
using Autodesk.BuildingConnected.Construction.Buildingconnected.V2.Projects.Item.Costs;
using Autodesk.BuildingConnected.Construction.Buildingconnected.V2.Projects.Item.Costs.Item;
using Autodesk.BuildingConnected.Construction.Buildingconnected.V2.Projects.Item.CostsBatchCreate;
using Autodesk.BuildingConnected.Construction.Buildingconnected.V2.Projects.Item.CostsBatchDelete;
using Autodesk.BuildingConnected.Construction.Buildingconnected.V2.Projects.Item.CostsBatchPatch;
using Microsoft.Kiota.Abstractions;
using static Autodesk.BuildingConnected.Construction.Buildingconnected.V2.Projects.ProjectsRequestBuilder;
using static Autodesk.BuildingConnected.Construction.Buildingconnected.V2.Projects.Item.Costs.CostsRequestBuilder;

namespace Autodesk.BuildingConnected.Managers;

/// <summary>
/// Manager for BuildingConnected Pro project and project cost operations.
/// </summary>
public class ProjectsManager
{
    private readonly BaseBuildingConnectedClient _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectsManager"/> class.
    /// </summary>
    /// <param name="api">The BuildingConnected API client.</param>
    public ProjectsManager(BaseBuildingConnectedClient api)
    {
        _api = api;
    }

    /// <summary>
    /// Lists BuildingConnected Pro projects for the current user&apos;s company with cursor-based pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/buildingconnected/v2/projects
    /// APS docs: https://aps.autodesk.com/en/docs/buildingconnected/v2/reference/http/buildingconnected-projects-GET
    /// </remarks>
    /// <param name="requestConfiguration">(Optional) Query parameters (including <see cref="ProjectsRequestBuilderGetQueryParameters.CursorState"/> for the first page), headers, and middleware options.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="ProjectsGetResponse_results"/> across all pages.</returns>
    /// <example>
    /// <code>
    /// await foreach (ProjectsGetResponse_results project in client.ProjectsManager.ListProjectsAsync())
    /// {
    ///     Console.WriteLine(project.Id);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<ProjectsGetResponse_results> ListProjectsAsync(
        RequestConfiguration<ProjectsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? cursor = requestConfiguration?.QueryParameters?.CursorState;

        while (true)
        {
            ProjectsGetResponse? response = await _api.Construction.Buildingconnected.V2.Projects
                .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                    r.QueryParameters.CursorState = cursor;
                }, cancellationToken);

            if (response?.Results is not { Count: > 0 })
                yield break;

            foreach (ProjectsGetResponse_results item in response.Results)
            {
                yield return item;
            }

            if (string.IsNullOrEmpty(response.Pagination?.CursorState))
                yield break;

            cursor = response.Pagination.CursorState;
        }
    }

    /// <summary>
    /// Retrieves a single BuildingConnected Pro project by identifier.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/buildingconnected/v2/projects/{projectId}
    /// APS docs: https://aps.autodesk.com/en/docs/buildingconnected/v2/reference/http/buildingconnected-projects-projectId-GET
    /// </remarks>
    /// <param name="projectId">The BuildingConnected project identifier.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for headers and middleware options (default query parameters).</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>A <see cref="WithProjectGetResponse"/> for the project, or <c>null</c> if not found.</returns>
    /// <example>
    /// <code>
    /// WithProjectGetResponse? project = await client.ProjectsManager.GetProjectAsync("project-id");
    /// </code>
    /// </example>
    public async Task<WithProjectGetResponse?> GetProjectAsync(
        string projectId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Buildingconnected.V2.Projects[projectId]
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Creates a new BuildingConnected Pro project in draft state.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /construction/buildingconnected/v2/projects
    /// APS docs: https://aps.autodesk.com/en/docs/buildingconnected/v2/reference/http/buildingconnected-projects-POST
    /// </remarks>
    /// <param name="body">The project creation payload.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>A <see cref="ProjectsPostResponse"/> describing the created project.</returns>
    /// <example>
    /// <code>
    /// ProjectsPostResponse? created = await client.ProjectsManager.CreateProjectAsync(new ProjectsPostRequestBody());
    /// </code>
    /// </example>
    public async Task<ProjectsPostResponse?> CreateProjectAsync(
        ProjectsPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Buildingconnected.V2.Projects
            .PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Updates fields on an existing BuildingConnected Pro project.
    /// </summary>
    /// <remarks>
    /// Wraps: PATCH /construction/buildingconnected/v2/projects/{projectId}
    /// APS docs: https://aps.autodesk.com/en/docs/buildingconnected/v2/reference/http/buildingconnected-projects-projectId-PATCH
    /// </remarks>
    /// <param name="projectId">The BuildingConnected project identifier.</param>
    /// <param name="body">The patch payload.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>A <see cref="WithProjectPatchResponse"/> with updated project data.</returns>
    /// <example>
    /// <code>
    /// WithProjectPatchResponse? updated = await client.ProjectsManager.UpdateProjectAsync(
    ///     "project-id",
    ///     new WithProjectPatchRequestBody());
    /// </code>
    /// </example>
    public async Task<WithProjectPatchResponse?> UpdateProjectAsync(
        string projectId,
        WithProjectPatchRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Buildingconnected.V2.Projects[projectId]
            .PatchAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Deletes a BuildingConnected Pro project that is in draft state.
    /// </summary>
    /// <remarks>
    /// Wraps: DELETE /construction/buildingconnected/v2/projects/{projectId}
    /// APS docs: https://aps.autodesk.com/en/docs/buildingconnected/v2/reference/http/buildingconnected-projects-projectId-DELETE
    /// </remarks>
    /// <param name="projectId">The BuildingConnected project identifier.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>A <see cref="Task"/> that completes when the project is deleted.</returns>
    /// <example>
    /// <code>
    /// await client.ProjectsManager.DeleteProjectAsync("project-id");
    /// </code>
    /// </example>
    public async Task DeleteProjectAsync(
        string projectId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        await _api.Construction.Buildingconnected.V2.Projects[projectId]
            .DeleteAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Lists indirect costs for a project with cursor-based pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/buildingconnected/v2/projects/{projectId}/costs
    /// APS docs: https://aps.autodesk.com/en/docs/buildingconnected/v2/reference/http/buildingconnected-projects-projectId-costs-GET
    /// </remarks>
    /// <param name="projectId">The BuildingConnected project identifier.</param>
    /// <param name="requestConfiguration">(Optional) Query parameters (including <see cref="CostsRequestBuilderGetQueryParameters.CursorState"/> for the first page), headers, and middleware options.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="CostsGetResponse_results"/> across all pages.</returns>
    /// <example>
    /// <code>
    /// await foreach (CostsGetResponse_results cost in client.ProjectsManager.ListCostsAsync("project-id"))
    /// {
    ///     Console.WriteLine(cost.Id);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<CostsGetResponse_results> ListCostsAsync(
        string projectId,
        RequestConfiguration<CostsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? cursor = requestConfiguration?.QueryParameters?.CursorState;

        while (true)
        {
            CostsGetResponse? response = await _api.Construction.Buildingconnected.V2.Projects[projectId].Costs
                .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                    r.QueryParameters.CursorState = cursor;
                }, cancellationToken);

            if (response?.Results is not { Count: > 0 })
                yield break;

            foreach (CostsGetResponse_results item in response.Results)
            {
                yield return item;
            }

            if (string.IsNullOrEmpty(response.Pagination?.CursorState))
                yield break;

            cursor = response.Pagination.CursorState;
        }
    }

    /// <summary>
    /// Creates a single indirect cost on the specified project.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /construction/buildingconnected/v2/projects/{projectId}/costs
    /// APS docs: https://aps.autodesk.com/en/docs/buildingconnected/v2/reference/http/buildingconnected-projects-projectId-costs-POST
    /// </remarks>
    /// <param name="projectId">The BuildingConnected project identifier.</param>
    /// <param name="body">The cost creation payload.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>A <see cref="CostsPostResponse"/> describing the created cost.</returns>
    /// <example>
    /// <code>
    /// CostsPostResponse? cost = await client.ProjectsManager.CreateCostAsync("project-id", new CostsPostRequestBody());
    /// </code>
    /// </example>
    public async Task<CostsPostResponse?> CreateCostAsync(
        string projectId,
        CostsPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Buildingconnected.V2.Projects[projectId].Costs
            .PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Creates multiple indirect cost items on the project (batch, max 200 per call).
    /// </summary>
    /// <remarks>
    /// Wraps: POST /construction/buildingconnected/v2/projects/{projectId}/costs:batch-create
    /// APS docs: https://aps.autodesk.com/en/docs/buildingconnected/v2/reference/http/buildingconnected-projects-projectId-costs-batch-create-POST
    /// </remarks>
    /// <param name="projectId">The BuildingConnected project identifier.</param>
    /// <param name="body">The batch-create payload.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>A <see cref="CostsBatchCreatePostResponse"/> with batch results.</returns>
    /// <example>
    /// <code>
    /// CostsBatchCreatePostResponse? batch = await client.ProjectsManager.BatchCreateCostsAsync(
    ///     "project-id",
    ///     new CostsBatchCreatePostRequestBody());
    /// </code>
    /// </example>
    public async Task<CostsBatchCreatePostResponse?> BatchCreateCostsAsync(
        string projectId,
        CostsBatchCreatePostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Buildingconnected.V2.Projects[projectId].CostsBatchCreate
            .PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Updates multiple indirect cost items on the project (batch patch, max 200 per call).
    /// </summary>
    /// <remarks>
    /// Wraps: PATCH /construction/buildingconnected/v2/projects/{projectId}/costs:batch-patch
    /// APS docs: https://aps.autodesk.com/en/docs/buildingconnected/v2/reference/http/buildingconnected-projects-projectId-costs-batch-patch-PATCH
    /// </remarks>
    /// <param name="projectId">The BuildingConnected project identifier.</param>
    /// <param name="body">The batch-patch payload.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>A <see cref="CostsBatchPatchPatchResponse"/> with batch results.</returns>
    /// <example>
    /// <code>
    /// CostsBatchPatchPatchResponse? batch = await client.ProjectsManager.BatchPatchCostsAsync(
    ///     "project-id",
    ///     new CostsBatchPatchPatchRequestBody());
    /// </code>
    /// </example>
    public async Task<CostsBatchPatchPatchResponse?> BatchPatchCostsAsync(
        string projectId,
        CostsBatchPatchPatchRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Buildingconnected.V2.Projects[projectId].CostsBatchPatch
            .PatchAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Deletes multiple indirect cost items on the project (batch delete).
    /// </summary>
    /// <remarks>
    /// Wraps: POST /construction/buildingconnected/v2/projects/{projectId}/costs:batch-delete
    /// APS docs: https://aps.autodesk.com/en/docs/buildingconnected/v2/reference/http/buildingconnected-projects-projectId-costs-batch-delete-POST
    /// </remarks>
    /// <param name="projectId">The BuildingConnected project identifier.</param>
    /// <param name="costIds">The list of cost identifiers to delete.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>A <see cref="Task"/> that completes when the API returns no content.</returns>
    /// <example>
    /// <code>
    /// List&lt;string&gt; ids = ["cost-1", "cost-2"];
    /// await client.ProjectsManager.BatchDeleteCostsAsync("project-id", ids);
    /// </code>
    /// </example>
    public async Task BatchDeleteCostsAsync(
        string projectId,
        List<string> costIds,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        await _api.Construction.Buildingconnected.V2.Projects[projectId].CostsBatchDelete
            .PostAsync(costIds, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Updates a single indirect cost item on the project.
    /// </summary>
    /// <remarks>
    /// Wraps: PATCH /construction/buildingconnected/v2/projects/{projectId}/costs/{costId}
    /// APS docs: https://aps.autodesk.com/en/docs/buildingconnected/v2/reference/http/buildingconnected-projects-projectId-costs-costId-PATCH
    /// </remarks>
    /// <param name="projectId">The BuildingConnected project identifier.</param>
    /// <param name="costId">The indirect cost identifier.</param>
    /// <param name="body">The patch payload.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>A <see cref="WithCostPatchResponse"/> with updated cost data.</returns>
    /// <example>
    /// <code>
    /// WithCostPatchResponse? updated = await client.ProjectsManager.UpdateCostAsync(
    ///     "project-id",
    ///     "cost-id",
    ///     new WithCostPatchRequestBody());
    /// </code>
    /// </example>
    public async Task<WithCostPatchResponse?> UpdateCostAsync(
        string projectId,
        string costId,
        WithCostPatchRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Buildingconnected.V2.Projects[projectId].Costs[costId]
            .PatchAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }
}

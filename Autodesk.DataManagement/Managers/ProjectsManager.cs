using System.Runtime.CompilerServices;
using Autodesk.DataManagement.Models;
using Autodesk.DataManagement.Project.V1.Hubs.Item.Projects;
using Autodesk.DataManagement.Project.V1.Hubs.Item.Projects.Item.TopFolders;
using Microsoft.Kiota.Abstractions;

namespace Autodesk.DataManagement.Managers;

/// <summary>
/// Manager for Project operations
/// </summary>
public class ProjectsManager
{
    private readonly BaseDataManagementClient _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectsManager"/> class.
    /// </summary>
    /// <param name="api">The Data Management API client</param>
    public ProjectsManager(BaseDataManagementClient api)
    {
        _api = api;
    }

    #region List & Get Projects

    /// <summary>
    /// Lists all projects for a given hub with automatic pagination.
    /// </summary>
    /// <remarks>API: GET /project/v1/hubs/{hub_id}/projects</remarks>
    /// <param name="hubId">The unique identifier of a hub</param>
    /// <param name="requestConfiguration">Optional configuration for the request (filter by id, extension type, page limit)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>An async enumerable of project data items across all pages</returns>
    /// <example>
    /// <code>
    /// await foreach (var project in client.Projects.ListProjectsAsync("b.my-account-id"))
    /// {
    ///     Console.WriteLine($"{project.Id} - {project.Attributes?.Name}");
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<Projects_data> ListProjectsAsync(
        string hubId,
        Action<RequestConfiguration<ProjectsRequestBuilder.ProjectsRequestBuilderGetQueryParameters>>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        int pageNumber = 0;

        while (true)
        {
            var capturedPage = pageNumber;
            var response = await _api.Project.V1.Hubs[hubId].Projects
                .GetAsync(config =>
                {
                    requestConfiguration?.Invoke(config);
                    config.QueryParameters.Pagenumber = capturedPage;
                }, cancellationToken);

            if (response?.Data != null)
            {
                foreach (var item in response.Data)
                {
                    yield return item;
                }
            }

            if (response?.Links?.Next?.Href == null)
                break;

            pageNumber++;
        }
    }

    /// <summary>
    /// Returns a project for a given project ID.
    /// For BIM 360 Docs, project IDs require a "b." prefix.
    /// </summary>
    /// <remarks>API: GET /project/v1/hubs/{hub_id}/projects/{project_id}</remarks>
    /// <param name="hubId">The unique identifier of a hub</param>
    /// <param name="projectId">The unique identifier of a project</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The project details</returns>
    /// <example>
    /// <code>
    /// var project = await client.Projects.GetProjectAsync("b.my-account-id", "b.my-project-id");
    /// Console.WriteLine($"Project: {project?.Data?.Attributes?.Name}");
    /// </code>
    /// </example>
    public async Task<Models.Project?> GetProjectAsync(
        string hubId,
        string projectId,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Project.V1.Hubs[hubId].Projects[projectId]
            .GetAsync(requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Returns the hub for a given project.
    /// </summary>
    /// <remarks>API: GET /project/v1/hubs/{hub_id}/projects/{project_id}/hub</remarks>
    /// <param name="hubId">The unique identifier of a hub</param>
    /// <param name="projectId">The unique identifier of a project</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The hub associated with the project</returns>
    /// <example>
    /// <code>
    /// var hub = await client.Projects.GetProjectHubAsync("b.my-account-id", "b.my-project-id");
    /// Console.WriteLine($"Hub: {hub?.Data?.Attributes?.Name}");
    /// </code>
    /// </example>
    public async Task<Hub?> GetProjectHubAsync(
        string hubId,
        string projectId,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Project.V1.Hubs[hubId].Projects[projectId].Hub
            .GetAsync(requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Returns the top-level folders the user has access to for a given project.
    /// </summary>
    /// <remarks>API: GET /project/v1/hubs/{hub_id}/projects/{project_id}/topFolders</remarks>
    /// <param name="hubId">The unique identifier of a hub</param>
    /// <param name="projectId">The unique identifier of a project</param>
    /// <param name="requestConfiguration">Optional configuration for the request (excludeDeleted, projectFilesOnly)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The top folders collection</returns>
    /// <example>
    /// <code>
    /// var topFolders = await client.Projects.GetTopFoldersAsync("b.my-account-id", "b.my-project-id");
    /// foreach (var folder in topFolders?.Data ?? Enumerable.Empty&lt;TopFolders_data&gt;())
    /// {
    ///     Console.WriteLine($"{folder.Id} - {folder.Attributes?.Name}");
    /// }
    /// </code>
    /// </example>
    public async Task<TopFolders?> GetTopFoldersAsync(
        string hubId,
        string projectId,
        Action<RequestConfiguration<TopFoldersRequestBuilder.TopFoldersRequestBuilderGetQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Project.V1.Hubs[hubId].Projects[projectId].TopFolders
            .GetAsync(requestConfiguration, cancellationToken);
    }

    #endregion

    #region Storage, Downloads & Jobs

    /// <summary>
    /// Creates a storage location in the OSS where data can be uploaded to.
    /// </summary>
    /// <remarks>API: POST /data/v1/projects/{project_id}/storage</remarks>
    /// <param name="projectId">The unique identifier of a project</param>
    /// <param name="body">The storage creation request</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created storage location</returns>
    /// <example>
    /// <code>
    /// var storage = await client.Projects.CreateStorageAsync("b.my-project-id",
    ///     new StorageRequest { /* ... */ });
    /// Console.WriteLine($"Storage ID: {storage?.Data?.Id}");
    /// </code>
    /// </example>
    public async Task<Storage?> CreateStorageAsync(
        string projectId,
        StorageRequest body,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Data.V1.Projects[projectId].Storage
            .PostAsync(body, requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Requests creation of a new download for a specific and supported file type.
    /// </summary>
    /// <remarks>API: POST /data/v1/projects/{project_id}/downloads</remarks>
    /// <param name="projectId">The unique identifier of a project</param>
    /// <param name="body">The download creation request</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created download job</returns>
    /// <example>
    /// <code>
    /// var download = await client.Projects.CreateDownloadAsync("b.my-project-id",
    ///     new CreateDownload { /* ... */ });
    /// Console.WriteLine($"Download ID: {download?.Data?.Id}");
    /// </code>
    /// </example>
    public async Task<CreatedDownload?> CreateDownloadAsync(
        string projectId,
        CreateDownload body,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Data.V1.Projects[projectId].Downloads
            .PostAsync(body, requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Returns the details for a specific download.
    /// </summary>
    /// <remarks>API: GET /data/v1/projects/{project_id}/downloads/{download_id}</remarks>
    /// <param name="projectId">The unique identifier of a project</param>
    /// <param name="downloadId">The unique identifier of a download</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The download details</returns>
    /// <example>
    /// <code>
    /// var download = await client.Projects.GetDownloadAsync("b.my-project-id", "download-id");
    /// Console.WriteLine($"Status: {download?.Data?.Attributes?.Status}");
    /// </code>
    /// </example>
    public async Task<Download?> GetDownloadAsync(
        string projectId,
        string downloadId,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Data.V1.Projects[projectId].Downloads[downloadId]
            .GetAsync(requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Returns the details for a specific job.
    /// </summary>
    /// <remarks>API: GET /data/v1/projects/{project_id}/jobs/{job_id}</remarks>
    /// <param name="projectId">The unique identifier of a project</param>
    /// <param name="jobId">The unique identifier of a job</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The job details</returns>
    /// <example>
    /// <code>
    /// var job = await client.Projects.GetJobAsync("b.my-project-id", "job-id");
    /// Console.WriteLine($"Job status: {job?.Data?.Attributes?.Status}");
    /// </code>
    /// </example>
    public async Task<Job?> GetJobAsync(
        string projectId,
        string jobId,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Data.V1.Projects[projectId].Jobs[jobId]
            .GetAsync(requestConfiguration, cancellationToken);
    }

    #endregion
}

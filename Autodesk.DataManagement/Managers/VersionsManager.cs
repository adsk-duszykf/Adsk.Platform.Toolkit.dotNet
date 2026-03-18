using Autodesk.DataManagement.Data.V1.Projects.Item.Versions;
using Autodesk.DataManagement.Data.V1.Projects.Item.Versions.Item.Downloads;
using Autodesk.DataManagement.Data.V1.Projects.Item.Versions.Item.Refs;
using Autodesk.DataManagement.Models;
using Microsoft.Kiota.Abstractions;
using VersionRelationshipRefsBuilder = Autodesk.DataManagement.Data.V1.Projects.Item.Versions.Item.Relationships.Refs.RefsRequestBuilder;

namespace Autodesk.DataManagement.Managers;

/// <summary>
/// Manager for Version operations
/// </summary>
public class VersionsManager
{
    private readonly BaseDataManagementClient _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="VersionsManager"/> class.
    /// </summary>
    /// <param name="api">The Data Management API client</param>
    public VersionsManager(BaseDataManagementClient api)
    {
        _api = api;
    }

    #region Get & Update Version

    /// <summary>
    /// Returns the version with the given version ID.
    /// </summary>
    /// <remarks>API: GET /data/v1/projects/{project_id}/versions/{version_id}</remarks>
    /// <param name="projectId">The unique identifier of a project</param>
    /// <param name="versionId">The unique identifier of a version</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The version details</returns>
    /// <example>
    /// <code>
    /// var version = await client.Versions.GetVersionAsync("b.my-project-id", "urn:adsk.wipprod:fs.file:vf...?version=1");
    /// Console.WriteLine($"Version: {version?.Data?.Attributes?.Name}");
    /// </code>
    /// </example>
    public async Task<VersionObject?> GetVersionAsync(
        string projectId,
        string versionId,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Data.V1.Projects[projectId].Versions[versionId]
            .GetAsync(requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Updates the properties of the given version.
    /// </summary>
    /// <remarks>API: PATCH /data/v1/projects/{project_id}/versions/{version_id}</remarks>
    /// <param name="projectId">The unique identifier of a project</param>
    /// <param name="versionId">The unique identifier of a version</param>
    /// <param name="body">The version update payload</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated version</returns>
    /// <example>
    /// <code>
    /// var updated = await client.Versions.UpdateVersionAsync("b.my-project-id",
    ///     "urn:adsk.wipprod:fs.file:vf...?version=1",
    ///     new VersionRequest { /* ... */ });
    /// </code>
    /// </example>
    public async Task<VersionObject?> UpdateVersionAsync(
        string projectId,
        string versionId,
        VersionRequest body,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Data.V1.Projects[projectId].Versions[versionId]
            .PatchAsync(body, requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Creates a new version of a file (item), except for the first version.
    /// To create the first version, use the Items manager's CreateItemAsync method.
    /// </summary>
    /// <remarks>API: POST /data/v1/projects/{project_id}/versions</remarks>
    /// <param name="projectId">The unique identifier of a project</param>
    /// <param name="body">The version creation payload</param>
    /// <param name="requestConfiguration">Optional configuration for the request (copyFrom)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created version</returns>
    /// <example>
    /// <code>
    /// var version = await client.Versions.CreateVersionAsync("b.my-project-id",
    ///     new CreateVersion { /* ... */ });
    /// Console.WriteLine($"Created version: {version?.Data?.Id}");
    /// </code>
    /// </example>
    public async Task<CreatedVersion?> CreateVersionAsync(
        string projectId,
        CreateVersion body,
        Action<RequestConfiguration<VersionsRequestBuilder.VersionsRequestBuilderPostQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Data.V1.Projects[projectId].Versions
            .PostAsync(body, requestConfiguration, cancellationToken);
    }

    #endregion

    #region Version Item & Downloads

    /// <summary>
    /// Returns the item the given version is associated with.
    /// </summary>
    /// <remarks>API: GET /data/v1/projects/{project_id}/versions/{version_id}/item</remarks>
    /// <param name="projectId">The unique identifier of a project</param>
    /// <param name="versionId">The unique identifier of a version</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The item associated with the version</returns>
    /// <example>
    /// <code>
    /// var item = await client.Versions.GetVersionItemAsync("b.my-project-id", "urn:adsk.wipprod:fs.file:vf...?version=1");
    /// Console.WriteLine($"Item: {item?.Data?.Attributes?.DisplayName}");
    /// </code>
    /// </example>
    public async Task<Models.Item?> GetVersionItemAsync(
        string projectId,
        string versionId,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Data.V1.Projects[projectId].Versions[versionId].Item
            .GetAsync(requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Returns the available download formats for a specific version.
    /// </summary>
    /// <remarks>API: GET /data/v1/projects/{project_id}/versions/{version_id}/downloadFormats</remarks>
    /// <param name="projectId">The unique identifier of a project</param>
    /// <param name="versionId">The unique identifier of a version</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The available download formats</returns>
    /// <example>
    /// <code>
    /// var formats = await client.Versions.GetVersionDownloadFormatsAsync("b.my-project-id",
    ///     "urn:adsk.wipprod:fs.file:vf...?version=1");
    /// </code>
    /// </example>
    public async Task<DownloadFormats?> GetVersionDownloadFormatsAsync(
        string projectId,
        string versionId,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Data.V1.Projects[projectId].Versions[versionId].DownloadFormats
            .GetAsync(requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Returns a set of already available downloads for this version.
    /// </summary>
    /// <remarks>API: GET /data/v1/projects/{project_id}/versions/{version_id}/downloads</remarks>
    /// <param name="projectId">The unique identifier of a project</param>
    /// <param name="versionId">The unique identifier of a version</param>
    /// <param name="requestConfiguration">Optional configuration for the request (filter by format file type)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The available downloads</returns>
    /// <example>
    /// <code>
    /// var downloads = await client.Versions.GetVersionDownloadsAsync("b.my-project-id",
    ///     "urn:adsk.wipprod:fs.file:vf...?version=1");
    /// </code>
    /// </example>
    public async Task<Downloads?> GetVersionDownloadsAsync(
        string projectId,
        string versionId,
        Action<RequestConfiguration<DownloadsRequestBuilder.DownloadsRequestBuilderGetQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Data.V1.Projects[projectId].Versions[versionId].Downloads
            .GetAsync(requestConfiguration, cancellationToken);
    }

    #endregion

    #region Refs & Relationships

    /// <summary>
    /// Returns the resources (items, folders, and versions) that have a custom relationship with the given version.
    /// </summary>
    /// <remarks>API: GET /data/v1/projects/{project_id}/versions/{version_id}/refs</remarks>
    /// <param name="projectId">The unique identifier of a project</param>
    /// <param name="versionId">The unique identifier of a version</param>
    /// <param name="requestConfiguration">Optional configuration for the request (filter by type, id, extension type)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The version refs</returns>
    /// <example>
    /// <code>
    /// var refs = await client.Versions.GetVersionRefsAsync("b.my-project-id",
    ///     "urn:adsk.wipprod:fs.file:vf...?version=1");
    /// </code>
    /// </example>
    public async Task<Refs?> GetVersionRefsAsync(
        string projectId,
        string versionId,
        Action<RequestConfiguration<RefsRequestBuilder.RefsRequestBuilderGetQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Data.V1.Projects[projectId].Versions[versionId].Refs
            .GetAsync(requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Returns the custom relationships associated with the given version.
    /// </summary>
    /// <remarks>API: GET /data/v1/projects/{project_id}/versions/{version_id}/relationships/refs</remarks>
    /// <param name="projectId">The unique identifier of a project</param>
    /// <param name="versionId">The unique identifier of a version</param>
    /// <param name="requestConfiguration">Optional configuration for the request (filter by type, id, direction, refType, extension type)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The version relationship refs</returns>
    /// <example>
    /// <code>
    /// var relRefs = await client.Versions.GetVersionRelationshipRefsAsync("b.my-project-id",
    ///     "urn:adsk.wipprod:fs.file:vf...?version=1");
    /// </code>
    /// </example>
    public async Task<RelationshipRefs?> GetVersionRelationshipRefsAsync(
        string projectId,
        string versionId,
        Action<RequestConfiguration<VersionRelationshipRefsBuilder.RefsRequestBuilderGetQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Data.V1.Projects[projectId].Versions[versionId].Relationships.Refs
            .GetAsync(requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Creates a custom relationship between a version and another resource (folder, item, or version).
    /// </summary>
    /// <remarks>API: POST /data/v1/projects/{project_id}/versions/{version_id}/relationships/refs</remarks>
    /// <param name="projectId">The unique identifier of a project</param>
    /// <param name="versionId">The unique identifier of a version</param>
    /// <param name="body">The relationship ref to create</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <example>
    /// <code>
    /// await client.Versions.CreateVersionRelationshipRefAsync("b.my-project-id",
    ///     "urn:adsk.wipprod:fs.file:vf...?version=1",
    ///     new RelationshipRefsRequest { /* ... */ });
    /// </code>
    /// </example>
    public async Task CreateVersionRelationshipRefAsync(
        string projectId,
        string versionId,
        RelationshipRefsRequest body,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        await _api.Data.V1.Projects[projectId].Versions[versionId].Relationships.Refs
            .PostAsync(body, requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Returns a collection of links for the given version.
    /// </summary>
    /// <remarks>API: GET /projects/{project_id}/versions/{version_id}/relationships/links</remarks>
    /// <param name="projectId">The unique identifier of a project</param>
    /// <param name="versionId">The unique identifier of a version</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The version relationship links</returns>
    /// <example>
    /// <code>
    /// var links = await client.Versions.GetVersionRelationshipLinksAsync("b.my-project-id",
    ///     "urn:adsk.wipprod:fs.file:vf...?version=1");
    /// </code>
    /// </example>
    public async Task<RelationshipLinks?> GetVersionRelationshipLinksAsync(
        string projectId,
        string versionId,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Projects[projectId].Versions[versionId].Relationships.Links
            .GetAsync(requestConfiguration, cancellationToken);
    }

    #endregion
}

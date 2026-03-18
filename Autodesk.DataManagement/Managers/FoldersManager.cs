using System.Runtime.CompilerServices;
using Autodesk.DataManagement.Data.V1.Projects.Item.Folders.Item.Contents;
using Autodesk.DataManagement.Data.V1.Projects.Item.Folders.Item.Refs;
using Autodesk.DataManagement.Data.V1.Projects.Item.Folders.Item.Search;
using Autodesk.DataManagement.Models;
using Microsoft.Kiota.Abstractions;
using FolderRelationshipRefsBuilder = Autodesk.DataManagement.Data.V1.Projects.Item.Folders.Item.Relationships.Refs.RefsRequestBuilder;

namespace Autodesk.DataManagement.Managers;

/// <summary>
/// Manager for Folder operations
/// </summary>
public class FoldersManager
{
    private readonly BaseDataManagementClient _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="FoldersManager"/> class.
    /// </summary>
    /// <param name="api">The Data Management API client</param>
    public FoldersManager(BaseDataManagementClient api)
    {
        _api = api;
    }

    #region Get & Update Folder

    /// <summary>
    /// Returns the folder by ID for any folder within a given project.
    /// </summary>
    /// <remarks>API: GET /data/v1/projects/{project_id}/folders/{folder_id}</remarks>
    /// <param name="projectId">The unique identifier of a project</param>
    /// <param name="folderId">The unique identifier of a folder</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The folder details</returns>
    /// <example>
    /// <code>
    /// var folder = await client.Folders.GetFolderAsync("b.my-project-id", "urn:adsk.wipprod:fs.folder:...");
    /// Console.WriteLine($"Folder: {folder?.Data?.Attributes?.Name}");
    /// </code>
    /// </example>
    public async Task<Folder?> GetFolderAsync(
        string projectId,
        string folderId,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Data.V1.Projects[projectId].Folders[folderId]
            .GetAsync(requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Modifies folder names. Can also hide/restore BIM 360 Docs folders or move them via parent relationships.
    /// </summary>
    /// <remarks>API: PATCH /data/v1/projects/{project_id}/folders/{folder_id}</remarks>
    /// <param name="projectId">The unique identifier of a project</param>
    /// <param name="folderId">The unique identifier of a folder</param>
    /// <param name="body">The folder modification payload</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated folder</returns>
    /// <example>
    /// <code>
    /// var updated = await client.Folders.UpdateFolderAsync("b.my-project-id",
    ///     "urn:adsk.wipprod:fs.folder:...",
    ///     new ModifyFolder { /* ... */ });
    /// Console.WriteLine($"Renamed to: {updated?.Data?.Attributes?.Name}");
    /// </code>
    /// </example>
    public async Task<Folder?> UpdateFolderAsync(
        string projectId,
        string folderId,
        ModifyFolder body,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Data.V1.Projects[projectId].Folders[folderId]
            .PatchAsync(body, requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Creates a new folder in the project.
    /// </summary>
    /// <remarks>API: POST /data/v1/projects/{project_id}/folders</remarks>
    /// <param name="projectId">The unique identifier of a project</param>
    /// <param name="body">The folder creation payload</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created folder</returns>
    /// <example>
    /// <code>
    /// var folder = await client.Folders.CreateFolderAsync("b.my-project-id",
    ///     new CreateFolder { /* ... */ });
    /// Console.WriteLine($"Created: {folder?.Data?.Id}");
    /// </code>
    /// </example>
    public async Task<Folder?> CreateFolderAsync(
        string projectId,
        CreateFolder body,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Data.V1.Projects[projectId].Folders
            .PostAsync(body, requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Returns the parent folder (if it exists).
    /// </summary>
    /// <remarks>API: GET /data/v1/projects/{project_id}/folders/{folder_id}/parent</remarks>
    /// <param name="projectId">The unique identifier of a project</param>
    /// <param name="folderId">The unique identifier of a folder</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The parent folder</returns>
    /// <example>
    /// <code>
    /// var parent = await client.Folders.GetFolderParentAsync("b.my-project-id", "urn:adsk.wipprod:fs.folder:...");
    /// Console.WriteLine($"Parent: {parent?.Data?.Attributes?.Name}");
    /// </code>
    /// </example>
    public async Task<Folder?> GetFolderParentAsync(
        string projectId,
        string folderId,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Data.V1.Projects[projectId].Folders[folderId].Parent
            .GetAsync(requestConfiguration, cancellationToken);
    }

    #endregion

    #region Folder Contents (Paginated)

    /// <summary>
    /// Lists items and folders within a folder with automatic pagination.
    /// </summary>
    /// <remarks>API: GET /data/v1/projects/{project_id}/folders/{folder_id}/contents</remarks>
    /// <param name="projectId">The unique identifier of a project</param>
    /// <param name="folderId">The unique identifier of a folder</param>
    /// <param name="requestConfiguration">Optional configuration for the request (filter by type, id, extension type, page limit, includeHidden)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>An async enumerable of folder content items across all pages</returns>
    /// <example>
    /// <code>
    /// // List all items in a folder
    /// await foreach (var item in client.Folders.ListFolderContentsAsync("b.my-project-id", "urn:adsk.wipprod:fs.folder:..."))
    /// {
    ///     Console.WriteLine($"{item.Id} - {item.Type}");
    /// }
    ///
    /// // List only items (not subfolders) with page size of 50
    /// await foreach (var item in client.Folders.ListFolderContentsAsync("b.my-project-id", folderId, config =>
    /// {
    ///     config.QueryParameters.FiltertypeAsGetFilterTypeQueryParameterType =
    ///         new[] { GetFilterTypeQueryParameterType.Items };
    ///     config.QueryParameters.Pagelimit = 50;
    /// }))
    /// {
    ///     Console.WriteLine(item.Id);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<FolderContents_data> ListFolderContentsAsync(
        string projectId,
        string folderId,
        Action<RequestConfiguration<ContentsRequestBuilder.ContentsRequestBuilderGetQueryParameters>>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        int pageNumber = 0;

        while (true)
        {
            var capturedPage = pageNumber;
            var response = await _api.Data.V1.Projects[projectId].Folders[folderId].Contents
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

    #endregion

    #region Search (Paginated)

    /// <summary>
    /// Searches a folder and its subfolders recursively with automatic pagination.
    /// Returns tip versions of items matching the filter conditions.
    /// </summary>
    /// <remarks>API: GET /data/v1/projects/{project_id}/folders/{folder_id}/search</remarks>
    /// <param name="projectId">The unique identifier of a project</param>
    /// <param name="folderId">The unique identifier of a folder</param>
    /// <param name="requestConfiguration">Optional configuration for the request (page number)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>An async enumerable of search results across all pages</returns>
    /// <example>
    /// <code>
    /// await foreach (var result in client.Folders.SearchFolderAsync("b.my-project-id", "urn:adsk.wipprod:fs.folder:..."))
    /// {
    ///     Console.WriteLine($"{result.Id} - {result.Type}");
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<Search_data> SearchFolderAsync(
        string projectId,
        string folderId,
        Action<RequestConfiguration<SearchRequestBuilder.SearchRequestBuilderGetQueryParameters>>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        int pageNumber = 0;

        while (true)
        {
            var capturedPage = pageNumber;
            var response = await _api.Data.V1.Projects[projectId].Folders[folderId].Search
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

    #endregion

    #region Refs & Relationships

    /// <summary>
    /// Returns the resources (items, folders, and versions) that have a custom relationship with the given folder.
    /// </summary>
    /// <remarks>API: GET /data/v1/projects/{project_id}/folders/{folder_id}/refs</remarks>
    /// <param name="projectId">The unique identifier of a project</param>
    /// <param name="folderId">The unique identifier of a folder</param>
    /// <param name="requestConfiguration">Optional configuration for the request (filter by type, id, extension type)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The folder refs</returns>
    /// <example>
    /// <code>
    /// var refs = await client.Folders.GetFolderRefsAsync("b.my-project-id", "urn:adsk.wipprod:fs.folder:...");
    /// </code>
    /// </example>
    public async Task<FolderRefs?> GetFolderRefsAsync(
        string projectId,
        string folderId,
        Action<RequestConfiguration<RefsRequestBuilder.RefsRequestBuilderGetQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Data.V1.Projects[projectId].Folders[folderId].Refs
            .GetAsync(requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Returns the custom relationships associated with the given folder.
    /// </summary>
    /// <remarks>API: GET /data/v1/projects/{project_id}/folders/{folder_id}/relationships/refs</remarks>
    /// <param name="projectId">The unique identifier of a project</param>
    /// <param name="folderId">The unique identifier of a folder</param>
    /// <param name="requestConfiguration">Optional configuration for the request (filter by type, id, direction, refType, extension type)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The folder relationship refs</returns>
    /// <example>
    /// <code>
    /// var relRefs = await client.Folders.GetFolderRelationshipRefsAsync("b.my-project-id", "urn:adsk.wipprod:fs.folder:...");
    /// </code>
    /// </example>
    public async Task<RelationshipRefs?> GetFolderRelationshipRefsAsync(
        string projectId,
        string folderId,
        Action<RequestConfiguration<FolderRelationshipRefsBuilder.RefsRequestBuilderGetQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Data.V1.Projects[projectId].Folders[folderId].Relationships.Refs
            .GetAsync(requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Creates a custom relationship between a folder and another resource (folder, item, or version).
    /// </summary>
    /// <remarks>API: POST /data/v1/projects/{project_id}/folders/{folder_id}/relationships/refs</remarks>
    /// <param name="projectId">The unique identifier of a project</param>
    /// <param name="folderId">The unique identifier of a folder</param>
    /// <param name="body">The relationship ref to create</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <example>
    /// <code>
    /// await client.Folders.CreateFolderRelationshipRefAsync("b.my-project-id",
    ///     "urn:adsk.wipprod:fs.folder:...",
    ///     new RelationshipRefsRequest { /* ... */ });
    /// </code>
    /// </example>
    public async Task CreateFolderRelationshipRefAsync(
        string projectId,
        string folderId,
        RelationshipRefsRequest body,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        await _api.Data.V1.Projects[projectId].Folders[folderId].Relationships.Refs
            .PostAsync(body, requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Returns a collection of links for the given folder.
    /// </summary>
    /// <remarks>API: GET /data/v1/projects/{project_id}/folders/{folder_id}/relationships/links</remarks>
    /// <param name="projectId">The unique identifier of a project</param>
    /// <param name="folderId">The unique identifier of a folder</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The folder relationship links</returns>
    /// <example>
    /// <code>
    /// var links = await client.Folders.GetFolderRelationshipLinksAsync("b.my-project-id", "urn:adsk.wipprod:fs.folder:...");
    /// </code>
    /// </example>
    public async Task<RelationshipLinks?> GetFolderRelationshipLinksAsync(
        string projectId,
        string folderId,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Data.V1.Projects[projectId].Folders[folderId].Relationships.Links
            .GetAsync(requestConfiguration, cancellationToken);
    }

    #endregion
}

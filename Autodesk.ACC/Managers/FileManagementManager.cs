using System.Runtime.CompilerServices;
using Autodesk.ACC.Bim360.Docs.V1.Projects.Item.Folders.Item.CustomAttributeDefinitions;
using Autodesk.ACC.Bim360.Docs.V1.Projects.Item.Folders.Item.Permissions;
using Autodesk.ACC.Bim360.Docs.V1.Projects.Item.Folders.Item.PermissionsBatchCreate;
using Autodesk.ACC.Bim360.Docs.V1.Projects.Item.Folders.Item.PermissionsBatchDelete;
using Autodesk.ACC.Bim360.Docs.V1.Projects.Item.Folders.Item.PermissionsBatchUpdate;
using Autodesk.ACC.Bim360.Docs.V1.Projects.Item.NamingStandards.Item;
using Autodesk.ACC.Bim360.Docs.V1.Projects.Item.Versions.Item.CustomAttributesBatchUpdate;
using Autodesk.ACC.Bim360.Docs.V1.Projects.Item.VersionsBatchGet;
using Autodesk.ACC.Construction.Files.V1.Projects.Item.Exports;
using Autodesk.ACC.Construction.Files.V1.Projects.Item.Exports.Item;
using Autodesk.ACC.Construction.Packages.V1.Projects.Item.Packages;
using Autodesk.ACC.Construction.Packages.V1.Projects.Item.Packages.Item.Resources;
using Autodesk.ACC.Construction.Rcm.V1.Projects.Item.PublishedVersions.Item.LinkedFiles;
using Microsoft.Kiota.Abstractions;

namespace Autodesk.ACC.Managers;

/// <summary>
/// Manager for File Management operations — manages PDF exports, permissions, custom attributes,
/// naming standards, Revit cloud models, and file packages.
/// </summary>
public class FileManagementManager
{
    private readonly BaseACCclient _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileManagementManager"/> class.
    /// </summary>
    /// <param name="api">The API client</param>
    public FileManagementManager(BaseACCclient api)
    {
        _api = api;
    }

    /// <summary>
    /// Exports one or more individual PDFs, or 2D views and sheets (from DWG or RVT files) as PDFs from the ACC files module.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /construction/files/v1/projects/{projectId}/exports
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/v1-files-export-pdf-files-POST
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="body">The export request body</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="ExportsPostResponse"/> containing the export job details, or <see langword="null"/>.</returns>
    /// <example>
    /// <code>
    /// ExportsPostResponse? response = await client.FileManagementManager.ExportPdfsAsync(projectId, new ExportsPostRequestBody());
    /// </code>
    /// </example>
    public async Task<ExportsPostResponse?> ExportPdfsAsync(
        Guid projectId,
        ExportsPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Files.V1.Projects[projectId]
            .Exports
            .PostAsync(body, r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Retrieves the status of an export job.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/files/v1/projects/{projectId}/exports/{exportId}
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/v1-files-export-status-and-result-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="exportId">The ID of the export job</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="WithExportGetResponse"/> containing the export status including signed URL when ready, or <see langword="null"/>.</returns>
    /// <example>
    /// <code>
    /// WithExportGetResponse? status = await client.FileManagementManager.GetExportStatusAsync(projectId, "exportId");
    /// </code>
    /// </example>
    public async Task<WithExportGetResponse?> GetExportStatusAsync(
        Guid projectId,
        string exportId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Files.V1.Projects[projectId]
            .Exports[exportId]
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Retrieves the permissions for a folder.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /bim360/docs/v1/projects/{project_id}/folders/{folder_id}/permissions
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/document-management-projects-project_id-folders-folder_id-permissions-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="folderId">The URN of the folder</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="PermissionsGetResponse"/> containing the folder permissions, or <see langword="null"/>.</returns>
    /// <example>
    /// <code>
    /// PermissionsGetResponse? permissions = await client.FileManagementManager.GetFolderPermissionsAsync(projectId, folderUrn);
    /// </code>
    /// </example>
    public async Task<PermissionsGetResponse?> GetFolderPermissionsAsync(
        Guid projectId,
        string folderId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Docs.V1.Projects[projectId]
            .Folders[folderId]
            .Permissions
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Creates permissions for users, roles, or companies on a folder.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /bim360/docs/v1/projects/{project_id}/folders/{folder_id}/permissions:batch-create
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/document-management-projects-project_id-folders-folder_id-permissionsbatch-create-POST
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="folderId">The URN of the folder</param>
    /// <param name="body">The batch create request body</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="PermissionsBatchCreatePostResponse"/> containing the batch create result, or <see langword="null"/>.</returns>
    /// <example>
    /// <code>
    /// PermissionsBatchCreatePostResponse? result = await client.FileManagementManager.BatchCreatePermissionsAsync(projectId, folderId, new PermissionsBatchCreatePostRequestBody());
    /// </code>
    /// </example>
    public async Task<PermissionsBatchCreatePostResponse?> BatchCreatePermissionsAsync(
        Guid projectId,
        string folderId,
        PermissionsBatchCreatePostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Docs.V1.Projects[projectId]
            .Folders[folderId]
            .PermissionsBatchCreate
            .PostAsync(body, r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Updates permissions for users, roles, or companies on a folder.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /bim360/docs/v1/projects/{project_id}/folders/{folder_id}/permissions:batch-update
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/document-management-projects-project_id-folders-folder_id-permissionsbatch-update-POST
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="folderId">The URN of the folder</param>
    /// <param name="body">The batch update request body</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="PermissionsBatchUpdatePostResponse"/> containing the batch update result, or <see langword="null"/>.</returns>
    /// <example>
    /// <code>
    /// PermissionsBatchUpdatePostResponse? result = await client.FileManagementManager.BatchUpdatePermissionsAsync(projectId, folderId, new PermissionsBatchUpdatePostRequestBody());
    /// </code>
    /// </example>
    public async Task<PermissionsBatchUpdatePostResponse?> BatchUpdatePermissionsAsync(
        Guid projectId,
        string folderId,
        PermissionsBatchUpdatePostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Docs.V1.Projects[projectId]
            .Folders[folderId]
            .PermissionsBatchUpdate
            .PostAsync(body, r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Deletes all permissions assigned to specified users, roles, and companies.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /bim360/docs/v1/projects/{project_id}/folders/{folder_id}/permissions:batch-delete
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/document-management-projects-project_id-folders-folder_id-permissionsbatch-delete-POST
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="folderId">The URN of the folder</param>
    /// <param name="body">The batch delete request body</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Stream"/> containing the response data (typically empty on success), or <see langword="null"/>.</returns>
    /// <example>
    /// <code>
    /// await client.FileManagementManager.BatchDeletePermissionsAsync(projectId, folderId, new PermissionsBatchDeletePostRequestBody());
    /// </code>
    /// </example>
    public async Task<Stream?> BatchDeletePermissionsAsync(
        Guid projectId,
        string folderId,
        PermissionsBatchDeletePostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Docs.V1.Projects[projectId]
            .Folders[folderId]
            .PermissionsBatchDelete
            .PostAsync(body, r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Returns a paginated list of custom attribute definitions for a folder with automatic pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /bim360/docs/v1/projects/{project_id}/folders/{folder_id}/custom-attribute-definitions
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/document-management-custom-attribute-definitions-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="folderId">The URN of the folder</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports Limit, Offset)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{CustomAttributeDefinitionsGetResponse_results}"/> of <see cref="CustomAttributeDefinitionsGetResponse_results"/> items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (CustomAttributeDefinitionsGetResponse_results def in client.FileManagementManager.ListCustomAttributeDefinitionsAsync(projectId, folderId))
    /// {
    ///     Console.WriteLine(def.Id);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<CustomAttributeDefinitionsGetResponse_results> ListCustomAttributeDefinitionsAsync(
        Guid projectId,
        string folderId,
        RequestConfiguration<CustomAttributeDefinitionsRequestBuilder.CustomAttributeDefinitionsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        int offset = requestConfiguration?.QueryParameters?.Offset ?? 0;

        while (true)
        {
            var response = await _api.Bim360.Docs.V1.Projects[projectId]
                .Folders[folderId]
                .CustomAttributeDefinitions
                .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                    r.QueryParameters.Offset = offset;
                }, cancellationToken);

            if (response?.Results is not { Count: > 0 })
                yield break;

            foreach (var item in response.Results)
            {
                yield return item;
            }

            if (string.IsNullOrEmpty(response.Pagination?.NextUrl))
                yield break;

            offset += response.Results.Count;
        }
    }

    /// <summary>
    /// Adds a custom attribute definition to a folder.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /bim360/docs/v1/projects/{project_id}/folders/{folder_id}/custom-attribute-definitions
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/document-management-custom-attribute-definitions-POST
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="folderId">The URN of the folder</param>
    /// <param name="body">The custom attribute definition</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="CustomAttributeDefinitionsPostResponse"/> containing the created custom attribute definition, or <see langword="null"/>.</returns>
    /// <example>
    /// <code>
    /// CustomAttributeDefinitionsPostResponse? result = await client.FileManagementManager.CreateCustomAttributeDefinitionAsync(projectId, folderId, new CustomAttributeDefinitionsPostRequestBody());
    /// </code>
    /// </example>
    public async Task<CustomAttributeDefinitionsPostResponse?> CreateCustomAttributeDefinitionAsync(
        Guid projectId,
        string folderId,
        CustomAttributeDefinitionsPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Docs.V1.Projects[projectId]
            .Folders[folderId]
            .CustomAttributeDefinitions
            .PostAsync(body, r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Batch retrieves versions with their custom attributes.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /bim360/docs/v1/projects/{project_id}/versions:batch-get
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/document-management-versionsbatch-get-POST
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="body">The batch get request body</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="VersionsBatchGetPostResponse"/> containing versions and custom attributes, or <see langword="null"/>.</returns>
    /// <example>
    /// <code>
    /// VersionsBatchGetPostResponse? result = await client.FileManagementManager.BatchGetVersionCustomAttributesAsync(projectId, new VersionsBatchGetPostRequestBody());
    /// </code>
    /// </example>
    public async Task<VersionsBatchGetPostResponse?> BatchGetVersionCustomAttributesAsync(
        Guid projectId,
        VersionsBatchGetPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Docs.V1.Projects[projectId]
            .VersionsBatchGet
            .PostAsync(body, r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Batch updates custom attributes for a version.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /bim360/docs/v1/projects/{project_id}/versions/{version_id}/custom-attributes:batch-update
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/document-management-custom-attributesbatch-update-POST
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="versionId">The URL-encoded URN of the version</param>
    /// <param name="body">The batch update request body</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="CustomAttributesBatchUpdatePostResponse"/> containing the batch update result, or <see langword="null"/>.</returns>
    /// <example>
    /// <code>
    /// CustomAttributesBatchUpdatePostResponse? result = await client.FileManagementManager.BatchUpdateCustomAttributesAsync(projectId, versionId, new CustomAttributesBatchUpdatePostRequestBody());
    /// </code>
    /// </example>
    public async Task<CustomAttributesBatchUpdatePostResponse?> BatchUpdateCustomAttributesAsync(
        Guid projectId,
        string versionId,
        CustomAttributesBatchUpdatePostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Docs.V1.Projects[projectId]
            .Versions[versionId]
            .CustomAttributesBatchUpdate
            .PostAsync(body, r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Retrieves the file naming standard for a project.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /bim360/docs/v1/projects/{projectId}/naming-standards/{id}
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/document-management-naming-standards-id-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="id">The ID of the naming standard</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="NamingStandardsGetResponse"/> containing the naming standard, or <see langword="null"/>.</returns>
    /// <example>
    /// <code>
    /// NamingStandardsGetResponse? namingStandard = await client.FileManagementManager.GetNamingStandardAsync(projectId, namingStandardId);
    /// </code>
    /// </example>
    public async Task<NamingStandardsGetResponse?> GetNamingStandardAsync(
        Guid projectId,
        Guid id,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Docs.V1.Projects[projectId]
            .NamingStandards[id]
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Retrieves metadata and signed download URLs for a published Revit cloud model and its linked files.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/rcm/v1/projects/{projectId}/published-versions/{versionId}/linked-files
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/rcm-linked-files-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project (with or without b. prefix)</param>
    /// <param name="versionId">The ID of the published version</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="LinkedFilesGetResponse"/> containing linked files metadata and signed download URLs, or <see langword="null"/>.</returns>
    /// <example>
    /// <code>
    /// LinkedFilesGetResponse? linkedFiles = await client.FileManagementManager.GetLinkedFilesAsync("projectId", versionId);
    /// </code>
    /// </example>
    public async Task<LinkedFilesGetResponse?> GetLinkedFilesAsync(
        string projectId,
        string versionId,
        RequestConfiguration<LinkedFilesRequestBuilder.LinkedFilesRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Rcm.V1.Projects[projectId]
            .PublishedVersions[versionId]
            .LinkedFiles
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Retrieves a list of all packages within a specified ACC project.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/packages/v1/projects/{projectId}/packages
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/packages-list-packages-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="PackagesGetResponse"/> containing the packages list, or <see langword="null"/>.</returns>
    /// <example>
    /// <code>
    /// PackagesGetResponse? packages = await client.FileManagementManager.GetPackagesAsync(projectId);
    /// </code>
    /// </example>
    public async Task<PackagesGetResponse?> GetPackagesAsync(
        Guid projectId,
        RequestConfiguration<PackagesRequestBuilder.PackagesRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Packages.V1.Projects[projectId]
            .Packages
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Retrieves a list of file versions (resources) within a specified package.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/packages/v1/projects/{projectId}/packages/{packageId}/resources
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/packages-list-package-resources-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="packageId">The ID of the package</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="ResourcesGetResponse"/> containing package file versions (resources), or <see langword="null"/>.</returns>
    /// <example>
    /// <code>
    /// ResourcesGetResponse? resources = await client.FileManagementManager.GetPackageResourcesAsync(projectId, packageId);
    /// </code>
    /// </example>
    public async Task<ResourcesGetResponse?> GetPackageResourcesAsync(
        Guid projectId,
        Guid packageId,
        RequestConfiguration<ResourcesRequestBuilder.ResourcesRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Packages.V1.Projects[projectId]
            .Packages[packageId]
            .Resources
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }
}

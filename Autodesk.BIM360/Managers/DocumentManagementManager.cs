using System.Runtime.CompilerServices;
using Autodesk.BIM360.Bim360.Docs.V1.Projects.Item.Folders.Item.CustomAttributeDefinitions;
using Autodesk.BIM360.Bim360.Docs.V1.Projects.Item.Folders.Item.Permissions;
using Autodesk.BIM360.Bim360.Docs.V1.Projects.Item.Folders.Item.PermissionsBatchCreate;
using Autodesk.BIM360.Bim360.Docs.V1.Projects.Item.Folders.Item.PermissionsBatchDelete;
using Autodesk.BIM360.Bim360.Docs.V1.Projects.Item.Folders.Item.PermissionsBatchUpdate;
using Autodesk.BIM360.Bim360.Docs.V1.Projects.Item.Versions.Item.CustomAttributesBatchUpdate;
using Autodesk.BIM360.Bim360.Docs.V1.Projects.Item.Versions.Item.Exports;
using Autodesk.BIM360.Bim360.Docs.V1.Projects.Item.VersionsBatchGet;
using Microsoft.Kiota.Abstractions;

namespace Autodesk.BIM360.Managers;

/// <summary>
/// Manager for BIM 360 Document Management — folder permissions, custom attribute definitions,
/// version batch operations, naming standards, and per-version exports.
/// </summary>
public class DocumentManagementManager
{
    private readonly BaseBIM360client _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="DocumentManagementManager"/> class.
    /// </summary>
    /// <param name="api">The API client</param>
    public DocumentManagementManager(BaseBIM360client api)
    {
        _api = api;
    }

    /// <summary>
    /// Retrieves the permissions for a folder.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /bim360/docs/v1/projects/{project_id}/folders/{folder_id}/permissions
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/document-management-projects-project_id-folders-folder_id-permissions-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project (without the Data Management <c>b.</c> prefix)</param>
    /// <param name="folderId">The URL-encoded URN of the folder</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="PermissionsGetResponse"/> containing the folder permissions, or <see langword="null"/>.</returns>
    /// <example>
    /// <code>
    /// Guid projectId = Guid.Parse("a4be0c34-a4ab-4ab7-9a4b-1234567890ab");
    /// string folderUrn = "urn:adsk.wipprod:fs.folder:co.xxx";
    /// PermissionsGetResponse? permissions = await client.DocumentManagementManager.GetFolderPermissionsAsync(projectId, folderUrn);
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
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/document-management-projects-project_id-folders-folder_id-permissionsbatch-create-POST
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="folderId">The URN of the folder</param>
    /// <param name="body">The batch create request body</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="PermissionsBatchCreatePostResponse"/> containing the batch create result, or <see langword="null"/>.</returns>
    /// <example>
    /// <code>
    /// Guid projectId = Guid.Parse("a4be0c34-a4ab-4ab7-9a4b-1234567890ab");
    /// string folderId = "urn:adsk.wipprod:fs.folder:co.xxx";
    /// PermissionsBatchCreatePostResponse? result = await client.DocumentManagementManager.BatchCreatePermissionsAsync(projectId, folderId, new PermissionsBatchCreatePostRequestBody());
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
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/document-management-projects-project_id-folders-folder_id-permissionsbatch-update-POST
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="folderId">The URN of the folder</param>
    /// <param name="body">The batch update request body</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="PermissionsBatchUpdatePostResponse"/> containing the batch update result, or <see langword="null"/>.</returns>
    /// <example>
    /// <code>
    /// Guid projectId = Guid.Parse("a4be0c34-a4ab-4ab7-9a4b-1234567890ab");
    /// string folderId = "urn:adsk.wipprod:fs.folder:co.xxx";
    /// PermissionsBatchUpdatePostResponse? result = await client.DocumentManagementManager.BatchUpdatePermissionsAsync(projectId, folderId, new PermissionsBatchUpdatePostRequestBody());
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
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/document-management-projects-project_id-folders-folder_id-permissionsbatch-delete-POST
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="folderId">The URN of the folder</param>
    /// <param name="body">The batch delete request body</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Stream"/> containing the response data (typically empty on success), or <see langword="null"/>.</returns>
    /// <example>
    /// <code>
    /// Guid projectId = Guid.Parse("a4be0c34-a4ab-4ab7-9a4b-1234567890ab");
    /// string folderId = "urn:adsk.wipprod:fs.folder:co.xxx";
    /// await client.DocumentManagementManager.BatchDeletePermissionsAsync(projectId, folderId, new PermissionsBatchDeletePostRequestBody());
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
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/document-management-custom-attribute-definitions-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="folderId">The URN of the folder</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports Limit, Offset)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{CustomAttributeDefinitionsGetResponse_results}"/> of <see cref="CustomAttributeDefinitionsGetResponse_results"/> items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// Guid projectId = Guid.Parse("a4be0c34-a4ab-4ab7-9a4b-1234567890ab");
    /// string folderId = "urn:adsk.wipprod:fs.folder:co.xxx";
    /// await foreach (CustomAttributeDefinitionsGetResponse_results def in client.DocumentManagementManager.ListCustomAttributeDefinitionsAsync(projectId, folderId))
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
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/document-management-custom-attribute-definitions-POST
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="folderId">The URN of the folder</param>
    /// <param name="body">The custom attribute definition</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="CustomAttributeDefinitionsPostResponse"/> containing the created custom attribute definition, or <see langword="null"/>.</returns>
    /// <example>
    /// <code>
    /// Guid projectId = Guid.Parse("a4be0c34-a4ab-4ab7-9a4b-1234567890ab");
    /// string folderId = "urn:adsk.wipprod:fs.folder:co.xxx";
    /// CustomAttributeDefinitionsPostResponse? result = await client.DocumentManagementManager.CreateCustomAttributeDefinitionAsync(projectId, folderId, new CustomAttributeDefinitionsPostRequestBody());
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
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/document-management-versionsbatch-get-POST
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="body">The batch get request body</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="VersionsBatchGetPostResponse"/> containing versions and custom attributes, or <see langword="null"/>.</returns>
    /// <example>
    /// <code>
    /// Guid projectId = Guid.Parse("a4be0c34-a4ab-4ab7-9a4b-1234567890ab");
    /// VersionsBatchGetPostResponse? result = await client.DocumentManagementManager.BatchGetVersionCustomAttributesAsync(projectId, new VersionsBatchGetPostRequestBody());
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
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/document-management-custom-attributesbatch-update-POST
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="versionId">The URL-encoded URN of the version</param>
    /// <param name="body">The batch update request body</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="CustomAttributesBatchUpdatePostResponse"/> containing the batch update result, or <see langword="null"/>.</returns>
    /// <example>
    /// <code>
    /// Guid projectId = Guid.Parse("a4be0c34-a4ab-4ab7-9a4b-1234567890ab");
    /// string versionId = "urn:adsk.wipprod:fs.file:vf.xxx?version=1";
    /// CustomAttributesBatchUpdatePostResponse? result = await client.DocumentManagementManager.BatchUpdateCustomAttributesAsync(projectId, versionId, new CustomAttributesBatchUpdatePostRequestBody());
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
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/document-management-naming-standards-id-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="namingStandardId">The ID of the naming standard</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.BIM360.Bim360.Docs.V1.Projects.Item.NamingStandards.Item.NamingStandardsGetResponse"/> containing the naming standard, or <see langword="null"/>.</returns>
    /// <example>
    /// <code>
    /// Guid projectId = Guid.Parse("a4be0c34-a4ab-4ab7-9a4b-1234567890ab");
    /// Guid namingStandardId = Guid.Parse("b5cf1d45-b5bc-5bc8-0b5c-2345678901bc");
    /// Autodesk.BIM360.Bim360.Docs.V1.Projects.Item.NamingStandards.Item.NamingStandardsGetResponse? standard =
    ///     await client.DocumentManagementManager.GetNamingStandardAsync(projectId, namingStandardId);
    /// </code>
    /// </example>
    public async Task<Autodesk.BIM360.Bim360.Docs.V1.Projects.Item.NamingStandards.Item.NamingStandardsGetResponse?> GetNamingStandardAsync(
        Guid projectId,
        Guid namingStandardId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Docs.V1.Projects[projectId]
            .NamingStandards[namingStandardId]
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Starts a PDF export for one or more views or sheets from a document version (BIM 360 Docs).
    /// </summary>
    /// <remarks>
    /// Wraps: POST /bim360/docs/v1/projects/{project_id}/versions/{version_id}/exports
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/document-management-projects-project_id-versions-version_id-exports-POST
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="versionId">The URL-encoded URN of the version</param>
    /// <param name="body">The export request body</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="ExportsPostResponse"/> containing the export job details, or <see langword="null"/>.</returns>
    /// <example>
    /// <code>
    /// Guid projectId = Guid.Parse("a4be0c34-a4ab-4ab7-9a4b-1234567890ab");
    /// string versionId = "urn:adsk.wipprod:fs.file:vf.xxx?version=1";
    /// ExportsPostResponse? job = await client.DocumentManagementManager.StartVersionExportAsync(projectId, versionId, new ExportsPostRequestBody());
    /// </code>
    /// </example>
    public async Task<ExportsPostResponse?> StartVersionExportAsync(
        Guid projectId,
        string versionId,
        ExportsPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Docs.V1.Projects[projectId]
            .Versions[versionId]
            .Exports
            .PostAsync(body, r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Retrieves the status (and result URL when ready) of a version PDF export job.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /bim360/docs/v1/projects/{project_id}/versions/{version_id}/exports/{export_id}
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/document-management-projects-project_id-versions-version_id-exports-export_id-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="versionId">The URL-encoded URN of the version</param>
    /// <param name="exportId">The export job ID returned from <see cref="StartVersionExportAsync"/></param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.BIM360.Bim360.Docs.V1.Projects.Item.Versions.Item.Exports.Item.WithExport_GetResponse"/> containing export status, or <see langword="null"/>.</returns>
    /// <example>
    /// <code>
    /// Guid projectId = Guid.Parse("a4be0c34-a4ab-4ab7-9a4b-1234567890ab");
    /// string versionId = "urn:adsk.wipprod:fs.file:vf.xxx?version=1";
    /// Guid exportId = Guid.Parse("c6d02e56-c6cd-6cd9-1c6d-3456789012cd");
    /// Autodesk.BIM360.Bim360.Docs.V1.Projects.Item.Versions.Item.Exports.Item.WithExport_GetResponse? status =
    ///     await client.DocumentManagementManager.GetVersionExportStatusAsync(projectId, versionId, exportId);
    /// </code>
    /// </example>
    public async Task<Autodesk.BIM360.Bim360.Docs.V1.Projects.Item.Versions.Item.Exports.Item.WithExport_GetResponse?> GetVersionExportStatusAsync(
        Guid projectId,
        string versionId,
        Guid exportId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Docs.V1.Projects[projectId]
            .Versions[versionId]
            .Exports[exportId]
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }
}

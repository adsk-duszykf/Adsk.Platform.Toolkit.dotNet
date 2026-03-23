using Autodesk.Vault.Models;
using Autodesk.Vault.Vaults.Item.Files.Item;
using Autodesk.Vault.Vaults.Item.FileVersions.Item.Content;
using Microsoft.Kiota.Abstractions;
using static Autodesk.Vault.Vaults.Item.Files.Item.Versions.VersionsRequestBuilder;
using static Autodesk.Vault.Vaults.Item.FileVersions.FileVersionsRequestBuilder;
using static Autodesk.Vault.Vaults.Item.Folders.Item.Contents.ContentsRequestBuilder;
using static Autodesk.Vault.Vaults.Item.Folders.Item.SubFolders.SubFoldersRequestBuilder;

namespace Autodesk.Vault.Managers;

/// <summary>
/// Manager for Files and Folders operations
/// </summary>
public class FilesAndFoldersManager
{
    private readonly BaseVaultClient _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="FilesAndFoldersManager"/> class.
    /// </summary>
    /// <param name="api">The API client</param>
    public FilesAndFoldersManager(BaseVaultClient api)
    {
        _api = api;
    }

    #region File Versions

    /// <summary>
    /// Get file versions for a specific vault
    /// </summary>
    /// <param name="vaultId">Vault ID</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of file versions</returns>
    public async Task<FileVersionCollection?> GetFileVersionsAsync(
        string vaultId,
        RequestConfiguration<FileVersionsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _api.Vaults[vaultId].FileVersions
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);

        return result;
    }

    /// <summary>
    /// Get a file version by ID
    /// </summary>
    /// <param name="vaultId">Vault ID</param>
    /// <param name="fileVersionId">File version ID</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>File version information</returns>
    public async Task<FileVersionExtended?> GetFileVersionByIdAsync(
        string vaultId,
        string fileVersionId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _api.Vaults[vaultId].FileVersions[fileVersionId]
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);

        return result;
    }

    /// <summary>
    /// Get file version content
    /// </summary>
    /// <param name="vaultId">Vault ID</param>
    /// <param name="fileVersionId">File version ID</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>File content stream</returns>
    public async Task<Stream?> GetFileVersionContentAsync(
        string vaultId,
        string fileVersionId,
        RequestConfiguration<ContentRequestBuilder.ContentRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _api.Vaults[vaultId].FileVersions[fileVersionId].Content
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);

        return result;
    }

    /// <summary>
    /// Get file version thumbnail
    /// </summary>
    /// <param name="vaultId">Vault ID</param>
    /// <param name="fileVersionId">File version ID</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Thumbnail image stream</returns>
    public async Task<Stream?> GetFileVersionThumbnailAsync(
        string vaultId,
        string fileVersionId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _api.Vaults[vaultId].FileVersions[fileVersionId].Thumbnail
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);

        return result;
    }

    #endregion

    #region Files

    /// <summary>
    /// Get a file by ID
    /// </summary>
    /// <param name="vaultId">Vault ID</param>
    /// <param name="fileId">File ID</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>File information</returns>
    public async Task<FileObject?> GetFileByIdAsync(
        string vaultId,
        string fileId,
        RequestConfiguration<FilesItemRequestBuilder.FilesItemRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _api.Vaults[vaultId].Files[fileId]
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);

        return result;
    }

    /// <summary>
    /// Get all versions for a specific file
    /// </summary>
    /// <param name="vaultId">Vault ID</param>
    /// <param name="fileId">File ID</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of file versions</returns>
    public async Task<FileVersionCollection?> GetFileVersionsForFileAsync(
        string vaultId,
        string fileId,
        RequestConfiguration<VersionsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _api.Vaults[vaultId].Files[fileId].Versions
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);

        return result;
    }

    #endregion

    #region Folders

    /// <summary>
    /// Get a folder by ID
    /// </summary>
    /// <param name="vaultId">Vault ID</param>
    /// <param name="folderId">Folder ID</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Folder information</returns>
    public async Task<Folder?> GetFolderByIdAsync(
        string vaultId,
        string folderId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _api.Vaults[vaultId].Folders[folderId]
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);

        return result;
    }

    /// <summary>
    /// Get folder contents (files and folders)
    /// </summary>
    /// <param name="vaultId">Vault ID</param>
    /// <param name="folderId">Folder ID</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Folder contents</returns>
    public async Task<EntityCollection?> GetFolderContentsAsync(
        string vaultId,
        string folderId,
        RequestConfiguration<ContentsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _api.Vaults[vaultId].Folders[folderId].Contents
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);

        return result;
    }

    /// <summary>
    /// Get sub-folders of a folder
    /// </summary>
    /// <param name="vaultId">Vault ID</param>
    /// <param name="folderId">Folder ID</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of sub-folders</returns>
    public async Task<FolderCollection?> GetSubFoldersAsync(
        string vaultId,
        string folderId,
        RequestConfiguration<SubFoldersRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _api.Vaults[vaultId].Folders[folderId].SubFolders
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);

        return result;
    }

    #endregion
}

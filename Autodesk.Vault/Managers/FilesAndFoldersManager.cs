using System.Runtime.CompilerServices;
using Autodesk.Vault.Models;
using Autodesk.Vault.Vaults.Item.Files.Item;
using Autodesk.Vault.Vaults.Item.FileVersions.Item.Content;
using Autodesk.Vault.Vaults.Item.FileVersions.Item.Signedurl;
using Autodesk.Vault.Vaults.Item.FileVersions.Item.Svf.BubbleJson;
using Microsoft.Kiota.Abstractions;
using static Autodesk.Vault.Vaults.Item.Files.Item.Versions.VersionsRequestBuilder;
using static Autodesk.Vault.Vaults.Item.FileVersions.FileVersionsRequestBuilder;
using static Autodesk.Vault.Vaults.Item.FileVersions.Item.Markups.MarkupsRequestBuilder;
using static Autodesk.Vault.Vaults.Item.FileVersions.Item.Parents.ParentsRequestBuilder;
using static Autodesk.Vault.Vaults.Item.FileVersions.Item.Uses.UsesRequestBuilder;
using static Autodesk.Vault.Vaults.Item.FileVersions.Item.VisualizationAttachments.VisualizationAttachmentsRequestBuilder;
using static Autodesk.Vault.Vaults.Item.Folders.Item.Contents.ContentsRequestBuilder;
using static Autodesk.Vault.Vaults.Item.Folders.Item.SubFolders.SubFoldersRequestBuilder;
using FileChangeOrdersRequestBuilder = Autodesk.Vault.Vaults.Item.Files.Item.ChangeOrders.ChangeOrdersRequestBuilder;
using FileVersionItemVersionsRequestBuilder = Autodesk.Vault.Vaults.Item.FileVersions.Item.ItemVersions.ItemVersionsRequestBuilder;

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
    /// Lists file versions for a specific vault with automatic cursor-based pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /vaults/{vaultId}/file-versions
    /// </remarks>
    /// <param name="vaultId">The unique identifier of a vault</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports filtering, sorting, limit, cursorState)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="FileVersionCollection.FileVersionCollection_results"/> items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (var version in client.FilesAndFolders.ListFileVersionsAsync("1"))
    /// {
    ///     Console.WriteLine(version.FileName);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<FileVersionCollection.FileVersionCollection_results> ListFileVersionsAsync(
        string vaultId,
        RequestConfiguration<FileVersionsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? cursor = requestConfiguration?.QueryParameters?.CursorState;

        while (true)
        {
            FileVersionCollection? response = await _api.Vaults[vaultId].FileVersions
                .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                    r.QueryParameters.CursorState = cursor;
                }, cancellationToken);

            if (response?.Results is not { Count: > 0 })
                yield break;

            foreach (var item in response.Results)
            {
                yield return item;
            }

            if (string.IsNullOrEmpty(response.Pagination?.NextUrl))
                yield break;

            cursor = PaginationHelper.ExtractBookmarkFromNextUrl(response.Pagination.NextUrl);
        }
    }

    /// <summary>
    /// Get a file version by its ID.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /vaults/{vaultId}/file-versions/{id}
    /// </remarks>
    /// <param name="vaultId">The unique identifier of a vault</param>
    /// <param name="fileVersionId">The unique identifier of a file version</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="FileVersionExtended"/> containing the file version information</returns>
    /// <example>
    /// <code>
    /// FileVersionExtended? version = await client.FilesAndFolders.GetFileVersionByIdAsync("1", "100");
    /// </code>
    /// </example>
    public async Task<FileVersionExtended?> GetFileVersionByIdAsync(
        string vaultId,
        string fileVersionId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Vaults[vaultId].FileVersions[fileVersionId]
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Retrieve the content of a specific file version. Supports both full file download and partial content via range requests.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /vaults/{vaultId}/file-versions/{id}/content
    /// </remarks>
    /// <param name="vaultId">The unique identifier of a vault</param>
    /// <param name="fileVersionId">The unique identifier of a file version</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports allowSync, contentDisposition, watermark parameters)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Stream"/> containing the file content</returns>
    /// <example>
    /// <code>
    /// Stream? content = await client.FilesAndFolders.GetFileVersionContentAsync("1", "100");
    /// </code>
    /// </example>
    public async Task<Stream?> GetFileVersionContentAsync(
        string vaultId,
        string fileVersionId,
        RequestConfiguration<ContentRequestBuilder.ContentRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Vaults[vaultId].FileVersions[fileVersionId].Content
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Retrieve metadata for a specific file version content without fetching the full content body.
    /// </summary>
    /// <remarks>
    /// Wraps: HEAD /vaults/{vaultId}/file-versions/{id}/content
    /// </remarks>
    /// <param name="vaultId">The unique identifier of a vault</param>
    /// <param name="fileVersionId">The unique identifier of a file version</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports allowSync, contentDisposition, watermark parameters)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Stream"/> containing the response metadata</returns>
    /// <example>
    /// <code>
    /// Stream? metadata = await client.FilesAndFolders.GetFileVersionContentHeadAsync("1", "100");
    /// </code>
    /// </example>
    public async Task<Stream?> GetFileVersionContentHeadAsync(
        string vaultId,
        string fileVersionId,
        RequestConfiguration<ContentRequestBuilder.ContentRequestBuilderHeadQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Vaults[vaultId].FileVersions[fileVersionId].Content
            .HeadAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Generate a time-limited, cryptographically signed URL for secure file download.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /vaults/{vaultId}/file-versions/{id}/signedurl
    /// </remarks>
    /// <param name="vaultId">The unique identifier of a vault</param>
    /// <param name="fileVersionId">The unique identifier of a file version</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports contentDisposition, expirationTime, watermark parameters)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="SignedurlGetResponse"/> containing the signed download URL</returns>
    /// <example>
    /// <code>
    /// SignedurlGetResponse? signed = await client.FilesAndFolders.GetFileVersionSignedUrlAsync("1", "100");
    /// </code>
    /// </example>
    public async Task<SignedurlGetResponse?> GetFileVersionSignedUrlAsync(
        string vaultId,
        string fileVersionId,
        RequestConfiguration<SignedurlRequestBuilder.SignedurlRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Vaults[vaultId].FileVersions[fileVersionId].Signedurl
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Retrieve the bubble.json file for LMV (Large Model Viewer) visualization. Should only be invoked for DWF/DWFx files.
    /// On first request, the server triggers a background job to generate the LMV files.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /vaults/{vaultId}/file-versions/{id}/svf/bubble.json
    /// </remarks>
    /// <param name="vaultId">The unique identifier of a vault</param>
    /// <param name="fileVersionId">The unique identifier of a file version</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports allowSync, watermark parameters)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="BubbleGetResponse"/> containing the LMV root metadata</returns>
    /// <example>
    /// <code>
    /// BubbleGetResponse? bubble = await client.FilesAndFolders.GetFileVersionLmvRootAsync("1", "100");
    /// </code>
    /// </example>
    public async Task<BubbleGetResponse?> GetFileVersionLmvRootAsync(
        string vaultId,
        string fileVersionId,
        RequestConfiguration<BubbleJsonRequestBuilder.BubbleJsonRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Vaults[vaultId].FileVersions[fileVersionId].Svf.BubbleJson
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Get the thumbnail image for a file version.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /vaults/{vaultId}/file-versions/{id}/thumbnail
    /// </remarks>
    /// <param name="vaultId">The unique identifier of a vault</param>
    /// <param name="fileVersionId">The unique identifier of a file version</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Stream"/> containing the thumbnail image</returns>
    /// <example>
    /// <code>
    /// Stream? thumbnail = await client.FilesAndFolders.GetFileVersionThumbnailAsync("1", "100");
    /// </code>
    /// </example>
    public async Task<Stream?> GetFileVersionThumbnailAsync(
        string vaultId,
        string fileVersionId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Vaults[vaultId].FileVersions[fileVersionId].Thumbnail
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Lists all item versions assigned to a file version with automatic cursor-based pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /vaults/{vaultId}/file-versions/{id}/item-versions
    /// </remarks>
    /// <param name="vaultId">The unique identifier of a vault</param>
    /// <param name="fileVersionId">The unique identifier of a file version</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports propDefIds, releasedOnly, limit, cursorState)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="ItemVersion"/> items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (ItemVersion iv in client.FilesAndFolders.ListFileVersionAssociatedItemVersionsAsync("1", "100"))
    /// {
    ///     Console.WriteLine(iv.ItemNumber);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<ItemVersion> ListFileVersionAssociatedItemVersionsAsync(
        string vaultId,
        string fileVersionId,
        RequestConfiguration<FileVersionItemVersionsRequestBuilder.ItemVersionsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? cursor = requestConfiguration?.QueryParameters?.CursorState;

        while (true)
        {
            ItemVersionCollection? response = await _api.Vaults[vaultId].FileVersions[fileVersionId].ItemVersions
                .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                    r.QueryParameters.CursorState = cursor;
                }, cancellationToken);

            if (response?.Results is not { Count: > 0 })
                yield break;

            foreach (ItemVersion item in response.Results)
            {
                yield return item;
            }

            if (string.IsNullOrEmpty(response.Pagination?.NextUrl))
                yield break;

            cursor = PaginationHelper.ExtractBookmarkFromNextUrl(response.Pagination.NextUrl);
        }
    }

    /// <summary>
    /// Lists all markups for a specific file version with automatic cursor-based pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /vaults/{vaultId}/file-versions/{id}/markups
    /// </remarks>
    /// <param name="vaultId">The unique identifier of a vault</param>
    /// <param name="fileVersionId">The unique identifier of a file version</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports limit, cursorState)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="Markup"/> items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (Markup markup in client.FilesAndFolders.ListFileVersionMarkupsAsync("1", "100"))
    /// {
    ///     Console.WriteLine(markup.Name);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<Markup> ListFileVersionMarkupsAsync(
        string vaultId,
        string fileVersionId,
        RequestConfiguration<MarkupsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? cursor = requestConfiguration?.QueryParameters?.CursorState;

        while (true)
        {
            MarkupCollection? response = await _api.Vaults[vaultId].FileVersions[fileVersionId].Markups
                .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                    r.QueryParameters.CursorState = cursor;
                }, cancellationToken);

            if (response?.Results is not { Count: > 0 })
                yield break;

            foreach (Markup item in response.Results)
            {
                yield return item;
            }

            if (string.IsNullOrEmpty(response.Pagination?.NextUrl))
                yield break;

            cursor = PaginationHelper.ExtractBookmarkFromNextUrl(response.Pagination.NextUrl);
        }
    }

    /// <summary>
    /// Get a specific markup for a file version.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /vaults/{vaultId}/file-versions/{id}/markups/{markupId}
    /// </remarks>
    /// <param name="vaultId">The unique identifier of a vault</param>
    /// <param name="fileVersionId">The unique identifier of a file version</param>
    /// <param name="markupId">The unique identifier of a markup</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Markup"/> containing the markup information</returns>
    /// <example>
    /// <code>
    /// Markup? markup = await client.FilesAndFolders.GetFileVersionMarkupByIdAsync("1", "100", "5");
    /// </code>
    /// </example>
    public async Task<Markup?> GetFileVersionMarkupByIdAsync(
        string vaultId,
        string fileVersionId,
        string markupId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Vaults[vaultId].FileVersions[fileVersionId].Markups[markupId]
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Lists the visualization attachments (DWF/DWFx) for a file version with automatic cursor-based pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /vaults/{vaultId}/file-versions/{id}/visualization-attachments
    /// </remarks>
    /// <param name="vaultId">The unique identifier of a vault</param>
    /// <param name="fileVersionId">The unique identifier of a file version</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports limit, cursorState)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="FileVersionCollection.FileVersionCollection_results"/> items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (var vizAttachment in client.FilesAndFolders.ListFileVersionVisualizationAttachmentsAsync("1", "100"))
    /// {
    ///     Console.WriteLine(vizAttachment.FileName);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<FileVersionCollection.FileVersionCollection_results> ListFileVersionVisualizationAttachmentsAsync(
        string vaultId,
        string fileVersionId,
        RequestConfiguration<VisualizationAttachmentsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? cursor = requestConfiguration?.QueryParameters?.CursorState;

        while (true)
        {
            FileVersionCollection? response = await _api.Vaults[vaultId].FileVersions[fileVersionId].VisualizationAttachments
                .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                    r.QueryParameters.CursorState = cursor;
                }, cancellationToken);

            if (response?.Results is not { Count: > 0 })
                yield break;

            foreach (var item in response.Results)
            {
                yield return item;
            }

            if (string.IsNullOrEmpty(response.Pagination?.NextUrl))
                yield break;

            cursor = PaginationHelper.ExtractBookmarkFromNextUrl(response.Pagination.NextUrl);
        }
    }

    /// <summary>
    /// Lists file dependencies and attachments for a given file version with automatic cursor-based pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /vaults/{vaultId}/file-versions/{id}/uses
    /// </remarks>
    /// <param name="vaultId">The unique identifier of a vault</param>
    /// <param name="fileVersionId">The unique identifier of a file version</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports recurse, releasedOnly, releaseBiased, includeHidden, getLatestAssocs, extendedModels, propDefIds, limit, cursorState)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="FileAssociation"/> items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (FileAssociation assoc in client.FilesAndFolders.ListFileVersionUsesAsync("1", "100"))
    /// {
    ///     Console.WriteLine(assoc.FileName);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<FileAssociation> ListFileVersionUsesAsync(
        string vaultId,
        string fileVersionId,
        RequestConfiguration<UsesRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? cursor = requestConfiguration?.QueryParameters?.CursorState;

        while (true)
        {
            FileAssocCollection? response = await _api.Vaults[vaultId].FileVersions[fileVersionId].Uses
                .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                    r.QueryParameters.CursorState = cursor;
                }, cancellationToken);

            if (response?.Results is not { Count: > 0 })
                yield break;

            foreach (FileAssociation item in response.Results)
            {
                yield return item;
            }

            if (string.IsNullOrEmpty(response.Pagination?.NextUrl))
                yield break;

            cursor = PaginationHelper.ExtractBookmarkFromNextUrl(response.Pagination.NextUrl);
        }
    }

    /// <summary>
    /// Lists parent associations (where used) for a given file version with automatic cursor-based pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /vaults/{vaultId}/file-versions/{id}/parents
    /// </remarks>
    /// <param name="vaultId">The unique identifier of a vault</param>
    /// <param name="fileVersionId">The unique identifier of a file version</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports recurse, releasedOnly, releaseBiased, includeHidden, getLatestAssocs, extendedModels, propDefIds, limit, cursorState)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="FileAssociation"/> items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (FileAssociation parent in client.FilesAndFolders.ListFileVersionWhereUsedAsync("1", "100"))
    /// {
    ///     Console.WriteLine(parent.FileName);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<FileAssociation> ListFileVersionWhereUsedAsync(
        string vaultId,
        string fileVersionId,
        RequestConfiguration<ParentsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? cursor = requestConfiguration?.QueryParameters?.CursorState;

        while (true)
        {
            FileAssocCollection? response = await _api.Vaults[vaultId].FileVersions[fileVersionId].Parents
                .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                    r.QueryParameters.CursorState = cursor;
                }, cancellationToken);

            if (response?.Results is not { Count: > 0 })
                yield break;

            foreach (FileAssociation item in response.Results)
            {
                yield return item;
            }

            if (string.IsNullOrEmpty(response.Pagination?.NextUrl))
                yield break;

            cursor = PaginationHelper.ExtractBookmarkFromNextUrl(response.Pagination.NextUrl);
        }
    }

    #endregion

    #region Files

    /// <summary>
    /// Get a file by its ID.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /vaults/{vaultId}/files/{id}
    /// </remarks>
    /// <param name="vaultId">The unique identifier of a vault</param>
    /// <param name="fileId">The unique identifier of a file (MasterId)</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports releasedOnly)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="FileObject"/> containing the file information</returns>
    /// <example>
    /// <code>
    /// FileObject? file = await client.FilesAndFolders.GetFileByIdAsync("1", "42");
    /// </code>
    /// </example>
    public async Task<FileObject?> GetFileByIdAsync(
        string vaultId,
        string fileId,
        RequestConfiguration<FilesItemRequestBuilder.FilesItemRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Vaults[vaultId].Files[fileId]
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Lists all versions (history) for a specific file with automatic cursor-based pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /vaults/{vaultId}/files/{id}/versions
    /// </remarks>
    /// <param name="vaultId">The unique identifier of a vault</param>
    /// <param name="fileId">The unique identifier of a file (MasterId)</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports extendedModels, propDefIds, limit, cursorState)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="FileVersionCollection.FileVersionCollection_results"/> items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (var version in client.FilesAndFolders.ListFileVersionsForFileAsync("1", "42"))
    /// {
    ///     Console.WriteLine(version.FileName);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<FileVersionCollection.FileVersionCollection_results> ListFileVersionsForFileAsync(
        string vaultId,
        string fileId,
        RequestConfiguration<VersionsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? cursor = requestConfiguration?.QueryParameters?.CursorState;

        while (true)
        {
            FileVersionCollection? response = await _api.Vaults[vaultId].Files[fileId].Versions
                .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                    r.QueryParameters.CursorState = cursor;
                }, cancellationToken);

            if (response?.Results is not { Count: > 0 })
                yield break;

            foreach (var item in response.Results)
            {
                yield return item;
            }

            if (string.IsNullOrEmpty(response.Pagination?.NextUrl))
                yield break;

            cursor = PaginationHelper.ExtractBookmarkFromNextUrl(response.Pagination.NextUrl);
        }
    }

    /// <summary>
    /// Lists change orders associated with a specific file with automatic cursor-based pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /vaults/{vaultId}/files/{id}/change-orders
    /// </remarks>
    /// <param name="vaultId">The unique identifier of a vault</param>
    /// <param name="fileId">The unique identifier of a file (MasterId)</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports extendedModels, includeClosedECOs, propDefIds, limit, cursorState)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="ChangeOrderCollection.ChangeOrderCollection_results"/> items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (var co in client.FilesAndFolders.ListFileAssociatedChangeOrdersAsync("1", "42"))
    /// {
    ///     Console.WriteLine(co.Name);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<ChangeOrderCollection.ChangeOrderCollection_results> ListFileAssociatedChangeOrdersAsync(
        string vaultId,
        string fileId,
        RequestConfiguration<FileChangeOrdersRequestBuilder.ChangeOrdersRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? cursor = requestConfiguration?.QueryParameters?.CursorState;

        while (true)
        {
            ChangeOrderCollection? response = await _api.Vaults[vaultId].Files[fileId].ChangeOrders
                .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                    r.QueryParameters.CursorState = cursor;
                }, cancellationToken);

            if (response?.Results is not { Count: > 0 })
                yield break;

            foreach (var item in response.Results)
            {
                yield return item;
            }

            if (string.IsNullOrEmpty(response.Pagination?.NextUrl))
                yield break;

            cursor = PaginationHelper.ExtractBookmarkFromNextUrl(response.Pagination.NextUrl);
        }
    }

    #endregion

    #region Folders

    /// <summary>
    /// Get a folder by its ID.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /vaults/{vaultId}/folders/{id}
    /// </remarks>
    /// <param name="vaultId">The unique identifier of a vault</param>
    /// <param name="folderId">The unique identifier of a folder</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Folder"/> containing the folder information</returns>
    /// <example>
    /// <code>
    /// Folder? folder = await client.FilesAndFolders.GetFolderByIdAsync("1", "10");
    /// </code>
    /// </example>
    public async Task<Folder?> GetFolderByIdAsync(
        string vaultId,
        string folderId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Vaults[vaultId].Folders[folderId]
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Lists the contents of a folder (files and folders) with automatic cursor-based pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /vaults/{vaultId}/folders/{id}/contents
    /// </remarks>
    /// <param name="vaultId">The unique identifier of a vault</param>
    /// <param name="folderId">The unique identifier of a folder</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports filtering, sorting, extendedModels, propDefIds, limit, cursorState)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="EntityCollection.EntityCollection_results"/> items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (var entity in client.FilesAndFolders.ListFolderContentsAsync("1", "10"))
    /// {
    ///     Console.WriteLine(entity.Name);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<EntityCollection.EntityCollection_results> ListFolderContentsAsync(
        string vaultId,
        string folderId,
        RequestConfiguration<ContentsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? cursor = requestConfiguration?.QueryParameters?.CursorState;

        while (true)
        {
            EntityCollection? response = await _api.Vaults[vaultId].Folders[folderId].Contents
                .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                    r.QueryParameters.CursorState = cursor;
                }, cancellationToken);

            if (response?.Results is not { Count: > 0 })
                yield break;

            foreach (var item in response.Results)
            {
                yield return item;
            }

            if (string.IsNullOrEmpty(response.Pagination?.NextUrl))
                yield break;

            cursor = PaginationHelper.ExtractBookmarkFromNextUrl(response.Pagination.NextUrl);
        }
    }

    /// <summary>
    /// Lists sub-folders of a folder with automatic cursor-based pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /vaults/{vaultId}/folders/{id}/sub-folders
    /// </remarks>
    /// <param name="vaultId">The unique identifier of a vault</param>
    /// <param name="folderId">The unique identifier of a folder</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports limit, cursorState)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="FolderCollection.FolderCollection_results"/> items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (var subFolder in client.FilesAndFolders.ListSubFoldersAsync("1", "10"))
    /// {
    ///     Console.WriteLine(subFolder.Name);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<FolderCollection.FolderCollection_results> ListSubFoldersAsync(
        string vaultId,
        string folderId,
        RequestConfiguration<SubFoldersRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? cursor = requestConfiguration?.QueryParameters?.CursorState;

        while (true)
        {
            FolderCollection? response = await _api.Vaults[vaultId].Folders[folderId].SubFolders
                .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                    r.QueryParameters.CursorState = cursor;
                }, cancellationToken);

            if (response?.Results is not { Count: > 0 })
                yield break;

            foreach (var item in response.Results)
            {
                yield return item;
            }

            if (string.IsNullOrEmpty(response.Pagination?.NextUrl))
                yield break;

            cursor = PaginationHelper.ExtractBookmarkFromNextUrl(response.Pagination.NextUrl);
        }
    }

    #endregion
}

using System.Runtime.CompilerServices;
using Autodesk.ACC.Construction.Sheets.V1.Projects.Item.Collections;
using Autodesk.ACC.Construction.Sheets.V1.Projects.Item.Collections.Item;
using Autodesk.ACC.Construction.Sheets.V1.Projects.Item.Exports;
using Autodesk.ACC.Construction.Sheets.V1.Projects.Item.Exports.Item;
using Autodesk.ACC.Construction.Sheets.V1.Projects.Item.Sheets;
using Autodesk.ACC.Construction.Sheets.V1.Projects.Item.SheetsBatchDelete;
using Autodesk.ACC.Construction.Sheets.V1.Projects.Item.SheetsBatchGet;
using Autodesk.ACC.Construction.Sheets.V1.Projects.Item.SheetsBatchRestore;
using Autodesk.ACC.Construction.Sheets.V1.Projects.Item.SheetsBatchUpdate;
using Autodesk.ACC.Construction.Sheets.V1.Projects.Item.Storage;
using Autodesk.ACC.Construction.Sheets.V1.Projects.Item.Uploads;
using Autodesk.ACC.Construction.Sheets.V1.Projects.Item.Uploads.Item;
using Autodesk.ACC.Construction.Sheets.V1.Projects.Item.Uploads.Item.ReviewSheets;
using Autodesk.ACC.Construction.Sheets.V1.Projects.Item.Uploads.Item.ThumbnailsBatchGet;
using Autodesk.ACC.Construction.Sheets.V1.Projects.Item.VersionSets;
using Autodesk.ACC.Construction.Sheets.V1.Projects.Item.VersionSets.Item;
using Autodesk.ACC.Construction.Sheets.V1.Projects.Item.VersionSetsBatchDelete;
using Autodesk.ACC.Construction.Sheets.V1.Projects.Item.VersionSetsBatchGet;
using Microsoft.Kiota.Abstractions;
using static Autodesk.ACC.Construction.Sheets.V1.Projects.Item.Collections.CollectionsRequestBuilder;
using static Autodesk.ACC.Construction.Sheets.V1.Projects.Item.Sheets.SheetsRequestBuilder;
using static Autodesk.ACC.Construction.Sheets.V1.Projects.Item.Uploads.UploadsRequestBuilder;
using static Autodesk.ACC.Construction.Sheets.V1.Projects.Item.Uploads.Item.ReviewSheets.ReviewSheetsRequestBuilder;
using static Autodesk.ACC.Construction.Sheets.V1.Projects.Item.VersionSets.VersionSetsRequestBuilder;

namespace Autodesk.ACC.Managers;

/// <summary>
/// Manager for Sheets operations — manages sheets, version sets, reviews, uploads, exports, and collections.
/// </summary>
public class SheetsManager
{
    private readonly BaseACCclient _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="SheetsManager"/> class.
    /// </summary>
    /// <param name="api">The API client</param>
    public SheetsManager(BaseACCclient api)
    {
        _api = api;
    }

    #region Version Sets

    /// <summary>
    /// Retrieves all version sets in the project with automatic pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/sheets/v1/projects/{projectId}/version-sets
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/sheets-version-sets-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports collectionId, limit, offset, sort)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{VersionSetsGetResponse_results}"/> of <see cref="VersionSetsGetResponse_results"/> items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (var vs in client.SheetsManager.ListVersionSetsAsync(projectId)) { Console.WriteLine(vs.Name); }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<VersionSetsGetResponse_results> ListVersionSetsAsync(
        Guid projectId,
        Action<RequestConfiguration<VersionSetsRequestBuilderGetQueryParameters>>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        int offset = 0;
        while (true)
        {
            var capturedOffset = offset;
            var response = await _api.Construction.Sheets.V1.Projects[projectId]
                .VersionSets
                .GetAsync(config =>
                {
                    requestConfiguration?.Invoke(config);
                    config.QueryParameters.Offset = capturedOffset;
                }, cancellationToken);

            if (response?.Results is not { Count: > 0 })
                yield break;

            foreach (var item in response.Results)
                yield return item;

            if (string.IsNullOrEmpty(response.Pagination?.NextUrl))
                yield break;

            offset += response.Results.Count;
        }
    }

    /// <summary>
    /// Retrieves a page of version sets.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/sheets/v1/projects/{projectId}/version-sets
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/sheets-version-sets-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports collectionId, limit, offset, sort)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="VersionSetsGetResponse"/> containing the version sets</returns>
    /// <example>
    /// <code>
    /// VersionSetsGetResponse? response = await client.SheetsManager.GetVersionSetsAsync(projectId);
    /// </code>
    /// </example>
    public async Task<VersionSetsGetResponse?> GetVersionSetsAsync(
        Guid projectId,
        Action<RequestConfiguration<VersionSetsRequestBuilderGetQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Sheets.V1.Projects[projectId]
            .VersionSets
            .GetAsync(requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Creates a version set.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /construction/sheets/v1/projects/{projectId}/version-sets
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/sheets-version-sets-POST
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="body">The version set creation data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="VersionSetsPostResponse"/> containing the created version set</returns>
    /// <example>
    /// <code>
    /// VersionSetsPostResponse? vs = await client.SheetsManager.CreateVersionSetAsync(projectId, new VersionSetsPostRequestBody { Name = "A101", IssuanceDate = DateTimeOffset.UtcNow });
    /// </code>
    /// </example>
    public async Task<VersionSetsPostResponse?> CreateVersionSetAsync(
        Guid projectId,
        VersionSetsPostRequestBody body,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Sheets.V1.Projects[projectId]
            .VersionSets
            .PostAsync(body, requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Updates a version set.
    /// </summary>
    /// <remarks>
    /// Wraps: PATCH /construction/sheets/v1/projects/{projectId}/version-sets/{versionSetId}
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/sheets-version-sets-versionSetId-PATCH
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="versionSetId">The ID of the version set</param>
    /// <param name="body">The update data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="WithVersionSetPatchResponse"/> containing the updated version set</returns>
    /// <example>
    /// <code>
    /// WithVersionSetPatchResponse? updated = await client.SheetsManager.UpdateVersionSetAsync(projectId, versionSetId, new WithVersionSetPatchRequestBody());
    /// </code>
    /// </example>
    public async Task<WithVersionSetPatchResponse?> UpdateVersionSetAsync(
        Guid projectId,
        string versionSetId,
        WithVersionSetPatchRequestBody body,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Sheets.V1.Projects[projectId]
            .VersionSets[versionSetId]
            .PatchAsync(body, requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Retrieves version sets by IDs.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /construction/sheets/v1/projects/{projectId}/version-sets:batch-get
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/sheets-version-setsbatch-get-POST
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="body">The request body containing version set IDs</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="VersionSetsBatchGetPostResponse"/> containing the requested version sets</returns>
    /// <example>
    /// <code>
    /// VersionSetsBatchGetPostResponse? result = await client.SheetsManager.BatchGetVersionSetsAsync(projectId, new VersionSetsBatchGetPostRequestBody { Ids = new List&lt;string&gt; { "id1" } });
    /// </code>
    /// </example>
    public async Task<VersionSetsBatchGetPostResponse?> BatchGetVersionSetsAsync(
        Guid projectId,
        VersionSetsBatchGetPostRequestBody body,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Sheets.V1.Projects[projectId]
            .VersionSetsBatchGet
            .PostAsync(body, requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Deletes version sets by IDs.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /construction/sheets/v1/projects/{projectId}/version-sets:batch-delete
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/sheets-version-setsbatch-delete-POST
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="body">The request body containing version set IDs to delete</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    /// <example>
    /// <code>
    /// await client.SheetsManager.BatchDeleteVersionSetsAsync(projectId, new VersionSetsBatchDeletePostRequestBody { Ids = new List&lt;string&gt; { "id1" } });
    /// </code>
    /// </example>
    public async Task BatchDeleteVersionSetsAsync(
        Guid projectId,
        VersionSetsBatchDeletePostRequestBody body,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        await _api.Construction.Sheets.V1.Projects[projectId]
            .VersionSetsBatchDelete
            .PostAsync(body, requestConfiguration, cancellationToken);
    }

    #endregion

    #region Storage

    /// <summary>
    /// Creates a storage location in OSS for uploading a file.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /construction/sheets/v1/projects/{projectId}/storage
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/sheets-storage-POST
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="body">The storage creation request</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="StoragePostResponse"/> containing the storage location and upload URL</returns>
    /// <example>
    /// <code>
    /// StoragePostResponse? storage = await client.SheetsManager.CreateStorageAsync(projectId, new StoragePostRequestBody());
    /// </code>
    /// </example>
    public async Task<StoragePostResponse?> CreateStorageAsync(
        Guid projectId,
        StoragePostRequestBody body,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Sheets.V1.Projects[projectId]
            .Storage
            .PostAsync(body, requestConfiguration, cancellationToken);
    }

    #endregion

    #region Uploads

    /// <summary>
    /// Retrieves all uploads in the project with automatic pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/sheets/v1/projects/{projectId}/uploads
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/sheets-uploads-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports limit, offset, sort)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{UploadsGetResponse_results}"/> of <see cref="UploadsGetResponse_results"/> items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (var upload in client.SheetsManager.ListUploadsAsync(projectId)) { Console.WriteLine(upload.Id); }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<UploadsGetResponse_results> ListUploadsAsync(
        Guid projectId,
        Action<RequestConfiguration<UploadsRequestBuilderGetQueryParameters>>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        int offset = 0;
        while (true)
        {
            var capturedOffset = offset;
            var response = await _api.Construction.Sheets.V1.Projects[projectId]
                .Uploads
                .GetAsync(config =>
                {
                    requestConfiguration?.Invoke(config);
                    config.QueryParameters.Offset = capturedOffset;
                }, cancellationToken);

            if (response?.Results is not { Count: > 0 })
                yield break;

            foreach (var item in response.Results)
                yield return item;

            if (string.IsNullOrEmpty(response.Pagination?.NextUrl))
                yield break;

            offset += response.Results.Count;
        }
    }

    /// <summary>
    /// Retrieves a page of uploads.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/sheets/v1/projects/{projectId}/uploads
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/sheets-uploads-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports limit, offset, sort)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="UploadsGetResponse"/> containing the uploads</returns>
    /// <example>
    /// <code>
    /// UploadsGetResponse? response = await client.SheetsManager.GetUploadsAsync(projectId);
    /// </code>
    /// </example>
    public async Task<UploadsGetResponse?> GetUploadsAsync(
        Guid projectId,
        Action<RequestConfiguration<UploadsRequestBuilderGetQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Sheets.V1.Projects[projectId]
            .Uploads
            .GetAsync(requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Creates an upload object and splits files into separate sheets.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /construction/sheets/v1/projects/{projectId}/uploads
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/sheets-uploads-POST
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="body">The upload creation data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="UploadsPostResponse"/> containing the created upload</returns>
    /// <example>
    /// <code>
    /// UploadsPostResponse? upload = await client.SheetsManager.CreateUploadAsync(projectId, new UploadsPostRequestBody());
    /// </code>
    /// </example>
    public async Task<UploadsPostResponse?> CreateUploadAsync(
        Guid projectId,
        UploadsPostRequestBody body,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Sheets.V1.Projects[projectId]
            .Uploads
            .PostAsync(body, requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Retrieves the processing status of a specific upload.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/sheets/v1/projects/{projectId}/uploads/{uploadId}
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/sheets-uploads-uploadId-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="uploadId">The ID of the upload</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="WithUploadGetResponse"/> containing the upload status</returns>
    /// <example>
    /// <code>
    /// WithUploadGetResponse? upload = await client.SheetsManager.GetUploadAsync(projectId, uploadId);
    /// </code>
    /// </example>
    public async Task<WithUploadGetResponse?> GetUploadAsync(
        Guid projectId,
        string uploadId,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Sheets.V1.Projects[projectId]
            .Uploads[uploadId]
            .GetAsync(requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Retrieves a page of review sheets for an upload.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/sheets/v1/projects/{projectId}/uploads/{uploadId}/review-sheets
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/sheets-review-sheets-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="uploadId">The ID of the upload</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports limit, offset)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="ReviewSheetsGetResponse"/> containing the review sheets</returns>
    /// <example>
    /// <code>
    /// ReviewSheetsGetResponse? sheets = await client.SheetsManager.GetReviewSheetsAsync(projectId, uploadId);
    /// </code>
    /// </example>
    public async Task<ReviewSheetsGetResponse?> GetReviewSheetsAsync(
        Guid projectId,
        string uploadId,
        Action<RequestConfiguration<ReviewSheetsRequestBuilderGetQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Sheets.V1.Projects[projectId]
            .Uploads[uploadId]
            .ReviewSheets
            .GetAsync(requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Updates review sheets for an upload.
    /// </summary>
    /// <remarks>
    /// Wraps: PATCH /construction/sheets/v1/projects/{projectId}/uploads/{uploadId}/review-sheets
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/sheets-review-sheets-PATCH
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="uploadId">The ID of the upload</param>
    /// <param name="body">The update data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="ReviewSheetsPatchResponse"/> containing the updated review sheets</returns>
    /// <example>
    /// <code>
    /// ReviewSheetsPatchResponse? result = await client.SheetsManager.UpdateReviewSheetsAsync(projectId, uploadId, new ReviewSheetsPatchRequestBody());
    /// </code>
    /// </example>
    public async Task<ReviewSheetsPatchResponse?> UpdateReviewSheetsAsync(
        Guid projectId,
        string uploadId,
        ReviewSheetsPatchRequestBody body,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Sheets.V1.Projects[projectId]
            .Uploads[uploadId]
            .ReviewSheets
            .PatchAsync(body, requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Publishes uploaded review sheets.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /construction/sheets/v1/projects/{projectId}/uploads/{uploadId}/review-sheets:publish
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/sheets-review-sheetspublish-POST
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="uploadId">The ID of the upload</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    /// <example>
    /// <code>
    /// await client.SheetsManager.PublishReviewSheetsAsync(projectId, uploadId);
    /// </code>
    /// </example>
    public async Task PublishReviewSheetsAsync(
        Guid projectId,
        string uploadId,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        await _api.Construction.Sheets.V1.Projects[projectId]
            .Uploads[uploadId]
            .ReviewSheetsPublish
            .PostAsync(requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Retrieves thumbnails for the specified review sheets.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /construction/sheets/v1/projects/{projectId}/uploads/{uploadId}/thumbnails:batch-get
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/sheets-thumbnailsbatch-get-POST
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="uploadId">The ID of the upload</param>
    /// <param name="body">The request body with sheet IDs</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="ThumbnailsBatchGetPostResponse"/> containing the thumbnails</returns>
    /// <example>
    /// <code>
    /// ThumbnailsBatchGetPostResponse? thumbnails = await client.SheetsManager.GetThumbnailsAsync(projectId, uploadId, new ThumbnailsBatchGetPostRequestBody());
    /// </code>
    /// </example>
    public async Task<ThumbnailsBatchGetPostResponse?> GetThumbnailsAsync(
        Guid projectId,
        string uploadId,
        ThumbnailsBatchGetPostRequestBody body,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Sheets.V1.Projects[projectId]
            .Uploads[uploadId]
            .ThumbnailsBatchGet
            .PostAsync(body, requestConfiguration, cancellationToken);
    }

    #endregion

    #region Sheets

    /// <summary>
    /// Retrieves all sheets in the project with automatic pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/sheets/v1/projects/{projectId}/sheets
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/sheets-sheets-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports collectionId, currentOnly, fields, filters, limit, offset, searchText, withAllTags)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{SheetsGetResponse_results}"/> of <see cref="SheetsGetResponse_results"/> items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (var sheet in client.SheetsManager.ListSheetsAsync(projectId)) { Console.WriteLine(sheet.Number); }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<SheetsGetResponse_results> ListSheetsAsync(
        Guid projectId,
        Action<RequestConfiguration<SheetsRequestBuilderGetQueryParameters>>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        int offset = 0;
        while (true)
        {
            var capturedOffset = offset;
            var response = await _api.Construction.Sheets.V1.Projects[projectId]
                .Sheets
                .GetAsync(config =>
                {
                    requestConfiguration?.Invoke(config);
                    config.QueryParameters.Offset = capturedOffset;
                }, cancellationToken);

            if (response?.Results is not { Count: > 0 })
                yield break;

            foreach (var item in response.Results)
                yield return item;

            if (string.IsNullOrEmpty(response.Pagination?.NextUrl))
                yield break;

            offset += response.Results.Count;
        }
    }

    /// <summary>
    /// Retrieves a page of sheets.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/sheets/v1/projects/{projectId}/sheets
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/sheets-sheets-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports collectionId, currentOnly, fields, filters, limit, offset, searchText, withAllTags)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="SheetsGetResponse"/> containing the sheets</returns>
    /// <example>
    /// <code>
    /// SheetsGetResponse? response = await client.SheetsManager.GetSheetsAsync(projectId);
    /// </code>
    /// </example>
    public async Task<SheetsGetResponse?> GetSheetsAsync(
        Guid projectId,
        Action<RequestConfiguration<SheetsRequestBuilderGetQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Sheets.V1.Projects[projectId]
            .Sheets
            .GetAsync(requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Retrieves sheets by IDs.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /construction/sheets/v1/projects/{projectId}/sheets:batch-get
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/sheets-sheetsbatch-get-POST
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="body">The request body containing sheet IDs</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="SheetsBatchGetPostResponse"/> containing the requested sheets</returns>
    /// <example>
    /// <code>
    /// SheetsBatchGetPostResponse? result = await client.SheetsManager.BatchGetSheetsAsync(projectId, new SheetsBatchGetPostRequestBody());
    /// </code>
    /// </example>
    public async Task<SheetsBatchGetPostResponse?> BatchGetSheetsAsync(
        Guid projectId,
        SheetsBatchGetPostRequestBody body,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Sheets.V1.Projects[projectId]
            .SheetsBatchGet
            .PostAsync(body, requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Updates sheets by IDs.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /construction/sheets/v1/projects/{projectId}/sheets:batch-update
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/sheets-sheetsbatch-update-POST
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="body">The update data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="SheetsBatchUpdatePostResponse"/> containing the batch update result</returns>
    /// <example>
    /// <code>
    /// SheetsBatchUpdatePostResponse? result = await client.SheetsManager.BatchUpdateSheetsAsync(projectId, new SheetsBatchUpdatePostRequestBody());
    /// </code>
    /// </example>
    public async Task<SheetsBatchUpdatePostResponse?> BatchUpdateSheetsAsync(
        Guid projectId,
        SheetsBatchUpdatePostRequestBody body,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Sheets.V1.Projects[projectId]
            .SheetsBatchUpdate
            .PostAsync(body, requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Deletes sheets by IDs.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /construction/sheets/v1/projects/{projectId}/sheets:batch-delete
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/sheets-sheetsbatch-delete-POST
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="body">The request body containing sheet IDs to delete</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    /// <example>
    /// <code>
    /// await client.SheetsManager.BatchDeleteSheetsAsync(projectId, new SheetsBatchDeletePostRequestBody());
    /// </code>
    /// </example>
    public async Task BatchDeleteSheetsAsync(
        Guid projectId,
        SheetsBatchDeletePostRequestBody body,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        await _api.Construction.Sheets.V1.Projects[projectId]
            .SheetsBatchDelete
            .PostAsync(body, requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Restores deleted sheets.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /construction/sheets/v1/projects/{projectId}/sheets:batch-restore
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/sheets-sheetsbatch-restore-POST
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="body">The request body containing sheet IDs to restore</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="SheetsBatchRestorePostResponse"/> containing the batch restore result</returns>
    /// <example>
    /// <code>
    /// SheetsBatchRestorePostResponse? result = await client.SheetsManager.BatchRestoreSheetsAsync(projectId, new SheetsBatchRestorePostRequestBody());
    /// </code>
    /// </example>
    public async Task<SheetsBatchRestorePostResponse?> BatchRestoreSheetsAsync(
        Guid projectId,
        SheetsBatchRestorePostRequestBody body,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Sheets.V1.Projects[projectId]
            .SheetsBatchRestore
            .PostAsync(body, requestConfiguration, cancellationToken);
    }

    #endregion

    #region Exports

    /// <summary>
    /// Exports up to 1000 sheets into a downloadable PDF.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /construction/sheets/v1/projects/{projectId}/exports
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/sheets-exports-POST
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="body">The export request</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="ExportsPostResponse"/> containing the export job</returns>
    /// <example>
    /// <code>
    /// ExportsPostResponse? export = await client.SheetsManager.CreateExportAsync(projectId, new ExportsPostRequestBody());
    /// </code>
    /// </example>
    public async Task<ExportsPostResponse?> CreateExportAsync(
        Guid projectId,
        ExportsPostRequestBody body,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Sheets.V1.Projects[projectId]
            .Exports
            .PostAsync(body, requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Retrieves the status and download URL of an export job.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/sheets/v1/projects/{projectId}/exports/{exportId}
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/sheets-exports-exportId-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="exportId">The ID of the export job</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="WithExportGetResponse"/> containing the export status</returns>
    /// <example>
    /// <code>
    /// WithExportGetResponse? status = await client.SheetsManager.GetExportAsync(projectId, exportId);
    /// </code>
    /// </example>
    public async Task<WithExportGetResponse?> GetExportAsync(
        Guid projectId,
        string exportId,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Sheets.V1.Projects[projectId]
            .Exports[exportId]
            .GetAsync(requestConfiguration, cancellationToken);
    }

    #endregion

    #region Collections

    /// <summary>
    /// Retrieves all collections in the project with automatic pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/sheets/v1/projects/{projectId}/collections
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/sheets-collections-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports limit, offset)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{CollectionsGetResponse_results}"/> of <see cref="CollectionsGetResponse_results"/> items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (var col in client.SheetsManager.ListCollectionsAsync(projectId)) { Console.WriteLine(col.Name); }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<CollectionsGetResponse_results> ListCollectionsAsync(
        Guid projectId,
        Action<RequestConfiguration<CollectionsRequestBuilderGetQueryParameters>>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        int offset = 0;
        while (true)
        {
            var capturedOffset = offset;
            var response = await _api.Construction.Sheets.V1.Projects[projectId]
                .Collections
                .GetAsync(config =>
                {
                    requestConfiguration?.Invoke(config);
                    config.QueryParameters.Offset = capturedOffset;
                }, cancellationToken);

            if (response?.Results is not { Count: > 0 })
                yield break;

            foreach (var item in response.Results)
                yield return item;

            if (string.IsNullOrEmpty(response.Pagination?.NextUrl))
                yield break;

            offset += response.Results.Count;
        }
    }

    /// <summary>
    /// Retrieves a page of collections.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/sheets/v1/projects/{projectId}/collections
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/sheets-collections-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports limit, offset)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="CollectionsGetResponse"/> containing the collections</returns>
    /// <example>
    /// <code>
    /// CollectionsGetResponse? response = await client.SheetsManager.GetCollectionsAsync(projectId);
    /// </code>
    /// </example>
    public async Task<CollectionsGetResponse?> GetCollectionsAsync(
        Guid projectId,
        Action<RequestConfiguration<CollectionsRequestBuilderGetQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Sheets.V1.Projects[projectId]
            .Collections
            .GetAsync(requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Retrieves a collection by ID.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/sheets/v1/projects/{projectId}/collections/{collectionId}
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/sheets-collections-collectionId-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="collectionId">The ID of the collection</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="WithCollectionGetResponse"/> containing the collection details</returns>
    /// <example>
    /// <code>
    /// WithCollectionGetResponse? collection = await client.SheetsManager.GetCollectionAsync(projectId, collectionId);
    /// </code>
    /// </example>
    public async Task<WithCollectionGetResponse?> GetCollectionAsync(
        Guid projectId,
        Guid collectionId,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Sheets.V1.Projects[projectId]
            .Collections[collectionId]
            .GetAsync(requestConfiguration, cancellationToken);
    }

    #endregion
}

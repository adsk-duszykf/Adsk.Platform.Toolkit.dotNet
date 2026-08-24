using Microsoft.Kiota.Abstractions;

namespace Autodesk.Datum.CLS360.Managers;

/// <summary>
/// Manager for file download operations.
/// </summary>
public class FileManager
{
    private readonly BaseDatumCLS360Client _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileManager"/> class.
    /// </summary>
    /// <param name="api">The API client.</param>
    public FileManager(BaseDatumCLS360Client api)
    {
        _api = api;
    }

    /// <summary>
    /// Downloads a file by file handle.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /files/{fileHdl}
    /// </remarks>
    /// <param name="fileHdl">File handle.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>A <see cref="string"/> containing the file response payload.</returns>
    /// <example>
    /// <code>
    /// string? content = await client.FileManager.GetFileByHandleAsync("file-handle");
    /// </code>
    /// </example>
    public async Task<string?> GetFileByHandleAsync(
        string fileHdl,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Files[fileHdl].GetAsync(r =>
        {
            r.Headers = requestConfiguration?.Headers ?? r.Headers;
            r.Options = requestConfiguration?.Options ?? r.Options;
        }, cancellationToken);
    }

    /// <summary>
    /// Downloads a file by dataset reference.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /files/dataset/{datasetReference}
    /// </remarks>
    /// <param name="datasetReference">Dataset reference.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>A <see cref="string"/> containing the file response payload.</returns>
    /// <example>
    /// <code>
    /// string? content = await client.FileManager.GetFileByDatasetReferenceAsync("dataset-reference");
    /// </code>
    /// </example>
    public async Task<string?> GetFileByDatasetReferenceAsync(
        string datasetReference,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Files.Dataset[datasetReference].GetAsync(r =>
        {
            r.Headers = requestConfiguration?.Headers ?? r.Headers;
            r.Options = requestConfiguration?.Options ?? r.Options;
        }, cancellationToken);
    }
}

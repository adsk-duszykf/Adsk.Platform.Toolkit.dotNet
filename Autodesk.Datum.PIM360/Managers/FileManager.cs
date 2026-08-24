using System.IO;
using Autodesk.Datum.PIM360.FileNamespace;
using Microsoft.Kiota.Abstractions;

namespace Autodesk.Datum.PIM360.Managers;

/// <summary>Manager for PIM360 file operations.</summary>
public sealed class FileManager : Pim360ManagerBase
{
    private readonly BaseDatumPIM360Client _api;

    /// <summary>Initializes a new instance of the <see cref="FileManager"/> class.</summary>
    /// <param name="api">The generated PIM360 API client.</param>
    public FileManager(BaseDatumPIM360Client api) => _api = api;

    /// <summary>Downloads a file by handle.</summary>
    /// <param name="handle">File handle.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>A <see cref="Stream"/> containing the file.</returns>
    public Task<Stream?> GetAsync(string handle, CancellationToken cancellationToken = default) =>
        _api.File[handle].GetAsync(null, cancellationToken);

    /// <summary>Uploads a file for use by an import activity.</summary>
    /// <param name="body">The multipart upload body.</param>
    /// <param name="requestConfiguration">(Optional) Request configuration.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>The generated uploaded-file response.</returns>
    public Task<FilePostResponse?> UploadAsync(
        MultipartBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default) =>
        _api.File.PostAsync(body, r =>
        {
            r.Headers = requestConfiguration?.Headers ?? r.Headers;
            r.Options = requestConfiguration?.Options ?? r.Options;
        }, cancellationToken);

    /// <summary>Provides direct access to generated file request builders.</summary>
    public FileRequestBuilder Api => _api.File;
}

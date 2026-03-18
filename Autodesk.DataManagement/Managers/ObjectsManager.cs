using System.Runtime.CompilerServices;
using Autodesk.DataManagement.OSS;
using Autodesk.DataManagement.OSS.Models;
using Autodesk.DataManagement.OSS.Oss.V2.Buckets.Item.Objects;
using Autodesk.DataManagement.OSS.Oss.V2.Buckets.Item.Objects.Batchcompleteupload;
using Autodesk.DataManagement.OSS.Oss.V2.Buckets.Item.Objects.Batchsigneds3download;
using Autodesk.DataManagement.OSS.Oss.V2.Buckets.Item.Objects.Batchsigneds3upload;
using Autodesk.DataManagement.OSS.Oss.V2.Buckets.Item.Objects.Item.Details;
using Autodesk.DataManagement.OSS.Oss.V2.Buckets.Item.Objects.Item.Signed;
using Autodesk.DataManagement.OSS.Oss.V2.Buckets.Item.Objects.Item.Signeds3download;
using Autodesk.DataManagement.OSS.Oss.V2.Buckets.Item.Objects.Item.Signeds3upload;
using Autodesk.DataManagement.OSS.Oss.V2.Signedresources.Item;
using Microsoft.Kiota.Abstractions;

namespace Autodesk.DataManagement.Managers;

/// <summary>
/// Manager for OSS Object operations
/// </summary>
public class ObjectsManager
{
    private readonly BaseOSSClient _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="ObjectsManager"/> class.
    /// </summary>
    /// <param name="api">The OSS API client</param>
    public ObjectsManager(BaseOSSClient api)
    {
        _api = api;
    }

    #region List Objects

    /// <summary>
    /// Lists all objects in a bucket with automatic pagination.
    /// </summary>
    /// <remarks>API: GET /oss/v2/buckets/{bucketKey}/objects</remarks>
    /// <param name="bucketKey">URL-encoded bucket key</param>
    /// <param name="requestConfiguration">Optional configuration for the request (limit, beginsWith)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>An async enumerable of object details across all pages</returns>
    public async IAsyncEnumerable<ObjectDetails> ListObjectsAsync(
        string bucketKey,
        Action<RequestConfiguration<ObjectsRequestBuilder.ObjectsRequestBuilderGetQueryParameters>>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? startAt = null;

        do
        {
            var capturedStartAt = startAt;
            var response = await _api.Oss.V2.Buckets[bucketKey].Objects
                .GetAsync(config =>
                {
                    requestConfiguration?.Invoke(config);
                    if (capturedStartAt != null)
                    {
                        config.QueryParameters.StartAt = capturedStartAt;
                    }
                }, cancellationToken);

            if (response?.Items != null)
            {
                foreach (var item in response.Items)
                {
                    yield return item;
                }

                startAt = response.Items.Count > 0
                    ? response.Items[^1].ObjectKey
                    : null;
            }
            else
            {
                startAt = null;
            }

        } while (!string.IsNullOrEmpty(startAt));
    }

    #endregion

    #region Object Details

    /// <summary>
    /// Returns object details in JSON format.
    /// </summary>
    /// <remarks>API: GET /oss/v2/buckets/{bucketKey}/objects/{objectKey}/details</remarks>
    /// <param name="bucketKey">URL-encoded bucket key</param>
    /// <param name="objectKey">URL-encoded object name</param>
    /// <param name="requestConfiguration">Optional configuration for the request (with extra info)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Full object details</returns>
    public async Task<ObjectFullDetails?> GetObjectDetailsAsync(
        string bucketKey,
        string objectKey,
        Action<RequestConfiguration<DetailsRequestBuilder.DetailsRequestBuilderGetQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Oss.V2.Buckets[bucketKey].Objects[objectKey].Details
            .GetAsync(requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Checks if an object exists (HEAD request). Returns a 200 if the object exists, 404 otherwise.
    /// </summary>
    /// <remarks>API: HEAD /oss/v2/buckets/{bucketKey}/objects/{objectKey}/details</remarks>
    /// <param name="bucketKey">URL-encoded bucket key</param>
    /// <param name="objectKey">URL-encoded object name</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public async Task ObjectExistsAsync(
        string bucketKey,
        string objectKey,
        Action<RequestConfiguration<DetailsRequestBuilder.DetailsRequestBuilderHeadQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        await _api.Oss.V2.Buckets[bucketKey].Objects[objectKey].Details
            .HeadAsync(requestConfiguration, cancellationToken);
    }

    #endregion

    #region Delete & Copy

    /// <summary>
    /// Deletes an object from the bucket.
    /// </summary>
    /// <remarks>API: DELETE /oss/v2/buckets/{bucketKey}/objects/{objectKey}</remarks>
    /// <param name="bucketKey">URL-encoded bucket key</param>
    /// <param name="objectKey">URL-encoded object name</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public async Task DeleteObjectAsync(
        string bucketKey,
        string objectKey,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        await _api.Oss.V2.Buckets[bucketKey].Objects[objectKey]
            .DeleteAsync(requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Copies an object to another object name in the same bucket.
    /// </summary>
    /// <remarks>API: PUT /oss/v2/buckets/{bucketKey}/objects/{objectKey}/copyto/{newObjName}</remarks>
    /// <param name="bucketKey">URL-encoded bucket key</param>
    /// <param name="objectKey">URL-encoded source object name</param>
    /// <param name="newObjectName">URL-encoded destination object name</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Details of the copied object</returns>
    public async Task<ObjectDetails?> CopyObjectAsync(
        string bucketKey,
        string objectKey,
        string newObjectName,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Oss.V2.Buckets[bucketKey].Objects[objectKey].Copyto[newObjectName]
            .PutAsync(requestConfiguration, cancellationToken);
    }

    #endregion

    #region Signed URLs (Legacy)

    /// <summary>
    /// Creates a signed URL that can be used to download or upload an object within the specified expiration time.
    /// Requires bucket owner access.
    /// </summary>
    /// <remarks>API: POST /oss/v2/buckets/{bucketKey}/objects/{objectKey}/signed</remarks>
    /// <param name="bucketKey">URL-encoded bucket key</param>
    /// <param name="objectKey">URL-encoded object name</param>
    /// <param name="body">Signed resource creation payload (e.g. minutesExpiration)</param>
    /// <param name="requestConfiguration">Optional configuration for the request (access, useCdn)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created signed URL details</returns>
    public async Task<Create_object_signed?> CreateSignedUrlAsync(
        string bucketKey,
        string objectKey,
        Create_signed_resource body,
        Action<RequestConfiguration<SignedRequestBuilder.SignedRequestBuilderPostQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Oss.V2.Buckets[bucketKey].Objects[objectKey].Signed
            .PostAsync(body, requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Downloads an object using a signed URL hash.
    /// </summary>
    /// <remarks>API: GET /oss/v2/signedresources/{hash}</remarks>
    /// <param name="hash">Hash of the signed resource</param>
    /// <param name="requestConfiguration">Optional configuration for the request (region, content type/disposition)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Stream of the downloaded object</returns>
    public async Task<Stream?> DownloadSignedResourceAsync(
        string hash,
        Action<RequestConfiguration<WithHashItemRequestBuilder.WithHashItemRequestBuilderGetQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Oss.V2.Signedresources[hash]
            .GetAsync(requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Deletes a signed URL. Requires bucket owner access.
    /// </summary>
    /// <remarks>API: DELETE /oss/v2/signedresources/{hash}</remarks>
    /// <param name="hash">Hash of the signed resource</param>
    /// <param name="requestConfiguration">Optional configuration for the request (region)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public async Task DeleteSignedResourceAsync(
        string hash,
        Action<RequestConfiguration<WithHashItemRequestBuilder.WithHashItemRequestBuilderDeleteQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        await _api.Oss.V2.Signedresources[hash]
            .DeleteAsync(requestConfiguration, cancellationToken);
    }

    #endregion

    #region S3 Signed URLs

    /// <summary>
    /// Gets a signed URL to download an object directly from S3, bypassing OSS servers.
    /// The signed URL expires in 60 seconds.
    /// </summary>
    /// <remarks>API: GET /oss/v2/buckets/{bucketKey}/objects/{objectKey}/signeds3download</remarks>
    /// <param name="bucketKey">URL-encoded bucket key</param>
    /// <param name="objectKey">URL-encoded object name</param>
    /// <param name="requestConfiguration">Optional configuration (minutesExpiration, publicResourceFallback, useCdn, etc.)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The signed S3 download response with URL(s)</returns>
    public async Task<Signeds3download_response?> GetS3SignedDownloadUrlAsync(
        string bucketKey,
        string objectKey,
        Action<RequestConfiguration<Signeds3downloadRequestBuilder.Signeds3downloadRequestBuilderGetQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Oss.V2.Buckets[bucketKey].Objects[objectKey].Signeds3download
            .GetAsync(requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Gets signed URL(s) to upload an object directly to S3. For multipart uploads, returns an array of URLs.
    /// </summary>
    /// <remarks>API: GET /oss/v2/buckets/{bucketKey}/objects/{objectKey}/signeds3upload</remarks>
    /// <param name="bucketKey">URL-encoded bucket key</param>
    /// <param name="objectKey">URL-encoded object name</param>
    /// <param name="requestConfiguration">Optional configuration (parts, firstPart, uploadKey, minutesExpiration)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The signed S3 upload response with URL(s) and upload key</returns>
    public async Task<Signeds3upload_response?> GetS3SignedUploadUrlAsync(
        string bucketKey,
        string objectKey,
        Action<RequestConfiguration<Signeds3uploadRequestBuilder.Signeds3uploadRequestBuilderGetQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Oss.V2.Buckets[bucketKey].Objects[objectKey].Signeds3upload
            .GetAsync(requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Instructs OSS to complete the object creation process after bytes have been uploaded directly to S3.
    /// </summary>
    /// <remarks>API: POST /oss/v2/buckets/{bucketKey}/objects/{objectKey}/signeds3upload</remarks>
    /// <param name="bucketKey">URL-encoded bucket key</param>
    /// <param name="objectKey">URL-encoded object name</param>
    /// <param name="body">The completion payload (uploadKey)</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The completion response</returns>
    public async Task<Completes3upload_response_200?> CompleteS3UploadAsync(
        string bucketKey,
        string objectKey,
        Completes3upload_body body,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Oss.V2.Buckets[bucketKey].Objects[objectKey].Signeds3upload
            .PostAsync(body, requestConfiguration, cancellationToken);
    }

    #endregion

    #region Batch S3 Operations

    /// <summary>
    /// Gets signed URLs to download multiple objects directly from S3 in a single request (max 25).
    /// The signed URLs expire in 60 seconds.
    /// </summary>
    /// <remarks>API: POST /oss/v2/buckets/{bucketKey}/objects/batchsigneds3download</remarks>
    /// <param name="bucketKey">URL-encoded bucket key</param>
    /// <param name="body">The batch download request body with object keys</param>
    /// <param name="requestConfiguration">Optional configuration (minutesExpiration, publicResourceFallback)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Batch signed download response with results per object</returns>
    public async Task<Batchsigneds3download_response?> BatchGetS3SignedDownloadUrlsAsync(
        string bucketKey,
        Batchsigneds3download_object body,
        Action<RequestConfiguration<Batchsigneds3downloadRequestBuilder.Batchsigneds3downloadRequestBuilderPostQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Oss.V2.Buckets[bucketKey].Objects.Batchsigneds3download
            .PostAsync(body, requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Requests a batch of S3 signed URLs to upload multiple objects or chunks (max 25 per request).
    /// </summary>
    /// <remarks>API: POST /oss/v2/buckets/{bucketKey}/objects/batchsigneds3upload</remarks>
    /// <param name="bucketKey">URL-encoded bucket key</param>
    /// <param name="body">The batch upload request body with object details</param>
    /// <param name="requestConfiguration">Optional configuration (minutesExpiration, useAcceleration)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Batch signed upload response with results per object</returns>
    public async Task<Batchsigneds3upload_response?> BatchGetS3SignedUploadUrlsAsync(
        string bucketKey,
        Batchsigneds3upload_object body,
        Action<RequestConfiguration<Batchsigneds3uploadRequestBuilder.Batchsigneds3uploadRequestBuilderPostQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Oss.V2.Buckets[bucketKey].Objects.Batchsigneds3upload
            .PostAsync(body, requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Instructs OSS to complete the object creation process for multiple objects (max 25) after bytes have been uploaded to S3.
    /// </summary>
    /// <remarks>API: POST /oss/v2/buckets/{bucketKey}/objects/batchcompleteupload</remarks>
    /// <param name="bucketKey">URL-encoded bucket key</param>
    /// <param name="body">The batch completion payload</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Batch completion response with results per object</returns>
    public async Task<Batchcompleteupload_response?> BatchCompleteUploadAsync(
        string bucketKey,
        Batchcompleteupload_object body,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Oss.V2.Buckets[bucketKey].Objects.Batchcompleteupload
            .PostAsync(body, requestConfiguration, cancellationToken);
    }

    #endregion
}

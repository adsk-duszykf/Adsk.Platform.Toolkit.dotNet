using System.Runtime.CompilerServices;
using Autodesk.DataManagement.OSS;
using Autodesk.DataManagement.OSS.Models;
using Autodesk.DataManagement.OSS.Oss.V2.Buckets;
using Microsoft.Kiota.Abstractions;

namespace Autodesk.DataManagement.Managers;

/// <summary>
/// Manager for OSS Bucket operations
/// </summary>
public class BucketsManager
{
    private readonly BaseOSSClient _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="BucketsManager"/> class.
    /// </summary>
    /// <param name="api">The OSS API client</param>
    public BucketsManager(BaseOSSClient api)
    {
        _api = api;
    }

    /// <summary>
    /// Lists all buckets owned by the application with automatic pagination.
    /// </summary>
    /// <remarks>API: GET /oss/v2/buckets</remarks>
    /// <param name="requestConfiguration">Optional configuration for the request (region, limit)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>An async enumerable of bucket items across all pages</returns>
    public async IAsyncEnumerable<Buckets_items> ListBucketsAsync(
        Action<RequestConfiguration<BucketsRequestBuilder.BucketsRequestBuilderGetQueryParameters>>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? startAt = null;

        do
        {
            var capturedStartAt = startAt;
            var response = await _api.Oss.V2.Buckets
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
                    ? response.Items[^1].BucketKey
                    : null;
            }
            else
            {
                startAt = null;
            }

        } while (!string.IsNullOrEmpty(startAt));
    }

    /// <summary>
    /// Creates a new bucket. Bucket keys are globally unique across all regions and cannot be changed.
    /// </summary>
    /// <remarks>API: POST /oss/v2/buckets</remarks>
    /// <param name="payload">The bucket creation payload (bucket key and retention policy)</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created bucket details</returns>
    public async Task<Bucket?> CreateBucketAsync(
        Create_buckets_payload payload,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Oss.V2.Buckets
            .PostAsync(payload, requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Returns details about the specified bucket.
    /// </summary>
    /// <remarks>API: GET /oss/v2/buckets/{bucketKey}/details</remarks>
    /// <param name="bucketKey">URL-encoded bucket key</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The bucket details</returns>
    public async Task<Bucket?> GetBucketDetailsAsync(
        string bucketKey,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Oss.V2.Buckets[bucketKey].Details
            .GetAsync(requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Deletes the bucket with the specified key.
    /// </summary>
    /// <remarks>API: DELETE /oss/v2/buckets/{bucketKey}</remarks>
    /// <param name="bucketKey">URL-encoded bucket key</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public async Task DeleteBucketAsync(
        string bucketKey,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        await _api.Oss.V2.Buckets[bucketKey]
            .DeleteAsync(requestConfiguration, cancellationToken);
    }
}

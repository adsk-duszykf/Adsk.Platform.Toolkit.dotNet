using Autodesk.ACC.Bim360.Relationship.V2.Containers.Item.Relationships;
using Autodesk.ACC.Bim360.Relationship.V2.Containers.Item.Relationships.Item;
using Autodesk.ACC.Bim360.Relationship.V2.Containers.Item.RelationshipsBatch;
using Autodesk.ACC.Bim360.Relationship.V2.Containers.Item.RelationshipsDelete;
using Autodesk.ACC.Bim360.Relationship.V2.Containers.Item.RelationshipsIntersect;
using Autodesk.ACC.Bim360.Relationship.V2.Containers.Item.RelationshipsSearch;
using Autodesk.ACC.Bim360.Relationship.V2.Containers.Item.RelationshipsSync;
using Autodesk.ACC.Bim360.Relationship.V2.Containers.Item.RelationshipsSyncStatus;
using Autodesk.ACC.Bim360.Relationship.V2.Utility.RelationshipsWritable;
using Microsoft.Kiota.Abstractions;

namespace Autodesk.ACC.Managers;

/// <summary>
/// Manager for Relationships operations — creates, searches, and manages relationships
/// between entities (e.g., assets and documents) in ACC.
/// </summary>
public class RelationshipsManager
{
    private readonly BaseACCclient _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="RelationshipsManager"/> class.
    /// </summary>
    /// <param name="api">The API client</param>
    public RelationshipsManager(BaseACCclient api)
    {
        _api = api;
    }

    /// <summary>
    /// Retrieves a list of entity types that are compatible with each other,
    /// to establish whether you can create relationships between them or delete those relationships.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /bim360/relationship/v2/utility/relationships:writable
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/relationship-service-v2-get-writable-relationship-domains-GET
    /// </remarks>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="RelationshipsWritableGetResponse"/> containing the writable relationship types</returns>
    /// <example>
    /// <code>
    /// RelationshipsWritableGetResponse? writable = await client.RelationshipsManager.GetWritableRelationshipTypesAsync();
    /// </code>
    /// </example>
    public async Task<RelationshipsWritableGetResponse?> GetWritableRelationshipTypesAsync(
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Relationship.V2.Utility.RelationshipsWritable
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Creates a relationship between two entities (for example, an asset and a document).
    /// </summary>
    /// <remarks>
    /// Wraps: PUT /bim360/relationship/v2/containers/{containerId}/relationships
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/relationship-service-v2-add-relationships-PUT
    /// </remarks>
    /// <param name="containerId">The container ID (typically the project ID without 'b.' prefix)</param>
    /// <param name="body">The relationship creation data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="RelationshipsPutResponse"/> containing the created relationship</returns>
    /// <example>
    /// <code>
    /// RelationshipsPutResponse? relationship = await client.RelationshipsManager.CreateRelationshipAsync(containerId, new RelationshipsPutRequestBody());
    /// </code>
    /// </example>
    public async Task<RelationshipsPutResponse?> CreateRelationshipAsync(
        Guid containerId,
        RelationshipsPutRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Relationship.V2.Containers[containerId]
            .Relationships
            .PutAsync(body, r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Retrieves a relationship by ID.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /bim360/relationship/v2/containers/{containerId}/relationships/{relationshipId}
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/relationship-service-v2-get-relationship-by-id-GET
    /// </remarks>
    /// <param name="containerId">The container ID</param>
    /// <param name="relationshipId">The relationship ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="WithRelationshipGetResponse"/> containing the relationship details</returns>
    /// <example>
    /// <code>
    /// WithRelationshipGetResponse? relationship = await client.RelationshipsManager.GetRelationshipAsync(containerId, "relationshipId");
    /// </code>
    /// </example>
    public async Task<WithRelationshipGetResponse?> GetRelationshipAsync(
        Guid containerId,
        Guid relationshipId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Relationship.V2.Containers[containerId]
            .Relationships[relationshipId]
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Deletes one or more relationships by passing an array of relationship UUIDs.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /bim360/relationship/v2/containers/{containerId}/relationships:delete
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/relationship-service-v2-delete-relationships-POST
    /// </remarks>
    /// <param name="containerId">The container ID</param>
    /// <param name="body">The delete request body containing relationship IDs</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="RelationshipsDeletePostResponse"/> containing the delete result</returns>
    /// <example>
    /// <code>
    /// RelationshipsDeletePostResponse? result = await client.RelationshipsManager.DeleteRelationshipsAsync(containerId, new RelationshipsDeletePostRequestBody());
    /// </code>
    /// </example>
    public async Task<RelationshipsDeletePostResponse?> DeleteRelationshipsAsync(
        Guid containerId,
        RelationshipsDeletePostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Relationship.V2.Containers[containerId]
            .RelationshipsDelete
            .PostAsync(body, r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Retrieves a list of relationships that match the provided search parameters.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /bim360/relationship/v2/containers/{containerId}/relationships:search
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/relationship-service-v2-search-relationships-GET
    /// </remarks>
    /// <param name="containerId">The container ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="RelationshipsSearchGetResponse"/> containing the search results</returns>
    /// <example>
    /// <code>
    /// RelationshipsSearchGetResponse? results = await client.RelationshipsManager.SearchRelationshipsAsync(containerId);
    /// </code>
    /// </example>
    public async Task<RelationshipsSearchGetResponse?> SearchRelationshipsAsync(
        Guid containerId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Relationship.V2.Containers[containerId]
            .RelationshipsSearch
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Retrieves a list of one or more relationships by passing an array of relationship IDs.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /bim360/relationship/v2/containers/{containerId}/relationships:batch
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/relationship-service-v2-get-relationships-batch-POST
    /// </remarks>
    /// <param name="containerId">The container ID</param>
    /// <param name="body">The batch request body containing relationship IDs</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="RelationshipsBatchPostResponse"/> containing the batch response with relationship details</returns>
    /// <example>
    /// <code>
    /// RelationshipsBatchPostResponse? batch = await client.RelationshipsManager.BatchGetRelationshipsAsync(containerId, new RelationshipsBatchPostRequestBody());
    /// </code>
    /// </example>
    public async Task<RelationshipsBatchPostResponse?> BatchGetRelationshipsAsync(
        Guid containerId,
        RelationshipsBatchPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Relationship.V2.Containers[containerId]
            .RelationshipsBatch
            .PostAsync(body, r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Retrieves a list of relationships that contain the specified relationship entities.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /bim360/relationship/v2/containers/{containerId}/relationships:intersect
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/relationship-service-v2-intersect-relationships-POST
    /// </remarks>
    /// <param name="containerId">The container ID</param>
    /// <param name="body">The intersect request body with entity references</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="RelationshipsIntersectPostResponse"/> containing the intersect results</returns>
    /// <example>
    /// <code>
    /// RelationshipsIntersectPostResponse? intersect = await client.RelationshipsManager.IntersectRelationshipsAsync(containerId, new RelationshipsIntersectPostRequestBody());
    /// </code>
    /// </example>
    public async Task<RelationshipsIntersectPostResponse?> IntersectRelationshipsAsync(
        Guid containerId,
        RelationshipsIntersectPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Relationship.V2.Containers[containerId]
            .RelationshipsIntersect
            .PostAsync(body, r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Retrieves the relationship synchronization status for the caller.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /bim360/relationship/v2/containers/{containerId}/relationships:syncStatus
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/relationship-service-v2-relationships-sync-status-POST
    /// </remarks>
    /// <param name="containerId">The container ID</param>
    /// <param name="body">The sync status request body</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="RelationshipsSyncStatusPostResponse"/> containing the sync status</returns>
    /// <example>
    /// <code>
    /// RelationshipsSyncStatusPostResponse? status = await client.RelationshipsManager.GetSyncStatusAsync(containerId, new RelationshipsSyncStatusPostRequestBody());
    /// </code>
    /// </example>
    public async Task<RelationshipsSyncStatusPostResponse?> GetSyncStatusAsync(
        Guid containerId,
        RelationshipsSyncStatusPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Relationship.V2.Containers[containerId]
            .RelationshipsSyncStatus
            .PostAsync(body, r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Synchronizes relationships using the optional synchronization token passed by the caller.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /bim360/relationship/v2/containers/{containerId}/relationships:sync
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/relationship-service-v2-relationships-sync-POST
    /// </remarks>
    /// <param name="containerId">The container ID</param>
    /// <param name="body">The sync request body with optional sync token</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="RelationshipsSyncPostResponse"/> containing the sync result</returns>
    /// <example>
    /// <code>
    /// RelationshipsSyncPostResponse? syncResult = await client.RelationshipsManager.SyncRelationshipsAsync(containerId, new RelationshipsSyncPostRequestBody());
    /// </code>
    /// </example>
    public async Task<RelationshipsSyncPostResponse?> SyncRelationshipsAsync(
        Guid containerId,
        RelationshipsSyncPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Relationship.V2.Containers[containerId]
            .RelationshipsSync
            .PostAsync(body, r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }
}

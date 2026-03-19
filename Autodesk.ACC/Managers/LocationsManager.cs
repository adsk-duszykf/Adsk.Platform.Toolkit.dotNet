using Autodesk.ACC.Construction.Locations.V2.Projects.Item.Trees.Item.Nodes;
using Autodesk.ACC.Construction.Locations.V2.Projects.Item.Trees.Item.Nodes.Item;
using Microsoft.Kiota.Abstractions;
using static Autodesk.ACC.Construction.Locations.V2.Projects.Item.Trees.Item.Nodes.NodesRequestBuilder;

namespace Autodesk.ACC.Managers;

/// <summary>
/// Manager for Locations operations — manages location nodes in the Location Breakdown Structure (LBS)
/// for ACC projects.
/// </summary>
public class LocationsManager
{
    private readonly BaseACCclient _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="LocationsManager"/> class.
    /// </summary>
    /// <param name="api">The API client</param>
    public LocationsManager(BaseACCclient api)
    {
        _api = api;
    }

    /// <summary>
    /// Retrieves an array of location nodes from the specified locations tree (LBS).
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/locations/v2/projects/{projectId}/trees/{treeId}/nodes
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/locations-nodes-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="treeId">The ID of the locations tree (use "default" for the default tree)</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports filter[id], Limit, Offset query parameters)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="NodesGetResponse"/> containing the nodes response</returns>
    /// <example>
    /// <code>
    /// NodesGetResponse? nodes = await client.LocationsManager.GetNodesAsync(projectId, "default");
    /// </code>
    /// </example>
    public async Task<NodesGetResponse?> GetNodesAsync(
        Guid projectId,
        string treeId,
        Action<RequestConfiguration<NodesRequestBuilderGetQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Locations.V2.Projects[projectId]
            .Trees[treeId]
            .Nodes
            .GetAsync(requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Creates a node in the specified locations tree.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /construction/locations/v2/projects/{projectId}/trees/{treeId}/nodes
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/locations-nodes-POST
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="treeId">The ID of the locations tree (use "default" for the default tree)</param>
    /// <param name="body">The node creation data (Name, ParentId, Type, Barcode, Description)</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports InsertOption, TargetNodeId query parameters)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="NodesPostResponse"/> containing the created node</returns>
    /// <example>
    /// <code>
    /// NodesPostResponse? node = await client.LocationsManager.CreateNodeAsync(projectId, "default", new NodesPostRequestBody
    /// {
    ///     Name = "Floor 1",
    ///     ParentId = parentNodeId
    /// });
    /// </code>
    /// </example>
    public async Task<NodesPostResponse?> CreateNodeAsync(
        Guid projectId,
        string treeId,
        NodesPostRequestBody body,
        Action<RequestConfiguration<NodesRequestBuilderPostQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Locations.V2.Projects[projectId]
            .Trees[treeId]
            .Nodes
            .PostAsync(body, requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Updates the name or barcode of the specified node in the specified locations tree.
    /// </summary>
    /// <remarks>
    /// Wraps: PATCH /construction/locations/v2/projects/{projectId}/trees/{treeId}/nodes/{nodeId}
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/locations-nodesnodeid-PATCH
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="treeId">The ID of the locations tree (use "default" for the default tree)</param>
    /// <param name="nodeId">The ID of the node to update</param>
    /// <param name="body">The update data (Name, Barcode)</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="WithNodePatchResponse"/> containing the updated node</returns>
    /// <example>
    /// <code>
    /// WithNodePatchResponse? updated = await client.LocationsManager.UpdateNodeAsync(projectId, "default", nodeId, new WithNodePatchRequestBody
    /// {
    ///     Name = "Floor 1 - Renamed"
    /// });
    /// </code>
    /// </example>
    public async Task<WithNodePatchResponse?> UpdateNodeAsync(
        Guid projectId,
        string treeId,
        Guid nodeId,
        WithNodePatchRequestBody body,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Locations.V2.Projects[projectId]
            .Trees[treeId]
            .Nodes[nodeId]
            .PatchAsync(body, requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Deletes the specified node from the specified locations tree.
    /// </summary>
    /// <remarks>
    /// Wraps: DELETE /construction/locations/v2/projects/{projectId}/trees/{treeId}/nodes/{nodeId}
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/locations-nodesnodeid-DELETE
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="treeId">The ID of the locations tree (use "default" for the default tree)</param>
    /// <param name="nodeId">The ID of the node to delete</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    /// <example>
    /// <code>
    /// await client.LocationsManager.DeleteNodeAsync(projectId, "default", nodeId);
    /// </code>
    /// </example>
    public async Task DeleteNodeAsync(
        Guid projectId,
        string treeId,
        Guid nodeId,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        await _api.Construction.Locations.V2.Projects[projectId]
            .Trees[treeId]
            .Nodes[nodeId]
            .DeleteAsync(requestConfiguration, cancellationToken);
    }
}

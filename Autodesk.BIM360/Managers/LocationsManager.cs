using System.Runtime.CompilerServices;
using Autodesk.BIM360.Bim360.Locations.V2.Containers.Item.Trees.Item.Nodes;
using Microsoft.Kiota.Abstractions;
using static Autodesk.BIM360.Bim360.Locations.V2.Containers.Item.Trees.Item.Nodes.NodesRequestBuilder;

namespace Autodesk.BIM360.Managers;

/// <summary>
/// Manager for BIM 360 Locations — reads location nodes from the Location Breakdown Structure (LBS)
/// for a project container. The generated BIM 360 client exposes GET <c>nodes</c> only for this API surface.
/// </summary>
public class LocationsManager
{
    private readonly BaseBIM360client _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="LocationsManager"/> class.
    /// </summary>
    /// <param name="api">The API client</param>
    public LocationsManager(BaseBIM360client api)
    {
        _api = api;
    }

    /// <summary>
    /// Retrieves location nodes from the specified locations tree (LBS).
    /// </summary>
    /// <remarks>
    /// Wraps: GET /bim360/locations/v2/containers/{containerId}/trees/{treeId}/nodes
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/locations-nodes-GET
    /// </remarks>
    /// <param name="containerId">The locations container ID for the project</param>
    /// <param name="treeId">The ID of the locations tree (use <c>default</c> for the default tree)</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports filter[id], Limit, Offset query parameters)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="NodesGetResponse"/> containing the nodes response</returns>
    /// <example>
    /// <code>
    /// Guid containerId = Guid.Parse("a4be0c34-a4ab-4ab7-9a4b-1234567890ab");
    /// NodesGetResponse? nodes = await client.LocationsManager.GetNodesAsync(containerId, "default");
    /// </code>
    /// </example>
    public async Task<NodesGetResponse?> GetNodesAsync(
        Guid containerId,
        string treeId,
        RequestConfiguration<NodesRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Locations.V2.Containers[containerId]
            .Trees[treeId]
            .Nodes
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Enumerates all location nodes for a tree, following <see cref="NodesGetResponse_pagination.NextUrl"/> until exhausted.
    /// </summary>
    /// <remarks>
    /// Wraps repeated GET /bim360/locations/v2/containers/{containerId}/trees/{treeId}/nodes with advancing <c>offset</c>.
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/locations-nodes-GET
    /// </remarks>
    /// <param name="containerId">The locations container ID for the project</param>
    /// <param name="treeId">The ID of the locations tree (use <c>default</c> for the default tree)</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (Limit and filter[id] are honored; Offset is advanced automatically)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{NodesGetResponse_results}"/> of <see cref="NodesGetResponse_results"/> items</returns>
    /// <example>
    /// <code>
    /// Guid containerId = Guid.Parse("a4be0c34-a4ab-4ab7-9a4b-1234567890ab");
    /// await foreach (NodesGetResponse_results node in client.LocationsManager.ListNodesAsync(containerId, "default"))
    /// {
    ///     Console.WriteLine(node.Id);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<NodesGetResponse_results> ListNodesAsync(
        Guid containerId,
        string treeId,
        RequestConfiguration<NodesRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        int offset = requestConfiguration?.QueryParameters?.Offset ?? 0;

        while (true)
        {
            var response = await _api.Bim360.Locations.V2.Containers[containerId]
                .Trees[treeId]
                .Nodes
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
                yield return item;

            if (string.IsNullOrEmpty(response.Pagination?.NextUrl))
                yield break;

            offset += response.Results.Count;
        }
    }
}

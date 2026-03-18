using Autodesk.DataManagement.Models;
using Autodesk.DataManagement.Project.V1.Hubs;
using Microsoft.Kiota.Abstractions;

namespace Autodesk.DataManagement.Managers;

/// <summary>
/// Manager for Hub operations
/// </summary>
public class HubsManager
{
    private readonly BaseDataManagementClient _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="HubsManager"/> class.
    /// </summary>
    /// <param name="api">The Data Management API client</param>
    public HubsManager(BaseDataManagementClient api)
    {
        _api = api;
    }

    /// <summary>
    /// Returns a collection of accessible hubs for this member.
    /// Hubs represent BIM 360 Team hubs, Fusion Team hubs, A360 Personal hubs, or BIM 360 Docs accounts.
    /// </summary>
    /// <remarks>API: GET /project/v1/hubs</remarks>
    /// <param name="requestConfiguration">Optional configuration for the request (filter by id, extension type, name)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The hubs collection</returns>
    /// <example>
    /// <code>
    /// var hubs = await client.Hubs.GetHubsAsync();
    /// foreach (var hub in hubs?.Data ?? Enumerable.Empty&lt;Hubs_data&gt;())
    /// {
    ///     Console.WriteLine($"{hub.Id} - {hub.Attributes?.Name}");
    /// }
    /// </code>
    /// </example>
    public async Task<Hubs?> GetHubsAsync(
        Action<RequestConfiguration<HubsRequestBuilder.HubsRequestBuilderGetQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Project.V1.Hubs
            .GetAsync(requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Returns data on a specific hub.
    /// For BIM 360 Docs, a hub ID corresponds to an account ID with a "b." prefix.
    /// </summary>
    /// <remarks>API: GET /project/v1/hubs/{hub_id}</remarks>
    /// <param name="hubId">The unique identifier of a hub</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The hub details</returns>
    /// <example>
    /// <code>
    /// var hub = await client.Hubs.GetHubAsync("b.my-account-id");
    /// Console.WriteLine($"Hub: {hub?.Data?.Attributes?.Name}");
    /// </code>
    /// </example>
    public async Task<Hub?> GetHubAsync(
        string hubId,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Project.V1.Hubs[hubId]
            .GetAsync(requestConfiguration, cancellationToken);
    }
}

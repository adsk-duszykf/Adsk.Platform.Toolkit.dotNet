using Autodesk.Tandem.Tandem.V1.Groups.Item.Twins;
using Autodesk.Tandem.Tandem.V1.Twins.Item;
using Autodesk.Tandem.Tandem.V1.Twins.Item.Defaultmodel;
using Autodesk.Tandem.Tandem.V1.Twins.Item.History;
using Autodesk.Tandem.Tandem.V1.Twins.Item.Users;
using Microsoft.Kiota.Abstractions;

namespace Autodesk.Tandem.Managers;

/// <summary>
/// Manager for Twins operations
/// </summary>
public class TwinsManager
{
    private readonly BaseTandemClient _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="TwinsManager"/> class.
    /// </summary>
    /// <param name="api">The API client</param>
    public TwinsManager(BaseTandemClient api)
    {
        _api = api;
    }

    /// <summary>
    /// Returns all twins for the given group.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /tandem/v1/groups/{groupID}/twins
    /// </remarks>
    /// <param name="groupId">Group URN</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="TwinsGetResponse"/> containing all twins for the group</returns>
    /// <example>
    /// <code>
    /// TwinsGetResponse? twins = await client.TwinsManager.GetTwinsByGroupAsync("urn:adsk.dtg:...");
    /// </code>
    /// </example>
    public async Task<TwinsGetResponse?> GetTwinsByGroupAsync(
        string groupId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Tandem.V1.Groups[groupId].Twins
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Creates a new twin according to the given definition.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /tandem/v1/groups/{groupID}/twins
    /// </remarks>
    /// <param name="groupId">Group URN</param>
    /// <param name="body">The twin creation request body</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="TwinsPostResponse"/> containing the URN and timestamp of the created twin</returns>
    /// <example>
    /// <code>
    /// TwinsPostResponse? result = await client.TwinsManager.CreateTwinAsync("urn:adsk.dtg:...", new TwinsPostRequestBody());
    /// </code>
    /// </example>
    public async Task<TwinsPostResponse?> CreateTwinAsync(
        string groupId,
        TwinsPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Tandem.V1.Groups[groupId].Twins
            .PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Returns the twin definition.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /tandem/v1/twins/{twinID}
    /// </remarks>
    /// <param name="twinId">Twin URN</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="WithTwinGetResponse"/> containing the twin definition</returns>
    /// <example>
    /// <code>
    /// WithTwinGetResponse? twin = await client.TwinsManager.GetTwinAsync("urn:adsk.dtt:...");
    /// </code>
    /// </example>
    public async Task<WithTwinGetResponse?> GetTwinAsync(
        string twinId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Tandem.V1.Twins[twinId]
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Creates a default model for the facility. The default model hosts streams, user-initiated elements, and custom geometry creation.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /tandem/v1/twins/{twinID}/defaultmodel
    /// </remarks>
    /// <param name="twinId">Twin URN</param>
    /// <param name="body">The request body</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="DefaultmodelPostResponse"/> containing the model ID and timestamp</returns>
    /// <example>
    /// <code>
    /// DefaultmodelPostResponse? result = await client.TwinsManager.CreateDefaultModelAsync("urn:adsk.dtt:...", new DefaultmodelPostRequestBody());
    /// </code>
    /// </example>
    public async Task<DefaultmodelPostResponse?> CreateDefaultModelAsync(
        string twinId,
        DefaultmodelPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Tandem.V1.Twins[twinId].Defaultmodel
            .PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Returns history of all changes for a given twin.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /tandem/v1/twins/{twinID}/history
    /// </remarks>
    /// <param name="twinId">Twin URN</param>
    /// <param name="body">The history query request body</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="HistoryPostResponse"/> containing the change history</returns>
    /// <example>
    /// <code>
    /// HistoryPostResponse? history = await client.TwinsManager.GetTwinHistoryAsync("urn:adsk.dtt:...", new HistoryPostRequestBody { IncludeChanges = true });
    /// </code>
    /// </example>
    public async Task<HistoryPostResponse?> GetTwinHistoryAsync(
        string twinId,
        HistoryPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Tandem.V1.Twins[twinId].History
            .PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Returns users from the given twin.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /tandem/v1/twins/{twinID}/users
    /// </remarks>
    /// <param name="twinId">Twin URN</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="UsersGetResponse"/> containing the twin users</returns>
    /// <example>
    /// <code>
    /// UsersGetResponse? users = await client.TwinsManager.GetTwinUsersAsync("urn:adsk.dtt:...");
    /// </code>
    /// </example>
    public async Task<UsersGetResponse?> GetTwinUsersAsync(
        string twinId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Tandem.V1.Twins[twinId].Users
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }
}

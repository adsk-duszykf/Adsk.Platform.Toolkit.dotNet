using Autodesk.Tandem.Tandem.V1.Groups;
using Autodesk.Tandem.Tandem.V1.Groups.Item;
using Autodesk.Tandem.Tandem.V1.Groups.Item.History;
using Autodesk.Tandem.Tandem.V1.Groups.Item.Users.Item;
using Microsoft.Kiota.Abstractions;

namespace Autodesk.Tandem.Managers;

/// <summary>
/// Manager for Groups operations
/// </summary>
public class GroupsManager
{
    private readonly BaseTandemClient _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="GroupsManager"/> class.
    /// </summary>
    /// <param name="api">The API client</param>
    public GroupsManager(BaseTandemClient api)
    {
        _api = api;
    }

    /// <summary>
    /// Returns subject's groups based on supplied auth token.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /tandem/v1/groups
    /// </remarks>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="GroupsGetResponse"/> containing the groups</returns>
    /// <example>
    /// <code>
    /// GroupsGetResponse? groups = await client.GroupsManager.GetGroupsAsync();
    /// </code>
    /// </example>
    public async Task<GroupsGetResponse?> GetGroupsAsync(
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Tandem.V1.Groups
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Returns a group definition.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /tandem/v1/groups/{groupID}
    /// </remarks>
    /// <param name="groupId">Group URN</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="WithGroupGetResponse"/> containing the group definition</returns>
    /// <example>
    /// <code>
    /// WithGroupGetResponse? group = await client.GroupsManager.GetGroupAsync("urn:adsk.dtg:...");
    /// </code>
    /// </example>
    public async Task<WithGroupGetResponse?> GetGroupAsync(
        string groupId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Tandem.V1.Groups[groupId]
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Returns history of all changes for a given group.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /tandem/v1/groups/{groupID}/history
    /// </remarks>
    /// <param name="groupId">Group URN</param>
    /// <param name="body">The history query request body</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="HistoryPostResponse"/> containing the change history</returns>
    /// <example>
    /// <code>
    /// HistoryPostResponse? history = await client.GroupsManager.GetGroupHistoryAsync("urn:adsk.dtg:...", new HistoryPostRequestBody { IncludeChanges = true });
    /// </code>
    /// </example>
    public async Task<HistoryPostResponse?> GetGroupHistoryAsync(
        string groupId,
        HistoryPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Tandem.V1.Groups[groupId].History
            .PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Add user to the given group.
    /// </summary>
    /// <remarks>
    /// Wraps: PUT /tandem/v1/groups/{groupID}/users/{userID}
    /// </remarks>
    /// <param name="groupId">Group URN</param>
    /// <param name="userId">User ID</param>
    /// <param name="body">The request body containing the access level</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    /// <example>
    /// <code>
    /// await client.GroupsManager.AddUserToGroupAsync("urn:adsk.dtg:...", "userId", new WithUserPutRequestBody { AccessLevel = 50 });
    /// </code>
    /// </example>
    public async Task AddUserToGroupAsync(
        string groupId,
        string userId,
        WithUserPutRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        await _api.Tandem.V1.Groups[groupId].Users[userId]
            .PutAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }
}

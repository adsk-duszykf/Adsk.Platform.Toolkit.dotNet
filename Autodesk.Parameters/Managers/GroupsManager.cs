using Autodesk.Parameters.Parameters.V1.Accounts.Item.Groups;
using Autodesk.Parameters.Parameters.V1.Accounts.Item.Groups.Item;
using Microsoft.Kiota.Abstractions;

namespace Autodesk.Parameters.Managers;

/// <summary>
/// Manager for Parameter group operations
/// </summary>
public class GroupsManager
{
    private readonly BaseParametersClient _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="GroupsManager"/> class.
    /// </summary>
    /// <param name="api">The API client</param>
    public GroupsManager(BaseParametersClient api)
    {
        _api = api;
    }

    /// <summary>
    /// Lists all groups in the specified account.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /parameters/v1/accounts/{accountId}/groups
    /// APS docs: https://aps.autodesk.com/en/docs/parameters/v1/reference/http/parameters-listgroups-GET
    /// </remarks>
    /// <param name="accountId">The Autodesk Construction Cloud account ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="GroupsGetResponse"/> containing the groups and pagination info</returns>
    /// <example>
    /// <code>
    /// GroupsGetResponse? groups = await client.GroupsManager.ListGroupsAsync(accountId);
    /// foreach (var group in groups?.Results ?? [])
    /// {
    ///     Console.WriteLine($"{group.Id}: {group.Title}");
    /// }
    /// </code>
    /// </example>
    public async Task<GroupsGetResponse?> ListGroupsAsync(
        Guid accountId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Parameters.V1.Accounts[accountId].Groups
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Returns the details of the specified group, including details of your access level to this group.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /parameters/v1/accounts/{accountId}/groups/{groupId}
    /// APS docs: https://aps.autodesk.com/en/docs/parameters/v1/reference/http/parameters-getgroup-GET
    /// </remarks>
    /// <param name="accountId">The Autodesk Construction Cloud account ID</param>
    /// <param name="groupId">The group ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="WithGroupGetResponse"/> containing the group details</returns>
    /// <example>
    /// <code>
    /// WithGroupGetResponse? group = await client.GroupsManager.GetGroupAsync(accountId, "my-group-id");
    /// Console.WriteLine(group?.Title);
    /// </code>
    /// </example>
    public async Task<WithGroupGetResponse?> GetGroupAsync(
        Guid accountId,
        string groupId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Parameters.V1.Accounts[accountId].Groups[groupId]
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Updates the details of an existing group. The title cannot be empty or null.
    /// </summary>
    /// <remarks>
    /// Wraps: PUT /parameters/v1/accounts/{accountId}/groups/{groupId}
    /// APS docs: https://aps.autodesk.com/en/docs/parameters/v1/reference/http/parameters-updategroup-PUT
    /// </remarks>
    /// <param name="accountId">The Autodesk Construction Cloud account ID</param>
    /// <param name="groupId">The group ID</param>
    /// <param name="body">The group update payload</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="WithGroupPutResponse"/> containing the updated group details</returns>
    /// <example>
    /// <code>
    /// WithGroupPutResponse? updated = await client.GroupsManager.UpdateGroupAsync(accountId, "my-group-id", body);
    /// </code>
    /// </example>
    public async Task<WithGroupPutResponse?> UpdateGroupAsync(
        Guid accountId,
        string groupId,
        WithGroupPutRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Parameters.V1.Accounts[accountId].Groups[groupId]
            .PutAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }
}

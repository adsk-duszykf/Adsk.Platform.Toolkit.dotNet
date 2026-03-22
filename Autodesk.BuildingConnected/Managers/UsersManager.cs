using System.Runtime.CompilerServices;
using Autodesk.BuildingConnected;
using Autodesk.BuildingConnected.Construction.Buildingconnected.V2.Users;
using Autodesk.BuildingConnected.Construction.Buildingconnected.V2.Users.Item;
using Autodesk.BuildingConnected.Construction.Buildingconnected.V2.Users.Me;
using Microsoft.Kiota.Abstractions;
using static Autodesk.BuildingConnected.Construction.Buildingconnected.V2.Users.UsersRequestBuilder;

namespace Autodesk.BuildingConnected.Managers;

/// <summary>
/// Manager for BuildingConnected user operations — lists company users, resolves users by id, and reads the current user.
/// </summary>
public class UsersManager
{
    private readonly BaseBuildingConnectedClient _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="UsersManager"/> class.
    /// </summary>
    /// <param name="api">The BuildingConnected API client.</param>
    public UsersManager(BaseBuildingConnectedClient api)
    {
        _api = api;
    }

    /// <summary>
    /// Lists BuildingConnected users for the requesting user&apos;s company with automatic cursor-based pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/buildingconnected/v2/users
    /// APS docs: https://aps.autodesk.com/en/docs/buildingconnected/v2/reference/http/buildingconnected-users-GET
    /// </remarks>
    /// <param name="requestConfiguration">(Optional) Configuration for the request, including <c>CursorState</c>, <c>Limit</c>, and <c>FilterofficeId</c>.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="UsersGetResponse_results"/>.</returns>
    /// <example>
    /// <code>
    /// await foreach (var user in client.UsersManager.ListUsersAsync())
    /// {
    ///     Console.WriteLine(user);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<UsersGetResponse_results> ListUsersAsync(
        RequestConfiguration<UsersRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? cursor = requestConfiguration?.QueryParameters?.CursorState;

        while (true)
        {
            UsersGetResponse? response = await _api.Construction.Buildingconnected.V2.Users
                .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                    r.QueryParameters.CursorState = cursor;
                }, cancellationToken);

            if (response?.Results is not { Count: > 0 })
                yield break;

            foreach (UsersGetResponse_results item in response.Results)
            {
                yield return item;
            }

            if (string.IsNullOrEmpty(response.Pagination?.CursorState))
                yield break;

            cursor = response.Pagination.CursorState;
        }
    }

    /// <summary>
    /// Gets a single BuildingConnected user by id.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/buildingconnected/v2/users/{userId}
    /// APS docs: https://aps.autodesk.com/en/docs/buildingconnected/v2/reference/http/buildingconnected-users-userId-GET
    /// </remarks>
    /// <param name="userId">The user&apos;s unique BuildingConnected id.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>A <see cref="WithUserGetResponse"/> instance, or <c>null</c> if the response is empty.</returns>
    /// <example>
    /// <code>
    /// WithUserGetResponse? user = await client.UsersManager.GetUserAsync("user-id-here");
    /// </code>
    /// </example>
    public async Task<WithUserGetResponse?> GetUserAsync(
        string userId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Buildingconnected.V2.Users[userId]
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Gets the BuildingConnected user profile for the authenticated caller.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/buildingconnected/v2/users/me
    /// APS docs: https://aps.autodesk.com/en/docs/buildingconnected/v2/reference/http/buildingconnected-users-me-GET
    /// </remarks>
    /// <param name="requestConfiguration">(Optional) Configuration for the request.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>A <see cref="MeGetResponse"/> instance, or <c>null</c> if the response is empty.</returns>
    /// <example>
    /// <code>
    /// MeGetResponse? me = await client.UsersManager.GetCurrentUserAsync();
    /// </code>
    /// </example>
    public async Task<MeGetResponse?> GetCurrentUserAsync(
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Buildingconnected.V2.Users.Me
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }
}

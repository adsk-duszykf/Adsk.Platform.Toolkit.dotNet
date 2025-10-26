using Autodesk.ACC.AccountAdmin.BIM.Hq.V1.Accounts.Item.Users;
using Autodesk.ACC.AccountAdmin.BIM.Hq.V1.Accounts.Item.Users.Import;
using Autodesk.ACC.AccountAdmin.BIM.Hq.V1.Accounts.Item.Users.Item;
using Microsoft.Kiota.Abstractions;

namespace Autodesk.ACC.AccountAdmin.Managers;

/// <summary>
/// Manager for Account Users operations (BIM360)
/// </summary>
public class AccountUsersManager
{
    private readonly ApiRequestBuilder _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="AccountUsersManager"/> class.
    /// </summary>
    /// <param name="api">The API request builder</param>
    public AccountUsersManager(ApiRequestBuilder api)
    {
        _api = api;
    }

    /// <summary>
    /// Query all the users in a specific BIM 360 account.
    /// </summary>
    /// <param name="accountId">The account ID of the users</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User data</returns>
    public async Task<UsersGetResponse?> ListUsersAsync(
        string accountId,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _api.Hq.V1.Accounts[accountId]
            .Users
            .GetAsUsersGetResponseAsync(requestConfiguration, cancellationToken);

        return result;
    }

    /// <summary>
    /// Create a new user in the BIM 360 member directory.
    /// </summary>
    /// <param name="accountId">The account ID</param>
    /// <param name="userData">The user creation data</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created user information</returns>
    public async Task<UsersPostResponse?> CreateUserAsync(
        string accountId,
        UsersPostRequestBody userData,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _api.Hq?.V1?.Accounts[accountId]
            .Users
            .PostAsUsersPostResponseAsync(userData, requestConfiguration, cancellationToken);

        return result;
    }

    /// <summary>
    /// Query the details of a specific user.
    /// </summary>
    /// <param name="accountId">The account ID of the user</param>
    /// <param name="userId">User ID</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The user information</returns>
    public async Task<WithUser_GetResponse?> GetUserAsync(
        string accountId,
        string userId,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _api.Hq!.V1.Accounts[accountId]
            .Users[userId]
            .GetAsWithUser_GetResponseAsync(requestConfiguration, cancellationToken);

        return result;
    }

    /// <summary>
    /// Bulk import users to the master member directory in a BIM 360 account.
    /// Maximum 50 users per call.
    /// </summary>
    /// <param name="accountId">The account ID</param>
    /// <param name="users">Array of users to import (max 50)</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Import result with success and failure counts</returns>
    public async Task<ImportPostResponse?> ImportUsersAsync(
        string accountId,
        List<Import> users,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _api.Hq!.V1.Accounts[accountId]
            .Users
            .Import
            .PostAsImportPostResponseAsync(users, requestConfiguration, cancellationToken);

        return result;
    }
}


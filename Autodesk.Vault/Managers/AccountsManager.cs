using System.Runtime.CompilerServices;
using Autodesk.Vault.Models;
using Microsoft.Kiota.Abstractions;
using static Autodesk.Vault.Groups.GroupsRequestBuilder;
using static Autodesk.Vault.ProfileAttributeDefinitions.ProfileAttributeDefinitionsRequestBuilder;
using static Autodesk.Vault.Roles.RolesRequestBuilder;
using static Autodesk.Vault.Users.UsersRequestBuilder;

namespace Autodesk.Vault.Managers;

/// <summary>
/// Manager for Accounts operations (Users, Groups, Roles, Profile Attributes)
/// </summary>
public class AccountsManager
{
    private readonly BaseVaultClient _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="AccountsManager"/> class.
    /// </summary>
    /// <param name="api">The API client</param>
    public AccountsManager(BaseVaultClient api)
    {
        _api = api;
    }

    #region Groups

    /// <summary>
    /// Get all groups.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /groups
    /// </remarks>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports limit, cursorState)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="GroupCollection"/> containing the list of groups</returns>
    /// <example>
    /// <code>
    /// GroupCollection? groups = await client.Accounts.GetGroupsAsync();
    /// </code>
    /// </example>
    public async Task<GroupCollection?> GetGroupsAsync(
        RequestConfiguration<GroupsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Groups
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Get a group by its ID.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /groups/{id}
    /// </remarks>
    /// <param name="groupId">The unique identifier of a group</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="GroupExtended"/> containing the group information</returns>
    /// <example>
    /// <code>
    /// GroupExtended? group = await client.Accounts.GetGroupByIdAsync("42");
    /// </code>
    /// </example>
    public async Task<GroupExtended?> GetGroupByIdAsync(
        string groupId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Groups[groupId]
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Get group account information by a specific authentication type.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /groups/{id}/accounts/{authType}
    /// </remarks>
    /// <param name="groupId">The unique identifier of a group</param>
    /// <param name="authType">Authentication type: ActiveDirectory, Vault, or Autodesk</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Account"/> containing the account information</returns>
    /// <example>
    /// <code>
    /// Account? account = await client.Accounts.GetGroupAccountByAuthTypeAsync("42", "Vault");
    /// </code>
    /// </example>
    public async Task<Account?> GetGroupAccountByAuthTypeAsync(
        string groupId,
        string authType,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Groups[groupId].Accounts[authType]
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    #endregion

    #region Profile Attributes

    /// <summary>
    /// Get all profile attribute definitions.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /profile-attribute-definitions
    /// </remarks>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports filter[association], limit, cursorState)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="ProfileAttributeDefinitionCollection"/> containing the profile attribute definitions</returns>
    /// <example>
    /// <code>
    /// ProfileAttributeDefinitionCollection? attrs = await client.Accounts.GetProfileAttributeDefinitionsAsync();
    /// </code>
    /// </example>
    public async Task<ProfileAttributeDefinitionCollection?> GetProfileAttributeDefinitionsAsync(
        RequestConfiguration<ProfileAttributeDefinitionsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.ProfileAttributeDefinitions
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Get a profile attribute definition by its ID.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /profile-attribute-definitions/{id}
    /// </remarks>
    /// <param name="definitionId">The unique identifier of a profile attribute definition</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="ProfileAttributeDefinition"/> containing the profile attribute definition</returns>
    /// <example>
    /// <code>
    /// ProfileAttributeDefinition? attrDef = await client.Accounts.GetProfileAttributeDefinitionByIdAsync("5");
    /// </code>
    /// </example>
    public async Task<ProfileAttributeDefinition?> GetProfileAttributeDefinitionByIdAsync(
        string definitionId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.ProfileAttributeDefinitions[definitionId]
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    #endregion

    #region Roles

    /// <summary>
    /// Lists user roles with automatic cursor-based pagination. The returned list depends on the permissions of the user passed in via BearerToken.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /roles
    /// </remarks>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports limit, cursorState)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="Role"/> items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (var role in client.Accounts.ListRolesAsync())
    /// {
    ///     Console.WriteLine(role.Name);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<Role> ListRolesAsync(
        RequestConfiguration<RolesRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? cursor = requestConfiguration?.QueryParameters?.CursorState;

        while (true)
        {
            RoleCollection? response = await _api.Roles
                .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                    r.QueryParameters.CursorState = cursor;
                }, cancellationToken);

            if (response?.Results is not { Count: > 0 })
                yield break;

            foreach (Role item in response.Results)
            {
                yield return item;
            }

            if (string.IsNullOrEmpty(response.Pagination?.NextUrl))
                yield break;

            cursor = PaginationHelper.ExtractBookmarkFromNextUrl(response.Pagination.NextUrl);
        }
    }

    /// <summary>
    /// Get a role by its ID.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /roles/{id}
    /// </remarks>
    /// <param name="roleId">The unique identifier of a role</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Role"/> containing the role information</returns>
    /// <example>
    /// <code>
    /// Role? role = await client.Accounts.GetRoleByIdAsync("3");
    /// </code>
    /// </example>
    public async Task<Role?> GetRoleByIdAsync(
        string roleId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Roles[roleId]
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    #endregion

    #region Users

    /// <summary>
    /// Lists all users in the Vault with automatic cursor-based pagination. Note: AdminUserRead permission is required.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /users
    /// </remarks>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports limit, cursorState)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="User"/> items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (var user in client.Accounts.ListUsersAsync())
    /// {
    ///     Console.WriteLine(user.Name);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<User> ListUsersAsync(
        RequestConfiguration<UsersRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? cursor = requestConfiguration?.QueryParameters?.CursorState;

        while (true)
        {
            UserCollection? response = await _api.Users
                .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                    r.QueryParameters.CursorState = cursor;
                }, cancellationToken);

            if (response?.Results is not { Count: > 0 })
                yield break;

            foreach (User item in response.Results)
            {
                yield return item;
            }

            if (string.IsNullOrEmpty(response.Pagination?.NextUrl))
                yield break;

            cursor = PaginationHelper.ExtractBookmarkFromNextUrl(response.Pagination.NextUrl);
        }
    }

    /// <summary>
    /// Get full user object associated with the specified user ID.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /users/{id}
    /// </remarks>
    /// <param name="userId">The unique identifier of a user</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="UserExtended"/> containing the user information</returns>
    /// <example>
    /// <code>
    /// UserExtended? user = await client.Accounts.GetUserByIdAsync("2");
    /// </code>
    /// </example>
    public async Task<UserExtended?> GetUserByIdAsync(
        string userId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Users[userId]
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Get all accounts associated with the specified user ID.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /users/{id}/accounts
    /// </remarks>
    /// <param name="userId">The unique identifier of a user</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="AccountCollection"/> containing the user accounts</returns>
    /// <example>
    /// <code>
    /// AccountCollection? accounts = await client.Accounts.GetUserAccountsAsync("2");
    /// </code>
    /// </example>
    public async Task<AccountCollection?> GetUserAccountsAsync(
        string userId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Users[userId].Accounts
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Get user account information for a specific authentication type.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /users/{id}/accounts/{authType}
    /// </remarks>
    /// <param name="userId">The unique identifier of a user</param>
    /// <param name="authType">Authentication type: ActiveDirectory, Vault, or Autodesk</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Account"/> containing the account information</returns>
    /// <example>
    /// <code>
    /// Account? account = await client.Accounts.GetUserAccountByAuthTypeAsync("2", "Autodesk");
    /// </code>
    /// </example>
    public async Task<Account?> GetUserAccountByAuthTypeAsync(
        string userId,
        string authType,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Users[userId].Accounts[authType]
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    #endregion
}

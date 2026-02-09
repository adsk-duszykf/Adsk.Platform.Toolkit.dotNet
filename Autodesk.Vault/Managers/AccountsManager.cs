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
    /// Get all groups
    /// </summary>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of groups</returns>
    public async Task<GroupCollection?> GetGroupsAsync(
        Action<RequestConfiguration<GroupsRequestBuilderGetQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _api.Groups
            .GetAsync(requestConfiguration, cancellationToken);

        return result;
    }

    /// <summary>
    /// Get group by its ID
    /// </summary>
    /// <param name="groupId">Group ID</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Group information</returns>
    public async Task<GroupExtended?> GetGroupByIdAsync(
        string groupId,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _api.Groups[groupId]
            .GetAsync(requestConfiguration, cancellationToken);

        return result;
    }

    /// <summary>
    /// Get group account information by a specific type
    /// </summary>
    /// <param name="groupId">Group ID</param>
    /// <param name="authType">Authentication type (ActiveDirectory, Vault, or Autodesk)</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Account information</returns>
    public async Task<Account?> GetGroupAccountByAuthTypeAsync(
        string groupId,
        string authType,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _api.Groups[groupId].Accounts[authType]
            .GetAsync(requestConfiguration, cancellationToken);

        return result;
    }

    #endregion

    #region Profile Attributes

    /// <summary>
    /// Get all profile attribute definitions
    /// </summary>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of profile attribute definitions</returns>
    public async Task<ProfileAttributeDefinitionCollection?> GetProfileAttributeDefinitionsAsync(
        Action<RequestConfiguration<ProfileAttributeDefinitionsRequestBuilderGetQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _api.ProfileAttributeDefinitions
            .GetAsync(requestConfiguration, cancellationToken);

        return result;
    }

    /// <summary>
    /// Get profile attribute definition by its ID
    /// </summary>
    /// <param name="definitionId">Profile attribute definition ID</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Profile attribute definition</returns>
    public async Task<ProfileAttributeDefinition?> GetProfileAttributeDefinitionByIdAsync(
        string definitionId,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _api.ProfileAttributeDefinitions[definitionId]
            .GetAsync(requestConfiguration, cancellationToken);

        return result;
    }

    #endregion

    #region Roles

    /// <summary>
    /// Get user roles. The returned list of roles depends on the permissions of the user passed in via BearerToken.
    /// </summary>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of roles</returns>
    public async Task<RoleCollection?> GetRolesAsync(
        Action<RequestConfiguration<RolesRequestBuilderGetQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _api.Roles
            .GetAsync(requestConfiguration, cancellationToken);

        return result;
    }

    /// <summary>
    /// Get the role object with given role id
    /// </summary>
    /// <param name="roleId">Role ID</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Role information</returns>
    public async Task<Role?> GetRoleByIdAsync(
        string roleId,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _api.Roles[roleId]
            .GetAsync(requestConfiguration, cancellationToken);

        return result;
    }

    #endregion

    #region Users

    /// <summary>
    /// Get the list of all users in the Vault. Note: AdminUserRead permission is required.
    /// </summary>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of users</returns>
    public async Task<UserCollection?> GetUsersAsync(
        Action<RequestConfiguration<UsersRequestBuilderGetQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _api.Users
            .GetAsync(requestConfiguration, cancellationToken);

        return result;
    }

    /// <summary>
    /// Get full user object associated with the specified userId
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User information</returns>
    public async Task<UserExtended?> GetUserByIdAsync(
        string userId,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _api.Users[userId]
            .GetAsync(requestConfiguration, cancellationToken);

        return result;
    }

    /// <summary>
    /// Get all accounts associated with the specified userId
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of user accounts</returns>
    public async Task<AccountCollection?> GetUserAccountsAsync(
        string userId,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _api.Users[userId].Accounts
            .GetAsync(requestConfiguration, cancellationToken);

        return result;
    }

    /// <summary>
    /// Get user account information for a specific type
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="authType">Authentication type (ActiveDirectory, Vault, or Autodesk)</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Account information</returns>
    public async Task<Account?> GetUserAccountByAuthTypeAsync(
        string userId,
        string authType,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _api.Users[userId].Accounts[authType]
            .GetAsync(requestConfiguration, cancellationToken);

        return result;
    }

    #endregion
}

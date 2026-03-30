using Autodesk.Vault.Models;
using Autodesk.Vault.Sessions;
using Microsoft.Kiota.Abstractions;

namespace Autodesk.Vault.Managers;

/// <summary>
/// Manager for Authentication operations
/// </summary>
public class AuthManager
{
    private readonly BaseVaultClient _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthManager"/> class.
    /// </summary>
    /// <param name="api">The API client</param>
    public AuthManager(BaseVaultClient api)
    {
        _api = api;
    }

    /// <summary>
    /// Create a new session with the input username/password and vault name.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /sessions
    /// </remarks>
    /// <param name="sessionData">Session creation data containing vault name, username, password, and app code</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Session"/> containing the session information including access token</returns>
    /// <example>
    /// <code>
    /// Session? session = await client.Auth.CreateSessionAsync(new SessionsPostRequestBody
    /// {
    ///     Input = new SessionsPostRequestBody_input
    ///     {
    ///         Vault = "MyVault",
    ///         UserName = "administrator",
    ///         Password = ""
    ///     }
    /// });
    /// </code>
    /// </example>
    public async Task<Session?> CreateSessionAsync(
        SessionsPostRequestBody sessionData,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Sessions
            .PostAsync(sessionData, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Create a new session with the Windows identity to a specific Knowledge Vault.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /sessions/win-auth
    /// </remarks>
    /// <param name="sessionData">Session creation data with Windows authentication containing vault name and app code</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Session"/> containing the session information including access token</returns>
    /// <example>
    /// <code>
    /// Session? session = await client.Auth.CreateSessionWithWinAuthAsync(new WinAuthPostRequestBody
    /// {
    ///     Input = new WinAuthPostRequestBody_input { Vault = "MyVault" }
    /// });
    /// </code>
    /// </example>
    public async Task<Session?> CreateSessionWithWinAuthAsync(
        Sessions.WinAuth.WinAuthPostRequestBody sessionData,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Sessions.WinAuth
            .PostAsync(sessionData, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Get user session tied to Bearer token. Pass @current to get the current active user session.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /sessions/{id}
    /// </remarks>
    /// <param name="sessionId">Session ID or "@current" for the current session</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Session"/> containing the session information</returns>
    /// <example>
    /// <code>
    /// Session? session = await client.Auth.GetSessionAsync("@current");
    /// </code>
    /// </example>
    public async Task<Session?> GetSessionAsync(
        string sessionId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Sessions[sessionId]
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Delete user session tied to Bearer token. Pass @current to delete the current active user session.
    /// </summary>
    /// <remarks>
    /// Wraps: DELETE /sessions/{id}
    /// </remarks>
    /// <param name="sessionId">Session ID or "@current" for the current session</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    /// <example>
    /// <code>
    /// await client.Auth.DeleteSessionAsync("@current");
    /// </code>
    /// </example>
    public async Task DeleteSessionAsync(
        string sessionId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        await _api.Sessions[sessionId]
            .DeleteAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }
}

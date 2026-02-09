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
    /// Create a new session with the input username/password and vault name
    /// </summary>
    /// <param name="sessionData">Session creation data</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Session information</returns>
    public async Task<Session?> CreateSessionAsync(
        SessionsPostRequestBody sessionData,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _api.Sessions
            .PostAsync(sessionData, requestConfiguration, cancellationToken);

        return result;
    }

    /// <summary>
    /// Create a new session with the windows identity to a specific Knowledge Vault
    /// </summary>
    /// <param name="sessionData">Session creation data with Windows authentication</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Session information</returns>
    public async Task<Session?> CreateSessionWithWinAuthAsync(
        Sessions.WinAuth.WinAuthPostRequestBody sessionData,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _api.Sessions.WinAuth
            .PostAsync(sessionData, requestConfiguration, cancellationToken);

        return result;
    }

    /// <summary>
    /// Get user session tied to Bearer token. Pass @current to get the current active user session.
    /// </summary>
    /// <param name="sessionId">Session ID or "@current" for the current session</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Session information</returns>
    public async Task<Session?> GetSessionAsync(
        string sessionId,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _api.Sessions[sessionId]
            .GetAsync(requestConfiguration, cancellationToken);

        return result;
    }

    /// <summary>
    /// Delete user session tied to Bearer token. Pass @current to delete the current active user session.
    /// </summary>
    /// <param name="sessionId">Session ID or "@current" for the current session</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public async Task DeleteSessionAsync(
        string sessionId,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        await _api.Sessions[sessionId]
            .DeleteAsync(requestConfiguration, cancellationToken);
    }
}

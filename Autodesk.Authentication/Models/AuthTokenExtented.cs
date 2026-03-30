using Autodesk.Authentication.Models;

namespace Autodesk.Authentication.Helpers.Models;

/// <summary>
/// Extends <see cref="AuthToken"/> with an absolute UTC expiration timestamp.
/// </summary>
public class AuthTokenExtended : AuthToken
{
    /// <summary>
    /// Initializes a new instance using the current UTC time as the creation timestamp.
    /// </summary>
    /// <param name="accessToken">The token returned by the authentication endpoint.</param>
    public AuthTokenExtended(AuthToken? accessToken)
    {
        Initialize(accessToken, DateTime.UtcNow);
    }
    /// <summary>
    /// Initializes a new instance with an explicit creation timestamp.
    /// </summary>
    /// <param name="accessToken">The token returned by the authentication endpoint.</param>
    /// <param name="createAt">The UTC timestamp when the token was created.</param>
    public AuthTokenExtended(AuthToken? accessToken, DateTime createAt)
    {
        Initialize(accessToken, createAt);
    }

    private void Initialize(AuthToken? accessToken, DateTime createAt)
    {
        AccessToken = accessToken?.AccessToken;
        ExpiresIn = accessToken?.ExpiresIn;
        RefreshToken = accessToken?.RefreshToken;
        IdToken = accessToken?.IdToken;
        ExpiresAt = createAt.AddSeconds(ExpiresIn ?? 0);
    }

    /// <summary>
    /// Return the expiration date of the access token in UTC
    /// </summary>
    public DateTime ExpiresAt { get; private set; }

}
